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
    ///     <a href="http://goo.gl/QFga96">REST API Reference: GET search/jobs/{search_id}/results</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class SearchResultArgs : Args<SearchResultArgs>
    {
        #region Constructors

        public SearchResultArgs()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// The maximum number of results to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is set to zero, then all available
        /// results are returned. The default value is 100.
        /// </remarks>
        [DataMember(Name = "count", EmitDefaultValue=false)]
        [DefaultValue(100)]
        public int Count
        { get; set; }

        /// <summary>
        /// The list of fields to return in the results.
        /// </summary>
        [DataMember(Name = "f", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> FieldList
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
        /// The post processing search to apply to the results.
        /// </summary>
        /// <remarks>
        /// The post processing search string can be any Splunk command.
        /// </remarks>
        [DataMember(Name = "search", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string Search
        { get; set; }

        #endregion
    }
}