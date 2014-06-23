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
//// [X] Contracts - there are none
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
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
        Context Context
        { get; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> used by this <see cref="Service"/>.
        /// </summary>
        Namespace Namespace
        { get; }

        /// <summary>
        /// Gets or sets the session key used by this <see cref="Service"/>.
        /// </summary>
        string SessionKey
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ApplicationCollection Applications 
        { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ConfigurationCollection Configurations
        { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IndexCollection Indexes
        { get; }
        
        /// <summary>
        /// 
        /// </summary>
        JobCollection Jobs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        SavedSearchCollection SavedSearches 
        { get; }

        /// <summary>
        /// 
        /// </summary>
        Server Server
        { get; }

        /// <summary>
        /// 
        /// </summary>
        StoragePasswordCollection StoragePasswords
        { get; }

        /// <summary>
        /// 
        /// </summary>
        Transmitter Transmitter 
        { get; }

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
        Task<IReadOnlyList<string>> GetCapabilitiesAsync();

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
        Task LoginAsync(string username, string password);

        /// <summary>
        /// Ends the session by associated with the current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">DELETE 
        /// authentication/httpauth-tokens/{name}</a> endpoint to end the
        /// the session by removing <see cref="SessionKey"/>.
        /// </remarks>
        Task LogoffAsync();

        #endregion

        #region Search

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
        Task<Job> DispatchSavedSearchAsync(string name, SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);

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
        /// search/jobs/export</a> endpoint to export an obervable sequence of
        /// search result previews.
        /// </remarks>
        Task<SearchPreviewStream> ExportSearchPreviewsAsync(string search, SearchExportArgs args = null);

        /// <summary>
        /// Asynchronously exports an observable sequence of search results.
        /// </summary>
        /// <param name="search">
        /// A Splunk search command.
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
        /// search/jobs/export</a> endpoint to export an observable sequence
        /// of search results.
        /// </remarks>
        Task<SearchResultStream> ExportSearchResultsAsync(string search, SearchExportArgs args = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search">
        /// 
        /// </param>
        /// <param name="args">
        /// 
        /// </param>
        /// <param name="customArgs">
        /// 
        /// </param>
        /// <param name="blocking">
        /// 
        /// </param>
        /// <returns>
        /// An object representing a Splunk search job.
        /// </returns>
        Task<Job> SearchAsync(string search, JobArgs args = null, CustomJobArgs customArgs = null);

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
        Task<SearchResultStream> SearchOneshotAsync(string search, JobArgs args = null, CustomJobArgs customArgs = null);

        #endregion

        #endregion
    }

    [ContractClassFor(typeof(IService))]
    abstract class IServiceContract : IService
    {
        #region Properties

        public abstract Context Context
        { get; }

        public abstract Namespace Namespace{ get; }

        public abstract string SessionKey
        { get; set; }

        public abstract ApplicationCollection Applications{ get; }

        public abstract ConfigurationCollection Configurations{ get; }

        public abstract IndexCollection Indexes{ get; }

        public abstract JobCollection Jobs{ get; }

        public abstract SavedSearchCollection SavedSearches{ get; }

        public abstract Server Server{ get; }

        public abstract StoragePasswordCollection StoragePasswords{ get; }

        public abstract Transmitter Transmitter{ get; }

        #endregion

        #region Methods

        #region Access control

        public abstract Task<IReadOnlyList<string>> GetCapabilitiesAsync();

        public Task LoginAsync(string username, string password)
        {
            Contract.Requires<ArgumentNullException>(username != null);
            Contract.Requires<ArgumentNullException>(password != null);
            return default(Task);
        }

        public Task LogoffAsync()
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

        public abstract Task<Job> SearchAsync(string search, JobArgs args = null, CustomJobArgs customArgs = null);

        public Task<SearchResultStream> SearchOneshotAsync(string search, JobArgs args = null, CustomJobArgs customArgs = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<SearchResultStream>);
        }

        #endregion

        #endregion
    }
}
