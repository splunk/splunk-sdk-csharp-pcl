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

// TODO:
// [ ] Documentation

namespace Splunk.Client
{
    public class SavedSearchCollection : EntityCollection<SavedSearchCollection, SavedSearch>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ns"></param>
        /// <param name="args"></param>
        internal SavedSearchCollection(Context context, Namespace ns, SavedSearchCollectionArgs args = null)
            : base(context, ns, ClassResourceName, args)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "SavedSearchCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetSavedSearchesAsync"/> to asynchronously retrieve a 
        /// collection of saved searches from Splunk.
        /// </remarks>
        public SavedSearchCollection()
        { }

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("saved", "searches");

        #endregion
    }
}
