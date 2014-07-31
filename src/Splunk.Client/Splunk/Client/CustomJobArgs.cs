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

    /// <summary>
    /// Provides custom arguments to a <see cref="Job"/>.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ArgumentSet"/>
    public sealed class CustomJobArgs : ArgumentSet
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJobArgs"/>
        /// class.
        /// </summary>
        public CustomJobArgs()
            : base("custom.")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJobArgs"/>
        /// class from a collection of <see cref="Argument"/> values.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public CustomJobArgs(IEnumerable<Argument> collection)
            : base("custom.", collection)
        { }

        #endregion
    }
}
