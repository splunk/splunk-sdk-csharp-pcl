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
        /// <remarks>
        /// Many entities support actions to access and change Splunk resources. The <see cref="!:http://goo.gl/2d0OGU">
        /// REST API User Manual</see> describes and shows examples of invoking them.
        /// </remarks>
        /// <param name="method">
        /// One of the following REST methods:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Method</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="HttpMethod"/>.Get</term>
        ///         <description>
        ///             Use this method to invoke an action that gets the current resource state as a list of key-value
        ///             pairs.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="HttpMethod"/>.Post</term>
        ///         <description>
        ///             Use this method to invoke an action that creates or updates an existing resource.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="HttpMethod"/>.Delete</term>
        ///         <description>
        ///             Use this method to invoke an action that deletes a resource from the resource hierarchy.
        ///         </description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="action">
        /// Common actions that may apply to an entity include:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Action</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>_reload</term>
        ///         <description>GET operation to refresh a resource.</description>
        ///     </item>
        ///     <item>
        ///         <term>create</term>
        ///         <description>POST operation to create a resource.</description>
        ///     </item>
        ///     <item>
        ///         <term>disable</term>
        ///         <description>POST operation to disable a resource.</description>
        ///     </item>
        ///     <item>
        ///         <term>edit</term>
        ///         <description>POST operation to change a resource.</description>
        ///     </item>
        ///     <item>
        ///         <term>enable</term>
        ///         <description>POST operation to enable a resource.</description>
        ///     </item>
        ///     <item>
        ///         <term>list</term>
        ///         <description>GET operation to obtain</description>
        ///     </item>
        ///     <item>
        ///         <term>move</term>
        ///         <description>GET operation to change the location of a resource.</description>
        ///     </item>
        ///     <item>
        ///         <term>remove</term>
        ///         <description>DELETE operation to remove a resource.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="arguments">
        /// Specifies the named parameter values required for executing <paramref name="action"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments);

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

        public Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments)
        {
            Contract.Requires<ArgumentException>(method == HttpMethod.Post || method == HttpMethod.Get || method == HttpMethod.Delete);
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
