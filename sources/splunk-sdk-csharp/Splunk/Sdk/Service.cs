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
        /// <param name="scheme">
        /// </param>
        /// <param name="host">
        /// </param>
        /// <param name="port">
        /// </param>
        /// <param name="sessionKey">
        /// </param>
        /// <param name="namespace">
        /// </param>
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

        #region Access Control

        /// <summary>
        /// Asynchronously retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kgTKvM">GET 
        /// authorization/capabilities</a> endpoint to construct a 
        /// of available capabilities.
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
        /// Provides user authentication asynchronously.
        /// </summary>
        /// <param name="username">
        /// Splunk account username.
        /// </param>
        /// <param name="password">
        /// Splunk account password for username.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">POST 
        /// auth/login</a> endpoint. The session key this endpoint returns is 
        /// used for subsequent requests. It is accessible via the <see cref=
        /// "SessionKey"/> property.
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
        /// Asynchronously retrieves a collection of installed applications.
        /// </summary>
        /// <param name="args">
        /// Specification of the collection of applications to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the collection of installed applications
        /// specified by <see cref="args"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/iiCmcY">GET apps/local</a> 
        /// endpoint to construct the <see cref="ApplicationCollection"/> object
        /// it returns.
        /// </remarks>
        public async Task<ApplicationCollection> GetApplicationsAsync(ApplicationCollectionArgs args = null)
        {
            var collection = new ApplicationCollection(this.Context, this.Namespace, args);
            await collection.GetAsync();
            return collection;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Asynchronously creates a new configuration file.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration file to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7">POST 
        /// properties</a> endpoint to create the <see cref="Configuration"/>
        /// identified by <see cref="name"/>.
        /// </remarks>
        public async Task<Configuration> CreateConfigurationAsync(string name)
        {
            var entity = new Configuration(this.Context, this.Namespace, name);
            await entity.CreateAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously creates a new stanza in a configuration file.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="fileName">
        /// Name of the configuration stanza to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST 
        /// properties/{file_name></a> endpoint to create the <see cref=
        /// "ConfigurationStanza"/> identified by <see cref="fileName"/> and
        /// <see cref="stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationStanza> CreateConfigurationAsync(string fileName, string stanzaName)
        {
            var entity = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await entity.CreateAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves a configuration file.
        /// </summary>
        /// <param name="name">
        /// The name of a configuration file.
        /// </param>
        /// <returns>
        /// An object representing the configuration file.
        /// <see cref="name"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JNbGtL">GET 
        /// properties/{file_name}</a> endpoint/> to construct the <see cref=
        /// "Configuration"/> it returns.
        /// </remarks>
        public async Task<Configuration> GetConfigurationAsync(string name)
        {
            var entity = new Configuration(this.Context, this.Namespace, name);
            await entity.GetAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves the collection of all configuration files 
        /// known to Splunk.
        /// </summary>
        /// <returns>
        /// An object representing the collection of all configuration files
        /// known to Splunk.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/Unj6fs">GET 
        /// properties</a> endpoint/> to construct the <see cref=
        /// "ConfigurationCollection"/> it returns.
        /// </remarks>
        public async Task<ConfigurationCollection> GetConfigurationsAsync()
        {
            var collection = new ConfigurationCollection(this.Context, this.Namespace);
            await collection.GetAsync();
            return collection;
        }

        /// <summary>
        /// Asynchronously retrieves a single configuration setting.
        /// </summary>
        /// <returns>
        /// An object representing the configuration setting.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/1jeyog">GET 
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint/> to 
        /// construct the <see cref="ConfigurationSetting"/> it returns.
        /// </remarks>
        public async Task<ConfigurationSetting> GetConfigurationSettingAsync(string fileName, string stanzaName, string keyName)
        {
            var entity = new ConfigurationSetting(this.Context, this.Namespace, fileName, stanzaName, keyName);
            await entity.GetAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves a configuration stanza.
        /// </summary>
        /// <returns>
        /// An object representing the configuration stanza.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sM63fa">GET 
        /// properties/{file_name}/{stanza_name}</a> endpoint/> to construct
        /// the <see cref="ConfigurationStanza"/> it returns.
        /// </remarks>
        public async Task<ConfigurationStanza> GetConfigurationStanzaAsync(string fileName, string stanzaName)
        {
            var collection = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await collection.GetAsync();
            return collection;
        }

        /// <summary>
        /// Asynchronously removes a configuration stanza.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in <see cref="fileName"/> to be
        /// removed.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// identified by <see cref="stanzaName"/>.
        /// </remarks>
        public async Task RemoveConfigurationStanzaAsync(string fileName, string stanzaName)
        {
            var entity = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await entity.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates an existing configuration setting.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a configuration stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the configuration setting to update.
        /// </param>
        /// <param name="value">
        /// A new <see cref="string"/> value for the configuration setting
        /// identified by <see cref="fileName"/>, <see cref="stanzaName"/>,
        /// and <see cref="keyName"/>.
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sSzcMy">POST 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// update the configuration setting identified by <see cref=
        /// "fileName"/>, <see cref="stanzaName"/>, and <see cref="keyName"/>.
        /// </remarks>
        public async Task UpdateConfigurationSettingAsync(string fileName, string stanzaName, string keyName, string value)
        {
            ConfigurationSetting setting = new ConfigurationSetting(this.Context, this.Namespace, fileName, stanzaName, keyName);
            await setting.UpdateAsync(value);
        }

        /// <summary>
        /// Adds or updates a list of settings in a configuration stanza.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within the configuration file identified by <see 
        /// cref="fileName"/>.
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update <see 
        /// cref="settings"/> in the stanza identified by <see cref=
        /// "fileName"/> and <see cref="stanzaName"/>.
        /// </remarks>
        public async Task UpdateConfigurationSettingsAsync(string fileName, string stanzaName, params Argument[] settings)
        {
            ConfigurationStanza stanza = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await stanza.UpdateAsync(settings);
        }

        #endregion

        #region Indexes

        /// <summary>
        /// Asynchronously creates a new index.
        /// </summary>
        /// <param name="name">
        /// Name of the index to create.
        /// </param>
        /// <param name="args">
        /// Specification of the index create.
        /// </param>
        /// <param name="attributes">
        /// Attributes to set on the newly created index.
        /// </param>
        /// <returns>
        /// An object representing the newly created index.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/yDfQ4T">POST
        /// data/indexes</a> endpoint to create the <see cref="Index"/> object
        /// it returns.
        /// </remarks>
        public async Task<Index> CreateIndexAsync(string name, IndexArgs args, IndexAttributes attributes = null)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.CreateAsync(args, attributes);
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves an <see cref="Index"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the index to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the named index.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/xljxjD">GET
        /// data/indexes/{name}</a> endpoint to construct the <see cref=
        /// "Index"/> object it returns.
        /// </remarks>
        public async Task<Index> GetIndexAsync(string name)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.GetAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of indexes.
        /// </summary>
        /// <param name="args">
        /// Specification of the collection of indexes to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the collection of indexes retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/qVZ6wJ">GET
        /// data/indexes</a> endpoint to construct the <see cref=
        /// "IndexCollection"/> object it returns.
        /// </remarks>
        public async Task<IndexCollection> GetIndexesAsync(IndexCollectionArgs args = null)
        {
            var collection = new IndexCollection(this.Context, this.Namespace);
            await collection.GetAsync();
            return collection;
        }

        /// <summary>
        /// Asynchronously removes an <see cref="Index"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the index to remove.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hCc1xe">DELETE
        /// data/indexes/{name}</a> endpoint to remove the <see cref=
        /// "Index"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task RemoveIndexAsync(string name)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates an index.
        /// </summary>
        /// <param name="name">
        /// Name of the index to update.
        /// </param>
        /// <param name="attributes">
        /// Attributes to set on the index.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/n3S22S">POST
        /// data/indexes/{name}</a> endpoint to update the <see cref=
        /// "Index"/> identified by <see cref="name"/> with a new set of <see 
        /// cref="attributes"/>.
        /// </remarks>
        public async Task UpdateIndexAsync(string name, IndexAttributes attributes)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.UpdateAsync(attributes);
        }

        #endregion

        #region Saved searches

        /// <summary>
        /// Asynchronously creates a new saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the saved search to be created.
        /// </param>
        /// <param name="attributes">
        /// Attributes of the saved search to be created.
        /// </param>
        /// <param name="dispatchArgs">
        /// Dispatch arguments for the saved search to be created.
        /// </param>
        /// <param name="templateArgs">
        /// Template arguments for the saved search to be created.
        /// </param>
        /// <returns>
        /// An object representing the saved search that was created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/EPQypw">POST 
        /// saved/searches</a> endpoint to create the <see cref="SavedSearch"/>
        /// object it returns.
        /// </remarks>
        public async Task<SavedSearch> CreateSavedSearchAsync(string name, SavedSearchAttributes attributes, 
            SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.CreateAsync(attributes, dispatchArgs, templateArgs);
            return resource;
        }

        /// <summary>
        /// Asynchronously dispatches a <see cref="SavedSearch"/> just like the
        /// scheduler would.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="SavedSearch"/> to dispatch.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <param name="templateArgs">
        /// A set of template arguments to the <see cref="SavedSearch"/>.
        /// </param>
        /// <returns>
        /// The search <see cref="Job"/> that was dispatched.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the <see 
        /// cref="SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task<Job> DispatchSavedSearchAsync(string name, SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            var savedSearch = new SavedSearch(this.Context, this.Namespace, name);
            var job = await savedSearch.DispatchAsync(dispatchArgs, templateArgs);
            return job;
        }

        /// <summary>
        /// Asynchronously retrieves the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be retrieved.
        /// </param>
        /// <param name="args">
        /// Constrains the information returned about the <see cref=
        /// "SavedSearch"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get the <see cref=
        /// "SavedSearch"/> identified by <see cref="name"/>.
        /// </remarks>
        public async Task<SavedSearch> GetSavedSearchAsync(string name, SavedSearchFilterArgs args = null)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.GetAsync(args);
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of saved searches.
        /// </summary>
        /// <param name="args">
        /// Arguments identifying the collection of <see cref="SavedSearch"/>
        /// entries to retrieve.
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
            var resource = new SavedSearchCollection(this.Context, this.Namespace, args);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously removes a saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the saved search to be removed.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sn7qC5">DELETE 
        /// saved/searches/{name}</a> endpoint to remove the saved search
        /// identified by <see cref="name"/>.
        /// </remarks>
        public async Task RemoveSavedSearchAsync(string name)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates a saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be updated.
        /// </param>
        /// <param name="name">
        /// Name of the saved search to be updated.
        /// </param>
        /// <param name="attributes">
        /// New attributes for the saved search to be updated.
        /// </param>
        /// <param name="dispatchArgs">
        /// New dispatch arguments for the saved search to be updated.
        /// </param>
        /// <param name="templateArgs">
        /// New template arguments for the saved search to be updated.
        /// </param>
        /// <returns>
        /// An object representing the saved search that was updated.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/aV9eiZ">POST 
        /// saved/searches/{name}</a> endpoint to update the saved search
        /// identified by <see cref="name"/>.
        /// </remarks>
        public async Task<SavedSearch> UpdateSavedSearchAsync(string name, SavedSearchAttributes attributes, SavedSearchDispatchArgs 
            dispatchArgs, SavedSearchTemplateArgs templateArgs)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.UpdateAsync(attributes, dispatchArgs, templateArgs);
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves the collection of jobs created from a
        /// saved search.
        /// </summary>
        /// <param name="name">
        /// Name of a saved search.
        /// </param>
        /// <returns>
        /// An object representing the collection of jobs created from the
        /// saved search identified by <see cref="name"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kv9L1l">GET 
        /// saved/searches/{name}/history</a> endpoint to get the collection of
        /// jobs created from the <see cref="SavedSearch"/> identified by <see 
        /// cref="name"/>.
        /// </remarks>
        public async Task<JobCollection> GetSavedSearchHistoryAsync(string name)
        {
            var savedSearch = new SavedSearch(this.Context, this.Namespace, name);
            var jobs = await savedSearch.GetHistoryAsync();
            return jobs;
        }

        #endregion

        #region Search jobs

        /// <summary>
        /// Asynchronously retrieves information about a search job.
        /// </summary>
        /// <param name="searchId">
        /// ID of a search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SFqSPI">GET 
        /// search/jobs/{search_id}</a> endpoint to get the <see cref="Job"/> 
        /// identified by identified by <see cref="searchId"/>.
        /// </remarks>
        public async Task<Job> GetJobAsync(string searchId)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of running search jobs.
        /// </summary>
        /// <param name="args">
        /// Specification of the collection of running search jobs to retrieve.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/ja2Sev">GET 
        /// search/jobs</a> endpoint to get the <see cref="JobCollelction"/> 
        /// specified by <see cref="args"/>.
        /// </remarks>
        public async Task<JobCollection> GetJobsAsync(JobCollectionArgs args = null)
        {
            var jobs = new JobCollection(this.Context, this.Namespace, args);
            await jobs.GetAsync();
            return jobs;
        }

        /// <summary>
        /// Asynchronously removes a search <see cref="Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// ID of a search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/TUqQUc">DELETE 
        /// search/jobs/{search_id}</a> endpoint to remove the <see cref="Job"/> 
        /// identified by <see cref="searchId"/>.
        /// </remarks>
        public async Task RemoveJobAsync(string searchId)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously starts a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="search">
        /// The search language string to execute.
        /// </param>
        /// <param name="mode">
        /// The search <see cref="ExecutionMode"/>. A value of <see cref=
        /// "ExecutionMode.Normal"/> or <see cref="ExecutionMode.Blocking"/> is
        /// required. The default value is <see cref="ExecutionMode.Normal"/>.
        /// <param name="mode">
        /// </param>
        /// <returns>
        /// An object representing the search job that was started.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JZcPEb">POST 
        /// search/jobs</a> endpoint to start a job to execute the specified
        /// <see cref="search"/> language string in <see cref="mode"/>.
        /// </remarks>
        public async Task<Job> StartJobAsync(string search, ExecutionMode mode = ExecutionMode.Normal)
        {
            return await this.StartJobAsync(new JobArgs(search) { ExecutionMode = mode });
        }

        /// <summary>
        /// Asynchronously starts a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="args">
        /// Specification of a search job.
        /// </param>
        /// <returns>
        /// An object representing the search job that was started.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JZcPEb">POST 
        /// search/jobs</a> endpoint to start a new search <see cref="Job"/> as
        /// specified by <see cref="args"/>.
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

            Job job = new Job(this.Context, this.Namespace, name: searchId);
            await job.GetAsync();

            return job;
        }

        /// <summary>
        /// Asynchronously updates the specification of a search <see cref="Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// ID of a search <see cref="Job"/>.
        /// </param>
        /// <param name="args">
        /// New specification of the search job.
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

        #endregion

        #region Privates

        bool disposed;

        #endregion
    }
}
