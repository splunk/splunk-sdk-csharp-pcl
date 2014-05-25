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
//// [ ] Resource aggregates Endpoint (?)

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public interface IResource<T> : IEquatable<T>
    {
        #region properties
        /// <summary>
        /// Gets the author of the current <see cref="Resource"/>.
        /// </summary>
        string Author
        { get; }

        /// <summary>
        /// Gets the version of the Atom Feed generator that produced the
        /// current <see cref="Resource"/>.
        /// </summary>
        Version GeneratorVersion
        { get; }

        /// <summary>
        /// Gets the Splunk management URI for accessing the current <see cref=
        /// "Resource"/>.
        /// </summary>
        Uri Id
        { get; }

        /// <summary>
        /// Gets the collection of service addresses supported by the current
        /// <see cref="Resource"/>.
        /// </summary>
        IReadOnlyDictionary<string, Uri> Links
        { get; }

        string Title
        { get; }

        /// <summary>
        /// Gets the date that the interface to this resource type was introduced
        /// or updated by Splunk.
        /// </summary>
        DateTime Updated
        { get; }

        #endregion
    }
}
