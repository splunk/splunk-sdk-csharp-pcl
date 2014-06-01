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

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to a Splunk saved search entity collection.
    /// </summary>
    /// <typeparam name="TSavedSearch">
    /// 
    /// </typeparam>
    [ContractClass(typeof(ISavedSearchCollectionContract<>))]
    public interface ISavedSearchCollection<TSavedSearch> : IPaginated, IEntityCollection<TSavedSearch>
        where TSavedSearch : ResourceEndpoint, ISavedSearch, new()
    {
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
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/EPQypw">POST 
        /// saved/searches</a> endpoint to create the <see cref="SavedSearch"/>
        /// represented by the current instance.
        /// </remarks>
        Task<TSavedSearch> CreateAsync(string name, string search, SavedSearchAttributes attributes, 
            SavedSearchDispatchArgs dispatchArgs, SavedSearchTemplateArgs templateArgs);

        /// <summary>
        /// Asynchronously retrieves select entities in the current saved 
        /// search entity collection.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entities in the
        /// current saved search entity collection will contain all changes 
        /// since the select entities were last retrieved.
        /// </remarks>
        Task GetSliceAsync(SavedSearchCollection.Filter criteria);
    }

    [ContractClassFor(typeof(ISavedSearchCollection<>))]
    abstract class ISavedSearchCollectionContract<TSavedSearch> : ISavedSearchCollection<TSavedSearch>
        where TSavedSearch : ResourceEndpoint, ISavedSearch, new()
    {
        #region Properties

        public abstract TSavedSearch this[int index] { get; }
        public abstract int Count { get; }
        public abstract IReadOnlyList<Message> Messages { get; }
        public abstract Pagination Pagination { get; }

        #endregion

        #region Methods

        public Task<TSavedSearch> CreateAsync(string name, string search, SavedSearchAttributes attributes,
            SavedSearchDispatchArgs dispatchArgs, SavedSearchTemplateArgs templateArgs)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Requires<ArgumentNullException>(name != null);
            return default(Task<TSavedSearch>);
        }

        public Task GetSliceAsync(SavedSearchCollection.Filter criteria)
        {
            Contract.Requires<ArgumentNullException>(criteria != null);
            return default(Task);
        }

        public abstract Task<TSavedSearch> GetAsync(string name);
        public abstract Task GetAllAsync();
        IEnumerator IEnumerable.GetEnumerator() { return default(IEnumerator); }
        public abstract IEnumerator<TSavedSearch> GetEnumerator();
        public abstract Task GetSliceAsync(params Argument[] arguments);
        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);
        public abstract Task ReloadAsync();

        #endregion
    }
}
