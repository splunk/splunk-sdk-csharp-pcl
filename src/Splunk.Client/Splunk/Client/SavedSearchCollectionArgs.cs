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
// [O] Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

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
    public sealed class SavedSearchCollectionArgs : Args<SavedSearchCollectionArgs>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollectionArgs"/> 
        /// class.
        /// </summary>
        public SavedSearchCollectionArgs()
        { }
 
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value specifying the maximum number of <see cref=
        /// "SavedSearch"/> entries to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is set to zero, then all <see cref=
        /// "SavedSearch"/> entries are returned. The default value is 30.
        /// </remarks>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        [DefaultValue(30)]
        public int Count
        { get; set; }

        /// <summary>
        /// Gets or sets the lower bound of the time window for which saved 
        /// search schedules should be returned.
        /// </summary>
        /// <remarks>
        /// This property specifies that all the scheduled times starting from 
        /// this time (not just the next run time) should be returned.
        /// </remarks>
        [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string EarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets the upper bound of the time window for which saved 
        /// search schedules should be returned.
        /// </summary>
        /// <remarks>
        /// This property specifies that all the scheduled times ending with 
        /// this time (not just the next run time) should be returned.
        /// </remarks>
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
        [DataMember(Name = "listDefaultActionArgs", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ListDefaultActions
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the first result (inclusive) from 
        /// which to begin returning <see cref="SavedSearch"/> entries.
        /// </summary>
        /// <remarks>
        /// This value is zero-based and cannot be negative. The default value
        /// is zero.
        /// </remarks>
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int Offset
        { get; set; }

        /// <summary>
        /// Gets or sets a search expression to filter <see cref=
        /// "SavedSearch"/> entries.
        /// </summary>
        /// <remarks>
        /// Use this expression to filter the entries returned based on <see
        /// cref="SavedSearch"/> properties.
        /// </remarks>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Search // TODO: Good search example for App
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sort returned <see cref=
        /// "SavedSearch"/>entries in ascending or descending order.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="SortDirection.Ascending"/>.
        /// </remarks>
        [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
        [DefaultValue(SortDirection.Ascending)]
        public SortDirection SortDirection
        { get; set; }

        /// <summary>
        /// <see cref="Job"/> property to use for sorting.
        /// </summary>
        /// <remarks>
        /// The default <see cref="SavedSearch"/> property to use for sorting 
        /// is <c>"name"</c>.
        /// </remarks>
        [DataMember(Name = "sort_key", EmitDefaultValue = false)]
        [DefaultValue("name")]
        public string SortKey
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the <see cref="SortMode"/> for <see
        /// cref="Application"/> entries.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="SortMode.Automatic"/>.
        /// </remarks>
        [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
        [DefaultValue(SortMode.Automatic)]
        public SortMode SortMode
        { get; set; }

        #endregion
    }
}
