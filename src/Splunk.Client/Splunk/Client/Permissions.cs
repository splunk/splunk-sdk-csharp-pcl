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
//// [ ] Name table (?)

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;

    /// <summary>
    /// Provides access to read/write permissions.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.Permissions}"/>
    public sealed class Permissions : ExpandoAdapter<Permissions>
    {
        /// <summary>
        /// Gets the set of Splunk user account names with read permissions.
        /// </summary>
        /// <value>
        /// The set of Splunk user account names with read permissions.
        /// </value>
        public ReadOnlyCollection<string> Read
        {
            get
            { 
                return this.GetValue(
                    "Read", ReadOnlyCollectionConverter<List<string>, StringConverter, string>.Instance); 
            }
        }

        /// <summary>
        /// Gets the set of Splunk user account names with write permissions.
        /// </summary>
        /// <value>
        /// The set of Splunk user account names with write permissions.
        /// </value>
        public ReadOnlyCollection<string> Write
        {
            get
            {
                return this.GetValue(
                    "Write", ReadOnlyCollectionConverter<List<string>, StringConverter, string>.Instance);
            }
        }
    }
}
