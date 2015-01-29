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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface common to all Splunk entities.
    /// </summary>
    [ContractClass(typeof(IEntityContract))]
    public interface IEntity
    {
        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current entity that
        /// contains all changes to it since it was last retrieved.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetAsync();

        /// <summary>
        /// Asynchronously invokes an action on the current entity from Splunk.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task<bool> SendAsync(string action, HttpMethod method = null, params Argument[] arguments);

        /// <summary>
        /// Asynchronously removes the current entity from Splunk.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task RemoveAsync();

        /// <summary>
        /// Asynchronously updates the attributes of the current entity on
        /// Splunk.
        /// </summary>
        /// <remarks>
        /// Splunk usually returns an updated snapshot on completion of the
        /// operation. When it does the current snapshot will also be
        /// updated.
        /// </remarks>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        Task<bool> UpdateAsync(params Argument[] arguments);

        /// <summary>
        /// Asynchronously updates the attributes of the current entity on
        /// on Splunk.
        /// </summary>
        /// <remarks>
        /// Splunk usually returns an updated snapshot on completion of the
        /// operation. When it does the current snapshot will be updated
        /// and returns <c>true</c>; otherwise, this method returns
        /// <c>false</c>.
        /// </remarks>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        Task<bool> UpdateAsync(IEnumerable<Argument> arguments);
    }

    /// <summary>
    /// An entity contract.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.IEntity"/>
    [ContractClassFor(typeof(IEntity))]
    abstract class IEntityContract : IEntity
    {
        public abstract Task GetAsync();

        public Task<bool> SendAsync(string action, HttpMethod method, params Argument[] arguments)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            Contract.Requires<ArgumentException>(action.Length != 0);
            return default(Task<bool>);
        }

        public abstract Task RemoveAsync();

        public Task<bool> UpdateAsync(params Argument[] arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            return default(Task<bool>);
        }

        public Task<bool> UpdateAsync(IEnumerable<Argument> arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            return default(Task<bool>);
        }
    }
}
