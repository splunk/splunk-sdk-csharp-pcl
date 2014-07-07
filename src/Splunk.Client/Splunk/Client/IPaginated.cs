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
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides an operational interface to the Splunk application collection.
    /// </summary>
    public interface IPaginated
    {
        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        ReadOnlyCollection<Message> Messages
        { get; }

        /// <summary>
        /// Gets the pagination properties for the current Splunk entity collection.
        /// </summary>
        /// <value>
        /// The pagination.
        /// </value>
        Pagination Pagination
        { get; }
    }
}
