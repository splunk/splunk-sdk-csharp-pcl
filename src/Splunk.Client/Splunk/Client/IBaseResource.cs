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

    /// <summary>
    /// Interface for base resource.
    /// </summary>
    public interface IBaseResource
    {
        /// <summary>
        /// Gets the generator version.
        /// </summary>
        /// <value>
        /// The generator version.
        /// </value>
        Version GeneratorVersion { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Uri Id { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Gets the Date/Time of the updated.
        /// </summary>
        /// <value>
        /// The updated.
        /// </value>
        DateTime Updated { get; }
    }
}
