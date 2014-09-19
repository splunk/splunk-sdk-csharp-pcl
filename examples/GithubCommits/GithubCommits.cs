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
            errorMessage = "";
            
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
            var checkpointFilePath = System.IO.Path.Combine(inputDefinition.CheckpointDirectory, owner + " " + name + ".txt");
            var client = new GitHubClient(new ProductHeaderValue("splunk-sdk-csharp-github-commits"));

            if (((SingleValueParameter)inputDefinition.Parameters["Token"]).Value != null)
            {
                var token = ((SingleValueParameter)inputDefinition.Parameters["Token"]).ToString();
                client.Credentials = new Credentials(token);
            }

            HashSet<string> sha_keys = new HashSet<string>();

            var commit_list = await client.Repository.Commits.GetAll(owner, name);
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(checkpointFilePath);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    sha_keys.Add(line);
                }
                file.Close();

            }
            catch
            {
                Directory.CreateDirectory(Path.GetDirectoryName(checkpointFilePath));
                FileStream file = System.IO.File.Create(checkpointFilePath);
                file.Close();
            }

            string error_msg = "";
            try
            {
                System.IO.StreamWriter file_writer = new System.IO.StreamWriter(checkpointFilePath, true);
                for (var i = 0; i < commit_list.Count; i++)
                {
                    if (!sha_keys.Contains(commit_list[i].Sha))
                    {
                        sha_keys.Add(commit_list[i].Sha);
                        file_writer.WriteLine(commit_list[i].Sha);

                        Event e = new Event();
                        e.Stanza = name;
                        e.SourceType = "github_commits";
                        e.Time = commit_list[i].Commit.Author.Date;

                        Dictionary<string, string> json = new Dictionary<string, string>();
                        json.Add("sha", commit_list[i].Sha);
                        json.Add("api_url", commit_list[i].Url);
                        json.Add("url", commit_list[i].HtmlUrl);
                        
                        
                        json.Add("message", Regex.Replace(commit_list[i].Commit.Message, "\\r|\\n", " "));
                        json.Add("author", commit_list[i].Commit.Author.Name);
                        json.Add("date", commit_list[i].Commit.Author.Date.ToString());

                        var jsonifyied = json.Select(d =>
                            string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));

                        e.Data = ("{" + string.Join(",", jsonifyied) + "}");

                        await eventWriter.QueueEventForWriting(e);
                        await eventWriter.LogAsync("INFO", name + " indexed a Github commit with sha: " + commit_list[i].Sha);

                    }
                }
                file_writer.Close();
            }
            catch (Exception e)
            {
                error_msg = e.Message;
            }
            if (error_msg != "")
            {
                await eventWriter.LogAsync("ERROR", error_msg);
            }
        }
    }
}
