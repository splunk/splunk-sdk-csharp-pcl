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
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to a Splunk saved search entity
    /// collection.
    /// </summary>
    /// <typeparam name="TSavedSearch">
    /// Type of the saved search.
    /// </typeparam>
    /// <seealso cref="T:IPaginated"/>
    /// <seealso cref="T:IEntityCollection{TSavedSearch"/>
    [ContractClass(typeof(ISavedSearchCollectionContract<>))]
    public interface ISavedSearchCollection<TSavedSearch> : IPaginated, IEntityCollection<TSavedSearch, Resource>
        where TSavedSearch : BaseEntity<Resource>, ISavedSearch, new()
    {
        /// <summary>
        /// Asynchronously creates a new saved search.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/EPQypw">POST
        /// saved/searches</a> endpoint to create the <see cref="SavedSearch"/>
        /// represented by the current instance.
        /// </remarks>
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
        /// The new asynchronous.
        /// </returns>
        Task<TSavedSearch> CreateAsync(string name, string search, SavedSearchAttributes attributes, 
            SavedSearchDispatchArgs dispatchArgs, SavedSearchTemplateArgs templateArgs);

        /// <summary>
        /// Asynchronously retrieves select entities in the current saved search
        /// entity collection.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// saved search entity collection will contain all changes since the select
        /// entities were last retrieved.
        /// </remarks>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetSliceAsync(SavedSearchCollection.Filter criteria);
    }

    /// <summary>
    /// A saved search collection contract.
    /// </summary>
    /// <typeparam name="TSavedSearch">
    /// Type of the saved search.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.ISavedSearchCollection{TSavedSearch}"/>
    [ContractClassFor(typeof(ISavedSearchCollection<>))]
    abstract class ISavedSearchCollectionContract<TSavedSearch> : ISavedSearchCollection<TSavedSearch>
        where TSavedSearch : BaseEntity<Resource>, ISavedSearch, new()
    {
        #region Properties

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index">
        /// Zero-based index of the entry to access.
        /// </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public abstract TSavedSearch this[int index] { get; }

        /// <summary>
        /// Gets the number of. 
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public abstract int Count { get; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public abstract ReadOnlyCollection<Message> Messages { get; }

        /// <summary>
        /// Gets the pagination.
        /// </summary>
        /// <value>
        /// The pagination.
        /// </value>
        public abstract Pagination Pagination { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asyncrhonously creates a new <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="search">
        /// The Splunk search command to save.
        /// </param>
        /// <param name="attributes">
        /// Optional attributes of the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="dispatchArgs">
        /// Optional dispatch arguments for the <see cref="SavedSearch"/>.
        /// </param>
        /// <param name="templateArgs">
        /// Optional template arguments for the <see cref="SavedSearch"/>.
        /// </param>
        /// <returns>
        /// The <see cref="SavedSearch"/> created.
        /// </returns>
        public Task<TSavedSearch> CreateAsync(string name, string search, SavedSearchAttributes attributes,
            SavedSearchDispatchArgs dispatchArgs, SavedSearchTemplateArgs templateArgs)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Requires<ArgumentNullException>(name != null);
            return default(Task<TSavedSearch>);
        }

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public Task GetSliceAsync(SavedSearchCollection.Filter criteria)
        {
            Contract.Requires<ArgumentNullException>(criteria != null);
            return default(Task);
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="SavedSearch"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to retrieve.
        /// </param>
        /// <returns>
        /// The <see cref="SavedSearch"/> retrieved.
        /// </returns>
        public abstract Task<TSavedSearch> GetAsync(string name);

        /// <summary>
        /// Gets all asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task GetAllAsync();
        IEnumerator IEnumerable.GetEnumerator() { return default(IEnumerator); }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public abstract IEnumerator<TSavedSearch> GetEnumerator();

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public abstract Task GetSliceAsync(params Argument[] arguments);

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);

        /// <summary>
        /// Reload asynchronous.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task ReloadAsync();

        #endregion
    }
}
