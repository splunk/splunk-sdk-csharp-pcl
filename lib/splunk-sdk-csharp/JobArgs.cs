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
    /// 
    /// </summary>
    [DataContract]
    public class JobArgs : Args<JobArgs>
    {
        [DataMember(Name = "search", IsRequired = true)]
        public string Search
        { get; set; }

        [DataMember(Name = "auto_cancel")]
        [DefaultValue(0)]
        public int AutoCancel
        { get; set; }
        
        [DataMember(Name = "auto_finalize_ec")]
        [DefaultValue(0)]
        public int AutoFinalizeEventCount
        { get; set; }

        [DataMember(Name = "auto_pause")]
        [DefaultValue(0)]
        public int AutoPause
        { get; set; }

        [DataMember(Name = "earliest_time")]
        [DefaultValue(null)]
        public string EarliestTime // TODO: Convenience class for specifying a time string. See 
        { get; set; }

        [DataMember(Name = "enable_lookups")]
        [DefaultValue(true)]
        public bool EnableLookups
        { get; set; }

        [DataMember(Name = "exec_mode")]
        [DefaultValue(typeof(ExecutionMode), "Normal")]
        public ExecutionMode ExecutionMode
        { get; set; }

        [DataMember(Name = "force_bundle_replication")]
        [DefaultValue(false)]
        public bool ForceBundleReplication
        { get; set; }

        [DataMember(Name = "id")]
        [DefaultValue(null)]
        public string Id
        { get; set; }

        [DataMember(Name = "index_earliest")]
        [DefaultValue(null)]
        public string IndexEarliest
        { get; set; }

        [DataMember(Name = "index_latest")]
        [DefaultValue(null)]
        public string IndexLatest
        { get; set; }

        [DataMember(Name = "latest_time")]
        [DefaultValue(null)]
        public string LatestTime
        { get; set; }

        [DataMember(Name = "max_count")]
        [DefaultValue(10000)]
        public int MaxCount
        { get; set; }

        [DataMember(Name = "max_time")]
        [DefaultValue(0)]
        public int MaxTime
        { get; set; }

        [DataMember(Name = "namespace")]
        [DefaultValue(null)]
        public string Namespace
        { get; set; }

        [DataMember(Name = "now")]
        [DefaultValue(null)]
        public string Now
        { get; set; }

        [DataMember(Name = "reduce_freq")]
        [DefaultValue(0)]
        public int ReduceFrequency
        { get; set; }

        [DataMember(Name = "reload_macros")]
        [DefaultValue(true)]
        public bool ReloadMacros
        { get; set; }

        [DataMember(Name = "remote_server_list")]
        [DefaultValue(null)]
        public string RemoteServerList
        { get; set; }

        [DataMember(Name = "rf")]
        [DefaultValue(null)]
        public IReadOnlyList<string> RequiredFieldList
        { get; set; }

        [DataMember(Name = "reuse_max_seconds_ago")]
        [DefaultValue(0)]
        public int ReuseMaxSecondsAgo
        { get; set; }

        [DataMember(Name = "rt_blocking")]
        [DefaultValue(false)]
        public bool RealTimeBlocking
        { get; set; }

        [DataMember(Name = "rt_indexfilter")]
        [DefaultValue(false)]
        public bool RealTimeIndexFilter
        { get; set; }

        [DataMember(Name = "rt_maxblocksecs")]
        [DefaultValue(60)]
        public int RealTimeMaxBlockSeconds
        { get; set; }

        [DataMember(Name = "rt_queue_size")]
        [DefaultValue(10000)]
        public int RealTimeQueueSize
        { get; set; }

        [DataMember(Name = "search_listener")]
        [DefaultValue(null)]
        public string SearchListener
        { get; set; }

        [DataMember(Name = "search_mode")]
        [DefaultValue(typeof(SearchMode), "Normal")]
        public SearchMode SearchMode
        { get; set; }

        [DataMember(Name = "spawn_process")]
        [DefaultValue(true)]
        public bool SpawnProcess
        { get; set; }

        [DataMember(Name = "status_buckets")]
        [DefaultValue(0)]
        public int StatusBuckets
        { get; set; }

        [DataMember(Name = "sync_bundle_replication")]
        [DefaultValue(false)]
        public bool SyncBundleReplication
        { get; set; }

        [DataMember(Name = "time_format")]
        [DefaultValue(null)]
        public string TimeFormat
        { get; set; }

        [DataMember(Name = "timeout")]
        [DefaultValue(86400)]
        public int Timeout
        { get; set; }
    }
}