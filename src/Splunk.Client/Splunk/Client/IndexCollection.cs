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
//// [O] Documentation

namespace Splunk.Client
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a collection of Splunk data indexes.
    /// </summary>
    public class IndexCollection : EntityCollection<IndexCollection, Index>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk services namespace.
        /// </param>
        internal IndexCollection(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "IndexCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetIndexesAsync"/> to asynchronously retrieve a collection 
        /// of Splunk data indexes.
        /// </remarks>
        public IndexCollection()
        { }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("data", "indexes");

        #endregion
    }
}
