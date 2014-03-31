/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

// TODO:
// [O] Contracts
// [O] Documentation
// [ ] Consider schema validation from schemas stored as resources.
//     See [XmlReaderSettings.Schemas Property](http://goo.gl/Syvj4V)

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public class Service : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namespace"></param>
        internal Service(Context context, Namespace @namespace = null)
        {
            this.Context = context;
            this.Namespace = @namespace ?? Namespace.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="sessionKey"></param>
        /// <param name="namespace"></param>
        public Service(Scheme scheme, string host, int port, Namespace @namespace = null)
            : this(new Context(scheme, host, port), @namespace)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Service"/>.
        /// </summary>
        protected internal Context Context
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> used by this <see cref="Service"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        public Server Server
        {
            get { return new Server(this); }
        }

        /// <summary>
        /// Gets or sets the session key used by this <see cref="Service"/>.
        /// </summary>
        public string SessionKey
        {
            get { return this.Context.SessionKey; }
            set { this.Context.SessionKey = value; }
        }

        #endregion

        #region Methods

        #region Authentication/authorization

        /// <summary>
        /// Retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/wvB9N5">authorization/capabilities</a> 
        /// REST API Reference.
        /// </remarks>
        public dynamic GetCapabilities()
        {
            return this.GetCapabilitiesAsync().Result;
        }

        /// <summary>
        /// Retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/wvB9N5">authorization/capabilities</a> 
        /// REST API Reference.
        /// </remarks>
        public async Task<dynamic> GetCapabilitiesAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, new ResourceName(ResourceName.AuthorizationCapabilities)))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count != 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }

                var entry = feed.Entries[0];
                dynamic capabilities = entry.Content.Capabilities; // TODO: Static type (?)

                return capabilities;
            }
        }

        /// <summary>
        /// Provides user authentication.
        /// </summary>
        /// <param name="username">
        /// Splunk account username.
        /// </param>
        /// <param name="password">
        /// Splunk account password for username.
        /// </param>
        /// <remarks>
        /// This method uses the Splunk <a href="http://goo.gl/yXJX75">auth/login</a>
        /// endpoint. The session key that it returns is used for subsequent requests.
        /// It is accessible via the <see cref="SessionKey"/> property.
        /// </remarks>
        public void Login(string username, string password)
        {
            this.LoginAsync(username, password).Wait();
        }

        /// <summary>
        /// Provides user authentication.
        /// </summary>
        /// <param name="username">
        /// Splunk account username.
        /// </param>
        /// <param name="password">
        /// Splunk account password for username.
        /// </param>
        /// <remarks>
        /// This method uses the Splunk <a href="http://goo.gl/yXJX75">auth/login</a>
        /// endpoint. The session key that it returns is used for subsequent requests.
        /// It is accessible via the <see cref="SessionKey"/> property.
        /// </remarks>
        public async Task LoginAsync(string username, string password)
        {
            Contract.Requires(username != null);
            Contract.Requires(password != null);

            using (var response = await this.Context.PostAsync(Namespace.Default, ResourceName.AuthLogin, new Argument[]
            {
                new Argument("username", username),
                new Argument("password", password)
            }))
            {
                if (!response.Message.IsSuccessStatusCode)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
                this.SessionKey = await response.XmlReader.ReadResponseElementAsync("sessionKey");
            }
        }

        #endregion

        #region Applications

        /// <summary>
        /// Retrieves the collection of installed apps.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/izvjYx">apps/local</a> REST API Reference.
        /// </remarks>
        public AppCollection GetApps(AppCollectionArgs args = null)
        {
            return this.GetAppsAsync(args).Result;
        }

        /// <summary>
        /// Retrieves the collection of installed apps.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/izvjYx">apps/local</a> REST API Reference.
        /// </remarks>
        public async Task<AppCollection> GetAppsAsync(AppCollectionArgs args = null)
        {
            var collection = new AppCollection(this.Context, this.Namespace, args);
            await collection.UpdateAsync();
            return collection;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Creates a new configuration file.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration file to create.
        /// </param>
        /// <returns>
        /// An object that represents the configuration file created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7"></a>POST 
        /// properties</a> endpoint to create the <see cref="Configuration"/>
        /// identified by <see cref="name"/>.
        /// </remarks>
        public void CreateConfiguration(string name)
        {
            this.CreateConfigurationAsync(name).Wait();
        }

        /// <summary>
        /// Creates a new configuration file.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration file to create.
        /// </param>
        /// <returns>
        /// An object that represents the configuration file created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7"></a>POST 
        /// properties</a> endpoint to create the <see cref="Configuration"/>
        /// identified by <see cref="name"/>.
        /// </remarks>
        public async Task CreateConfigurationAsync(string name)
        {
            // TODO: Must I delete the configuration file manually, restart Splunk, and then create? No delete operation?

            var args = new Argument[] { new Argument("__conf", name) };

            using (var response = await this.Context.PostAsync(this.Namespace, ResourceName.Properties, args))
            {
                if (response.Message.StatusCode != HttpStatusCode.Created)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Configuration"/> object for manipulating a
        /// configuration file.
        /// </summary>
        /// <param name="name">
        /// The name of a configuration file.
        /// </param>
        /// <returns>
        /// An object for manipulating the configuration file identified by
        /// <see cref="name"/>.
        /// </returns>
        public Configuration GetConfiguration(string name)
        {
            return this.GetConfigurationAsync(name).Result;
        }

        /// <summary>
        /// Gets a <see cref="Configuration"/> object for manipulating a
        /// configuration file.
        /// </summary>
        /// <param name="name">
        /// The name of a configuration file.
        /// </param>
        /// <returns>
        /// An object for manipulating the configuration file identified by
        /// <see cref="name"/>.
        /// </returns>
        public async Task<Configuration> GetConfigurationAsync(string name)
        {
            var entity = new Configuration(this.Context, this.Namespace, name);
            await entity.UpdateAsync();
            return entity;
        }

        /// <summary>
        /// Retrieves the collection of all configuration files known to 
        /// Splunk.
        /// </summary>
        /// <returns>
        /// An object representing the collection of all configuration files
        /// known to Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/Unj6fs"></a>GET 
        /// properties</a> endpoint/> to construct the <see cref=
        /// "ConfigurationCollection"/> it returns.
        /// </remarks>
        public ConfigurationInfoCollection GetConfigurationInfos()
        {
            return this.GetConfigurationInfosAsync().Result;
        }

        /// <summary>
        /// Retrieves the collection of all configuration files known to 
        /// Splunk.
        /// </summary>
        /// <returns>
        /// An object representing the collection of all configuration files
        /// known to Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/Unj6fs"></a>GET 
        /// properties</a> endpoint/> to construct the <see cref=
        /// "ConfigurationCollection"/> it returns.
        /// </remarks>
        public async Task<ConfigurationInfoCollection> GetConfigurationInfosAsync()
        {
            var collection = new ConfigurationInfoCollection(this.Context, this.Namespace);
            await collection.UpdateAsync();
            return collection;
        }

        /// <summary>
        /// Retrieves the collection of all configuration files known to 
        /// Splunk.
        /// </summary>
        /// <returns>
        /// An object representing the collection of all configuration files
        /// known to Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/Unj6fs"></a>GET 
        /// properties</a> endpoint/> to construct the <see cref=
        /// "ConfigurationCollection"/> it returns.
        /// </remarks>
        public Stanza GetConfigurationStanza(string configurationName, string stanzaName)
        {
            return this.GetConfigurationStanzaAsync(configurationName, stanzaName).Result;
        }

        /// <summary>
        /// Retrieves the collection of all configuration files known to 
        /// Splunk.
        /// </summary>
        /// <returns>
        /// An object representing the collection of all configuration files
        /// known to Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/Unj6fs"></a>GET 
        /// properties</a> endpoint/> to construct the <see cref=
        /// "ConfigurationCollection"/> it returns.
        /// </remarks>
        public async Task<Stanza> GetConfigurationStanzaAsync(string configurationName, string stanzaName)
        {
            var resourceName = new ResourceName(ResourceName.Properties, configurationName, stanzaName);
            var collection = new Stanza(this.Context, this.Namespace, resourceName);

            await collection.UpdateAsync();
            return collection;
        }

        #endregion

        #region Saved searches

        /// <summary>
        /// Creates a new <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="searchName">
        /// The name of the <see cref="SavedSearch"/> to dispatch.
        /// </param>
        /// <param name="searchArgs">
        /// A set of arguments to the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <returns>
        /// The search <see cref="Job"/> that was dispatched.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the <see 
        /// cref="SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public SavedSearch CreateSavedSearch(SavedSearchCreationArgs creationArgs, SavedSearchTemplateArgs templateArgs = null)
        {
            return this.CreateSavedSearchAsync(creationArgs, templateArgs).Result;
        }

        /// <summary>
        /// Creates a new <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="searchName">
        /// The name of the <see cref="SavedSearch"/> to dispatch.
        /// </param>
        /// <param name="searchArgs">
        /// A set of arguments to the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <returns>
        /// The search <see cref="Job"/> that was dispatched.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the <see 
        /// cref="SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task<SavedSearch> CreateSavedSearchAsync(SavedSearchCreationArgs creationArgs, SavedSearchTemplateArgs templateArgs = null)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, ResourceName.SavedSearches, creationArgs, templateArgs))
            {
                if (response.Message.StatusCode != HttpStatusCode.Created)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }

                var atomFeed = new AtomFeed();
                await atomFeed.ReadXmlAsync(response.XmlReader);

                if (atomFeed.Entries.Count != 1)
                {
                    throw new InvalidDataException();  // TODO: Diagnostics
                }

                var entity = SavedSearch.CreateEntity(this.Context, this.Namespace, ResourceName.SavedSearches, atomFeed.Entries[0]);
                return entity;
            }
        }

        /// <summary>
        /// Dispatches a <see cref="SavedSearch"/> just like the scheduler would.
        /// </summary>
        /// <param name="searchName">
        /// The name of the <see cref="SavedSearch"/> to dispatch.
        /// </param>
        /// <param name="searchArgs">
        /// A set of arguments to the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <returns>
        /// The search <see cref="Job"/> that was dispatched.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the <see 
        /// cref="SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public Job DispatchSavedSearch(string searchName, SavedSearchTemplateArgs searchArgs = null, SavedSearchDispatchArgs dispatchArgs = null)
        {
            return this.DispatchSavedSearchAsync(searchName, searchArgs, dispatchArgs).Result;
        }

        /// <summary>
        /// Dispatches a <see cref="SavedSearch"/> just like the scheduler would.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="SavedSearch"/> to dispatch.
        /// </param>
        /// <param name="searchArgs">
        /// A set of arguments to the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <returns>
        /// The search <see cref="Job"/> that was dispatched.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the <see 
        /// cref="SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task<Job> DispatchSavedSearchAsync(string name, SavedSearchTemplateArgs searchArgs = null, SavedSearchDispatchArgs dispatchArgs = null)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(name), "searchName");

            var resourceName = new ResourceName(ResourceName.SavedSearches, name, "dispatch");
            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, searchArgs, dispatchArgs))
            {
                switch (response.Message.StatusCode)
                {
                    case HttpStatusCode.Created:
                    case HttpStatusCode.OK:

                        searchId = await response.XmlReader.ReadResponseElementAsync("sid");
                        break;

                    default: throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }

            Job job = new Job(this.Context, this.Namespace, ResourceName.SearchJobs, name: searchId);
            await job.UpdateAsync();
            return job;
        }

        /// <summary>
        /// Gets the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be retrieved.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public SavedSearch GetSavedSearch(string name, SavedSearchArgs args = null)
        {
            return this.GetSavedSearchAsync(name, args).Result;
        }

        /// <summary>
        /// Gets the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be retrieved.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task<SavedSearch> GetSavedSearchAsync(string name, SavedSearchArgs args = null)
        {
            using (var response = await this.Context.GetAsync(this.Namespace, new ResourceName(ResourceName.SavedSearches, name)))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }

                var atomFeed = new AtomFeed();
                await atomFeed.ReadXmlAsync(response.XmlReader);

                if (atomFeed.Entries.Count != 1)
                {
                    throw new InvalidDataException();  // TODO: Diagnostics
                }

                var entity = SavedSearch.CreateEntity(this.Context, this.Namespace, ResourceName.SavedSearches, atomFeed.Entries[0]);
                return entity;
            }
        }

        /// <summary>
        /// Gets the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be retrieved.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public JobCollection GetSavedSearchHistory(string name)
        {
            return this.GetSavedSearchHistoryAsync(name).Result;
        }

        /// <summary>
        /// Gets the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be retrieved.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task<JobCollection> GetSavedSearchHistoryAsync(string name)
        {
            var resourceName = new ResourceName(ResourceName.SavedSearches, name, "history");
            var jobs = new JobCollection(this.Context, this.Namespace, resourceName);
            await jobs.UpdateAsync();
            return jobs;
        }

        /// <summary>
        /// Retrieves information on a collection of saved searches.
        /// </summary>
        /// <param name="args">
        /// Arguments identifying the collection of <see cref="SavedSearch"/>
        /// entries to return.
        /// </param>
        /// <returns>
        /// A new <see cref="SavedSearchCollection"/> containing the <see cref=
        /// "SavedSearch"/> entries identified by <see cref="args"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bKrRK0">GET 
        /// saved/searches</a> endpoint to retrieve a new <see cref=
        /// "SavedSearchCollection"/> containing the <see cref="SavedSearch"/> 
        /// entries identified by <see cref="args"/>.
        /// </remarks>
        public SavedSearchCollection GetSavedSearches(SavedSearchCollectionArgs args = null)
        {
            return this.GetSavedSearchesAsync(args).Result;
        }

        /// <summary>
        /// Retrieves information on a collection of saved searches.
        /// </summary>
        /// <param name="args">
        /// Arguments identifying the collection of <see cref="SavedSearch"/>
        /// entries to return.
        /// </param>
        /// <returns>
        /// A new <see cref="SavedSearchCollection"/> containing the <see cref=
        /// "SavedSearch"/> entries identified by <see cref="args"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bKrRK0">GET 
        /// saved/searches</a> endpoint to retrieve a new <see cref=
        /// "SavedSearchCollection"/> containing the <see cref="SavedSearch"/> 
        /// entries identified by <see cref="args"/>.
        /// </remarks>
        public async Task<SavedSearchCollection> GetSavedSearchesAsync(SavedSearchCollectionArgs args = null)
        {
            var collection = new SavedSearchCollection(this.Context, this.Namespace, args);
            await collection.UpdateAsync();
            return collection;
        }

        /// <summary>
        /// Removes the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be removed.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/0IrFbY">DELETE 
        /// saved/searches{name}</a> endpoint to remove the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public void RemoveSavedSearch(string name)
        {
            this.RemoveSavedSearchAsync(name).Wait();
        }

        /// <summary>
        /// Removes the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be removed.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/0IrFbY">DELETE 
        /// saved/searches{name}</a> endpoint to remove the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task RemoveSavedSearchAsync(string name)
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, new ResourceName(ResourceName.SavedSearches, name)))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        #endregion

        #region Search jobs

        /// <summary>
        /// Gets details about the search <see cref="Job"/> identified by
        /// <c>searchId</c>.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/X4smdW">search/jobs/{search_id}</a>
        /// REST API Reference.
        /// </remarks>
        public Job GetJob(string searchId)
        {
            return this.GetJobAsync(searchId).Result;
        }

        /// <summary>
        /// Gets details about the search <see cref="Job"/> identified by
        /// <c>searchId</c>.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/X4smdW">search/jobs/{search_id}</a>
        /// REST API Reference.
        /// </remarks>
        public async Task<Job> GetJobAsync(string searchId)
        {
            using (var response = await this.Context.GetAsync(this.Namespace, new ResourceName(ResourceName.SearchJobs, searchId)))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }

                var atomEntry = new AtomEntry();
                await atomEntry.ReadXmlAsync(response.XmlReader);
                var job = Job.CreateEntity(this.Context, this.Namespace, ResourceName.SearchJobs, atomEntry);

                return job;
            }
        }

        /// <summary>
        /// Retrieves the collection of all running search jobs.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/gf67qS">search/jobs</a> REST API Reference.
        /// </remarks>
        public JobCollection GetJobs(JobCollectionArgs args)
        {
            return this.GetJobsAsync(args).Result;
        }

        /// <summary>
        /// Retrieves the collection of all running search jobs.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/gf67qS">search/jobs</a> REST API Reference.
        /// </remarks>
        public async Task<JobCollection> GetJobsAsync(JobCollectionArgs args = null)
        {
            var jobs = new JobCollection(this.Context, this.Namespace, ResourceName.SearchJobs, args);
            await jobs.UpdateAsync();
            return jobs;
        }

        /// <summary>
        /// Removes the search <see cref="Job"/> identified by <c>searchId</c>.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/X4smdW">search/jobs/{search_id}</a>
        /// REST API Reference.
        /// </remarks>
        public void RemoveJob(string searchId)
        {
            this.RemoveJobAsync(searchId).Wait();
        }

        /// <summary>
        /// Removes the search <see cref="Job"/> identified by <c>searchId</c>.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/X4smdW">search/jobs/{search_id}</a>
        /// REST API Reference.
        /// </remarks>
        public async Task RemoveJobAsync(string searchId)
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, new ResourceName(ResourceName.SearchJobs, searchId)))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        /// <summary>
        /// Starts a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="search">
        /// The search language string to execute.
        /// </param>
        /// <param name="mode">
        /// The search <see cref="ExecutionMode"/>.
        /// </param>
        /// <returns>
        /// A new search <see cref="Job"/>.
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST API Reference.
        /// </remarks>
        public Job StartJob(string search, ExecutionMode mode = ExecutionMode.Normal)
        {
            return StartJob(new JobArgs(search) { ExecutionMode = mode });
        }

        /// <summary>
        /// Starts a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="args">
        /// Search <see cref="JobArgs"/> to pass when starting the search <see 
        /// cref="Job"/>.
        /// </param>
        /// <returns>
        /// A new search <see cref="Job"/>.
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST 
        /// API Reference.
        /// </remarks>
        public Job StartJob(JobArgs args)
        {
            return StartJobAsync(args).Result;
        }

        /// <summary>
        /// Starts a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="search">
        /// The search language string to execute.
        /// </param>
        /// The search <see cref="ExecutionMode"/>.
        /// <param name="mode">
        /// </param>
        /// <returns>
        /// A new search <see cref="Job"/>.
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST API Reference.
        /// </remarks>
        public async Task<Job> StartJobAsync(string search, ExecutionMode mode = ExecutionMode.Normal)
        {
            return await this.StartJobAsync(new JobArgs(search) { ExecutionMode = mode });
        }

        /// <summary>
        /// Starts a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="args">
        /// Search <see cref="JobArgs"/> to pass when starting the search <see 
        /// cref="Job"/>.
        /// </param>
        /// <returns>
        /// A new search <see cref="Job"/>.
        /// </returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST 
        /// API Reference.
        /// </remarks>
        public async Task<Job> StartJobAsync(JobArgs args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Requires<ArgumentNullException>(args.Search != null, "args.Search");
            Contract.Requires<ArgumentException>(args.ExecutionMode != ExecutionMode.Oneshot, "args.ExecutionMode: ExecutionMode.Oneshot");

            // FJR: Also check that it's not export, which also won't return a job.
            // DSN: JobArgs does not include SearchExportArgs

            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, ResourceName.SearchJobs, args))
            {
                if (response.Message.StatusCode != HttpStatusCode.Created)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
                searchId = await response.XmlReader.ReadResponseElementAsync("sid");
            }

            // FJR: Jobs need to be handled a little more delicately. Let's talk about the patterns here.
            // In the other SDKs, we've been doing functions to wait for ready and for done. Async means
            // that we can probably make that a little slicker, but let's talk about how.

            Job job = new Job(this.Context, this.Namespace, ResourceName.SearchJobs, name: searchId);
            await job.UpdateAsync();

            return job;
        }

        /// <summary>
        /// Updates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// </param>
        /// <param name="args">
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/8HjDNS">POST 
        /// search/jobs/{search_id}</a> to update the <see cref="JobArgs"/> for
        /// search <see cref="Job"/> identified by <see cref="searchId"/>.
        /// </remarks>
        public void UpdateJobArgs(string searchId, JobArgs args)
        {
            UpdateJobArgsAsync(searchId, args).Wait();
        }

        /// <summary>
        /// Updates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// </param>
        /// <param name="args">
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/8HjDNS">POST 
        /// search/jobs/{search_id}</a> to update the <see cref="JobArgs"/> for
        /// search <see cref="Job"/> identified by <see cref="searchId"/>.
        /// </remarks>
        public async Task UpdateJobArgsAsync(string searchId, JobArgs args)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, new ResourceName(ResourceName.SearchJobs, searchId), args))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        /// <summary>
        /// Creates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/vJvIXv">GET search/jobs/export</a> REST API Reference.
        /// </remarks>
        public async Task<SearchResultsReader> SearchExportAsync(string command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "command");
            return await this.SearchExportAsync(new SearchExportArgs(command));
        }

        /// <summary>
        /// Creates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/vJvIXv">GET search/jobs/export</a> REST API Reference.
        /// </remarks>
        public async Task<SearchResultsReader> SearchExportAsync(SearchExportArgs args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Response response = null;

            try
            {
                response = await this.Context.GetAsync(this.Namespace, ResourceName.SearchJobsExport, args);

                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }

                // FJR: We should probably return a stream here and keep the parsers separate. That lets someone
                // else plug in and use their own parser if they really want to. We don't particularly support the
                // scenario, but it doesn't block the user.

                // DSN: The search results reader is a stream of SearchResultSet objects. TODO: Explanation...

                return await SearchResultsReader.CreateAsync(response); // Transfers response ownership
            }
            catch
            {
                if (response != null)
                {
                    response.Dispose();
                }
                throw;
            }
        }

        /// <summary>
        /// Creates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST API Reference.
        /// </remarks>
        public async Task<SearchResults> SearchOneshotAsync(string command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "command");
            return await this.SearchOneshotAsync(new JobArgs(command));
        }

        /// <summary>
        /// Creates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST API Reference.
        /// </remarks>
        public async Task<SearchResults> SearchOneshotAsync(JobArgs args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            args.ExecutionMode = ExecutionMode.Oneshot;

            Response response = null;

            try
            {
                response = await this.Context.PostAsync(this.Namespace, ResourceName.SearchJobs, args);

                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }

                // FJR: Like export, we should probably return a stream instead of parsing it here.
                // DSN: The SearchResultsSet class is a stream of Record objects. TODO: Explain

                return await SearchResults.CreateAsync(response, leaveOpen: false); // Transfers response ownership
            }
            catch
            {
                if (response != null)
                {
                    response.Dispose();
                }
                throw;
            }
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Releases all resources used by the <see cref="Service"/>.
        /// </summary>
        /// <remarks>
        /// Do not override this method. Override 
        /// <see cref="Service.Dispose(bool disposing)"/> instead.
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Service"/>.
        /// </summary>
        /// <remarks>
        /// Subclasses should implement the disposable pattern as follows:
        /// <list type="bullet">
        /// <item><description>
        ///     Override this method and call it from the override.
        ///     </description></item>
        /// <item><description>
        ///     Provide a finalizer, if needed, and call this method from it.
        ///     </description></item>
        /// <item><description>
        ///     To help ensure that resources are always cleaned up 
        ///     appropriately, ensure that the override is callable multiple
        ///     times without throwing an exception.
        ///     </description></item>
        /// </list>
        /// There is no performance benefit in overriding this method on types
        /// that use only managed resources (such as arrays) because they are 
        /// automatically reclaimed by the garbage collector. See 
        /// <a href="http://goo.gl/VPIovn">Implementing a Dispose Method</a>.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.Context.Dispose();
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Gets the URI string for this <see cref="Service"/> instance. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString());
        }

        #endregion

#if false
. Say
    ////
    // Returns a +Collection+ of all the configuration files visible on Splunk.
    //
    // The configurations you see are dependent on the namespace your +Service+
    // is connected with. So if you are connected in the system namespace, you
    // may see different values than if you're connected in the app namespace
    // associated with a particular app, since the app may override some values
    // within its scope.
    //
    // The configuration files which are the contents of this +Collection+ are
    // not +Entity+ objects, but +Collection+ objects in their own right. They
    // contain +Entity+ objects representing the stanzas in that configuration
    // file.
    //
    // Returns: +Configurations+ (a subclass of +Collection+ containing
    // +ConfigurationFile+ objects).
    //
    public void Getconfs
      Configurations.new(self)
    end

    ////
    // Creates an asynchronous search job.
    //
    // The search job requires a _query_, and takes a hash of other, optional
    // arguments, which are documented in the {Splunk REST documentation}[http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTsearch//search.2Fjobs].
    //
    public void Getcreate_search(query, args={})
      jobs.create(query, args)
    end

    ////
    // Creates a blocking search without transforming search commands.
    //
    // The +create_export+ method starts a search _query_, and any optional 
    // arguments specified in a hash (which are identical to those taken by 
    // the +create+ methods). It then blocks until the job is finished, and 
    // returns the events found by the job before any transforming search 
    // commands (equivalent to calling +events+ on a +Job+).
    //
    // Returns: a stream readable by +MultiResultsReader+.
    //
    public void Getcreate_export(query, args={}) // :nodoc:
      jobs.create_export(query, args)
    end

    // DEPRECATED. Use create_export instead.
    public void Getcreate_stream(query, args={})
      warn "[DEPRECATION] Service//create_stream is deprecated. Use Service//create_export instead."
      jobs.create_export(query, args)
    end

    ////
    // Returns a +Collection+ of all +Index+ objects.
    //
    // +Index+ is a subclass of +Entity+, with additional methods for
    // manipulating indexes in particular.
    //
    public void Getindexes
      Collection.new(self, PATH_INDEXES, entity_class=Index)
    end

    ////
    // Returns a +Hash+ containing Splunk's runtime information.
    //
    // The +Hash+ has keys such as "+build+" (the number of the build of this
    // Splunk instance) and "+cpu_arch+" (what CPU Splunk is running on), and
    // "+os_name+" (the name of the operating system Splunk is running on).
    //
    // Returns: A +Hash+ that has +Strings+ as both keys and values.
    //
    public void Getinfo
      response = request(:namespace => Splunk::namespace(:sharing => "default"),
                         :resource => PATH_INFO)
      feed = AtomFeed.new(response.body)
      feed.entries[0]["content"]
    end

    ////
    // Return a collection of the input kinds.
    //
    public void Getinputs
      InputKinds.new(self, PATH_INPUTS)
    end

    ////
    // Returns a collection of the loggers in Splunk.
    //
    // Each logger logs errors, warnings, debug info, or informational
    // information about a specific part of the Splunk system (e.g.,
    // +WARN+ on +DeploymentClient+).
    //
    // Returns: a +Collection+ of +Entity+ objects representing loggers.
    //
    // *Example*:
    //     service = Splunk::connect(:username => 'admin', :password => 'foo')
    //     service.loggers.each do |logger|
    //       puts logger.name + ":" + logger['level']
    //     end
    //     // Prints:
    //     //   ...
    //     //   DedupProcessor:WARN
    //     //   DeployedApplication:INFO
    //     //   DeployedServerClass:WARN
    //     //   DeploymentClient:WARN
    //     //   DeploymentClientAdminHandler:WARN
    //     //   DeploymentMetrics:INFO
    //     //   ...
    //
    public void Getloggers
      Collection.new(self, PATH_LOGGER)
    end

    ////
    // Returns a collection of the global messages on Splunk.
    //
    // Messages include such things as warnings and notices that Splunk needs to
    // restart.
    //
    // Returns: A +Collection+ of +Message+ objects (which are subclasses 
    // of +Entity+).
    //
    public void Getmessages
      Messages.new(self, PATH_MESSAGES, entity_class=Message)
    end

    ////
    // Returns a read only collection of modular input kinds.
    //
    // The modular input kinds are custom input kinds on this Splunk instance.
    // To access the actual inputs of these kinds, use the +Service+//+inputs+
    // method. This method gives access to the metadata describing the input
    // kinds.
    //
    // Returns: A +ReadOnlyCollection+ of +ModularInputKind+ objects representing
    // all the custom input types added to this Splunk instance.
    //
    public void Getmodular_input_kinds
      if self.splunk_version[0] < 5
        raise IllegalOperation.new("Modular input kinds are " +
                                       "not supported before Splunk 5.0")
      else
        ReadOnlyCollection.new(self, PATH_MODULAR_INPUT_KINDS,
                               entity_class=ModularInputKind)
      end
    end

    ////
    // Returns a collection of the roles on the system.
    //
    // Returns: A +Collection+ of +Entity+ objects representing the roles on
    // this Splunk instance.
    //
    public void Getroles
      CaseInsensitiveCollection.new(self, PATH_ROLES)
    end

    ////
    //
    //
    //
    public void Getsaved_searches
      Collection.new(self, PATH_SAVED_SEARCHES, entity_class=SavedSearch)
    end

    ////
    // Returns an +Entity+ of Splunk's mutable runtime information.
    //
    // The +settings+ method includes values such as "+SPLUNK_DB+" 
    // and "+SPLUNK_HOME+". Unlike the values returned by the +info+ method, 
    // these settings can be updated.
    //
    // Returns: an +Entity+ with all server settings.
    //
    // *Example*:
    //     service = Splunk::connect(:username => 'admin', :password => 'foo')
    //     puts svc.settings.read
    //     // Prints:
    //     //    {"SPLUNK_DB" => "/path/to/splunk_home/var/lib/splunk",
    //     //     "SPLUNK_HOME" => "/path/to/splunk_home",
    //     //     ...}
    //
    public void Getsettings
      // Though settings looks like a collection on the server, it always has
      // a single entity, of the same name, giving the actual settings. We access
      // that entity directly rather than putting a collection inbetween.
      Entity.new(self, Splunk::namespace(:sharing => "default"),
                 PATH_SETTINGS, "settings").refresh()
    end

    ////
    // Returns the version of Splunk this +Service+ is connected to.
    //
    // The version is represented as an +Array+ of length 3, with each
    // of its components an integer. For example, on Splunk 4.3.5,
    // +splunk_version+ would return [+4+, +3+, +5+], while on Splunk 5.0.2,
    // +splunk_version+ would return [+5+, +0+, +2+].
    //
    // Returns: An +Array+ of +Integers+ of length 3.
    //
    public void Getsplunk_version
      info["version"].split(".").map() {|v| Integer(v)}
    end

    ////
    // Return a +Collection+ of the users defined on Splunk.
    //
    public void Getusers
      CaseInsensitiveCollection.new(self, PATH_USERS)
    end

#endif
        #endregion

        #region Privates

        bool disposed;

        #endregion
    }
}
