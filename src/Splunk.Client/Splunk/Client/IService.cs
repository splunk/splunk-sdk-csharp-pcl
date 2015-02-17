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

//// TODO
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to the Splunk REST API.
    /// </summary>
    [ContractClass(typeof(IServiceContract))]
    public interface IService
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Service"/>.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        Context Context
        { get; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> used by this <see cref="Service"/>.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        Namespace Namespace
        { get; }

        /// <summary>
        /// Gets or sets the session key used by this <see cref="Service"/>.
        /// </summary>
        /// <value>
        /// The session key.
        /// </value>
        string SessionKey
        { get; set; }

        /// <summary>
        /// Gets the applications.
        /// </summary>
        /// <value>
        /// The applications.
        /// </value>
        ApplicationCollection Applications 
        { get; }

        /// <summary>
        /// Gets the configurations.
        /// </summary>
        /// <value>
        /// The configurations.
        /// </value>
        ConfigurationCollection Configurations
        { get; }

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <value>
        /// The indexes.
        /// </value>
        IndexCollection Indexes
        { get; }

        /// <summary>
        /// Gets an object representing the collection of Splunk search jobs.
        /// </summary>
        /// <value>
        /// An object representing the collection of Splunk search jobs.
        /// </value>
        JobCollection Jobs
        { get; }

        /// <summary>
        /// Gets an object representing the collection of Splunk saved searches.
        /// </summary>
        /// <value>
        /// An object representing the collection of Splunk saved searches.
        /// </value>
        SavedSearchCollection SavedSearches 
        { get; }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        IServer Server
        { get; }

        /// <summary>
        /// Gets the storage passwords.
        /// </summary>
        /// <value>
        /// The storage passwords.
        /// </value>
        StoragePasswordCollection StoragePasswords
        { get; }

        /// <summary>
        /// Gets the transmitter.
        /// </summary>
        /// <value>
        /// The transmitter.
        /// </value>
        ITransmitter Transmitter 
        { get; }

        #endregion

        #region Methods

        #region Methods for accessing entities and entity collections not specifically supported by Splunk.Client

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns>
        /// An object representing the entity identified by <paramref name="resourceName"/> in the context of
        /// the service namespace.
        /// </returns>
        Entity<Resource> CreateEntity(params string[] resourceName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="user"></param>
        /// <param name="app"></param>
        /// <returns>
        /// An object representing the entity identified by <paramref name="resourceName"/> in the context of
        /// the namespace identified by <paramref name="user"/> and <paramref name="app"/>.
        /// </returns>
        Entity<Resource> CreateEntity(IEnumerable<string> resourceName, string user, string app);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns>
        /// An object representing the entity collection identified by <paramref name="resourceName"/> in the context of
        /// the service namespace.
        /// </returns>
        EntityCollection<Entity<Resource>, Resource> CreateEntityCollection(params string[] resourceName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="user"></param>
        /// <param name="app"></param>
        /// <returns>
        /// An object representing the entity identified by <paramref name="resourceName"/> in the context of
        /// the namespace identified by <paramref name="user"/> and <paramref name="app"/>.
        /// </returns>
        EntityCollection<Entity<Resource>, Resource> CreateEntityCollection(IEnumerable<string> resourceName, string user, string app);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns>
        /// An object representing the entity collection identified by <paramref name="resourceName"/> in the context of
        /// the service namespace.
        /// </returns>
        EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection> CreateEntityCollectionCollection(params string[] resourceName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="user"></param>
        /// <param name="app"></param>
        /// <returns>
        /// An object representing the entity identified by <paramref name="resourceName"/> in the context of
        /// the namespace identified by <paramref name="user"/> and <paramref name="app"/>.
        /// </returns>
        EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection> CreateEntityCollectionCollection(IEnumerable<string> resourceName, string user, string app);

        #endregion

        #region Access control

        /// <summary>
        /// Asynchronously retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kgTKvM">GET
        /// authorization/capabilities</a> endpoint to construct a list of all Splunk
        /// system capabilities.
        /// </remarks>
        /// <returns>
        /// An object representing the list of all Splunk system capabilities.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification =
            "This is by design")
        ]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification =
            "This is not a property")
        ]
        Task<ReadOnlyCollection<string>> GetCapabilitiesAsync();

        /// <summary>
        /// Provides user authentication asynchronously.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">POST auth/login</a>
        /// endpoint. The session key this endpoint returns is used for subsequent
        /// requests. It is accessible via the <see cref= "SessionKey"/> property.
        /// </remarks>
        /// <param name="username">
        /// Splunk account username.
        /// </param>
        /// <param name="password">
        /// Splunk account password for username.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task LogOnAsync(string username, string password);

        /// <summary>
        /// Ends the session by associated with the current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">DELETE
        /// authentication/httpauth-tokens/{name}</a> endpoint to end the the session
        /// by removing <see cref="SessionKey"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task LogOffAsync();

        #endregion

        #region Search

        /// <summary>
        /// Asynchronously dispatches a <see cref="SavedSearch"/> just like the
        /// scheduler would.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the
        /// <see cref="SavedSearch"/> identified by <paramref name="name"/>.
        /// </remarks>
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
        Task<Job> DispatchSavedSearchAsync(string name, SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);

        /// <summary>
        /// Asynchronously exports an observable sequence of search result previews.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/vJvIXv">GET
        /// search/jobs/export</a> endpoint to export an obervable sequence of search
        /// result previews.
        /// </remarks>
        /// <param name="search">
        /// Splunk search command.
        /// </param>
        /// <param name="args">
        /// Optional export arguments.
        /// </param>
        /// <returns>
        /// An object representing an observable sequence of search result previews.
        /// </returns>
        Task<SearchPreviewStream> ExportSearchPreviewsAsync(string search, SearchExportArgs args = null);

        /// <summary>
        /// Asynchronously exports an observable sequence of search results.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/vJvIXv">GET
        /// search/jobs/export</a> endpoint to export an observable sequence of
        /// search results.
        /// </remarks>
        /// <param name="search">
        /// A Splunk search command.
        /// </param>
        /// <param name="args">
        /// Optional export arguments.
        /// </param>
        /// <returns>
        /// An object representing an observable sequence of search result previews.
        /// </returns>
        Task<SearchResultStream> ExportSearchResultsAsync(string search, SearchExportArgs args = null);

        /// <summary>
        /// Asycnrononusly creates and executes a normal or blocking Splunk search job.
        /// </summary>
        /// <param name="search">
        /// 
        /// </param>
        /// <param name="count">
        ///                     
        /// </param>
        /// <param name="mode">
        /// 
        /// </param>
        /// <param name="args">
        /// 
        /// </param>
        /// <param name="customArgs">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the Splunk search job created.
        /// </returns>
        Task<Job> SearchAsync(string search, int count = 100, ExecutionMode mode = ExecutionMode.Normal, 
            JobArgs args = null, CustomJobArgs customArgs = null);

        /// <summary>
        /// Asynchronously executes a one shot search.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/b02g1d">POST search/jobs</a>
        /// endpoint to execute a one shot search.
        /// </remarks>
        /// <param name="search">
        /// Search string.
        /// </param>
        /// <param name="count">
        /// 
        /// </param>
        /// <param name="args">
        /// Optional job arguments.
        /// </param>
        /// <param name="customArgs">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the stream search results.
        /// </returns>
        Task<SearchResultStream> SearchOneShotAsync(string search, int count = 100, JobArgs args = null, CustomJobArgs customArgs = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// A service contract.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.IService"/>
    [ContractClassFor(typeof(IService))]
    abstract class IServiceContract : IService
    {
        #region Properties

        public abstract Context Context 
        { get; }
        
        public abstract Namespace Namespace 
        { get; }
        
        public abstract string SessionKey 
        { get; set; }

        public abstract ApplicationCollection Applications
        { get; }

        public abstract ConfigurationCollection Configurations
        { get; }

        public abstract IndexCollection Indexes
        { get; }
        
        public abstract JobCollection Jobs
        { get; }
        
        public abstract SavedSearchCollection SavedSearches
        { get; }
        
        public abstract IServer Server
        { get; }

        public abstract StoragePasswordCollection StoragePasswords
        { get; }
        
        public abstract ITransmitter Transmitter
        { get; }

        #endregion

        #region Methods

        #region Methods for accessing entities and entity collections not specifically supported by Splunk.Client

        public Entity<Resource> CreateEntity(params string[] resourceName)
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            return default(Entity<Resource>);
        }

        public Entity<Resource> CreateEntity(IEnumerable<string> resourceName, string user, string app)
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(app != null);
            return default(Entity<Resource>);
        }

        public EntityCollection<Entity<Resource>, Resource> CreateEntityCollection(params string[] resourceName)
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            return default(EntityCollection<Entity<Resource>, Resource>);
        }

        public EntityCollection<Entity<Resource>, Resource> CreateEntityCollection(IEnumerable<string> resourceName, string user, string app)
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(app != null);
            return default(EntityCollection<Entity<Resource>, Resource>);
        }

        /// <inheritdoc/>
        public EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection> CreateEntityCollectionCollection(params string[] resourceName)
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            return default(EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection>);
        }

        /// <inheritdoc/>
        public EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection> CreateEntityCollectionCollection(IEnumerable<string> resourceName, string user, string app)
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(app != null);
            return default(EntityCollection<EntityCollection<Entity<ResourceCollection>, ResourceCollection>, ResourceCollection>);
        }

        #endregion

        #region Access control

        public abstract Task<ReadOnlyCollection<string>> GetCapabilitiesAsync();
        
        public Task LogOnAsync(string username, string password)
        {
            Contract.Requires<ArgumentNullException>(username != null);
            Contract.Requires<ArgumentNullException>(password != null);
            return default(Task);
        }
        
        public Task LogOffAsync()
        {
            Contract.Requires<InvalidOperationException>(this.SessionKey != null);
            return default(Task);
        }

        #endregion

        #region Search
        
        public abstract Task<Job> DispatchSavedSearchAsync(string name, SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);
        
        public Task<SearchPreviewStream> ExportSearchPreviewsAsync(string search, SearchExportArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<SearchPreviewStream>);
        }

        public Task<SearchResultStream> ExportSearchResultsAsync(string search, SearchExportArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<SearchResultStream>);
        }
        
        public Task<Job> SearchAsync(string search, int count = 0, 
            ExecutionMode mode = ExecutionMode.Normal, JobArgs args = null,
            CustomJobArgs customArgs = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<Job>);
        }
        
        public Task<SearchResultStream> SearchOneShotAsync(string search, int count = 0, JobArgs args = null, CustomJobArgs customArgs = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<SearchResultStream>);
        }

        #endregion

        #endregion
    }
}
