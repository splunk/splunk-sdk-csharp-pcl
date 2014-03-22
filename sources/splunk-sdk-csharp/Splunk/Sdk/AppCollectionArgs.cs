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

namespace Splunk.Sdk
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving transformed search results.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/izvjYx">REST API Reference: GET search/jobs/{search_id}/results</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class AppCollectionArgs : Args<AppCollectionArgs>
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        public AppCollectionArgs()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        public AppCollectionArgs(string search)
        {
            this.Search = search;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value specifying the maximum number of <see cref=
        /// "App"/> entries to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is set to zero, then all <see cref=
        /// "App"/> entries are returned. The default value is 30.
        /// </remarks>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        [DefaultValue(30)]
        public int Count
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the first result (inclusive) from 
        /// which to begin returning <see cref="App"/> entries.
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
        /// Gets or sets a value indicating whether to scan for new <see cref=
        /// "App"/> instances and reload any objects those new <see cref="App"/>
        /// instances contain.
        /// </summary>
        /// <remarks>
        /// The default is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "refresh", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool Refresh // TODO: Verify default value (it's not in the docs)
        { get; set; }

        /// <summary>
        /// Gets or sets a search expression to filter <see cref="App"/> 
        /// instances. 
        /// </summary>
        /// <remarks>
        /// Use this expression to filter the entries returned based on <see
        /// cref="App"/> properties.
        /// </remarks>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Search // TODO: Good search example for App
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sort returned <see cref=
        /// "App"/>entries in ascending or descending order.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="SortDirection.Ascending"/>.
        /// </remarks>
        [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
        [DefaultValue(SortDirection.Ascending)]
        public SortDirection SortDirection
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the <see cref="SortMode"/> for <see
        /// cref="App"/> entries.
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
