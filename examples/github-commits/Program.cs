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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace github_commits
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
        /// Define a Scheme for a GitHub repository. Here we have 3 arguments:
        /// Owner, Repository, and Token (which is optional).
        /// </summary>
        /// <remarks>
        /// You must define a Title, Description, and a list of Arguments. Each argument
        /// you list here must also be specified in
        /// <tt>github-commits\README\inputs.conf.spec</tt>.
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
                                Description = "The name of the public repository. (Private repositories are allowed if a token is specified).",
                                DataType = DataType.String,
                                RequiredOnCreate = true,
                                RequiredOnEdit = false
                            },
                            new Argument
                            {
                                Name = "Token",
                                Description = "(Optional) A Github API access token, required for private repositories. More info can found here https://github.com/blog/1509-personal-api-tokens",
                                DataType = DataType.String,
                                RequiredOnCreate = false,
                                RequiredOnEdit = false
                            }
                    }
                };
            }
        }

        /// <summary>
        /// Asynchronously gets the specified repository using the Octokit.net <tt>GithubClient</tt>.
        /// </summary>
        /// <param name="client">The Octokit.net GithubClient.</param>
        /// <param name="owner">The GitHub repository owner's name.</param>
        /// <param name="repositoryName">The GitHub repository's name.</param>
        public static async Task<Repository> GetRepository(GitHubClient client, String owner, String repositoryName)
        {
            return await client.Repository.Get(owner, repositoryName);
        }

        /// <summary>
        /// In this example, we make sure the GitHub repository exists and validate the access token
        /// if it's set.
        /// </summary>
        /// <param name="validation">A Validation object specifying the new argument values.</param>
        /// <param name="errorMessage">An output parameter to pass back an error message.</param>
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

        /// <summary>
        /// Handles creating an Event object from a GitHub commit which will be streamed into Splunk.
        /// </summary>
        /// <param name="githubCommit">The individual GithubCommit object which holds the data for a commit.</param>
        /// <param name="eventWriter">The EventWriter for streaming events to Splunk.</param>
        /// <param name="owner">The GitHub repository owner's name.</param>
        /// <param name="repositoryName">The GitHub repository's name.</param>
        public async Task StreamCommit(GitHubCommit githubCommit, EventWriter eventWriter, string owner, string repositoryName)
        {
            string authorName = githubCommit.Commit.Author.Name;
            DateTime date = githubCommit.Commit.Author.Date.DateTime;

            // Replace any newlines with a space
            string commitMessage = Regex.Replace(githubCommit.Commit.Message, "\\n|\\r", " ");
           
            dynamic json = new JObject();
            json.sha = githubCommit.Sha;
            json.api_url = githubCommit.Url;
            json.url = "http://github.com/" + owner + "/" + repositoryName + "/commit/" + githubCommit.Sha;
            json.message = commitMessage;
            json.author = authorName;
            json.date = date.ToString();

            var commitEvent = new Event();
            commitEvent.Stanza = repositoryName;
            commitEvent.SourceType = "github_commits";
            commitEvent.Time = date;
            commitEvent.Data = json.ToString(Formatting.None);

            await eventWriter.QueueEventForWriting(commitEvent);
        }

        /// <summary>
        /// Pulls down commit data from GitHub and creates events for each commit, which are then streamed to Splunk.
        /// </summary>
        /// <remarks>
        /// This function will be invoked once for each instance of the modular input, though that invocation
        /// may or may not be in separate processes, depending on how the modular input is configured. It should
        /// extract the arguments it needs from <tt>inputDefinition</tt>, then write events to <tt>eventWriter</tt>
        /// (which is thread safe).
        /// </remarks>
        /// <param name="inputDefinition">The definition for this instance of the GitHub input, representing a GitHub repository.</param>
        /// <param name="eventWriter">An object that handles writing events to Splunk.</param>
        public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
        {
            var owner = ((SingleValueParameter)inputDefinition.Parameters["Owner"]).ToString();
            var repository = ((SingleValueParameter)inputDefinition.Parameters["Repository"]).ToString();
            var checkpointFilePath = Path.Combine(inputDefinition.CheckpointDirectory, owner + " " + repository + ".txt");

            var productHeader = new ProductHeaderValue("splunk-sdk-csharp-github-commits");
            ObservableGitHubClient client;

            if (!inputDefinition.Parameters.ContainsKey("Token") || String.IsNullOrWhiteSpace(((SingleValueParameter)inputDefinition.Parameters["Token"]).ToString()))
            {
                client = new ObservableGitHubClient(productHeader);
            }
            else
            {
                client = new ObservableGitHubClient(productHeader, new InMemoryCredentialStore(new Credentials(((SingleValueParameter)inputDefinition.Parameters["Token"]).ToString())));
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
            // Use Rx to stream an event for each commit as they come in
            client.Repository.Commit.GetAll(owner, repository).Subscribe(
                async githubCommit =>
                {
                    if (!shaKeys.Contains(githubCommit.Sha))
                    {
                        await StreamCommit(githubCommit, eventWriter, owner, repository);
                        await fileWriter.WriteLineAsync(githubCommit.Sha); // Write to the checkpoint file
                        shaKeys.Add(githubCommit.Sha);
                        await eventWriter.LogAsync(Severity.Info, repository + " indexed a Github commit with sha: " + githubCommit.Sha);
                    }
                },
                async e =>
                {
                    //error handing goes here
                    await eventWriter.LogAsync(Severity.Error, e.GetType() + " - " + e.StackTrace);
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
