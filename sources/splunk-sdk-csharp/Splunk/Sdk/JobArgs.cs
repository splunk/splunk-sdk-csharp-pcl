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

namespace Splunk.Sdk
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for starting a new search job.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/OWTUws">REST API Reference: POST search/jobs</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class JobArgs : Args<JobArgs>
    {
        #region Constructors

        public JobArgs()
        { }

        public JobArgs(string search)
        {
            this.Search = search;
        }

        #endregion

        #region Properties

        [DataMember(Name = "search", IsRequired = true)]
        public string Search
        { get; set; }

        [DataMember(Name = "auto_cancel", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int AutoCancel
        { get; set; }
        
        [DataMember(Name = "auto_finalize_ec", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int AutoFinalizeEventCount
        { get; set; }

        [DataMember(Name = "auto_pause", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int AutoPause
        { get; set; }

        [DataMember(Name = "earliest_time", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string EarliestTime // TODO: Convenience class for specifying a time string. See 
        { get; set; }

        [DataMember(Name = "enable_lookups", EmitDefaultValue=false)]
        [DefaultValue(true)]
        public bool EnableLookups
        { get; set; }

        [DataMember(Name = "exec_mode", EmitDefaultValue=false)]
        [DefaultValue(typeof(ExecutionMode), "Normal")]
        public ExecutionMode ExecutionMode
        { get; set; }

        [DataMember(Name = "force_bundle_replication", EmitDefaultValue=false)]
        [DefaultValue(false)]
        public bool ForceBundleReplication
        { get; set; }

        [DataMember(Name = "id", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string Id
        { get; set; }

        [DataMember(Name = "index_earliest", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string IndexEarliest
        { get; set; }

        [DataMember(Name = "index_latest", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string IndexLatest
        { get; set; }

        [DataMember(Name = "latest_time", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string LatestTime
        { get; set; }

        [DataMember(Name = "max_count", EmitDefaultValue=false)]
        [DefaultValue(10000)]
        public int MaxCount
        { get; set; }

        [DataMember(Name = "max_time", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int MaxTime
        { get; set; }

        [DataMember(Name = "namespace", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string Namespace
        { get; set; }

        [DataMember(Name = "now", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string Now
        { get; set; }

        [DataMember(Name = "reduce_freq", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int ReduceFrequency
        { get; set; }

        [DataMember(Name = "reload_macros", EmitDefaultValue=false)]
        [DefaultValue(true)]
        public bool ReloadMacros
        { get; set; }

        [DataMember(Name = "remote_server_list", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string RemoteServerList
        { get; set; }

        [DataMember(Name = "rf", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> RequiredFieldList
        { get; set; }

        [DataMember(Name = "reuse_max_seconds_ago", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int ReuseMaxSecondsAgo
        { get; set; }

        [DataMember(Name = "rt_blocking", EmitDefaultValue=false)]
        [DefaultValue(false)]
        public bool RealTimeBlocking
        { get; set; }

        [DataMember(Name = "rt_indexfilter", EmitDefaultValue=false)]
        [DefaultValue(false)]
        public bool RealTimeIndexFilter
        { get; set; }

        [DataMember(Name = "rt_maxblocksecs", EmitDefaultValue=false)]
        [DefaultValue(60)]
        public int RealTimeMaxBlockSeconds
        { get; set; }

        [DataMember(Name = "rt_queue_size", EmitDefaultValue=false)]
        [DefaultValue(10000)]
        public int RealTimeQueueSize
        { get; set; }

        [DataMember(Name = "search_listener", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string SearchListener
        { get; set; }

        [DataMember(Name = "search_mode", EmitDefaultValue=false)]
        [DefaultValue(typeof(SearchMode), "Normal")]
        public SearchMode SearchMode
        { get; set; }

        [DataMember(Name = "spawn_process", EmitDefaultValue=false)]
        [DefaultValue(true)]
        public bool SpawnProcess
        { get; set; }

        [DataMember(Name = "status_buckets", EmitDefaultValue=false)]
        [DefaultValue(0)]
        public int StatusBuckets
        { get; set; }

        [DataMember(Name = "sync_bundle_replication", EmitDefaultValue=false)]
        [DefaultValue(false)]
        public bool SyncBundleReplication
        { get; set; }

        [DataMember(Name = "time_format", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public string TimeFormat
        { get; set; }

        [DataMember(Name = "timeout", EmitDefaultValue=false)]
        [DefaultValue(86400)]
        public int Timeout
        { get; set; }

        #endregion
    }
}