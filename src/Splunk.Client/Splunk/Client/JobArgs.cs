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
//// [ ] Class for specifying a time string.

namespace Splunk.Client
{
    using System.Collections.Generic;
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
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.JobArgs}"/>
    public sealed class JobArgs : Args<JobArgs>
    {
        /// <summary>
        /// Gets or sets a value that specifies how long a <see cref="Job"/>
        /// may be inactive before it is automatically cancelled.
        /// </summary>
        /// <remarks>
        /// A value of <c>0</c> specifies that a <see cref="Job"/> is never
        /// automatically cancelled.
        /// </remarks>
        /// <value>
        /// A value that specifies how long a <see cref="Job"/> may be inactive
        /// before it is automatically cancelled.
        /// </value>
        [DataMember(Name = "auto_cancel", EmitDefaultValue = false)]
        public int? AutoCancel
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies how many events a <see cref= "Job"/>
        /// must process before it is automatically finalized.
        /// </summary>
        /// <remarks>
        /// A value of <c>0</c> specifies that a <see cref="Job"/> is never
        /// automatically finalized.
        /// </remarks>
        /// <value>
        /// A value that specifies how many events a <see cref="Job"/> must process
        /// before it is automatically finalized.
        /// </value>
        [DataMember(Name = "auto_finalize_ec", EmitDefaultValue = false)]
        public int? AutoFinalizeEventCount
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies how long a <see cref="Job"/>
        /// may run before it is automatically paused.
        /// </summary>
        /// <remarks>
        /// A value of <c>0</c> specifies that a <see cref="Job"/> is never
        /// automatically paused.
        /// </remarks>
        /// <value>
        /// A value that specifies how long a <see cref="Job"/> may run before it is
        /// automatically paused.
        /// </value>
        [DataMember(Name = "auto_pause", EmitDefaultValue = false)]
        public int? AutoPause
        { get; set; }

        /// <summary>
        /// Gets or sets a time string that specifies the earliest inclusive time
        /// bounds for a <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The time string can be either a UTC time (with fractional seconds), a
        /// relative time specifier (to now) or a formatted time string. Refer to
        /// <a href="http://goo.gl/4P41jH">Time modifiers for search
        /// </a> for information and examples of specifying a time string.
        /// </remarks>
        /// <value>
        /// A time string that specifies the earliest inclusive time bounds for a
        /// <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
        public string EarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether lookups should be applied to
        /// events processed by a search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>. Depending on the nature of the lookups
        /// a search <see cref="Job"/> may take significantly longer to execute when
        /// the value of this property is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if lookups should be applied to events processed by the
        /// search <see cref="Job"/>; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "enable_lookups", EmitDefaultValue = false)]
        public bool? EnableLookups
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether a <see cref="Job"/>
        /// should cause (and wait, depending on the value of
        /// <see cref= "SyncBundleReplication"/> for bundle synchronization 
        /// with all search peers.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the <see cref="Job"/> should cause (and wait, 
        /// depending on the value of <see cref="SyncBundleReplication"/>) for
        /// bundle synchronization with all search peers.
        /// </value>
        [DataMember(Name = "force_bundle_replication", EmitDefaultValue = false)]
        public bool? ForceBundleReplication
        { get; set; }

        /// <summary>
        /// Gets or sets the search ID (SID) for a <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// A random SID is generated by default.
        /// </remarks>
        /// <value>
        /// The SID for the <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id
        { get; set; }

        /// <summary>
        /// Gets or sets a time string that specifies the earliest inclusive time
        /// bounds for a <see cref="Job"/> based on the index time bounds.
        /// </summary>
        /// <remarks>
        /// The time string can be either a UTC time (with fractional seconds), a
        /// relative time specifier (to now) or a formatted time string. Refer to
        /// <a href="http://goo.gl/4P41jH">Time modifiers for search
        /// </a> for information and examples of specifying a time string.
        /// </remarks>
        /// <value>
        /// A time string that specifies the earliest inclusive time bounds for a
        /// <see cref="Job"/> based on the index time bounds.
        /// </value>
        [DataMember(Name = "index_earliest", EmitDefaultValue = false)]
        public string IndexEarliest
        { get; set; }

        /// <summary>
        /// Gets or sets a time string that specifies the latest exclusive time
        /// bounds for a <see cref="Job"/> based on the index time bounds.
        /// </summary>
        /// <remarks>
        /// The time string can be either a UTC time (with fractional seconds), a
        /// relative time specifier (to now) or a formatted time string. Refer to
        /// <a href="http://goo.gl/4P41jH">Time modifiers for search
        /// </a> for information and examples of specifying a time string.
        /// </remarks>
        /// <value>
        /// A time string that specifies the latest exclusive time bounds for a
        /// <see cref="Job"/> based on the index time bounds.
        /// </value>
        [DataMember(Name = "index_latest", EmitDefaultValue = false)]
        public string IndexLatest
        { get; set; }

        /// <summary>
        /// Gets or sets a time string that specifies the latest exclusive time
        /// bounds for a <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The time string can be either a UTC time (with fractional seconds), a
        /// relative time specifier (to now) or a formatted time string. Refer to
        /// <a href="http://goo.gl/4P41jH">Time modifiers for search
        /// </a> for information and examples of specifying a time string.
        /// </remarks>
        /// <value>
        /// A time string that specifies the latest exclusive time bounds for a
        /// <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        public string LatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets the number of events that can be accessible in any given
        /// status bucket.
        /// </summary>
        /// <remarks>
        /// In transforming mode this value also specifies the maximum number of
        /// results to store. The default value is <c>10000</c>.
        /// </remarks>
        /// <value>
        /// The number of events that can be accessible in any given status bucket.
        /// </value>
        [DataMember(Name = "max_count", EmitDefaultValue = false)]
        public int? MaxCount
        { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds to run a <see cref="Job"/>
        /// before finalizing.
        /// </summary>
        /// <remarks>
        /// A value of <c>0</c> specifies that a <see cref="Job"/> should never
        /// finalize.
        /// </remarks>
        /// <value>
        /// The number of seconds to run a <see cref="Job"/> before finalizing.
        /// </value>
        [DataMember(Name = "max_time", EmitDefaultValue = false)]
        public int? MaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets the application namespace in which to restrict searches.
        /// </summary>
        /// <remarks>
        /// The namespace corresponds to the identifier recognized in the
        /// /services/apps/local endpoint.
        /// </remarks>
        /// <value>
        /// The application namespace in which to restrict searches.
        /// </value>
        [DataMember(Name = "namespace", EmitDefaultValue = false)]
        public string Namespace
        { get; set; }

        /// <summary>
        /// Gets or sets a time string to set the absolute time used for any relative
        /// time specifier in a search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The default value is the current system time. You can specify a relative
        /// time modifier for this property. For example, set the value to
        /// <c>"+2d"</c> to specify the current time plus two days. If you specify a
        /// relative time modifier both in this parameter and in the search string,
        /// the search string modifier takes precedence. See
        /// <a href="http://goo.gl/PbEO2j">Time modifiers for search</a>
        /// for details on relative time modifiers.
        /// </remarks>
        /// <value>
        /// A time string to set the absolute time used for any relative time
        /// specifier in a search <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "now", EmitDefaultValue = false)]
        public string Now
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the indexer blocks if the queue
        /// for a realtime search <see cref="Job"/> is full.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>. This property only applies to realtime
        /// search jobs.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the indexer should block if the queue for a realtime
        /// search <see cref="Job"/> is full; <c>false</c> otherwise.
        /// </value>
        [DataMember(Name = "rt_blocking", EmitDefaultValue = false)]
        public bool? RealTimeBlocking
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the indexer prefilters events.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>. This property only applies to realtime
        /// search jobs.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the indexer should prefilter events; <c>false</c>
        /// otherwise.
        /// </value>
        [DataMember(Name = "rt_indexfilter", EmitDefaultValue = false)]
        public bool? RealTimeIndexFilter
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum time to block when
        /// <see cref="RealTimeBlocking"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This property only applies to realtime search jobs. The default value is
        /// <c>60</c>, equivalent to one minute. Specify a value of
        /// <c>0</c> to block for an indefinite period of time.
        /// </remarks>
        /// <value>
        /// The maximum time to block when <see cref="RealTimeBlocking"/> is
        /// <c>true</c>.
        /// </value>
        [DataMember(Name = "rt_maxblocksecs", EmitDefaultValue = false)]
        public int? RealTimeMaxBlockSeconds
        { get; set; }

        /// <summary>
        /// Gets or sets the queue size in events that the indexer should use for a
        /// realtime search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// This property only applies to realtime search jobs. The default value is
        /// <c>10000</c> events.
        /// </remarks>
        /// <value>
        /// The queue size in events for the realtime search <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "rt_queue_size", EmitDefaultValue = false)]
        public int? RealTimeQueueSize
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies how frequently to run the map/reduce
        /// reduce phase on accumulated map values.
        /// </summary>
        /// <remarks>
        /// The default value is <c>0</c>.
        /// </remarks>
        /// <value>
        /// A value that specifies how frequently to run the map/reduce reduce phase
        /// on accumulated map values.
        /// </value>
        [DataMember(Name = "reduce_freq", EmitDefaultValue = false)]
        public int? ReduceFrequency
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether macro definitions should be
        /// reloaded from <a href="http://goo.gl/4iCRz4"><c>
        /// macros.conf</c></a>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if macro definitions should be reloaded from <a href=
        /// "http://goo.gl/4iCRz4"><c>macros.conf</c></a>.
        /// </value>
        [DataMember(Name = "reload_macros", EmitDefaultValue = false)]
        public bool? ReloadMacros
        { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of servers from which raw events
        /// should be pulled.
        /// </summary>
        /// <remarks>
        /// The server names may be specified as wildcard names.
        /// </remarks>
        /// <value>
        /// A comma-separated list of servers from which raw events should be pulled.
        /// </value>
        [DataMember(Name = "remote_server_list", EmitDefaultValue = false)]
        public string RemoteServerList
        { get; set; }

        /// <summary>
        /// Gets or sets a required field list to a search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// These fields, even if not referenced or used directly by the search
        /// <see cref="Job"/>, are still included by the events and summary
        /// endpoints. Splunk Web uses them to prepopulate panels in the Search view.
        /// </remarks>
        /// <value>
        /// A list of required fields for the search <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "rf", EmitDefaultValue = false)]
        public IReadOnlyList<string> RequiredFieldList
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of seconds ago to check
        /// when an identical search <see cref="Job"/> was started and returns that
        /// search ID instead of starting a new <see cref="Job"/>.
        /// </summary>
        /// <value>
        /// The number of seconds ago to check when an identical search
        /// <see cref="Job"/> was started.
        /// </value>
        [DataMember(Name = "reuse_max_seconds_ago", EmitDefaultValue = false)]
        public int? ReuseMaxSecondsAgo
        { get; set; }

        /// <summary>
        /// Gets or sets a value used to register a search state listener with a
        /// search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <![CDATA[<search-state>;<results-condition>;<http-method>;<uri>;]]>.
        /// <example>Example:</example>
        /// <code>
        /// job.SearchListener = "onResults;true;POST;/servicesNS/admin/search/saved/search/foobar/notify;";
        /// </code>
        /// </remarks>
        /// <value>
        /// Search state listener registration entry.
        /// </value>
        [DataMember(Name = "search_listener", EmitDefaultValue = false)]
        public string SearchListener
        { get; set; }

        /// <summary>
        /// Gets or sets the mode for a search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="SearchMode"/><c>.Normal</c>. If set to
        /// <see cref="SearchMode"/><c>.RealTime</c>realtime, the search
        /// <see cref="Job"/> runs over live data. A real-time search may also be
        /// indicated by setting the <see cref="EarliestTime"/> and
        /// <see cref="LatestTime"/> properties to <c>"rt"</c> even if the
        /// <c>SearchMode</c> is set to <see cref="SearchMode"/><c>.Normal</c>.
        /// <para>
        /// For a real-time search, if both <see cref="EarliestTime"/> and
        /// <see cref="LatestTime"/> are set to <c>"rt"</c>, the search
        /// represents all appropriate live data received since the start of the
        /// search. Additionally, if <see cref="EarliestTime"/> and/or
        /// <see cref="LatestTime"/> are set to <c>"rt"</c> followed by a
        /// relative time specifier then a sliding window is used where the time
        /// bounds of the window are determined by the relative time specifiers and
        /// are continuously updated based on the wall-clock time.
        /// </para>
        /// </remarks>
        /// <value>
        /// The mode for a search <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "search_mode", EmitDefaultValue = false)]
        public SearchMode? SearchMode
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a search <see cref= "Job"/>
        /// should run in a separate spawned process.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>. Searches against indexes must run in a
        /// separate process.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the search <see cref="Job"/> should run in a separate
        /// spawned process; <c>false</c> otherwise.
        /// </value>
        [DataMember(Name = "spawn_process", EmitDefaultValue = false)]
        public bool? SpawnProcess
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum number of status buckets
        /// to generate for a search <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>0</c>.
        /// </remarks>
        /// <value>
        /// The maximum number of status buckets to generate for the search
        /// <see cref="Job"/>.
        /// </value>
        [DataMember(Name = "status_buckets", EmitDefaultValue = false)]
        public int? StatusBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a <see cref="Job"/>
        /// should wait for bundle synchronization with all search peers.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the <see cref="Job"/> should wait for bundle
        /// synchronization with all search peers; <c>false</c> otherwise.
        /// </value>
        [DataMember(Name = "sync_bundle_replication", EmitDefaultValue = false)]
        public bool? SyncBundleReplication
        { get; set; }

        /// <summary>
        /// Gets or sets a format string used to convert a formatted time string from
        /// {start,end}_time into UTC seconds.
        /// </summary>
        /// <remarks>
        /// The default value is the ISO-8601 format.
        /// </remarks>
        /// <value>
        /// A time format string.
        /// </value>
        [DataMember(Name = "time_format", EmitDefaultValue = false)]
        public string TimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of seconds to keep a
        /// search <see cref="Job"/> after processing has stopped.
        /// </summary>
        /// <remarks>
        /// The default value is <c>86400</c>, equivalent to 24 hours.
        /// </remarks>
        /// <value>
        /// The number of seconds to keep the search <see cref="Job"/>after
        /// processing has stopped.
        /// </value>
        [DataMember(Name = "timeout", EmitDefaultValue = false)]
        public int? Timeout
        { get; set; }
    }
}