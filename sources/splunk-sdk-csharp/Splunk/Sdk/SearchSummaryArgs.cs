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

namespace Splunk.Sdk
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving a search <see cref=
    /// "Job"/> summary.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/rfH6qN">REST API Reference: GET search/jobs/{search_id}/summary</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class SearchSummaryArgs : Args<SearchSummaryArgs>
    {
        #region Constructors

        public SearchSummaryArgs()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a string representing the earliest time bounds for the
        /// search.
        /// </summary>
        /// <remarks>
        /// The string can be either a UTC time (with fractional seconds), a 
        /// relative time specifier or a formatted time string.
        /// </remarks>
        [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string EarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets the list of fields to include in the search <see cref=
        /// "Job"/> summary.
        /// </summary>
        [DataMember(Name = "f", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> FieldList
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add histogram data to the
        /// search <see cref="Job"/> summary.
        /// </summary>
        [DataMember(Name = "histogram", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public int Histogram
        { get; set; }

        /// <summary>
        /// Gets or sets a string representing the latest time bounds for a
        /// search <see cref="Job"/> summary.
        /// </summary>
        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public string LatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that filters fields in <see cref="FieldList"/>
        /// based on their rate of occurrence in the event stream.
        /// </summary>
        /// <remarks>
        /// Express the rate as a <see cref="Double"/> in the closed interval
        /// between zero and one ([0,1]). The default is zero, indicating that
        /// all fields should be included in the search <see cref="Job"/>
        /// summary.
        /// </remarks>
        [DataMember(Name = "min_freq", EmitDefaultValue = false)]
        [DefaultValue(0D)]
        public double MinimumFrequency
        { get; set; }

        /// <summary>
        /// Gets or sets a string specifying the time format to use in a
        /// search <see cref="Job"/> summary.
        /// </summary>
        /// <remarks>
        /// The default value is the <c>TIME_FORMAT</c> specified in 
        /// <a href="http://goo.gl/S0QXN3">props.conf</a>.
        /// </remarks>
        [DataMember(Name = "output_time_format", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string OutputTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets a search string that filters events from a search 
        /// <see cref="Job"/> summary based on field values or tags.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="String.Empty"/>
        /// </remarks>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [DefaultValue("")]
        public string Search
        { get; set; }

        /// <summary>
        /// Gets or sets the formatted time string used to convert formatted
        /// time strings <c>{start,end}_time</c> into UTC seconds.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"%m/%d/%Y:%H:%M:%S"</c>.
        /// </remarks>
        [DataMember(Name = "time_format", EmitDefaultValue = false)]
        [DefaultValue("%m/%d/%Y:%H:%M:%S")]
        public string TimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets a count that filters events from a search summary based
        /// on rank.
        /// </summary>
        /// <remarks>
        /// The default value is 10, indicating that the top 10 values for each
        /// field in <see cref="FieldList"/> should be included in the search 
        /// <see cref="Job"/> summary.
        /// </remarks>
        [DataMember(Name = "top_count", EmitDefaultValue = false)]
        [DefaultValue(10)]
        public int TopCount
        { get; set; }

        #endregion
    }
}