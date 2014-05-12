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
//// [O]  Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for retrieving an <see cref="IndexCollection"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/qVZ6wJ">REST API Reference: GET data/indexes</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class IndexCollectionArgs : Args<IndexCollectionArgs>
    {
        #region Properties

        /// <summary>
        /// The maximum number of <see cref="Index"/> entries to return.
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
        /// The first result (inclusive) from which to begin returning data.
        /// </summary>
        /// <remarks>
        /// The value of <c>Offset</c> is zero-based and cannot be 
        /// negative. The default value is zero.
        /// </remarks>
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int Offset
        { get; set; }

        /// <summary>
        /// Search expression to filter <see cref="Index"/> entries. 
        /// </summary>
        /// <remarks>
        /// Use this expression to filter the entries returned based on 
        /// search <see cref="Index"/> properties. The default is <c>null</c>.
        /// </remarks>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Search
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sort returned <see cref=
        /// "Index"/> entries in ascending or descending order.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="SortDirection"/>.Ascending.
        /// </remarks>
        [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
        [DefaultValue(SortDirection.Ascending)]
        public SortDirection SortDirection
        { get; set; }

        /// <summary>
        /// <see cref="Index"/> property to use for sorting.
        /// </summary>
        /// <remarks>
        /// The default <see cref="Index"/> property to use for sorting is 
        /// <c>"name"</c>.
        /// </remarks>
        [DataMember(Name = "sort_key", EmitDefaultValue = false)]
        [DefaultValue("name")]
        public string SortKey
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the <see cref="SortMode"/> for <see
        /// cref="Index"/> entries.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="SortMode"/>.Automatic.
        /// </remarks>
        [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
        [DefaultValue(SortMode.Automatic)]
        public SortMode SortMode
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to leave out certain index 
        /// details in order to provide a faster response.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "summarize", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool Summarize
        { get; set; }

        #endregion
    }
}