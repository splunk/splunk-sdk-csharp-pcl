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
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides a class that represents a Splunk ACL.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.EaiAttributes}"/>
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
        /// Gets the optional fields.
        /// </summary>
        /// <value>
        /// The optional fields.
        /// </value>
		public ReadOnlyCollection<string> OptionalFields
        {
            get
            {
                return this.GetValue(
                    "OptionalFields", ReadOnlyCollectionConverter<List<string>, StringConverter, string>.Instance);
            }
        }

        /// <summary>
        /// Gets the required fields.
        /// </summary>
        /// <value>
        /// The required fields.
        /// </value>
		public ReadOnlyCollection<string> RequiredFields
        {
            get
            {
                return this.GetValue(
                    "RequiredFields", ReadOnlyCollectionConverter<List<string>, StringConverter, string>.Instance);
            }
        }

        /// <summary>
        /// Gets the wildcard fields.
        /// </summary>
        /// <value>
        /// The wildcard fields.
        /// </value>
		public ReadOnlyCollection<string> WildcardFields
        {
            get
            {
                return this.GetValue(
                    "WildcardFields", ReadOnlyCollectionConverter<List<string>, StringConverter, string>.Instance);
            }
        }

        #endregion
    }
}
