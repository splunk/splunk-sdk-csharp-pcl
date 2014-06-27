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
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving untransformed search
    /// results.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/eZzuBh">REST API Reference: POST
    ///   search/jobs/{search_id}/events</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.SearchEventArgs}"/>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification =
        "This is by design.")
    ]
    public sealed class SearchEventArgs : Args<SearchEventArgs>
    {
        /// <summary>
        /// Gets or sets the maximum number of results to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is set to zero, then all available results
        /// are returned. The default value is 100.
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
        /// Gets or sets a time string representing the latest (exclusive),
        /// respectively, time bounds for the results to be returned.
        /// </summary>
        /// <remarks>
        /// If not specified, the range applies to all results found.
        /// </remarks>
        /// <value>
        /// A time string representing the latest (exclusive), respectively, time
        /// bounds for the results to be returned.
        /// </value>
        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string LatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum lines that any single event's <c>_raw</c>
        /// field should contain.
        /// </summary>
        /// <remarks>
        /// Specify <c>0</c> to indicate that all lines should be returned. The
        /// default value is <c>0</c>.
        /// </remarks>
        /// <value>
        /// The maximum lines that any single event's <c>_raw</c> field should
        /// contain.
        /// </value>
        [DataMember(Name = "max_lines", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int MaxLines
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the first result (inclusive) from which
        /// to begin returning entries.
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
        /// Gets or sets the output format for a UTC time.
        /// </summary>
        /// <remarks>
        /// The default value is specified in <see cref="TimeFormat"/>.
        /// </remarks>
        /// <value>
        /// The output format for a UTC time.
        /// </value>
        [DataMember(Name = "output_time_format", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string OutputTimeFormat
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

        /// <summary>
        /// Gets or sets the type of segmentation to perform on the data.
        /// </summary>
        /// <remarks>
        /// This incudes an option to perform k/v segmentation.
        /// </remarks>
        /// <value>
        /// The type of segmentation to perform on the data.
        /// </value>
        [DataMember(Name = "segmentation", EmitDefaultValue = false)]
        [DefaultValue("raw")]
        public string Segmentation
        { get; set; }

        /// <summary>
        /// Gets or sets an expression to convert a formatted time string from
        /// {start,end}_time into UTC seconds.
        /// </summary>
        /// <remarks>
        /// The default value is <c>%m/%d/%Y:%H:%M:%S</c>.
        /// </remarks>
        /// <value>
        /// An expression to convert a formatted time string from
        /// {start,end}_time into UTC seconds.
        /// </value>
        [DataMember(Name = "time_format", EmitDefaultValue = false)]
        [DefaultValue("%m/%d/%Y:%H:%M:%S")]
        public string TimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies how <see cref="MaxLines"/>
        /// should be achieved.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="TruncationMode"/><c>.Abstract</c>.
        /// </remarks>
        /// <value>
        /// A truncation mode that specifies how <see cref="MaxLines"/> should be
        /// achieved.
        /// </value>
        [DataMember(Name = "truncation_mode", EmitDefaultValue = false)]
        [DefaultValue(TruncationMode.Abstract)]
        public TruncationMode TruncationMode
        { get; set; }
    }
}