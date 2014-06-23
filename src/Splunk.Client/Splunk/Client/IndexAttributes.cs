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
//// [O]  Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for setting the attributes of an <see cref="Index"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/L6GlMC">REST API Reference: POST data/indexes</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/r5rZ7i">REST API Reference: POST data/indexes/{name}</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class IndexAttributes : Args<IndexAttributes>
    {
        /// <summary>
        /// Gets or sets the number of events that make up a block for block
        /// signatures on an index.
        /// </summary>
        /// <remarks>
        /// The default value is zero (0) indicating that block signatures are
        /// disabled. If your index requires block signatures, a value is 100
        /// is recommended. 
        /// </remarks>
        [DataMember(Name = "blockSignSize", EmitDefaultValue = false)]
        public int? BlockSignSize
        { get; set; }

        /// <summary> 
        /// Gets or sets a suggested size for the time-series (tsidx) file 
        /// created by the Splunk bucket rebuild process.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"auto"</c> suggests that the size for the
        /// tsidx file created by the Splunk bucker rebuild process should vary
        /// based on the amount of physical RAM. Values other than <c>"auto"</c> 
        /// must be 16MB-1GB. You can specify a value using a size prefix:
        /// "16777216" and "16MB" are equivalent. 
        /// <para>
        /// <b>Caution:</b> Do not set this value unless instructed by Splunk 
        /// Support. It is an advanced parameter. If set incorrectly Splunk 
        /// will not start when a rebuild is required.</para>
        /// </remarks>
        [DataMember(Name = "bucketRebuildMemoryHint", EmitDefaultValue = false)]
        public string BucketRebuildMemoryHint
        { get; set; }

        /// <summary>
        /// Gets or sets the path to a directory for the frozen archive of an 
        /// index.
        /// </summary>
        /// <remarks>
        /// This property is an alternative to <see cref= "ColdToFrozenScript"/>. 
        /// If <see cref="ColdToFrozenDir"/> and <see cref="ColdToFrozenScript"/> 
        /// are specified, <see cref="ColdToFrozenDir"/> takes precedence. 
        /// Splunk will automatically put frozen buckets in this directory.
        /// </remarks>
        [DataMember(Name = "coldToFrozenDir", EmitDefaultValue = false)]
        public string ColdToFrozenDir
        { get; set; }

        /// <summary>
        /// Gets or sets the path to an archiving script for the frozen archive
        /// of an index.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python), 
        /// specify the program followed by the path. The script must be in 
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        [DataMember(Name = "coldToFrozenScript", EmitDefaultValue = false)]
        public string ColdToFrozenScript
        { get; set; }

        /// <summary>
        /// Gets or sets the path to an archiving script for the frozen archive
        /// of an index.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python), 
        /// specify the program followed by the path. The script must be in 
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        [DataMember(Name = "compressRawdata", EmitDefaultValue = false)]
        public bool? CompressRawData
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether asynchronous bucket repair 
        /// should be enabled on an index.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c> indicating that asynchronous 
        /// bucker repair should be enabled. You do not have to wait until 
        /// buckets are repaired to start Splunk. You may observe a slight 
        /// performance degradation while bucket repair is underway.
        /// </remarks>
        [DataMember(Name = "enableOnlineBucketRepair", EmitDefaultValue = false)]
        public bool? EnableOnlineBucketRepair
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of seconds after 
        /// which indexed data rolls to frozen.
        /// </summary>
        /// <remarks>
        /// The default value is <c>188697600</c>, equivalent to 6 years.
        /// </remarks>
        [DataMember(Name = "frozenTimePeriodInSecs", EmitDefaultValue = false)]
        public int? FrozenTimePeriodInSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies when to stop rebuilding bloom
        /// filters for the warm or cold buckets of an <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <![CDATA[<integer> ("d"|"h"|"m"|"s"]]>.
        /// The default is <c>"30d"</c> which tells Splunk to stop rebuilding
        /// bloom filters on warm or cold buckets when they are 30 days old.
        /// </remarks>
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
        [DataMember(Name = "maxConcurrentOptimizes", EmitDefaultValue = false)]
        public int? MaxConcurrentOptimizes
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies the maximum size in megabytes 
        /// for a hot database to reach before a roll to warm is triggered.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <![CDATA[<integer>|"auto"|"auto_high_volume"]]>
        /// Specifying <c>"auto"</c> or <c>"auto_high_volume"</c> causes Splunk
        /// to autotune the size at which a roll to warm is triggered. Use 
        /// <c>"auto_high_volume"</c> for high volume indexes (such as the main
        /// index); otherwise, use <c>"auto"</c>. The defaultis <c>"auto"</c>.
        /// <para>
        /// A "high volume index" would typically be considered one that gets 
        /// over 10GB of data per day. Although the maximum value you can set 
        /// is <c>1048576</c>, equivalent to 1 TB, a reasonable number ranges 
        /// from <c>100</c> to <c>50000</c>. Any number outside this range 
        /// should be approved by Splunk Support before proceeding. If you 
        /// specify an invalid number or string, this value will be auto tuned.
        /// <note type="note">
        /// The precise size of your warm buckets may vary from <see cref=
        /// "MaxDataSize"/> due to post-processing and timing issues with the 
        /// rolling policy.
        /// </remarks>
        [DataMember(Name = "maxDataSize", EmitDefaultValue = false)]
        public string MaxDataSize
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum number of hot 
        /// buckets that an <see cref="Index"/> can have.
        /// </summary>
        /// <remarks>
        /// When this value is is exceeded, Splunk rolls the least recently 
        /// used hot bucket to warm. Both normal hot buckets and quarantined 
        /// hot buckets count towards this total. The default is <c>3</c>.
        /// <note type="note">
        /// This setting operates independently of <see cref="MaxHotIdleSecs"/> 
        /// which can also cause hot buckets to roll.
        /// </note>
        /// </remarks>
        [DataMember(Name = "maxHotBuckets", EmitDefaultValue = false)]
        public int? MaxHotBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the life time of an idle hot 
        /// bucket in seconds.
        /// </summary>
        /// <remarks>
        /// If the lifetime of an idle hot bucket exceeds this value, Splunk 
        /// rolls it to warm. The default is <c>0</c> which specifies that
        /// the lifetime.
        /// </remarks>
        [DataMember(Name = "maxHotIdleSecs", EmitDefaultValue = false)]
        public int? MaxHotIdleSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the upper bound of target 
        /// maximum timespan of hot/warm buckets in seconds.
        /// </summary>
        /// <remarks>
        /// The default value is <c>7776000</c>, equivalent to 90 days. Splunk 
        /// enforces a lower bound of <c>3600</c>, equivalent to one hour.
        /// <note type="caution">
        /// This is an advanced property that should be set with care and
        /// understanding of the characteristics of your data. If it is too 
        /// small, you can get an explosion of hot/warm buckets in the file 
        /// system.
        /// </remarks>
        [DataMember(Name = "maxHotSpanSecs", EmitDefaultValue = false)]
        public int? MaxHotSpanSecs
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of megabytes of
        /// memroy to allocate for buffering a single index file before
        /// flushing it to disk.
        /// </summary>
        /// <remarks>
        /// The default value is <c>5</c>. This is the recommended value for 
        /// all environments.
        /// <note type="caution">
        /// Calculate this value carefully. Setting it incorrectly may have 
        /// adverse effects on your system's memory and/or splunkd stability 
        /// and performance.
        /// </note>
        /// </remarks>
        [DataMember(Name = "maxMemMB", EmitDefaultValue = false)]
        public int? MaxMemMB
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum number of unique 
        /// lines in the .data files in a bucket.
        /// </summary>
        /// <remarks>
        /// Setting this value may help reduce memory consumption. The default
        /// value is <c>1000000</c>. If this limit is exceeded, a hot bucket is
        /// rolled to prevent further increase. If your buckets are rolling due 
        /// to Strings.data hitting this limit, the culprit may be the <c>punct</c>
        /// field. If you don't use <c>punct</c>, it may be best to simply 
        /// disable Strings.data. See <a href="">props.conf.spec</a>.
        /// <note type="note">
        /// There is a small time delay between detecting and acting on this 
        /// limit. Consequently a bucket may grow beyond this limit before it
        /// is rolled.
        /// </note>
        /// </remarks>
        [DataMember(Name = "maxMetaEntries", EmitDefaultValue = false)]
        public int? MaxMetaEntries
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>300</c>.
        /// </remarks>
        [DataMember(Name = "maxTimeUnreplicatedNoAcks", EmitDefaultValue = false)]
        public int? MaxTimeUnreplicatedNoAcks
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>60</c>.
        /// </remarks>
        [DataMember(Name = "maxTimeUnreplicatedWithAcks", EmitDefaultValue = false)]
        public int? MaxTimeUnreplicatedWithAcks
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>500000</c>.
        /// </remarks>
        [DataMember(Name = "maxTotalDataSizeMB", EmitDefaultValue = false)]
        public int? MaxTotalDataSizeMB
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>300</c>.
        /// </remarks>
        [DataMember(Name = "maxWarmDBCount", EmitDefaultValue = false)]
        public int? MaxWarmDBCount
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>"disable"</c>.
        /// </remarks>
        [DataMember(Name = "minRawFileSyncSecs", EmitDefaultValue = false)]
        public string MinRawFileSyncSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>2000</c>.
        /// </remarks>
        [DataMember(Name = "minStreamGroupQueueSize", EmitDefaultValue = false)]
        public int? MinStreamGroupQueueSize
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>0</c>.
        /// </remarks>
        [DataMember(Name = "partialServiceMetaPeriod", EmitDefaultValue = false)]
        public int? PartialServiceMetaPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>1</c>.
        /// </remarks>
        [DataMember(Name = "processTrackerServiceInterval", EmitDefaultValue = false)]
        public int? ProcessTrackerServiceInterval
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>2592000</c>.
        /// </remarks>
        [DataMember(Name = "quarantineFutureSecs", EmitDefaultValue = false)]
        public int? QuarantineFutureSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>77760000</c>.
        /// </remarks>
        [DataMember(Name = "quarantinePastSecs", EmitDefaultValue = false)]
        public int? QuarantinePastSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>131072</c>.
        /// </remarks>
        [DataMember(Name = "rawChunkSizeBytes", EmitDefaultValue = false)]
        public int? RawChunkSizeBytes
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>"0"</c>.
        /// </remarks>
        [DataMember(Name = "repFactor", EmitDefaultValue = false)]
        public string RepFactor
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>60</c>.
        /// </remarks>
        [DataMember(Name = "rotatePeriodInSecs", EmitDefaultValue = false)]
        public int? RotatePeriodInSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>25</c>.
        /// </remarks>
        [DataMember(Name = "serviceMetaPeriod", EmitDefaultValue = false)]
        public int? ServiceMetaPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "syncMeta", EmitDefaultValue = false)]
        public bool? SyncMeta
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>15</c>.
        /// </remarks>
        [DataMember(Name = "throttleCheckPeriod", EmitDefaultValue = false)]
        public int? ThrottleCheckPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        [DataMember(Name = "tstatsHomePath", EmitDefaultValue = false)]
        public string TStatsHomePath
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        [DataMember(Name = "warmToColdScript", EmitDefaultValue = false)]
        public string WarmToColdScript
        { get; set; }
    }
}