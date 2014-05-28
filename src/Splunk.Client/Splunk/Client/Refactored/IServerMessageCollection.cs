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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

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
    /// problems. Splunk Web typically displays these as bulletin board 
    /// messages.
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/w3Rmjp">REST API: messages</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    [ContractClass(typeof(IServerMessageCollectionContract<>))]
    public interface IServerMessageCollection<TServerMessage> : IPaginated, IEntityCollection<TServerMessage>
        where TServerMessage : ResourceEndpoint, IServerMessage, new()
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
        /// Asynchronously retrieves select entities from the list of entites
        /// in the current <see cref="ServerMessageCollection"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entities in the
        /// current <see cref="ServerMessageCollection"/> will contain all 
        /// changes since the select entites were last retrieved.
        /// </remarks>
        Task GetSliceAsync(ServerMessageCollection.SelectionCriteria selectionCriteria);
    }

    [ContractClassFor(typeof(IServerMessageCollection<>))]
    abstract class IServerMessageCollectionContract<TServerMessage> : IServerMessageCollection<TServerMessage> 
        where TServerMessage : ResourceEndpoint, IServerMessage, new()
    {
        public Task<TServerMessage> CreateAsync(string name, ServerMessageSeverity type, string text)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(text != null);
            return default(Task<TServerMessage>);
        }

        public Task GetSliceAsync(ServerMessageCollection.SelectionCriteria selectionCriteria)
        {
            throw new NotImplementedException();
        }

        public Pagination Pagination
        {
            get { return default(Pagination); }
        }

        public Task<TServerMessage> GetAsync(string name)
        {
            return default(Task<TServerMessage>);
        }

        public Task GetAllAsync()
        {
            return default(Task);
        }

        public Task GetSliceAsync(params Argument[] arguments)
        {
            return default(Task);
        }

        public Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            return default(Task);
        }

        public Task ReloadAsync()
        {
            return default(Task);
        }

        public TServerMessage this[int index]
        {
            get { return default(TServerMessage); }
        }

        public int Count
        {
            get { return default(int); }
        }

        public IEnumerator<TServerMessage> GetEnumerator()
        {
            return default(IEnumerator<TServerMessage>);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator<TServerMessage>);
        }
    }
}
