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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for retrieving transformed
    /// <see cref="SearchResultStream"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/QFga96">REST API Reference: GET
    ///   search/jobs/{search_id}/results</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.SearchResultArgs}"/>
    public sealed class SearchResultArgs : Args<SearchResultArgs>
    {
        /// <summary>
        /// Gets or sets the maximum number of results to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is <c>0</c>, then all available results are
        /// returned. The default value is <c>100</c>.
        /// </remarks>
        /// <value>
        /// The maximum number of results to return.
        /// </value>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int Count
        { get; set; }

        /// <summary>
        /// Gets or sets the list of fields to return in the results.
        /// </summary>
        /// <value>
        /// The list of fields to return in the results.
        /// </value>
        [DataMember(Name = "f", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> FieldList
        { get; set; }

        /// <summary>
        /// Gets or sets an offset value specifying the first result inclusive from
        /// which to begin returning entries.
        /// </summary>
        /// <remarks>
        /// The <c>Offset</c> property is zero-based and cannot be negative. The
        /// default value is zero.
        /// </remarks>
        /// <value>
        /// An offset value specifying the first result inclusive from which to begin
        /// returning entries.
        /// </value>
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int Offset
        { get; set; }

        /// <summary>
        /// Gets or sets the post processing search to apply to the results.
        /// </summary>
        /// <remarks>
        /// The post processing search string can be any Splunk command.
        /// </remarks>
        /// <value>
        /// The post processing search to apply to the results.
        /// </value>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Search
        { get; set; }
    }
}