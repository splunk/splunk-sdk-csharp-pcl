using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splunk.ModularInputs;
using System.IO;
using System.Linq;
using Octokit;
using System.Text.RegularExpressions;
using Octokit.Reactive;
using Octokit.Internal;

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

        // TODO: comment
        public static async Task<Repository> GetRepository(GitHubClient client, String owner, String name)
        {
            return await client.Repository.Get(owner, name);
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
        public override bool Validate(Validation validation, out string errorMessage)
        {
            errorMessage = String.Empty;

            var client = new GitHubClient(new ProductHeaderValue("splunk-sdk-csharp-github-commits"));
            var owner = ((SingleValueParameter)validation.Parameters["Owner"]).ToString();
            var name = ((SingleValueParameter)validation.Parameters["Repository"]).ToString();

            if (!String.IsNullOrEmpty(((SingleValueParameter)validation.Parameters["Token"]).Value))
            {
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

        // TODO: comment
        public async Task StreamCommit(GitHubCommit githubCommit, EventWriter eventWriter, string owner, string repositoryName)
        {
            string authorName = githubCommit.Commit.Author.Name;
            DateTime date = githubCommit.Commit.Author.Date;

            // Replace any newlines with a space
            string commitMessage = Regex.Replace(githubCommit.Commit.Message, "\\n|\\r", " ");
            
            var json = new Dictionary<string, string>()
            {
                {"sha", githubCommit.Sha},
                {"api_url", githubCommit.Url},
                {"url", "http://github.com/" + owner + "/" + repositoryName + "/commit/" + githubCommit.Sha},
                {"message", commitMessage},
                {"author", authorName}
            };

            json["date"] = date.ToString();

            var formattedJSON = json.Select(d =>
                string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));

            var commitEvent = new Event();
            commitEvent.Stanza = repositoryName;
            commitEvent.SourceType = "github_commits";
            commitEvent.Time = date;
            commitEvent.Data = ("{" + string.Join(",", formattedJSON) + "}");

            await eventWriter.QueueEventForWriting(commitEvent);
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
            var repository = ((SingleValueParameter)inputDefinition.Parameters["Repository"]).ToString();
            var token = ((SingleValueParameter)inputDefinition.Parameters["Token"]);
            var checkpointFilePath = Path.Combine(inputDefinition.CheckpointDirectory, owner + " " + repository + ".txt");

            var productHeader = new ProductHeaderValue("splunk-sdk-csharp-github-commits");
            ObservableGitHubClient client;

            if (token == null || String.IsNullOrWhiteSpace(token.ToString()))
            {
                client = new ObservableGitHubClient(productHeader);
            }
            else
            {
                client = new ObservableGitHubClient(productHeader, new InMemoryCredentialStore(new Credentials(token.ToString())));
            }

            var shaKeys = new HashSet<string>();

            var fileReader = new StreamReader(File.Open(checkpointFilePath, System.IO.FileMode.OpenOrCreate));
            string line;
            while (!String.IsNullOrWhiteSpace(line = await fileReader.ReadLineAsync()))
            {
                shaKeys.Add(line);
            }
            fileReader.Close(); 

            bool done = false;
            var fileWriter = new StreamWriter(checkpointFilePath);
            client.Repository.Commits.GetAll(owner, repository).Subscribe(
                async githubCommit =>
                {
                    if (!shaKeys.Contains(githubCommit.Sha))
                    {
                        await StreamCommit(githubCommit, eventWriter, owner, repository);
                        await fileWriter.WriteLineAsync(githubCommit.Sha);
                        shaKeys.Add(githubCommit.Sha);
                        await eventWriter.LogAsync("INFO", repository + " indexed a Github commit with sha: " + githubCommit.Sha);
                    }
                },
                async e =>
                {
                    //error handing goes here
                    await eventWriter.LogAsync("ERROR", e.GetType() + " - " + e.StackTrace);
                },
                () =>
                {
                    //completion handling goes here
                    fileWriter.Close();
                    done = true;
                }
            );

            // Wait for Rx subscribe to finish above
            while (!done)
            {
                await Task.Delay(100);
            }

        }
    }
}
