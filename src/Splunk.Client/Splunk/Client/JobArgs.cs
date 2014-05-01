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
//// [ ]  Documentation
//// [ ] Class for specifying a time string.

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for creating a search <see cref="Job"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/OWTUws">REST API Reference: POST search/jobs</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class JobArgs : Args<JobArgs>
    {
        #region Properties

        [DataMember(Name = "auto_cancel", EmitDefaultValue = false)]
        public int? AutoCancel
        { get; set; }
        
        [DataMember(Name = "auto_finalize_ec", EmitDefaultValue = false)]
        public int? AutoFinalizeEventCount
        { get; set; }

        [DataMember(Name = "auto_pause", EmitDefaultValue = false)]
        public int? AutoPause
        { get; set; }

        [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
        public string EarliestTime
        { get; set; }

        [DataMember(Name = "enable_lookups", EmitDefaultValue = false)]
        public bool? EnableLookups
        { get; set; }

        [DataMember(Name = "exec_mode", EmitDefaultValue = false)]
        public ExecutionMode? ExecutionMode
        { get; set; }

        [DataMember(Name = "force_bundle_replication", EmitDefaultValue = false)]
        public bool? ForceBundleReplication
        { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id
        { get; set; }

        [DataMember(Name = "index_earliest", EmitDefaultValue = false)]
        public string IndexEarliest
        { get; set; }

        [DataMember(Name = "index_latest", EmitDefaultValue = false)]
        public string IndexLatest
        { get; set; }

        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        public string LatestTime
        { get; set; }

        [DataMember(Name = "max_count", EmitDefaultValue = false)]
        public int? MaxCount
        { get; set; }

        [DataMember(Name = "max_time", EmitDefaultValue = false)]
        public int? MaxTime
        { get; set; }

        /// <summary>
        /// The application namespace in which to restrict searches. 
        /// </summary>
        /// <remarks>
        /// The namespace corresponds to the identifier recognized in the 
        /// /services/apps/local endpoint.
        /// </remarks>
        [DataMember(Name = "namespace", EmitDefaultValue = false)]
        public string Namespace
        { get; set; }

        [DataMember(Name = "now", EmitDefaultValue = false)]
        public string Now
        { get; set; }

        [DataMember(Name = "reduce_freq", EmitDefaultValue = false)]
        public int? ReduceFrequency
        { get; set; }

        [DataMember(Name = "reload_macros", EmitDefaultValue = false)]
        public bool? ReloadMacros
        { get; set; }

        [DataMember(Name = "remote_server_list", EmitDefaultValue = false)]
        public string RemoteServerList
        { get; set; }

        [DataMember(Name = "rf", EmitDefaultValue = false)]
        public IReadOnlyList<string> RequiredFieldList
        { get; set; }

        [DataMember(Name = "reuse_max_seconds_ago", EmitDefaultValue = false)]
        public int? ReuseMaxSecondsAgo
        { get; set; }

        [DataMember(Name = "rt_blocking", EmitDefaultValue = false)]
        public bool? RealTimeBlocking
        { get; set; }

        [DataMember(Name = "rt_indexfilter", EmitDefaultValue = false)]
        public bool? RealTimeIndexFilter
        { get; set; }

        [DataMember(Name = "rt_maxblocksecs", EmitDefaultValue = false)]
        public int? RealTimeMaxBlockSeconds
        { get; set; }

        [DataMember(Name = "rt_queue_size", EmitDefaultValue = false)]
        public int? RealTimeQueueSize
        { get; set; }

        [DataMember(Name = "search_listener", EmitDefaultValue = false)]
        public string SearchListener
        { get; set; }

        [DataMember(Name = "search_mode", EmitDefaultValue = false)]
        public SearchMode? SearchMode
        { get; set; }

        [DataMember(Name = "spawn_process", EmitDefaultValue = false)]
        public bool? SpawnProcess
        { get; set; }

        [DataMember(Name = "status_buckets", EmitDefaultValue = false)]
        public int? StatusBuckets
        { get; set; }

        [DataMember(Name = "sync_bundle_replication", EmitDefaultValue = false)]
        public bool? SyncBundleReplication
        { get; set; }

        [DataMember(Name = "time_format", EmitDefaultValue = false)]
        public string TimeFormat
        { get; set; }

        [DataMember(Name = "timeout", EmitDefaultValue = false)]
        public int? Timeout
        { get; set; }

        #endregion
    }
}