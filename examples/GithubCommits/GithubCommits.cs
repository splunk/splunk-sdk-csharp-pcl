using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splunk.ModularInputs;
using System.IO;
using System.Linq;
using Octokit;
using System.Text.RegularExpressions;

namespace GithubCommits
{
    public class Program : ModularInput
    {
        /// <summary>
        /// Main method which dispatches to ModularInput.Run&lt;T&gt;.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>An exit code.</returns>
        public static int Main(string[] args)
        {
            return Run<Program>(args);
        }

        /// <summary>
        /// Define a Scheme instance that describes this modular input's behavior. The scheme
        /// will be serialized to XML and printed to stdout when this program is invoked with
        /// the sole argument <tt>--scheme</tt>, which Splunk does when starting up and each time
        /// the app containing the modular input is enabled.
        /// </summary>
        /// <remarks>
        /// You must define a Title, Description, and a list of Arguments. Each argument
        /// you list here must also be specified in
        /// <tt>GithubCommits\README\inputs.conf.spec</tt>.
        /// </remarks>
        public override Scheme Scheme
        {
            get
            {
                return new Scheme
                {
                    Title = "Github Commits",
                    Description = "Streams events of commits in the specified Github repository.",
                    Arguments = new List<Argument>
                    {
                            new Argument
                            {
                                Name = "Owner",
                                Description = "The owner of the repository to stream the commits from.",
                                DataType = DataType.String,
                                RequiredOnCreate = true,
                                RequiredOnEdit = false
                            },
                            new Argument
                            {
                                Name = "Repository",
                                Description = "The name of the public repository. (Private repositories are allowed with specific token)",
                                DataType = DataType.String,
                                RequiredOnCreate = true,
                                RequiredOnEdit = false
                            },
                            new Argument
                            {
                                Name = "Token",
                                Description = "(Optional) A Github API access token. Required for private repositories. More info found here https://github.com/blog/1509-personal-api-tokens",
                                DataType = DataType.String,
                                RequiredOnCreate = false,
                                RequiredOnEdit = false
                            }
                    }
                };
            }
        }

        /// <summary>
        /// Check that the values of arguments specified for a newly created or edited instance of
        /// this modular input are valid. If they are valid, set <tt>errorMessage</tt> to <tt>""</tt>
        /// and return <tt>true</tt>. Otherwise, set <tt>errorMessage</tt> to an informative explanation 
        /// of what makes the arguments invalid and return <tt>false</tt>.
        /// </summary>
        /// <param name="validation">a Validation object specifying the new argument values.</param>
        /// <param name="errorMessage">an output parameter to pass back an error message.</param>
        /// <returns><tt>true</tt> if the arguments are valid and <tt>false</tt> otherwise.</returns>
        public static async Task<Repository> GetRepository(GitHubClient client, String owner, String name)
        {
            return await client.Repository.Get(owner, name);
        }

        public override bool Validate(Validation validation, out string errorMessage)
        {
            errorMessage = String.Empty;
            
            var client = new GitHubClient(new ProductHeaderValue("splunk-sdk-csharp-github-commits"));
            var owner = ((SingleValueParameter)validation.Parameters["Owner"]).ToString();
            var name = ((SingleValueParameter)validation.Parameters["Repository"]).ToString();

            if(((SingleValueParameter)validation.Parameters["Token"]).Value != null){
                var token = ((SingleValueParameter)validation.Parameters["Token"]).ToString();
                client.Credentials = new Credentials(token);
            }

            try
            {
                GetRepository(client, owner, name).Wait();
                return true;
            }
            catch (Exception e)
            {
                errorMessage = "The validation failed, error recieved: " + e.Message;
                return false;
            }
        }

        /// <summary>
        /// Write events to Splunk from this modular input.
        /// </summary>
        /// <remarks>
        /// This function will be invoked once for each instance of the modular input, though that invocation
        /// may or may not be in separate processes, depending on how the modular input is configured. It should
        /// extract the arguments it needs from <tt>inputDefinition</tt>, then write events to <tt>eventWriter</tt>
        /// (which is thread safe).
        /// </remarks>
        /// <param name="inputDefinition">a specification of this instance of the modular input.</param>
        /// <param name="eventWriter">an object that handles writing events to Splunk.</param>
        public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
        {
            var owner = ((SingleValueParameter)inputDefinition.Parameters["Owner"]).ToString();
            var name = ((SingleValueParameter)inputDefinition.Parameters["Repository"]).ToString();
            var checkpointFilePath = Path.Combine(inputDefinition.CheckpointDirectory, owner + " " + name + ".txt");
            var client = new GitHubClient(new ProductHeaderValue("splunk-sdk-csharp-github-commits"));

            if (((SingleValueParameter)inputDefinition.Parameters["Token"]).Value != null)
            {
                var token = ((SingleValueParameter)inputDefinition.Parameters["Token"]).ToString();
                client.Credentials = new Credentials(token);
            }

            var shaKeys = new HashSet<string>();

            var commitList = await client.Repository.Commits.GetAll(owner, name);
            try
            {
                var file = new StreamReader(checkpointFilePath);
                string line;
                while ((line = file.ReadLineAsync()) != null)
                {
                    shaKeys.Add(line);
                }
                file.Close();

            }
            catch (IOException e)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(checkpointFilePath));
                var file = File.Create(checkpointFilePath);
                file.Close();
            }

            var errorMsg = String.Empty;
            try
            {
                var fileWriter = new StreamWriter(checkpointFilePath, true);
                for (var i = 0; i < commitList.Count; i++)
                {
                    if (!shaKeys.Contains(commitList[i].Sha))
                    {
                        shaKeys.Add(commitList[i].Sha);
                        fileWriter.WriteLineAsync(commitList[i].Sha);

                        var commitEvent = new Event();
                        commitEvent.Stanza = name;
                        commitEvent.SourceType = "github_commits";
                        commitEvent.Time = commitList[i].Commit.Author.Date;

                        var json = new Dictionary<string, string>();
                        json["sha"] = commitList[i].Sha;
                        json["api_url"] = commitList[i].Url;
                        json["url"] = commitList[i].HtmlUrl;
                        json["message"] = Regex.Replace(commitList[i].Commit.Message, "\\r|\\n", " ");
                        json["author"] = commitList[i].Commit.Author.Name;
                        json["date"] = commitList[i].Commit.Author.Date.ToString();

                        var formattedJSON = json.Select(d =>
                            string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));

                        commitEvent.Data = ("{" + string.Join(",", formattedJSON) + "}");

                        await eventWriter.QueueEventForWriting(commitEvent);
                        await eventWriter.LogAsync("INFO", name + " indexed a Github commit with sha: " + commitList[i].Sha);

                    }
                }
                fileWriter.Close();
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }

            // Can't use await inside a catch block, if we got an error log it
            if (!errorMsg.StringEmpty)
            {
                await eventWriter.LogAsync("ERROR", errorMsg);
            }
        }
    }
}
