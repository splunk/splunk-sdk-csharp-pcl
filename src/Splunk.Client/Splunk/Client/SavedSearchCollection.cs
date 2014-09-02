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
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a collection of saved searches.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.EntityCollection{Splunk.Client.SavedSearch,Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.ISavedSearchCollection{Splunk.Client.SavedSearch}"/>
    public class SavedSearchCollection : EntityCollection<SavedSearch, Resource>, ISavedSearchCollection<SavedSearch>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal SavedSearchCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollection"/>
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
        protected internal SavedSearchCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        protected internal SavedSearchCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "SavedSearchCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public SavedSearchCollection()
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

        /// <inheritdoc/>
        public virtual async Task<SavedSearch> CreateAsync(string name, string search, 
            SavedSearchAttributes attributes = null, 
            SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            var args = new Argument[]
            { 
                new Argument("name", name), 
                new Argument("search", search) 
            }
            .AsEnumerable();
            
            if (attributes != null)
            {
                args = args.Concat(attributes);
            }

            if (dispatchArgs != null)
            {
                args = args.Concat(dispatchArgs);
            }

            if (templateArgs != null)
            {
                args = args.Concat(templateArgs);
            }

            var savedSearch = await this.CreateAsync(args).ConfigureAwait(false);
            return savedSearch;
        }

        /// <inheritdoc/>
        public virtual async Task GetSliceAsync(Filter criteria)
        {
            await this.GetSliceAsync(criteria.AsEnumerable()).ConfigureAwait(false);
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Name of the class resource.
        /// </summary>
        internal static readonly ResourceName ClassResourceName = new ResourceName("saved", "searches");

        #endregion

        #region Types

        /// <summary>
        /// Provides the arguments required for retrieving <see cref="SavedSearch"/>
        /// entries.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///     <a href="http://goo.gl/bKrRK0">REST API: GET saved/searches</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.SavedSearchCollection.Filter}"/>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets a value specifying the maximum number of
            /// <see cref= "SavedSearch"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is set to zero, then all
            /// <see cref= "SavedSearch"/> entries are returned. The default value is
            /// <c>30</c>.
            /// </remarks>
            /// <value>
            /// A value specifying the maximum number of <see cref="SavedSearch"/>
            /// entries to return.
            /// </value>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets the lower bound of the time window for which
            /// <see cref="SavedSearch"/> schedules should be returned.
            /// </summary>
            /// <remarks>
            /// This property specifies that all the scheduled times starting from this
            /// time (not just the next run time) should be returned.
            /// </remarks>
            /// <value>
            /// The lower bound of the time window for which <see cref= "SavedSearch"/>
            /// schedules should be returned.
            /// </value>
            [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string EarliestTime
            { get; set; }

            /// <summary>
            /// Gets or sets the upper bound of the time window for which
            /// <see cref="SavedSearch"/> schedules should be returned.
            /// </summary>
            /// <remarks>
            /// This property specifies that all the scheduled times ending with this
            /// time (not just the next run time) should be returned.
            /// </remarks>
            /// <value>
            /// The upper bound of the time window for which <see cref= "SavedSearch"/>
            /// schedules should be returned.
            /// </value>
            [DataMember(Name = "latest_time", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string LatestTime
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to list default actions for
            /// <see cref="SavedSearch"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <c>false</c>.
            /// </remarks>
            /// <value>
            /// <c>true</c>, if default actions should be listed; <c>false</c>
            /// otherwise.
            /// </value>
            [DataMember(Name = "listDefaultActionArgs", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool ListDefaultActions
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive)
            /// from which to begin returning entries.
            /// </summary>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. The
            /// default value is zero.
            /// </remarks>
            /// <value>
            /// A value specifying the first result (inclusive) from which to begin
            /// returning entries.
            /// </value>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref= "SavedSearch"/>
            /// entries.
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on
            /// <see cref="SavedSearch"/> properties.
            /// </remarks>
            /// <value>
            /// A search expression to filter <see cref="SavedSearch"/> entries.
            /// </value>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search // TODO: Good search example for App
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort direction for
            /// <see cref="SavedSearch"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            /// <value>
            /// The sort direction for <see cref="SavedSearch"/> entries.
            /// </value>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Ascending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="SavedSearch"/> property to use for sorting
            /// entries.
            /// </summary>
            /// <remarks>
            /// The default <see cref="SavedSearch"/> property to use for sorting is
            /// <c>"name"</c>.
            /// </remarks>
            /// <value>
            /// The <see cref="SavedSearch"/> property to use for sorting entries.
            /// </value>
            [DataMember(Name = "sort_key", EmitDefaultValue = false)]
            [DefaultValue("name")]
            public string SortKey
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort mode for
            /// <see cref= "SavedSearch"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortMode"/>.Automatic.
            /// </remarks>
            /// <value>
            /// The sort mode for <see cref="SavedSearch"/> entries.
            /// </value>
            [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
            [DefaultValue(SortMode.Automatic)]
            public SortMode SortMode
            { get; set; }
        }

        #endregion
    }
}
