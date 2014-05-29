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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    /// <summary>
    /// Provides an object representation of a collection of Splunk search jobs.
    /// </summary>
    public class JobCollection : EntityCollection<Job>
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

        #region Types

        /// <summary>
        /// Provides arguments for retrieving a <see cref="JobCollection"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/ja2Sev">REST API Reference: GET search/jobs</a>.
        /// </description></item>
        /// </list>
        /// </remarks>
        public sealed class JobCollectionArgs : Args<JobCollectionArgs>
        {
            /// <summary>
            /// The maximum number of <see cref="Job"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is set to zero, then all available
            /// results are returned. The default value is 30.
            /// </remarks>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive) from 
            /// which to begin returning entries.
            /// </summary>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. 
            /// The default value is zero.
            /// </remarks>
            /// <remarks>
            /// This value is zero-based and cannot be negative. The default value
            /// is zero.
            /// </remarks>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Search expression to filter <see cref="Job"/> entries. 
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on 
            /// search <see cref="Job"/> properties. For example, specify 
            /// <c>eventCount>100</c>. The default is <c>null</c>.
            /// </remarks>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to sort returned <see cref=
            /// "Application"/>entries in ascending or descending order.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Descending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// <see cref="Job"/> property to use for sorting.
            /// </summary>
            /// <remarks>
            /// The default <see cref="Job"/> property to use for sorting is 
            /// <c>"dispatch_time"</c>.
            /// </remarks>
            [DataMember(Name = "sort_key", EmitDefaultValue = false)]
            [DefaultValue("dispatch_time")]
            public string SortKey
            { get; set; }
        }

        #endregion
    }
}
