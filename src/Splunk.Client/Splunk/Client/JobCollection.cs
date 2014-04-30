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
    /// <summary>
    /// 
    /// </summary>
    public class JobCollection : EntityCollection<JobCollection, Job>
    {
        #region Constructors

        internal JobCollection(Context context, Namespace @namespace, JobCollectionArgs args = null)
            : base(context, @namespace, ClassResourceName, args)
        { }

        internal JobCollection(Context context, Namespace @namespace, ResourceName resourceName)
            : base(context, @namespace, resourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "JobCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public JobCollection()
        { }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("search", "jobs");

        #endregion
    }
}
