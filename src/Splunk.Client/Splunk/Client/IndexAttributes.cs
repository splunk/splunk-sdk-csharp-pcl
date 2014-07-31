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
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for setting the attributes of an <see cref="Index"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/L6GlMC">REST API Reference: POST
    ///   data/indexes</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/r5rZ7i">REST API Reference: POST
    ///   data/indexes/{name}</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.IndexAttributes}"/>
    public class IndexAttributes : Args<IndexAttributes>
    {
        /// <summary>
        /// Gets or sets the number of events that make up a block for block
        /// signatures on an <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// The default value is zero (0) indicating that block signatures are
        /// disabled. If your index requires block signatures, a value is 100 is
        /// recommended.
        /// </remarks>
        /// <value>
        /// The number of events that make up a block for block signatures on an
        /// <see cref="Index"/>.
        /// </value>
        [DataMember(Name = "blockSignSize", EmitDefaultValue = false)]
        public int? BlockSignSize
        { get; set; }

        /// <summary>
        /// Gets or sets a suggested size for the time-series (tsidx) file created by
        /// the Splunk bucket rebuild process.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"auto"</c> suggests that the size for the tsidx
        /// file created by the Splunk bucker rebuild process should vary based on
        /// the amount of physical RAM. Values other than <c>"auto"</c>
        /// must be 16MB-1GB. You can specify a value using a size prefix: "16777216"
        /// and "16MB" are equivalent.
        /// <para>
        /// <b>Caution:</b> Do not set this value unless instructed by Splunk
        /// Support. It is an advanced parameter. If set incorrectly Splunk will not
        /// start when a rebuild is required.</para>
        /// </remarks>
        /// <value>
        /// A suggested size for the time-series (tsidx) file created by the Splunk
        /// bucket rebuild process.
        /// </value>
        [DataMember(Name = "bucketRebuildMemoryHint", EmitDefaultValue = false)]
        public string BucketRebuildMemoryHint
        { get; set; }

        /// <summary>
        /// Gets or sets the path to a directory for the frozen archive of an
        /// <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// This property is an alternative to <see cref= "ColdToFrozenScript"/>. If
        /// <see cref="ColdToFrozenDir"/> and <see cref="ColdToFrozenScript"/>
        /// are specified, <see cref="ColdToFrozenDir"/> takes precedence. Splunk
        /// will automatically put frozen buckets in this directory.
        /// </remarks>
        /// <value>
        /// The path to a directory for the frozen archive of an <see cref= "Index"/>
        /// </value>
        [DataMember(Name = "coldToFrozenDir", EmitDefaultValue = false)]
        public string ColdToFrozenDir
        { get; set; }

        /// <summary>
        /// Gets or sets the path to an archiving script for the frozen archive of an
        /// <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python),
        /// specify the program followed by the path. The script must be in
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        /// <value>
        /// The path to an archiving script for the frozen archive of an
        /// <see cref="Index"/>.
        /// </value>
        [DataMember(Name = "coldToFrozenScript", EmitDefaultValue = false)]
        public string ColdToFrozenScript
        { get; set; }

        /// <summary>
        /// Gets or sets the path to an archiving script for the frozen archive of an
        /// <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python),
        /// specify the program followed by the path. The script must be in
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        /// <value>
        /// The path to an archiving script for the frozen archive of an
        /// <see cref="Index"/>.
        /// </value>
        [DataMember(Name = "compressRawdata", EmitDefaultValue = false)]
        public bool? CompressRawData
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether asynchronous bucket repair should
        /// be enabled on an <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c> indicating that asynchronous bucker
        /// repair should be enabled. You do not have to wait until buckets are
        /// repaired to start Splunk. You may observe a slight performance
        /// degradation while bucket repair is underway.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if asynchronous bucket repair should be enabled on an
        /// <see cref="Index"/>; <c>false</c> otherwise.
        /// </value>
        [DataMember(Name = "enableOnlineBucketRepair", EmitDefaultValue = false)]
        public bool? EnableOnlineBucketRepair
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of seconds after which
        /// indexed data rolls to frozen.
        /// </summary>
        /// <remarks>
        /// The default value is <c>188697600</c>, equivalent to six years.
        /// </remarks>
        /// <value>
        /// A value that specifies the number of seconds after which indexed data
        /// rolls to frozen.
        /// </value>
        [DataMember(Name = "frozenTimePeriodInSecs", EmitDefaultValue = false)]
        public int? FrozenTimePeriodInSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies when to stop rebuilding bloom
        /// filters for the warm or cold buckets of an <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form &lt;![CDATA[&lt;integer&gt;
        /// ("d"|"h"|"m"|"s"]]&gt;. The default is <c>"30d"</c> which tells Splunk to
        /// stop rebuilding bloom filters on warm or cold buckets when they are 30
        /// days old.
        /// </remarks>
        /// <value>
        /// A string that specifies when to stop rebuilding bloom filters for the
        /// warm or cold buckets of an <see cref="Index"/>.
        /// </value>
        [DataMember(Name = "maxBloomBackfillBucketAge", EmitDefaultValue = false)]
        public string MaxBloomBackfillBucketAge
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of concurrent optimize
        /// processes that can run against a hot bucket.
        /// </summary>
        /// <remarks>
        /// The default value is <c>6</c>.
        /// </remarks>
        /// <value>
        /// A value that specifies the number of concurrent optimize processes that
        /// can run against a hot bucket.
        /// </value>
        [DataMember(Name = "maxConcurrentOptimizes", EmitDefaultValue = false)]
        public int? MaxConcurrentOptimizes
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies the maximum size in megabytes for a
        /// hot database to reach before a roll to warm is triggered.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form &lt;![CDATA[&lt;integer&gt;
        /// |"auto"|"auto_high_volume"]]&gt;
        /// Specifying <c>"auto"</c> or <c>"auto_high_volume"</c> causes Splunk to
        /// autotune the size at which a roll to warm is triggered. Use
        /// <c>"auto_high_volume"</c> for high volume indexes (such as the main
        /// index); otherwise, use <c>"auto"</c>. The defaultis <c>"auto"</c>. &lt;
        /// para&gt;
        /// A "high volume index" would typically be considered one that gets over
        /// 10GB of data per day. Although the maximum value you can set is
        /// <c>1048576</c>, equivalent to 1 TB, a reasonable number ranges from
        /// <c>100</c> to <c>50000</c>. Any number outside this range should be
        /// approved by Splunk Support before proceeding. If you specify an invalid
        /// number or string, this value will be auto tuned. &lt;note type="note"&gt;
        /// The precise size of your warm buckets may vary from
        /// <see cref= "MaxDataSize"/> due to post-processing and timing issues with
        /// the rolling policy.
        /// </remarks>
        /// <value>
        /// A string that specifies the maximum size in megabytes for a hot database
        /// to reach before a roll to warm is triggered.
        /// </value>
        [DataMember(Name = "maxDataSize", EmitDefaultValue = false)]
        public string MaxDataSize
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum number of hot buckets
        /// that an <see cref="Index"/> can have.
        /// </summary>
        /// <remarks>
        /// When this value is is exceeded, Splunk rolls the least recently used hot
        /// bucket to warm. Both normal hot buckets and quarantined hot buckets count
        /// towards this total. The default is <c>3</c>.
        /// <note type="note">
        /// This setting operates independently of <see cref="MaxHotIdleSecs"/>
        /// which can also cause hot buckets to roll.
        /// </note>
        /// </remarks>
        /// <value>
        /// A value that specifies the maximum number of hot buckets that an
        /// <see cref="Index"/> can have.
        /// </value>
        [DataMember(Name = "maxHotBuckets", EmitDefaultValue = false)]
        public int? MaxHotBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the life time of an idle hot bucket
        /// in seconds.
        /// </summary>
        /// <remarks>
        /// If the lifetime of an idle hot bucket exceeds this value, Splunk rolls it
        /// to warm. The default is <c>0</c> which specifies that the lifetime.
        /// </remarks>
        /// <value>
        /// A value that specifies the life time of an idle hot bucket in seconds.
        /// </value>
        [DataMember(Name = "maxHotIdleSecs", EmitDefaultValue = false)]
        public int? MaxHotIdleSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the upper bound of target maximum
        /// timespan of hot/warm buckets in seconds.
        /// </summary>
        /// <remarks>
        /// The default value is <c>7776000</c>, equivalent to 90 days. Splunk
        /// enforces a lower bound of <c>3600</c>, equivalent to one hour. &lt;note
        /// type="caution"&gt;
        /// This is an advanced property that should be set with care and
        /// understanding of the characteristics of your data. If it is too small,
        /// you can get an explosion of hot/warm buckets in the file system.
        /// </remarks>
        /// <value>
        /// A value that specifies the upper bound of target maximum timespan of
        /// hot/warm buckets in seconds.
        /// </value>
        [DataMember(Name = "maxHotSpanSecs", EmitDefaultValue = false)]
        public int? MaxHotSpanSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of megabytes of memroy to
        /// allocate for buffering a single index file before flushing it to disk.
        /// </summary>
        /// <remarks>
        /// The default value is <c>5</c>. This is the recommended value for all
        /// environments.
        /// <note type="caution">
        /// Calculate this value carefully. Setting it incorrectly may have adverse
        /// effects on your system's memory and/or splunkd stability and performance.
        /// </note>
        /// </remarks>
        /// <value>
        /// A value that specifies the number of megabytes of memroy to allocate for
        /// buffering a single index file before flushing it to disk.
        /// </value>
        [DataMember(Name = "maxMemMB", EmitDefaultValue = false)]
        public int? MaxMemMB
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum number of unique lines in
        /// the .data files in a bucket.
        /// </summary>
        /// <remarks>
        /// Setting this value may help reduce memory consumption. The default value
        /// is <c>1000000</c>. If this limit is exceeded, a hot bucket is rolled to
        /// prevent further increase. If your buckets are rolling due to Strings.data
        /// hitting this limit, the culprit may be the <c>punct</c>
        /// field. If you don't use <c>punct</c>, it may be best to simply disable
        /// Strings.data. See <a href="">props.conf.spec</a>.
        /// <note type="note">
        /// There is a small time delay between detecting and acting on this limit.
        /// Consequently a bucket may grow beyond this limit before it is rolled.
        /// </note>
        /// </remarks>
        /// <value>
        /// A value that specifies the maximum number of unique lines in the data
        /// files in a bucket.
        /// </value>
        [DataMember(Name = "maxMetaEntries", EmitDefaultValue = false)]
        public int? MaxMetaEntries
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the upper limit in seconds on how
        /// long an event can sit in raw slice.
        /// </summary>
        /// <remarks>
        /// This value only applies if replication is enabled. It is otherwise
        /// ignored. The default value is <c>60</c>, equivalent to one minute.
        /// </remarks>
        /// <value>
        /// An upper limit in seconds on how long an event can sit in raw slice.
        /// </value>
        [DataMember(Name = "maxTimeUnreplicatedNoAcks", EmitDefaultValue = false)]
        public int? MaxTimeUnreplicatedNoAcks
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the upper limit in seconds on how
        /// long events can sit unacknowledged in a raw slice.
        /// </summary>
        /// <remarks>
        /// This value only applies if you have enabled acks on forwarders and have
        /// replication enabled with clustering. It is otherwise ignored. The default
        /// value is <c>300</c>, equivalent to five minutes.
        /// </remarks>
        /// <value>
        /// An upper limit in seconds on how long events can sit unacknowledged in a
        /// raw slice.
        /// </value>
        [DataMember(Name = "maxTimeUnreplicatedWithAcks", EmitDefaultValue = false)]
        public int? MaxTimeUnreplicatedWithAcks
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum size of an
        /// <see cref="Index"/> in megabytes.
        /// </summary>
        /// <remarks>
        /// The default value is <c>500000</c>. If an <see cref="Index"/> grows
        /// larger than the specified number of megabytes, the oldest data is frozen.
        /// </remarks>
        /// <value>
        /// The maximum size of an <see cref="Index"/> in megabytes.
        /// </value>
        [DataMember(Name = "maxTotalDataSizeMB", EmitDefaultValue = false)]
        public int? MaxTotalDataSizeMB
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum number of warm buckets.
        /// </summary>
        /// <remarks>
        /// The default value is <c>300</c>. If the number of buckets grows larger
        /// than this, the warm buckets with the lowest value for their latest times
        /// will be moved to cold.
        /// </remarks>
        /// <value>
        /// The maximum number of warm buckets.
        /// </value>
        [DataMember(Name = "maxWarmDBCount", EmitDefaultValue = false)]
        public int? MaxWarmDBCount
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies how frequently splunkd forces a file
        /// system sync while compressing journal slices.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form &lt;![CDATA[(&lt;integer&gt;
        /// |"disable")]]&gt;. If you specify a value of <c>"0"</c>, splunkd forces a
        /// file system sync after every slice completes compressing. If you specify
        /// a value of <c>"disable"</c>, splunkd disables syncing entirely:
        /// Uncompressed slices are removed as soon as compression is complete. The
        /// default is <c>"disable"</c>.
        /// </remarks>
        /// <value>
        /// A string that specifies how frequently splunkd forces a file system sync
        /// while compressing journal slices.
        /// </value>
        [DataMember(Name = "minRawFileSyncSecs", EmitDefaultValue = false)]
        public string MinRawFileSyncSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the minimum size of the queue that
        /// stores events in memory before committing them to a .tsidx file.
        /// </summary>
        /// <remarks>
        /// The default value is <c>2000</c>.
        /// <note type="caution">
        /// Do not set this value, except under advice from Splunk support.
        /// </note>
        /// </remarks>
        /// <value>
        /// The minimum size of the queue that stores events in memory before
        /// committing them to a .tsidx file.
        /// </value>
        [DataMember(Name = "minStreamGroupQueueSize", EmitDefaultValue = false)]
        public int? MinStreamGroupQueueSize
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds how often metadata sync
        /// occurs for records where the sync can be done efficiently in-place,
        /// without requiring a full re-write of the metadata file.
        /// </summary>
        /// <remarks>
        /// The default value is <c>0</c>. Zero means that this feature is disabled
        /// and <see cref="ServiceMetaPeriod"/> is the only time when metadata sync
        /// occurs.
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds how often metadata sync occurs for
        /// records where the sync can be done efficiently in-place, without
        /// requiring a full re-write of the metadata file.
        /// </value>
        [DataMember(Name = "partialServiceMetaPeriod", EmitDefaultValue = false)]
        public int? PartialServiceMetaPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds how often the indexer
        /// checks the status of the child operating system processes it has launched
        /// to see if it can launch new processes for queued requests.
        /// </summary>
        /// <remarks>
        /// The default value is <c>15</c>. Zero means the indexer will check child
        /// process status every second. This value is capped at
        /// <c>4294967295</c>, equivalent to <see cref="System.UInt32.MaxValue"/>.
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds how often the indexer checks the status
        /// of the child operating system processes it has launched to see if it can
        /// launch new processes for queued requests.
        /// </value>
        [DataMember(Name = "processTrackerServiceInterval", EmitDefaultValue = false)]
        public uint? ProcessTrackerServiceInterval
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds when new events are
        /// dropped into a quarantine bucket.
        /// </summary>
        /// <remarks>
        /// Events with a timestamp newer than <c>now</c> plus this value are dropped
        /// into a quarantine bucket. This is a mechanism to prevent main hot buckets
        /// from being polluted with fringe events. The default value is
        /// <c>2592000</c>, equivalent to thirty days.
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds when new events are dropped into a
        /// quarantine bucket.
        /// </value>
        [DataMember(Name = "quarantineFutureSecs", EmitDefaultValue = false)]
        public int? QuarantineFutureSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds when old events are
        /// dropped into a quarantine bucket.
        /// </summary>
        /// <remarks>
        /// Events with a timestamp older than <c>now</c> plus this value are dropped
        /// into a quarantine bucket. This is a mechanism to prevent main hot buckets
        /// from being polluted with fringe events. The default value is
        /// <c>77760000</c>, equivalent to 900 days.
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds when old events are dropped into a
        /// quarantine bucket.
        /// </value>
        [DataMember(Name = "quarantinePastSecs", EmitDefaultValue = false)]
        public int? QuarantinePastSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies a target uncompressed size in bytes
        /// for individual raw slice in the raw data journal of an
        /// <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>131072</c>, equivalent to 128 KB. Zero is not a
        /// valid value. If zero is specified, rawChunkSizeBytes is set to the
        /// default value.
        /// <note type="note">
        /// This value only specifies a target chunk size. The actual chunk size may
        /// be slightly larger by an amount proportional to an individual event
        /// size.</note>
        /// <note type="caution">
        /// This is an advanced property. Only change it if you are instructed to do
        /// so by Splunk Support.</note>
        /// </remarks>
        /// <value>
        /// A value that specifies a target uncompressed size in bytes for individual
        /// raw slice in the raw data journal of an <see cref= "Index"/>.
        /// </value>
        [DataMember(Name = "rawChunkSizeBytes", EmitDefaultValue = false)]
        public int? RawChunkSizeBytes
        { get; set; }

        /// <summary>
        /// Gets or sets a value that controls index replication.
        /// </summary>
        /// <remarks>
        /// This property only applies to Splunk Enterprise clustering slaves.
        /// Specify a value of <c>"auto</c> to use the master index replication
        /// configuration value. Specify a value of <c>"0"</c> to turn replication
        /// off. The default is <c>"0"</c>.
        /// </remarks>
        /// <value>
        /// A value that controls index replication.
        /// </value>
        [DataMember(Name = "repFactor", EmitDefaultValue = false)]
        public string RepFactor
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds how frequently to check if
        /// a new hot bucket needs to be created as well as if there are any
        /// warm/cold buckets that should be rolled/frozen.
        /// </summary>
        /// <remarks>
        /// The default value is <c>60</c>, equivalent to one minute.
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds how frequently to check if a new hot
        /// bucket needs to be created as well as if there are any warm/cold buckets
        /// that should be rolled/frozen.
        /// </value>
        [DataMember(Name = "rotatePeriodInSecs", EmitDefaultValue = false)]
        public int? RotatePeriodInSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds how frequently metadata is
        /// synced to disk.
        /// </summary>
        /// <remarks>
        /// The default value is <c>25</c>. You may want to set this to a higher
        /// value if the sum of your metadata file sizes is larger than many tens of
        /// megabytes to avoid the hit on I/O in the indexing fast path.
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds how frequently metadata is synced to
        /// disk.
        /// </value>
        [DataMember(Name = "serviceMetaPeriod", EmitDefaultValue = false)]
        public int? ServiceMetaPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a sync operation is called
        /// before a file descriptor is closed on metadata file updates.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>. This improves integrity of metadata
        /// files, especially in regards to operating system crashes and machine
        /// failures.
        /// <notes type="caution">
        /// Do not change this parameter without the input of a Splunk Support.
        /// </notes>
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a sync operation is called before a file descriptor is
        /// closed on metadata file updates; <c>false</c> otherwise.
        /// </value>
        [DataMember(Name = "syncMeta", EmitDefaultValue = false)]
        public bool? SyncMeta
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies in seconds how frequently to check
        /// for an <see cref="Index"/> throttling condition.
        /// </summary>
        /// <remarks>
        /// The default value is <c>15</c>.
        /// <notes type="caution">
        /// Do not change this parameter without the input of a Splunk Support.
        /// </notes>
        /// </remarks>
        /// <value>
        /// A value that specifies in seconds how frequently to check for an
        /// <see cref="Index"/> throttling condition.
        /// </value>
        [DataMember(Name = "throttleCheckPeriod", EmitDefaultValue = false)]
        public int? ThrottleCheckPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets the location to store datamodel acceleration .tsidx data for
        /// an <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// Restart splunkd after changing this value. It must be defined in terms of
        /// a volume definition and the location must be writable. The default is
        /// volume:_splunk_summaries/$_index_name/tstats.
        /// </remarks>
        /// <value>
        /// The location to store datamodel acceleration .tsidx data for an
        /// <see cref="Index"/>.
        /// </value>
        [DataMember(Name = "tstatsHomePath", EmitDefaultValue = false)]
        public string TStatsHomePath
        { get; set; }

        /// <summary>
        /// Gets or sets the location of a script to run when moving data from warm
        /// to cold.
        /// </summary>
        /// <remarks>
        /// This property is supported for backwards compatibility with Splunk
        /// versions older than 4.0. Contact Splunk support if you need help.
        /// <note type="caution">
        /// Migrating data across file systems is now handled natively by splunkd. If
        /// you specify a script here, the script becomes responsible for moving the
        /// event data, and Splunk-native data migration will not be used.</note>
        /// </remarks>
        /// <value>
        /// The location of a script to run when moving data from warm to cold.
        /// </value>
        [DataMember(Name = "warmToColdScript", EmitDefaultValue = false)]
        public string WarmToColdScript
        { get; set; }
    }
}