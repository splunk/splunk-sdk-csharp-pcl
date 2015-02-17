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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to the Splunk REST API offered on a Splunk management port.
    /// </summary>
    /// <seealso cref="T:System.IDisposable"/>
    /// <seealso cref="T:Splunk.Client.IService"/>
    public class Service : IDisposable, IService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="context">
        /// The context for requests by the new <see cref="Service"/>.
        /// </param>
        /// <param name="ns">
        /// The namespace for requests by the new <see cref="Service"/>. The default
        /// value is <c>null</c> indicating that <see cref= "Namespace"/>.Default
        /// should be used.
        /// </param>
        ///
        /// ### <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public Service(Context context, Namespace ns = null)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            this.context = context;
            this.ns = ns ?? Namespace.Default;

            this.storagePasswords = new StoragePasswordCollection(this);
            this.configurations = new ConfigurationCollection(this);
            this.savedSearches = new SavedSearchCollection(this);
            this.applications = new ApplicationCollection(this);
            this.indexes = new IndexCollection(this);
            this.transmitter = new Transmitter(this);
            this.jobs = new JobCollection(this);
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
        ///
        /// ### <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>
        public Service(Scheme scheme, string host, int port, Namespace ns = null)
            : this(new Context(scheme, host, port), ns)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="uri">
        /// Uri for the service <see cref="Service"/>.
        /// </param>
        /// <param name="ns">
        /// The namespace for requests issue by the new <see cref="Service"/>.
        /// </param>
        /// <exception name="ArgumentNullException">
        /// <paramref name="uri"/> is null.
        /// </exception>
        /// <exception name="ArgumentException">
        /// The <paramref name="uri"/> scheme is not http or https or the <paramref name="uri"/> port number is less than
        /// zero or greater than 65,535.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="uri"/> represents a relative URI, and this constructor can only be used with absolute URIs.
        /// </exception>
        public Service(Uri uri, Namespace ns = null)
            : this(new Context(GetScheme(uri.Scheme), uri.Host, uri.Port), ns)
        {
            Contract.Requires<ArgumentNullException>(uri != null);
        }

        private static Scheme GetScheme(string schemeString)
        {
            var scheme = (Scheme)Enum.Parse(typeof(Scheme), schemeString, true);
            return scheme;
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Service"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public Service()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual Context Context
        {
            get { return this.context; }
        }

        /// <inheritdoc/>
        public virtual Namespace Namespace
        {
            get { return this.ns; }
        }

        /// <inheritdoc/>
        public virtual string SessionKey
        {
            get { return this.Context.SessionKey; }
            set { this.Context.SessionKey = value; }
        }

        /// <inheritdoc/>
        public virtual ApplicationCollection Applications
        {
            get { return this.applications; }
        }

        /// <inheritdoc/>
        public virtual ConfigurationCollection Configurations
        {
            get { return this.configurations; }
        }

        /// <inheritdoc/>
        public virtual IndexCollection Indexes
        {
            get { return this.indexes; }
        }

        /// <inheritdoc/>
        public virtual JobCollection Jobs
        {
            get { return this.jobs; }
        }

        /// <inheritdoc/>
        public virtual SavedSearchCollection SavedSearches
        {
            get { return this.savedSearches; }
        }

        /// <inheritdoc/>
        public virtual IServer Server
        {
            get { return this.server; }
        }

        /// <inheritdoc/>
        public virtual StoragePasswordCollection StoragePasswords
        {
            get { return this.storagePasswords; }
        }

        /// <inheritdoc/>
        public virtual ITransmitter Transmitter
        {
            get { return this.transmitter; }
        }

        #endregion

        #region Methods

        #region Methods for accessing entities and entity collections not specifically supported by Splunk.Client

        /// <inheritdoc/>
        public Entity<Resource> CreateEntity(params string[] resourceName)
        {
            return new Entity<Resource>(this, new ResourceName(resourceName));
        }

        /// <inheritdoc/>
        public Entity<Resource> CreateEntity(IEnumerable<string> resourceName, string user, string app)
        {
            return new Entity<Resource>(this.Context, new Namespace(user, app), new ResourceName(resourceName));
        }

        /// <inheritdoc/>
        public EntityCollection<Entity<Resource>, Resource> CreateEntityCollection(params string[] resourceName)
        {
            return new EntityCollection<Entity<Resource>, Resource>(this, new ResourceName(resourceName));
        }

        /// <inheritdoc/>
        public EntityCollection<Entity<Resource>, Resource> CreateEntityCollection(IEnumerable<string> resourceName, string user, string app)
        {
            return new EntityCollection<Entity<Resource>, Resource>(this.Context, new Namespace(user, app), 
                new ResourceName(resourceName));
        }

        /// <inheritdoc/>
        public EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection> CreateEntityCollectionCollection(params string[] resourceName)
        {
            return new EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection>(this, new ResourceName(resourceName));
        }

        /// <inheritdoc/>
        public EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection> CreateEntityCollectionCollection(IEnumerable<string> resourceName, string user, string app)
        {
            return new EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection>(this.Context, new Namespace(user, app), new ResourceName(resourceName));
        }

        #endregion

        #region Access control

        /// <inheritdoc/>
        public virtual async Task<ReadOnlyCollection<string>> GetCapabilitiesAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, AuthorizationCapabilities).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader).ConfigureAwait(false);

                if (feed.Entries.Count != 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics : cardinality violation
                }

                var entry = feed.Entries[0];

                ReadOnlyCollection<dynamic> capabilities = entry.Content.Capabilities;
                return new ReadOnlyCollection<string>(capabilities.Cast<string>().ToList());
            }
        }

        /// <inheritdoc/>
        public virtual async Task LogOnAsync(string username, string password)
        {
            using (var response = await this.Context.PostAsync(Namespace.Default, AuthLogin, new Argument[]
            {
                new Argument("username", username),
                new Argument("password", password)
            }).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                this.SessionKey = await response.XmlReader.ReadResponseElementAsync("sessionKey").ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task LogOffAsync()
        {
            var resourceName = new ResourceName(AuthenticationHttpAuthTokens, this.SessionKey);

            using (var response = await this.Context.DeleteAsync(Namespace.Default, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                this.SessionKey = null;
            }
        }

        #endregion

        #region Search

        /// <inheritdoc/>
        public virtual async Task<Job> DispatchSavedSearchAsync(string name,
            SavedSearchDispatchArgs dispatchArgs = null,
            SavedSearchTemplateArgs templateArgs = null)
        {
            var savedSearch = new SavedSearch(this.Context, this.Namespace, name);
            var job = await savedSearch.DispatchAsync(dispatchArgs, templateArgs).ConfigureAwait(false);
            return job;
        }

        /// <inheritdoc/>
        public virtual async Task<SearchPreviewStream> ExportSearchPreviewsAsync(string search, SearchExportArgs args = null)
        {
            var arguments = new Argument[] 
            { 
                new Argument("search", search) 
            }
            .AsEnumerable();

            if (args != null)
            {
                arguments = arguments.Concat(args);
            }

            var response = await this.Context.GetAsync(this.Namespace, SearchJobsExport, arguments).ConfigureAwait(false);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                var stream = await SearchPreviewStream.CreateAsync(response).ConfigureAwait(false);
                return stream;
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<SearchResultStream> ExportSearchResultsAsync(string search, SearchExportArgs args = null)
        {
            var arguments = new Argument[] 
            { 
                new Argument("search", search) 
            }
            .AsEnumerable();

            if (args != null)
            {
                arguments = arguments.Concat(args);
            }

            var response = await this.Context.GetAsync(this.Namespace, SearchJobsExport, arguments).ConfigureAwait(false);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                return await SearchResultStream.CreateAsync(response).ConfigureAwait(false); // Transfers response ownership
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<Job> SearchAsync(string search, int count = 0, 
            ExecutionMode mode = ExecutionMode.Normal, JobArgs args = null, 
            CustomJobArgs customArgs = null)
        {
            var job = await this.Jobs.CreateAsync(search, count, mode, args, customArgs).ConfigureAwait(false);
            return job;
        }

        /// <inheritdoc/>
        public virtual async Task<SearchResultStream> SearchOneShotAsync(string search, int count = 0, 
            JobArgs args = null,  CustomJobArgs customArgs = null)
        {
            var resourceName = JobCollection.ClassResourceName;

            var arguments = new SearchArgs
            {
                Search = search,
                Count = count,
                ExecutionMode = ExecutionMode.OneShot
            }
            .AsEnumerable();

            if (args != null)
            {
                arguments = arguments.Concat(args.Where(arg => arg.Name != "exec_mode"));
            }

            if (customArgs != null)
            {
                arguments = arguments.Concat(customArgs);
            }

            Response response = await this.Context.PostAsync(this.Namespace, resourceName, arguments).ConfigureAwait(false);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                var stream = await SearchResultStream.CreateAsync(response).ConfigureAwait(false); // Transfers response ownership
                return stream;
            }
            catch
            {
                response.Dispose();
                throw;
            }
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
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Service"/>.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
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
        ///     To help ensure that resources are always cleaned up appropriately,
        ///     ensure that the override is callable multiple times without throwing
        ///     an exception.
        ///     </description></item>
        /// </list>
        /// There is no performance benefit in overriding this method on types that
        /// use only managed resources (such as arrays) because they are
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
        /// <returns>
        /// A string that represents this object.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
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
        readonly SavedSearchCollection savedSearches;
        readonly ApplicationCollection applications;
        readonly IndexCollection indexes;
        readonly Transmitter transmitter;
        readonly JobCollection jobs;
        readonly Server server;

        bool disposed;

        #endregion

        #region Types

        class SearchArgs : Args<SearchArgs>
        {
            [DataMember(Name = "search", IsRequired = true)]
            public string Search
            { get; set; }

            [DataMember(Name = "count")]
            [DefaultValue(100)]
            public int Count
            { get; set; }

            [DataMember(Name = "exec_mode")]
            [DefaultValue(ExecutionMode.Normal)]
            public ExecutionMode ExecutionMode
            { get; set; }
        }

        #endregion
    }
}
