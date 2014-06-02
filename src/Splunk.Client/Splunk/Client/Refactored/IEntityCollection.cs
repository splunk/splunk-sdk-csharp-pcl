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
    /// Provides an operational interface common to all Splunk entity collections.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity in the collection.
    /// </typeparam>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/TDthxd">Accessing Splunk resources</a>, 
    ///   especially "Other actions for Splunk REST API endpoints".
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/oc65Bo">REST API Reference</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    [ContractClass(typeof(IEntityCollectionContract<>))]
    public interface IEntityCollection<TEntity> : IReadOnlyList<TEntity> 
        where TEntity : ResourceEndpoint, new()
    {
        /// <summary>
        /// Asynchronously retrieves a <see cref="TEntity"/> in the current
        /// <see cref="EntityCollection&lt;TEntity&gt;"/> by name.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task<TEntity> GetAsync(string name);

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the full list of entities
        /// in the current <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entites in the
        /// current <see cref="EntityCollection&lt;TEntity&gt;"/> will contain 
        /// all changes since the list was last retrieved.
        /// </remarks>
        Task GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites
        /// in the current <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entities in the
        /// current <see cref="EntityCollection&lt;TEntity&gt;"/> will contain 
        /// all changes since the select entites were last retrieved.
        /// </remarks>
        Task GetSliceAsync(params Argument[] arguments);

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites
        /// in the current <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entities in the
        /// current <see cref="EntityCollection&lt;TEntity&gt;"/> will contain 
        /// all changes since the select entites were last retrieved.
        /// </remarks>
        Task GetSliceAsync(IEnumerable<Argument> arguments);

        /// <summary>
        /// Asynchronously forces the Splunk server to reload data for the current
        /// <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task ReloadAsync();
    }

    [ContractClassFor(typeof(IEntityCollection<>))]
    abstract class IEntityCollectionContract<TEntity> : IEntityCollection<TEntity>
        where TEntity : ResourceEndpoint, new()
    {
        public Task<TEntity> GetAsync(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            return default(Task<TEntity>);
        }

        public Task GetAllAsync()
        {
            return default(Task);
        }

        public Task GetSliceAsync(params Argument[] arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            return default(Task);
        }

        public Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            return default(Task);
        }

        public Task ReloadAsync()
        {
            return default(Task);
        }

        public TEntity this[int index]
        {
            get { return default(TEntity); }
        }

        public int Count
        {
            get { return default(int); }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return default(IEnumerator<TEntity>);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }
    }
}
