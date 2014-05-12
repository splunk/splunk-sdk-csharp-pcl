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
    /// <summary>
    /// Provides an object representation of a collection of Splunk search jobs.
    /// </summary>
    public class JobCollection : EntityCollection<JobCollection, Job>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="args">
        /// Arguments for retrieving the <see cref="JobCollection"/>.
        /// </param>
        internal JobCollection(Context context, Namespace ns, JobCollectionArgs args = null)
            : base(context, ns, ClassResourceName, args)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resourceName">
        /// </param>
        /// <remarks>
        /// This constructor is used by the <see cref="SavedSearch.GetHistoryAsync"/>
        /// method to retrieve the collection of jobs created from a saved
        /// search.
        /// </remarks>
        internal JobCollection(Context context, Namespace ns, ResourceName resourceName)
            : base(context, ns, resourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "JobCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetJobsAsync"/> to asynchronously retrieve a collection of
        /// running Splunk jobs.
        /// </remarks>
        public JobCollection()
        { }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("search", "jobs");

        #endregion
    }
}
