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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for storage password collection.
    /// </summary>
    /// <typeparam name="TStoragePassword">
    /// Type of the storage password.
    /// </typeparam>
    /// <seealso cref="T:IPaginated"/>
    /// <seealso cref="T:IEntityCollection{TStoragePassword"/>
    [ContractClass(typeof(IStoragePasswordCollectionContract<>))]
    public interface IStoragePasswordCollection<TStoragePassword> : IPaginated, IEntityCollection<TStoragePassword, Resource>
        where TStoragePassword : BaseEntity<Resource>, IStoragePassword, new()
    {
        /// <summary>
        /// Asynchronously creates a new <see cref="StoragePassword"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JgyIeN">POST
        /// storage/passwords</a> endpoint to create a <see cref= "StoragePassword"/>
        /// identified by <paramref name="name"/> and
        /// <paramref name="realm"/>.
        /// </remarks>
        /// <param name="password">
        /// Password to be stored.
        /// </param>
        /// <param name="username">
        /// The username associated with the password to be stored.
        /// </param>
        /// <param name="realm">
        /// Optional domain or realm name associated with the password to be stored.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        Task<TStoragePassword> CreateAsync(string password, string username, string realm = null);

        /// <summary>
        /// Asynchronously retrieves a <see cref="StoragePassword"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/HL3c0T">GET
        /// storage/passwords/{name}</a> endpoint to retrieve the
        /// <see cref= "StoragePassword"/> identified by <paramref name="username"/>
        /// and
        /// <paramref name="realm"/>.
        /// </remarks>
        /// <param name="username">
        /// The username associated with the password to be retrieved.
        /// </param>
        /// <param name="realm">
        /// Optional domain or realm name associated with the password to be
        /// retrieved.
        /// </param>
        /// <returns>
        /// An object representing the storage password retrieved.
        /// </returns>
        Task<TStoragePassword> GetAsync(string username, string realm = null);

        /// <summary>
        /// Asynchronously retrieves a <see cref="StoragePassword"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/HL3c0T">GET
        /// storage/passwords/{name}</a> endpoint to retrieve the
        /// <see cref= "StoragePassword"/> identified by <paramref name="username"/>
        /// and
        /// <paramref name="realm"/>.
        /// </remarks>
        /// <param name="username">
        /// The username associated with the password to be retrieved.
        /// </param>
        /// <param name="realm">
        /// Optional domain or realm name associated with the password to be
        /// retrieved.
        /// </param>
        /// <returns>
        /// An object representing the storage password retrieved or <c>null</c>, if
        /// no such storage password exists.
        /// </returns>
        Task<TStoragePassword> GetOrNullAsync(string username, string realm = null);

        /// <summary>
        /// Asynchronously retrieves select storage passwords from Splunk.
        /// </summary>
        /// <param name="criteria">
        /// Specifies the criteria used in selecting storage passwords.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetSliceAsync(StoragePasswordCollection.Filter criteria);
    }

    /// <summary>
    /// A storage password collection contract.
    /// </summary>
    /// <typeparam name="TStoragePassword">
    /// Type of the storage password.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.IStoragePasswordCollection{TStoragePassword}"/>
    [ContractClassFor(typeof(IStoragePasswordCollection<>))]
    abstract class IStoragePasswordCollectionContract<TStoragePassword> : IStoragePasswordCollection<TStoragePassword>
        where TStoragePassword : BaseEntity<Resource>, IStoragePassword, new()
    {
        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index">
        /// Zero-based index of the entry to access.
        /// </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public abstract TStoragePassword this[int index] { get; }

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
        public abstract IReadOnlyList<Message> Messages { get; }

        /// <summary>
        /// Gets the pagination.
        /// </summary>
        /// <value>
        /// The pagination.
        /// </value>
        public abstract Pagination Pagination { get; }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="realm">
        /// The realm.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        public Task<TStoragePassword> CreateAsync(string password, string username, string realm)
        {
            Contract.Requires<ArgumentException>(password != null);
            Contract.Requires<ArgumentException>(username != null);
            return default(Task<TStoragePassword>);
        }

        /// <summary>
        /// Gets all asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"> representing this operation.
        /// </returns>
        public abstract Task GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves a <see cref="StoragePassword"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="StoragePassword"/> to retrieve.
        /// </param>
        /// <returns>
        /// The <see cref="StoragePassword"/> retrieved.
        /// </returns>
        public abstract Task<TStoragePassword> GetAsync(string name);

        /// <summary>
        /// Asynchronously retrieves a <see cref="StoragePassword"/> by username
        /// and realm.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="realm">
        /// The realm.
        /// </param>
        /// <returns>
        /// The <see cref="StoragePassword"/> retrieved.
        /// </returns>
        public Task<TStoragePassword> GetAsync(string username, string realm)
        {
            Contract.Requires<ArgumentNullException>(username != null);
            return default(Task<TStoragePassword>);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public abstract IEnumerator<TStoragePassword> GetEnumerator();

        /// <summary>
        /// Gets or null asynchronous.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="realm">
        /// The realm.
        /// </param>
        /// <returns>
        /// The or null asynchronous.
        /// </returns>
        public Task<TStoragePassword> GetOrNullAsync(string username, string realm)
        {
            Contract.Requires<ArgumentNullException>(username != null);
            return default(Task<TStoragePassword>);
        }

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
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public Task GetSliceAsync(StoragePasswordCollection.Filter criteria)
        {
            Contract.Requires<ArgumentException>(criteria != null);
            return default(Task);
        }

        /// <summary>
        /// Reload asynchronous.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task ReloadAsync();
    }
}
