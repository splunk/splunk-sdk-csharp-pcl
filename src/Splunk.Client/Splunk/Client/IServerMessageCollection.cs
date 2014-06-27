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
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for accessing Splunk server messages.
    /// </summary>
    /// <remarks>
    /// Most messages are created by splunkd to inform the user of system
    /// problems. Splunk Web typically displays these as bulletin board messages.
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/w3Rmjp">REST API: messages</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TServerMessage">
    /// Type of the server message.
    /// </typeparam>
    /// <seealso cref="T:IPaginated"/>
    /// <seealso cref="T:IEntityCollection{TServerMessage"/>
    [ContractClass(typeof(IServerMessageCollectionContract<>))]
    public interface IServerMessageCollection<TServerMessage> : IPaginated, IEntityCollection<TServerMessage, Resource>
        where TServerMessage : BaseEntity<Resource>, IServerMessage, new()
    {
        /// <summary>
        /// Asyncrhonously creates a new <see cref="ServerMessage"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the server message to create.
        /// </param>
        /// <param name="type">
        /// The severity of the server message.
        /// </param>
        /// <param name="text">
        /// The text of the server message.
        /// </param>
        /// <returns>
        /// An object representing the newly created server message.
        /// </returns>
        Task<TServerMessage> CreateAsync(string name, ServerMessageSeverity type, string text);

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites in the
        /// current <see cref="ServerMessageCollection"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// <see cref="ServerMessageCollection"/> will contain all changes since the
        /// select entites were last retrieved.
        /// </remarks>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetSliceAsync(ServerMessageCollection.Filter criteria);
    }

    /// <summary>
    /// A server message collection contract.
    /// </summary>
    /// <typeparam name="TServerMessage">
    /// Type of the server message.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.IServerMessageCollection{TServerMessage}"/>
    [ContractClassFor(typeof(IServerMessageCollection<>))]
    abstract class IServerMessageCollectionContract<TServerMessage> : IServerMessageCollection<TServerMessage> 
        where TServerMessage : BaseEntity<Resource>, IServerMessage, new()
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
        public abstract TServerMessage this[int index] { get; }

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

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new <see cref="ServerMessage"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="ServerMessage"/> to create.
        /// </param>
        /// <param name="type">
        /// Type of the <see cref="ServerMessage"/> to create.
        /// </param>
        /// <param name="text">
        /// Text of the <see cref="ServerMessage"/> to create.
        /// </param>
        /// <returns>
        /// The <see cref="ServerMessage"/> created.
        /// </returns>
        public Task<TServerMessage> CreateAsync(string name, ServerMessageSeverity type, string text)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(text != null);
            return default(Task<TServerMessage>);
        }

        /// <summary>
        /// Gets all asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"> representing this operation.
        /// </returns>
        public abstract Task GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves a <see cref="ServerMessage"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="ServerMessage"/> to retrieve.
        /// </param>
        /// <returns>
        /// The <see cref="ServerMessage"/> retrieved.
        /// </returns>
        public abstract Task<TServerMessage> GetAsync(string name);

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public abstract IEnumerator<TServerMessage> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator<TServerMessage>);
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
        public Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            return default(Task);
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
        public Task GetSliceAsync(ServerMessageCollection.Filter criteria)
        {
            Contract.Requires<ArgumentNullException>(criteria != null);
            return default(Task);
        }

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
