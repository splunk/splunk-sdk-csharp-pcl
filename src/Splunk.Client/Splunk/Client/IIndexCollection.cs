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
//// [0] Contracts
//// [O] Documentation
//// 
namespace Splunk.Client
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to the Splunk application collection.
    /// </summary>
    /// <typeparam name="TIndex">
    /// Type of the index.
    /// </typeparam>
    /// <seealso cref="T:IPaginated"/>
    /// <seealso cref="T:IEntityCollection{TIndex"/>
    public interface IIndexCollection<TIndex> : IPaginated, IEntityCollection<TIndex, Resource> 
        where TIndex : BaseEntity<Resource>, IIndex, new()
    {
        /// <summary>
        /// Asyncrhonously creates the index represented by the current index.
        /// </summary>
        /// <param name="name">
        /// Name of the index to create.
        /// </param>
        /// <param name="attributes">
        /// Attributes to set on the newly created index.
        /// </param>
        /// <param name="coldPath">
        /// Location for storing the cold databases for the current
        /// <see cref= "Index"/>. A value of <c>null</c> or <c>""</c> specifies that
        /// the cold databases should be stored at the default location.
        /// </param>
        /// <param name="homePath">
        /// Location for storing the hot and warm buckets for the current index. A
        /// value of <c>null</c> or <c>""</c> specifies that the hot and warm buckets
        /// should be stored at the default location.
        /// </param>
        /// <param name="thawedPath">
        /// Location for storing the resurrected databases for the current
        /// <see cref="Index"/>. A value of <c>null</c> or <c>""</c> specifies that
        /// the resurrected databases should be stored at the default location.
        /// </param>
        /// <returns>
        /// An object representing the newly created index.
        /// </returns>
        Task<TIndex> CreateAsync(string name, IndexAttributes attributes, string coldPath, string homePath, string 
            thawedPath = null);

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites in the
        /// current <see cref="IndexCollection"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// <see cref="IndexCollection"/> will contain all changes since the select
        /// entites were last retrieved.
        /// </remarks>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetSliceAsync(IndexCollection.Filter criteria);
    }
}
