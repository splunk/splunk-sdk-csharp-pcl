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
    /// </item></description>
    /// <item><description>
    ///   <a href="http://goo.gl/r5rZ7i">REST API Reference: POST data/indexes/{name}</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class IndexAttributes : Args<IndexAttributes>
    {
        #region Properties

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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
        public bool? EnableOnlineBucketRepair
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>188697600</c>.
        /// </remarks>
        [DataMember(Name = "frozenTimePeriodInSecs", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public int? FrozenTimePeriodInSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>"30d"</c>.
        /// </remarks>
        [DataMember(Name = "maxBloomBackfillBucketAge", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string MaxBloomBackfillBucketAge
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>6</c>.
        /// </remarks>
        [DataMember(Name = "maxConcurrentOptimizes", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public int? MaxConcurrentOptimizes
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>"auto"</c>.
        /// </remarks>
        [DataMember(Name = "maxDataSize", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string MaxDataSize
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>3</c>.
        /// </remarks>
        [DataMember(Name = "maxHotBuckets", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public int? MaxHotBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>0</c>.
        /// </remarks>
        [DataMember(Name = "maxHotIdleSecs", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public int? MaxHotIdleSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>7776000</c>.
        /// </remarks>
        [DataMember(Name = "maxHotSpanSecs", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public int? MaxHotSpanSecs
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>5</c>.
        /// </remarks>
        [DataMember(Name = "maxMemMB", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public int? MaxMemMB
        { get; set; }

        /// <summary>
        /// Gets or sets
        ///
        /// </summary>
        /// <remarks>
        /// The default value is <c>1000000</c>.
        /// </remarks>
        [DataMember(Name = "maxMetaEntries", EmitDefaultValue = false)]
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
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
        [DefaultValue(null)]
        public string WarmToColdScript
        { get; set; }

        #endregion
    }
}