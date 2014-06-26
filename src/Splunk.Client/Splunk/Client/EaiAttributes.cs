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
    /// Provides a class that represents a Splunk ACL.
    /// </summary>
    public sealed class EaiAttributes : ExpandoAdapter<EaiAttributes>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EaiAttributes"/> class.
        /// </summary>
        public EaiAttributes()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
		public IReadOnlyList<string> OptionalFields
        {
            get { return this.GetValue("OptionalFields", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
		public IReadOnlyList<string> RequiredFields
        {
            get { return this.GetValue("RequiredFields", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
		public IReadOnlyList<string> WildcardFields
        {
            get { return this.GetValue("WildcardFields", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        #endregion
    }
}
