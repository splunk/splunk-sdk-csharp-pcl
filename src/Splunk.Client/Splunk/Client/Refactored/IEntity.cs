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
//// [ ] Check for HTTP Status Code 204 (No Content) and empty atoms in 
////     Entity<TEntity>.UpdateAsync.
////
//// [O] Contracts
////
//// [O] Documentation
////
//// [X] Pick up standard properties from AtomEntry on Update, not just AtomEntry.Content
////     See [Splunk responses to REST operations](http://goo.gl/tyXDfs).
////
//// [X] Remove Entity<TEntity>.Invalidate method
////     FJR: This gets called when we set the record value. Add a comment saying what it's
////     supposed to do when it's overridden.
////     DSN: I've adopted an alternative method for getting strongly-typed values. See, for
////     example, Job.DispatchState or ServerInfo.Guid.

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides the operational interface common to all Splunk entities.
    /// </summary>
    public interface IEntity
    {
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
    }
}
