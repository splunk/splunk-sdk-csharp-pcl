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
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class that represents a Splunk data index.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>References:</b>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/UscaUQ">REST API Reference: Indexes</a>.
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class Index : Entity, IIndex
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        protected internal Index(Service service, string name)
            : this(service.Context, service.Namespace, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal Index(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// Name of the index to be represented by the current instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        internal Index(Context context, Namespace ns, string name)
            : base(context, ns, ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Index"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain an <see cref="Index"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Service.CreateIndexAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="Index"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetIndexAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing index.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public Index()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public bool AssureUTF8
        {
            get { return this.Content.GetValue("AssureUTF8", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public int BlockSignSize
        {
            get { return this.Content.GetValue("BlockSignSize", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string BlockSignatureDatabase
        {
            get { return this.Content.GetValue("BlockSignatureDatabase", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public int BloomFilterTotalSizeKB
        {
            get { return this.Content.GetValue("BloomfilterTotalSizeKB", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string BucketRebuildMemoryHint
        {
            get { return this.Content.GetValue("BucketRebuildMemoryHint", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ColdPath
        {
            get { return this.Content.GetValue("ColdPath", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ColdPathExpanded
        {
            get { return this.Content.GetValue("ColdPathExpanded", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ColdPathMaxDataSizeMB
        {
            get { return this.Content.GetValue("ColdPathMaxDataSizeMB", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ColdToFrozenDir
        {
            get { return this.Content.GetValue("ColdToFrozenDir", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ColdToFrozenScript
        {
            get { return this.Content.GetValue("ColdToFrozenScript", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool CompressRawData
        {
            get { return this.Content.GetValue("CompressRawdata", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public long CurrentDBSizeMB
        {
            get { return this.Content.GetValue("CurrentDBSizeMB", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string DefaultDatabase
        {
            get { return this.Content.GetValue("DefaultDatabase", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool Disabled
        {
            get { return this.Content.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public bool EnableOnlineBucketRepair
        {
            get { return this.Content.GetValue("EnableOnlineBucketRepair", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool EnableRealtimeSearch
        {
            get { return this.Content.GetValue("EnableRealtimeSearch", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public int FrozenTimePeriodInSecs
        {
            get { return this.Content.GetValue("FrozenTimePeriodInSecs", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string HomePath
        {
            get { return this.Content.GetValue("HomePath", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string HomePathExpanded
        {
            get { return this.Content.GetValue("HomePathExpanded", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string HomePathMaxDataSizeMB
        {
            get { return this.Content.GetValue("HomePathMaxDataSizeMB", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string IndexThreads
        {
            get { return this.Content.GetValue("IndexThreads", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool IsInternal
        {
            get { return this.Content.GetValue("IsInternal", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool IsReady
        {
            get { return this.Content.GetValue("IsReady", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool IsVirtual
        {
            get { return this.Content.GetValue("IsVirtual", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public long LastInitSequenceNumber
        {
            get { return this.Content.GetValue("LastInitSequenceNumber", Int64Converter.Instance); }
        }

        /// <inheritdoc/>
        public long LastInitTime
        {
            get { return this.Content.GetValue("LastInitTime", Int64Converter.Instance); }
        }
        
        /// <inheritdoc/>
        public string MaxBloomBackfillBucketAge
        {
            get { return this.Content.GetValue("MaxBloomBackfillBucketAge", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxBucketSizeCacheEntries
        {
            get { return this.Content.GetValue("MaxBucketSizeCacheEntries", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxConcurrentOptimizes
        {
            get { return this.Content.GetValue("MaxConcurrentOptimizes", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string MaxDataSize
        {
            get { return this.Content.GetValue("MaxDataSize", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxHotBuckets
        {
            get { return this.Content.GetValue("MaxHotBuckets", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxHotIdleSecs
        {
            get { return this.Content.GetValue("MaxHotIdleSecs", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxHotSpanSecs
        {
            get { return this.Content.GetValue("MaxHotSpanSecs", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxMemMB
        {
            get { return this.Content.GetValue("MaxMemMB", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxMetaEntries
        {
            get { return this.Content.GetValue("MaxMetaEntries", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxRunningProcessGroups
        {
            get { return this.Content.GetValue("MaxRunningProcessGroups", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxRunningProcessGroupsLowPriority
        {
            get { return this.Content.GetValue("MaxRunningProcessGroupsLowPriority", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public DateTime MaxTime
        {
            get { return this.Content.GetValue("MaxTime", DateTimeConverter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxTimeUnreplicatedNoAcks
        {
            get { return this.Content.GetValue("MaxTimeUnreplicatedNoAcks", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxTimeUnreplicatedWithAcks
        {
            get { return this.Content.GetValue("MaxTimeUnreplicatedWithAcks", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxTotalDataSizeMB
        {
            get { return this.Content.GetValue("MaxTotalDataSizeMB", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int MaxWarmDBCount
        {
            get { return this.Content.GetValue("MaxWarmDBCount", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string MemPoolMB
        {
            get { return this.Content.GetValue("MemPoolMB", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string MinRawFileSyncSecs
        {
            get { return this.Content.GetValue("MinRawFileSyncSecs", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public DateTime MinTime
        {
            get { return this.Content.GetValue("MinTime", DateTimeConverter.Instance); }
        }

        /// <inheritdoc/>
        public int NumBloomFilters
        {
            get { return this.Content.GetValue("NumBloomfilters", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int NumHotBuckets
        {
            get { return this.Content.GetValue("NumHotBuckets", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int NumWarmBuckets
        {
            get { return this.Content.GetValue("NumWarmBuckets", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int PartialServiceMetaPeriod
        {
            get { return this.Content.GetValue("PartialServiceMetaPeriod", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int ProcessTrackerServiceInterval
        {
            get { return this.Content.GetValue("ProcessTrackerServiceInterval", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int QuarantineFutureSecs
        {
            get { return this.Content.GetValue("QuarantineFutureSecs", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int QuarantinePastSecs
        {
            get { return this.Content.GetValue("QuarantinePastSecs", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int RawChunkSizeBytes
        {
            get { return this.Content.GetValue("RawChunkSizeBytes", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int RepFactor
        {
            get { return this.Content.GetValue("RepFactor", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int RotatePeriodInSecs
        {
            get { return this.Content.GetValue("RotatePeriodInSecs", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public int ServiceMetaPeriod
        {
            get { return this.Content.GetValue("ServiceMetaPeriod", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public bool ServiceOnlyAsNeeded
        {
            get { return this.Content.GetValue("ServiceOnlyAsNeeded", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public int ServiceSubtaskTimingPeriod
        {
            get { return this.Content.GetValue("ServiceSubtaskTimingPeriod", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public string SummaryHomePathExpanded
        {
            get { return this.Content.GetValue("SummaryHomePathExpanded", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string SuppressBannerList
        {
            get { return this.Content.GetValue("SuppressBannerList", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool Sync
        {
            get { return this.Content.GetValue("Sync", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool SyncMeta
        {
            get { return this.Content.GetValue("SyncMeta", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ThawedPath
        {
            get { return this.Content.GetValue("ThawedPath", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string ThawedPathExpanded
        {
            get { return this.Content.GetValue("ThawedPathExpanded", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public int ThrottleCheckPeriod
        {
            get { return this.Content.GetValue("ThrottleCheckPeriod", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public long TotalEventCount
        {
            get { return this.Content.GetValue("TotalEventCount", Int64Converter.Instance); }
        }

        /// <inheritdoc/>
        public string TStatsHomePath
        {
            get { return this.Content.GetValue("TstatsHomePath", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public string TStatsHomePathExpanded
        {
            get { return this.Content.GetValue("TstatsHomePathExpanded", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public async Task DisableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "disable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.ReconstructSnapshotAsync(response);
            }
        }

        /// <inheritdoc/>
        public async Task EnableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "enable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.ReconstructSnapshotAsync(response);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(IndexAttributes attributes)
        {
            return await this.UpdateAsync(attributes.AsEnumerable());
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("data", "indexes");

        #endregion

        #region Types

        class CreationArgs : Args<CreationArgs>
        {
            /// <summary>
            /// Gets or sets a name for an index.
            /// </summary>
            /// <remarks>
            /// This value is required.
            /// </remarks>
            [DataMember(Name = "name", IsRequired = true)]
            public string Name
            { get; set; }

            /// <summary>
            /// Gets or sets the absolute path for the cold databases of an 
            /// index.
            /// </summary>
            /// <remarks>
            /// <para>
            /// The path must be readable and writable. The path may be defined
            /// in terms of a volume definition. The default value is <c>""</c>
            /// indicating that the cold databases should be stored at the 
            /// default location.</para>
            /// <para>
            /// <b>Caution:</b> Splunk will not start if an index lacks a valid
            /// <see cref="ColdPath"/>.</para>
            /// </remarks>
            [DataMember(Name = "coldPath")]
            [DefaultValue("")]
            public string ColdPath
            { get; set; }

            /// <summary>
            /// Gets or sets an absolute path that contains the hot and warm 
            /// buckets for an index.
            /// </summary>
            /// <remarks>
            /// The specified path must be readable and writable. The default 
            /// value is <c>""</c> indicating that the hot and warm buckets
            /// should be stored at the default location.
            /// <para>
            /// <b>Caution:</b> Splunk will not start if an index lacks a valid
            /// <see cref="HomePath"/>.</para>
            /// </remarks>
            [DataMember(Name = "homePath")]
            [DefaultValue("")]
            public string HomePath
            { get; set; }

            /// <summary>
            /// Gets or sets an absolute path that contains the thawed 
            /// (resurrected) databases for an index.
            /// </summary>
            /// <remarks>
            /// The path must be readable and writable. The path cannot be 
            /// defined in terms of a volume definition. The default value is 
            /// <c>""</c> indicating that resurrected databases should be 
            /// stored at the default location.
            /// <para>
            /// <b>Caution:</b> Splunk will not start if an index lacks a valid
            /// <see cref="ThawedPath"/>.</para>
            /// </remarks>
            [DataMember(Name = "thawedPath")]
            [DefaultValue("")]
            public string ThawedPath
            { get; set; }
        }

        #endregion
    }
}
