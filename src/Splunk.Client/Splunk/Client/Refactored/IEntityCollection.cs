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
//// [ ] Remove EntityCollection.args and put optional arguments on the GetAsync
////     method (?) args does NOT belong on the constructor. One difficulty:
////     not all collections take arguments. Examples: ConfigurationCollection
////     and IndexCollection.
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System.Collections.Generic;
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
    public interface IEntityCollection<TEntity> : IReadOnlyList<TEntity> where TEntity : ResourceEndpoint, new()
    {
        #region Properties

        /// <summary>
        /// Gets the Splunk server messages delivered with the current Splunk
        /// entity collection when it was last updated.
        /// </summary>
        IReadOnlyList<Message> Messages
        { get; }

        #endregion

        #region Methods

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

        #endregion
    }
}
