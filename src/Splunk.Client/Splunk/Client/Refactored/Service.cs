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

//// TODO:
//// [O] Contracts
//// [O] Documentation (check all links!)
//// [ ] Consider schema validation from schemas stored as resources.
////     See [XmlReaderSettings.Schemas Property](http://goo.gl/Syvj4V)
//// [ ] Strong type and tests for Service.GetCapabilities


namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class Service : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="context">
        /// The context for requests by the new <see cref="Service"/>.
        /// </param>
        /// <param name="ns">
        /// The namespace for requests by the new <see cref="Service"/>. The
        /// default value is <c>null</c> indicating that <see cref=
        /// "Namespace"/>.Default should be used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public Service(Context context, Namespace ns = null)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            this.context = context;
            this.ns = ns ?? Namespace.Default;

            this.storagePasswords = new StoragePasswordCollection(this);
            this.configurations = new ConfigurationCollection(this);
            this.applications = new ApplicationCollection(this);
            this.indexes = new IndexCollection(this);
            this.transmitter = new Transmitter(this);
            this.server = new Server(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="scheme">
        /// The scheme for the new <see cref="Service"/>.
        /// </param>
        /// <param name="host">
        /// The host for the new <see cref="Service"/>.
        /// </param>
        /// <param name="port">
        /// The port for the new <see cref="Service"/>.
        /// </param>
        /// <param name="ns">
        /// The namespace for requests issue by the new <see cref="Service"/>.
        /// </param>
        /// <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>
        public Service(Scheme scheme, string host, int port, Namespace ns = null)
            : this(new Context(scheme, host, port), ns)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Service"/>.
        /// </summary>
        protected internal Context Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the <see cref="Namespace"/> used by this <see cref="Service"/>.
        /// </summary>
        public Namespace Namespace
        {
            get { return this.ns; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationCollection Applications
        {
            get { return this.applications; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationCollection Configurations
        {
            get { return this.configurations; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationCollection Indexes
        {
            get { return this.configurations; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Server Server
        {
            get { return this.server; }
        }

        /// <summary>
        /// Gets or sets the session key used by this <see cref="Service"/>.
        /// </summary>
        public string SessionKey
        {
            get { return this.Context.SessionKey; }
            set { this.Context.SessionKey = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public StoragePasswordCollection StoragePasswords
        {
            get { return this.StoragePasswords; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Transmitter Transmitter
        {
            get { return this.transmitter; }
        }

        #endregion

        #region Methods

        #region Access control

        /// <summary>
        /// Asynchronously retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns>
        /// An object representing the list of all Splunk system capabilities.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kgTKvM">GET 
        /// authorization/capabilities</a> endpoint to construct a list of all 
        /// Splunk system capabilities.
        /// </remarks>
        public async Task<dynamic> GetCapabilitiesAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, AuthorizationCapabilities))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count != 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics : cardinality violation
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
            Contract.Requires<ArgumentNullException>(username != null);
            Contract.Requires<ArgumentNullException>(password != null);

            using (var response = await this.Context.PostAsync(Namespace.Default, AuthLogin, new Argument[]
            {
                new Argument("username", username),
                new Argument("password", password)
            }))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                this.SessionKey = await response.XmlReader.ReadResponseElementAsync("sessionKey");
            }
        }

        /// <summary>
        /// Ends the session by associated with the current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">DELETE 
        /// authentication/httpauth-tokens/{name}</a> endpoint to end the
        /// the session by removing <see cref="SessionKey"/>.
        /// </remarks>
        public async Task LogoffAsync()
        {
            Contract.Requires<InvalidOperationException>(this.SessionKey != null);

            var resourceName = new ResourceName(AuthenticationHttpAuthTokens, this.SessionKey);

            using (var response = await this.Context.DeleteAsync(Namespace.Default, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                this.SessionKey = null;
            }
        }

        /// <summary>
        /// Asynchronously removes a storage password.
        /// </summary>
        /// <param name="name">
        /// Storage password name.
        /// </param>
        /// <param name="realm">
        /// Storage password realm.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JGm0JP">DELETE 
        /// storage/passwords/{name}</a> endpoint to remove the <see cref=
        /// "StoragePassword"/> identified by <paramref name="name"/>.
        /// </remarks>
        public async Task RemoveStoragePasswordAsync(string name, string realm = null)
        {
            var resource = new StoragePassword(this.Context, this.Namespace, name, realm);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates a storage password.
        /// </summary>
        /// <param name="password">
        /// Storage password.
        /// </param>
        /// <param name="name">
        /// Storage password name.
        /// </param>
        /// <param name="realm">
        /// Storage password realm.
        /// </param>
        /// <returns>
        /// An object representing the updated storage password.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/HL3c0T">POST 
        /// storage/passwords/{name}</a> endpoint to update the storage
        /// password identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<StoragePassword> UpdateStoragePasswordAsync(string password, string name, string realm = null)
        {
            var resource = new StoragePassword(this.Context, this.Namespace, name, realm);
            await resource.UpdateAsync(password);
            return resource;
        }

        #endregion

        #region Saved searches

        /// <summary>
        /// Asynchronously creates a new saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the saved search to be created.
        /// </param>
        /// <param name="search">
        /// A Splunk search command.
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
        public async Task<SavedSearch> CreateSavedSearchAsync(string name, string search, 
            SavedSearchAttributes attributes = null, SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.CreateAsync(search, attributes, dispatchArgs, templateArgs);
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
        /// cref="SavedSearch"/> identified by <paramref name="name"/>.
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
        /// "SavedSearch"/> identified by <paramref name="name"/>.
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
        /// "SavedSearch"/> entries identified by <paramref name="args"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bKrRK0">GET 
        /// saved/searches</a> endpoint to retrieve a new <see cref=
        /// "SavedSearchCollection"/> containing the <see cref="SavedSearch"/> 
        /// entries identified by <paramref name="args"/>.
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
        /// identified by <paramref name="name"/>.
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
        /// identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<SavedSearch> UpdateSavedSearchAsync(string name, SavedSearchAttributes attributes = null, 
            SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
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
        /// saved search identified by <paramref name="name"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kv9L1l">GET 
        /// saved/searches/{name}/history</a> endpoint to get the collection of
        /// jobs created from the <see cref="SavedSearch"/> identified by <paramref 
        /// name="name"/>.
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
        /// Asynchronously creates a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="search">
        /// Search string.
        /// </param>
        /// <param name="args">
        /// Optional search arguments.
        /// </param>
        /// <param name="customArgs">
        /// 
        /// </param>
        /// <param name="requiredState">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the search job that was created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JZcPEb">POST 
        /// search/jobs</a> endpoint to start a new search <see cref="Job"/> as
        /// specified by <paramref name="args"/>.
        /// </remarks>
        public async Task<Job> CreateJobAsync(string search, JobArgs args = null, CustomJobArgs customArgs = null,
            DispatchState requiredState = DispatchState.Running)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Requires<ArgumentOutOfRangeException>(args == null || args.ExecutionMode != ExecutionMode.Oneshot);

            //// FJR: Also check that it's not export, which also won't return a job.
            //// DSN: JobArgs does not include SearchExportArgs

            var resourceName = JobCollection.ClassResourceName;

            var command = new Argument[] 
            {
               new Argument("search", search)
            };

            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, command, args, customArgs))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                searchId = await response.XmlReader.ReadResponseElementAsync("sid");
            }

            //// FJR: Jobs need to be handled a little more delicately. Let's talk about the patterns here.
            //// In the other SDKs, we've been doing functions to wait for ready and for done. Async means
            //// that we can probably make that a little slicker, but let's talk about how.

            Job job = new Job(this.Context, this.Namespace, name: searchId);

            await job.GetAsync();
            await job.TransitionAsync(requiredState);

            return job;
        }

        /// <summary>
        /// Asynchronously retrieves information about a search job.
        /// </summary>
        /// <param name="searchId">
        /// ID of a search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SFqSPI">GET 
        /// search/jobs/{search_id}</a> endpoint to get the <see cref="Job"/> 
        /// identified by <paramref name="searchId"/>.
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
        /// search/jobs</a> endpoint to get the <see cref="JobCollection"/>
        /// specified by <paramref name="args"/>.
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
        /// identified by <paramref name="searchId"/>.
        /// </remarks>
        public async Task RemoveJobAsync(string searchId)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Executes a oneshot search.
        /// </summary>
        /// <param name="search">
        /// Search string.
        /// </param>
        /// <param name="args">
        /// Optional job arguments.
        /// </param>
        /// <returns>
        /// An object representing the stream search results.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/b02g1d">POST 
        /// search/jobs</a> endpoint to execute a oneshot search.
        /// </remarks>
        public async Task<SearchResultStream> SearchOneshotAsync(string search, JobArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);

            var resourceName = JobCollection.ClassResourceName;

            var command = new Argument[]
            {
                new Argument("search", search),
                new Argument("exec_mode", "oneshot")
            };

            if (args != null)
            {
                args.ExecutionMode = null;
            }

            Response response = await this.Context.PostAsync(this.Namespace, resourceName, command, args);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                var stream = await SearchResultStream.CreateAsync(response); // Transfers response ownership
                return stream;
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously exports an observable sequence of search result previews.
        /// </summary>
        /// <param name="search">
        /// Splunk search command.
        /// </param>
        /// <param name="args">
        /// Optional export arguments.
        /// </param>
        /// <returns>
        /// An object representing an observable sequence of search result 
        /// previews.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/vJvIXv">GET 
        /// search/jobs/export</a> endpoint to start the 
        /// </remarks>
        public async Task<SearchPreviewStream> ExportSearchPreviewsAsync(string search, SearchExportArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");

            var command = new Argument[] 
            { 
                new Argument("search", search) 
            };

            var response = await this.Context.GetAsync(this.Namespace, SearchJobsExport, command, args);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                return new SearchPreviewStream(response); // Transfers response ownership
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously exports an observable sequence of search results.
        /// </summary>
        /// <param name="search">
        /// Splunk search command.
        /// </param>
        /// <param name="args">
        /// Optional export arguments.
        /// </param>
        /// <returns>
        /// An object representing an observable sequence of search result 
        /// previews.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/vJvIXv">GET 
        /// search/jobs/export</a> endpoint to start the 
        /// </remarks>
        public async Task<SearchResultStream> ExportSearchResultsAsync(string search, SearchExportArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");

            var command = new Argument[] 
            { 
                new Argument("search", search) 
            };

            var response = await this.Context.GetAsync(this.Namespace, SearchJobsExport, command, args);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                return await SearchResultStream.CreateAsync(response); // Transfers response ownership
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously updates custom arguments to a search <see cref=
        /// "Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// ID of the search <see cref="Job"/> to be updated.
        /// </param>
        /// <param name="args">
        /// New custom arguments to the search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bL4tFk">POST 
        /// search/jobs/{search_id}</a> to update the specificiation of
        /// search <see cref="Job"/> identified by <paramref name="searchId"/>.
        /// </remarks>
        public async Task UpdateJobAsync(string searchId, CustomJobArgs args)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.UpdateAsync(args);
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Releases all resources used by the <see cref="Service"/>.
        /// </summary>
        /// <remarks>
        /// Do not override this method. Override <see cref="Service.Dispose(bool)"/> 
        /// instead.
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

        #region Privates/internals

        static readonly ResourceName AuthLogin = new ResourceName("auth", "login");
        static readonly ResourceName AuthenticationHttpAuthTokens = new ResourceName("authentication", "httpauth-tokens");
        static readonly ResourceName AuthorizationCapabilities = new ResourceName("authorization", "capabilities");
        static readonly ResourceName SearchJobsExport = new ResourceName("search", "jobs", "export");
        
        readonly Context context;
        readonly Namespace ns;

        readonly StoragePasswordCollection storagePasswords;
        readonly ConfigurationCollection configurations;
        readonly ApplicationCollection applications;
        readonly IndexCollection indexes;
        readonly Transmitter transmitter;
        readonly Server server;

        bool disposed;

        #endregion
    }
}
