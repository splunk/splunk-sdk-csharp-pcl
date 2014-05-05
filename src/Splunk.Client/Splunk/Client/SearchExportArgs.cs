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
// [ ]  Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for starting a new search export job.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/vJvIXv">REST API Reference: POST search/jobs/export</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class SearchExportArgs : Args<SearchExportArgs>
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "auto_cancel", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int AutoCancel
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "auto_finalize_ec", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int AutoFinalizeEventCount
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "auto_pause", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int AutoPause
        { get; set; }

        /// <summary>
        /// The maximum number of results to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is set to zero, then all available
        /// results are returned. The default value is 100.
        /// </remarks>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int Count
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string EarliestTime // TODO: Convenience class for specifying a time string. See 
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "enable_lookups", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool EnableLookups
        { get; set; }

        /// <summary>
        /// The list of fields to return in the results.
        /// </summary>
        [DataMember(Name = "f", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> FieldList
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "force_bundle_replication", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ForceBundleReplication
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Id
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "index_earliest", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string IndexEarliest
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "index_latest", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string IndexLatest
        { get; set; }

        /// <summary>
        /// The maximum lines that any single event's _raw field should contain. 
        /// </summary>
        /// <remarks>
        /// Specify zero to to indicate that all lines should be returned. The 
        /// default value is zero.
        /// </remarks>
        [DataMember(Name = "max_lines", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int MaxLines
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string LatestTime
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "max_time", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int MaxTime
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "namespace", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Namespace
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "now", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Now
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
        /// Formats a UTC time.
        /// </summary>
        /// <remarks>
        /// The default value is specified in time_format.
        /// </remarks>
        [DataMember(Name = "output_time_format", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string OutputTimeFormat
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "reduce_freq", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int ReduceFrequency
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "reload_macros", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool ReloadMacros
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "remote_server_list", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string RemoteServerList
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "rf", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> RequiredFieldList
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "rt_blocking", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool RealTimeBlocking
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "rt_indexfilter", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool RealTimeIndexFilter
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "rt_maxblocksecs", EmitDefaultValue = false)]
        [DefaultValue(60)]
        public int RealTimeMaxBlockSeconds
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "rt_queue_size", EmitDefaultValue = false)]
        [DefaultValue(10000)]
        public int RealTimeQueueSize
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "search_listener", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string SearchListener
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "search_mode", EmitDefaultValue = false)]
        [DefaultValue(typeof(SearchMode), "Normal")]
        public SearchMode SearchMode
        { get; set; }

        /// <summary>
        /// The type of segmentation to perform on the data.
        /// </summary>
        /// <remarks>
        /// This incudes an option to perform k/v segmentation.
        /// </remarks>
        [DataMember(Name = "segmentation", EmitDefaultValue = false)]
        [DefaultValue("raw")]
        public string Segmentation
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "sync_bundle_replication", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool SyncBundleReplication
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "time_format", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string TimeFormat
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "timeout", EmitDefaultValue = false)]
        [DefaultValue(86400)]
        public int Timeout
        { get; set; }

        /// <summary>
        /// Expression to convert a formatted time string from {start,end}_time
        /// into UTC seconds. 
        /// </summary>
        /// <remarks>
        /// The default value is <c>%m/%d/%Y:%H:%M:%S</c>.
        /// </remarks>
        [DataMember(Name = "truncation_mode", EmitDefaultValue = false)]
        [DefaultValue(TruncationMode.Abstract)]
        public TruncationMode TruncationMode
        { get; set; }

        #endregion
    }
}