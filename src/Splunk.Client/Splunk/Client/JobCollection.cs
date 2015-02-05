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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk search jobs.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.EntityCollection{Splunk.Client.Job,Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IJobCollection{Splunk.Client.Job}"/>
    public class JobCollection : EntityCollection<Job, Resource>, IJobCollection<Job>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        ///
        /// ### <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal JobCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal JobCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal JobCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "JobCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public JobCollection()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual ReadOnlyCollection<Message> Messages
        {
            get { return this.Snapshot.GetValue("Messages") ?? NoMessages; }
        }

        /// <inheritdoc/>
        public virtual Pagination Pagination
        {
            get { return this.Snapshot.GetValue("Pagination") ?? Pagination.None; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// An object representing the search job that was created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JZcPEb">POST search/jobs</a>
        /// endpoint to start a new search <see cref="Job"/> as specified by
        /// <paramref name="arguments"/>.
        /// </remarks>
        public override async Task<Job> CreateAsync(IEnumerable<Argument> arguments)
        {
            return await this.CreateAsync(arguments, DispatchState.Running).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a new search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JZcPEb">POST search/jobs</a>
        /// endpoint to start a new search <see cref="Job"/> as specified by
        /// <paramref name="args"/>.
        /// </remarks>
        /// <param name="search">
        /// Search string.
        /// </param>
        /// <param name="count">
        ///                     
        /// </param>
        /// <param name="mode">
        ///                    
        /// </param>
        /// <param name="args">
        /// Optional search arguments.
        /// </param>
        /// <param name="customArgs">
        /// 
        /// </param>
        /// <param name="requiredState">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the search job that was created.
        /// </returns>
        public virtual async Task<Job> CreateAsync(string search, int count = 0, 
            ExecutionMode mode = ExecutionMode.Normal, JobArgs args = null, 
            CustomJobArgs customArgs = null, 
            DispatchState requiredState = DispatchState.Running)
        {
            var arguments = new Argument[] 
            {
               new Argument("search", search),
               new Argument("count", count)
            }
            .AsEnumerable();

            if (args != null)
            {
                arguments = arguments.Concat(args);
            }

            if (customArgs != null)
            {
                arguments = arguments.Concat(customArgs);
            }

            var job = await this.CreateAsync(arguments, requiredState).ConfigureAwait(false);
            return job;
        }

        /// <summary>
        /// Asynchronously retrieves a filtered collection of all running search jobs.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/ja2Sev">GET search/jobs</a>
        /// endpoint to get the filtered collection of running search jobs.
        /// </remarks>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public virtual async Task GetSliceAsync(JobCollection.Filter criteria)
        {
            await this.GetSliceAsync(criteria.AsEnumerable()).ConfigureAwait(false);
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Name of the class resource.
        /// </summary>
        internal static readonly ResourceName ClassResourceName = new ResourceName("search", "jobs");

        async Task<Job> CreateAsync(IEnumerable<Argument> arguments, DispatchState requiredState)
        {
            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, ClassResourceName, arguments).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created).ConfigureAwait(false);
                searchId = await response.XmlReader.ReadResponseElementAsync("sid").ConfigureAwait(false);
            }

            Job job = new Job(this.Context, this.Namespace, name: searchId);

            await job.GetAsync().ConfigureAwait(false);
            await job.TransitionAsync(requiredState).ConfigureAwait(false);

            return job;
        }

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
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.JobCollection.Filter}"/>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets the maximum number of <see cref="Job"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is <c>0</c>, then all available entries are
            /// returned. The default is <c>30</c>.
            /// </remarks>
            /// <value>
            /// The maximum number of <see cref="Job"/> entries to return.
            /// </value>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive)
            /// from which to begin returning <see cref="Job"/> entries.
            /// </summary>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. The
            /// default value is zero.
            /// </remarks>
            /// <value>
            /// Index of the first result (inclusive) from which to begin returning
            /// <see cref="Job"/> entries.
            /// </value>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref="Job"/>
            /// entries.
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on search
            /// <see cref="Job"/> properties. The default is <c>null</c>.
            /// </remarks>
            /// <value>
            /// A search expression to filter <see cref="Job"/> entries.
            /// </value>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort direction for <see cref="Job"/>
            /// entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            /// <value>
            /// The sort direction for <see cref="Job"/> entries.
            /// </value>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Descending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Job"/> property to use for sorting entries.
            /// </summary>
            /// <remarks>
            /// The default <see cref="Job"/> property to use for sorting is
            /// <c>"dispatch_time"</c>.
            /// </remarks>
            /// <value>
            /// The <see cref="Job"/> property to use for sorting entries.
            /// </value>
            [DataMember(Name = "sort_key", EmitDefaultValue = false)]
            [DefaultValue("dispatch_time")]
            public string SortKey
            { get; set; }
        }

        #endregion
    }
}
