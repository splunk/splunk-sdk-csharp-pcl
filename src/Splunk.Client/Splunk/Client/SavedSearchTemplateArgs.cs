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
    using System.Linq;

    /// <summary>
    /// Provides custom arguments to a <see cref="SavedSearch"/>.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ArgumentSet"/>
    public sealed class SavedSearchTemplateArgs : ArgumentSet
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchTemplateArgs"/>
        /// class.
        /// </summary>
        public SavedSearchTemplateArgs()
            : base("args.")
        { }

        /// <summary>
        /// Initializes a new instance of the Splunk.Client.SavedSearchTemplateArgs
        /// class.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public SavedSearchTemplateArgs(params Argument[] arguments)
            : this(arguments.AsEnumerable())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchTemplateArgs"/>
        /// class from a collection of <see cref="Argument"/> values.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public SavedSearchTemplateArgs(IEnumerable<Argument> arguments)
            : base("args.", arguments)
        { }

        #endregion
    }
}
