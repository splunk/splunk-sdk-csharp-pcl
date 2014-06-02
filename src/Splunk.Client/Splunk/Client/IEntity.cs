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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface common to all Splunk entities.
    /// </summary>
    public interface IEntity
    {
        #region Properties

        
        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "Entity"/> that contains all changes to it since it was last 
        /// retrieved.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetAsync();

        /// <summary>
        /// Asynchronously removes the current <see cref="Entity"/> from Splunk.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task RemoveAsync();

        /// <summary>
        /// Asynchronously updates the attributes of the current <see cref=
        /// "Entity"/> on Splunk.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSnapshot"/> was also updated.
        /// </returns>
        /// <remarks>
        /// Splunk usually returns an updated snapshot on completion of the
        /// operation. When it does the <see cref="CurrentSnapshot"/> will
        /// also be updated.
        /// </remarks>
        Task<bool> UpdateAsync(params Argument[] arguments);

        /// <summary>
        /// Asynchronously updates the attributes of the current <see cref=
        /// "Entity"/> on Splunk.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSnapshot"/> was also updated.
        /// </returns>
        /// <remarks>
        /// Splunk usually returns an updated snapshot on completion of the
        /// operation. When it does the <see cref="CurrentSnapshot"/> will
        /// be updated and returns <c>true</c>; otherwise, this method returns
        /// <c>false</c>.
        /// </remarks>
        Task<bool> UpdateAsync(IEnumerable<Argument> arguments);

        #endregion
    }
}
