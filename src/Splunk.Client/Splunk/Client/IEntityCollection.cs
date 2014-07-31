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
    /// Provides an operational interface common to all Splunk entity collections.
    /// </summary>
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
    /// <typeparam name="TEntity">
    /// Type of the entity.
    /// </typeparam>
    /// <typeparam name="TResource">
    /// Type of the resource.
    /// </typeparam>
    /// <seealso cref="T:IReadOnlyList{TEntity}"/>
    [ContractClass(typeof(IEntityCollectionContract<,>))]
    public interface IEntityCollection<TEntity, TResource> : IReadOnlyList<TEntity> 
        where TEntity : BaseEntity<TResource>, new()
        where TResource : BaseResource, new()
    {
        /// <summary>
        /// Asynchronously retrieves a <see cref="Entity&lt;TResource&gt;"/> in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the entity to retrieve.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task<TEntity> GetAsync(string name);

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the full list of entities in the
        /// current <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entites in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> will contain all changes
        /// since the list was last retrieved.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites in the
        /// current <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> will contain all changes
        /// since the select entites were last retrieved.
        /// </remarks>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetSliceAsync(params Argument[] arguments);

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites in the
        /// current <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> will contain all changes
        /// since the select entites were last retrieved.
        /// </remarks>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetSliceAsync(IEnumerable<Argument> arguments);

        /// <summary>
        /// Asynchronously forces the Splunk server to reload data for the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task ReloadAsync();
    }

    /// <summary>
    /// An entity collection contract.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Type of the entity.
    /// </typeparam>
    /// <typeparam name="TResource">
    /// Type of the resource.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.IEntityCollection{TEntity,TResource}"/>
    [ContractClassFor(typeof(IEntityCollection<,>))]
    abstract class IEntityCollectionContract<TEntity, TResource> : IEntityCollection<TEntity, TResource>
        where TEntity : BaseEntity<TResource>, new()
        where TResource : BaseResource, new()
    {
        public abstract TEntity this[int index]
        { get; }

        public abstract int Count
        { get; }

        public Task<TEntity> GetAsync(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            return default(Task<TEntity>);
        }

        public abstract IEnumerator<TEntity> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        { 
            return default(IEnumerator); 
        }

        public abstract Task GetAllAsync();

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

        public abstract Task ReloadAsync();
    }
}
