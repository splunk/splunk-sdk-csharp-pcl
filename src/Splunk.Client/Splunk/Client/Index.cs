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
//// [X] Properties & Methods

namespace Splunk.Client
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class that represents a Splunk data index.
    /// </summary>
    public class Index : Entity<Index>
    {
        #region Constructors

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

        /// <summary>
        /// 
        /// </summary>
        public bool AssureUTF8
        {
            get { return this.GetValue("AssureUTF8", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BlockSignSize
        {
            get { return this.GetValue("BlockSignSize", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BlockSignatureDatabase
        {
            get { return this.GetValue("BlockSignatureDatabase", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BloomFilterTotalSizeKB
        {
            get { return this.GetValue("BloomfilterTotalSizeKB", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BucketRebuildMemoryHint
        {
            get { return this.GetValue("BucketRebuildMemoryHint", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColdPath
        {
            get { return this.GetValue("ColdPath", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColdPathExpanded
        {
            get { return this.GetValue("ColdPathExpanded", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColdPathMaxDataSizeMB
        {
            get { return this.GetValue("ColdPathMaxDataSizeMB", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColdToFrozenDir
        {
            get { return this.GetValue("ColdToFrozenDir", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColdToFrozenScript
        {
            get { return this.GetValue("ColdToFrozenScript", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CompressRawData
        {
            get { return this.GetValue("CompressRawdata", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentDBSizeMB
        {
            get { return this.GetValue("CurrentDBSizeMB", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultDatabase
        {
            get { return this.GetValue("DefaultDatabase", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Disabled
        {
            get { return this.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the current <see cref="Index"/>.
        /// </summary>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableOnlineBucketRepair
        {
            get { return this.GetValue("EnableOnlineBucketRepair", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableRealtimeSearch
        {
            get { return this.GetValue("EnableRealtimeSearch", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int FrozenTimePeriodInSecs
        {
            get { return this.GetValue("FrozenTimePeriodInSecs", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HomePath
        {
            get { return this.GetValue("HomePath", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HomePathExpanded
        {
            get { return this.GetValue("HomePathExpanded", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HomePathMaxDataSizeMB
        {
            get { return this.GetValue("ColdPathMaxDataSizeMB", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string IndexThreads
        {
            get { return this.GetValue("IndexThreads", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInternal
        {
            get { return this.GetValue("IsInternal", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReady
        {
            get { return this.GetValue("IsReady", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVirtual
        {
            get { return this.GetValue("IsVirtual", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long LastInitSequenceNumber
        {
            get { return this.GetValue("LastInitSequenceNumber", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long LastInitTime
        {
            get { return this.GetValue("LastInitTime", Int64Converter.Instance); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string MaxBloomBackfillBucketAge
        {
            get { return this.GetValue("MaxBloomBackfillBucketAge", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxBucketSizeCacheEntries
        {
            get { return this.GetValue("MaxBucketSizeCacheEntries", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxConcurrentOptimizes
        {
            get { return this.GetValue("MaxConcurrentOptimizes", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MaxDataSize
        {
            get { return this.GetValue("MaxDataSize", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxHotBuckets
        {
            get { return this.GetValue("MaxHotBuckets", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxHotIdleSecs
        {
            get { return this.GetValue("MaxHotIdleSecs", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxHotSpanSecs
        {
            get { return this.GetValue("MaxHotSpanSecs", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxMemMB
        {
            get { return this.GetValue("MaxMemMB", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxMetaEntries
        {
            get { return this.GetValue("MaxMetaEntries", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxRunningProcessGroups
        {
            get { return this.GetValue("MaxRunningProcessGroups", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxRunningProcessGroupsLowPriority
        {
            get { return this.GetValue("MaxRunningProcessGroupsLowPriority", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime MaxTime
        {
            get { return this.GetValue("MaxTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxTimeUnreplicatedNoAcks
        {
            get { return this.GetValue("MaxTimeUnreplicatedNoAcks", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxTimeUnreplicatedWithAcks
        {
            get { return this.GetValue("MaxTimeUnreplicatedWithAcks", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxTotalDataSizeMB
        {
            get { return this.GetValue("MaxTotalDataSizeMB", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxWarmDBCount
        {
            get { return this.GetValue("MaxWarmDBCount", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MemPoolMB
        {
            get { return this.GetValue("MemPoolMB", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MinRawFileSyncSecs
        {
            get { return this.GetValue("MinRawFileSyncSecs", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime MinTime
        {
            get { return this.GetValue("MinTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of bloom filters that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        public int NumBloomFilters
        {
            get { return this.GetValue("NumBloomfilters", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of hot buckets that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        public int NumHotBuckets
        {
            get { return this.GetValue("NumHotBuckets", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of warm buckets that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        public int NumWarmBuckets
        {
            get { return this.GetValue("NumWarmBuckets", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PartialServiceMetaPeriod
        {
            get { return this.GetValue("PartialServiceMetaPeriod", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ProcessTrackerServiceInterval
        {
            get { return this.GetValue("ProcessTrackerServiceInterval", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int QuarantineFutureSecs
        {
            get { return this.GetValue("QuarantineFutureSecs", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int QuarantinePastSecs
        {
            get { return this.GetValue("QuarantinePastSecs", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RawChunkSizeBytes
        {
            get { return this.GetValue("RawChunkSizeBytes", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RepFactor
        {
            get { return this.GetValue("RepFactor", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RotatePeriodInSecs
        {
            get { return this.GetValue("RotatePeriodInSecs", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ServiceMetaPeriod
        {
            get { return this.GetValue("ServiceMetaPeriod", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ServiceOnlyAsNeeded
        {
            get { return this.GetValue("ServiceOnlyAsNeeded", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ServiceSubtaskTimingPeriod
        {
            get { return this.GetValue("ServiceSubtaskTimingPeriod", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryHomePathExpanded
        {
            get { return this.GetValue("SummaryHomePathExpanded", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the list of indexes for the "index missing" warning banner 
        /// messages are suppressed.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per index setting.
        /// </remarks>
        public string SuppressBannerList
        {
            get { return this.GetValue("SuppressBannerList", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Sync
        {
            get { return this.GetValue("Sync", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SyncMeta
        {
            get { return this.GetValue("SyncMeta", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the path to the resurrected databases for the current <see 
        /// cref="Index"/>.
        /// </summary>
        public string ThawedPath
        {
            get { return this.GetValue("ThawedPath", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ThawedPathExpanded
        {
            get { return this.GetValue("ThawedPathExpanded", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ThrottleCheckPeriod
        {
            get { return this.GetValue("ThrottleCheckPeriod", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long TotalEventCount
        {
            get { return this.GetValue("TotalEventCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TStatsHomePath
        {
            get { return this.GetValue("TstatsHomePath", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TStatsHomePathExpanded
        {
            get { return this.GetValue("TstatsHomePathExpanded", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asyncrhonously creates the index represented by the current index
        /// </summary>
        /// <param name="coldPath">
        /// Location for storing the cold databases for the current <see cref=
        /// "Index"/>. A value of <c>null</c> or <c>""</c> specifies that the 
        /// cold databases should be stored at the default location.
        /// </param>
        /// <param name="homePath">
        /// Location for storing the hot and warm buckets for the current 
        /// index. A value of <c>null</c> or <c>""</c> specifies that the hot
        /// and warm buckets should be stored at the default location.
        /// </param>
        /// <param name="thawedPath">
        /// Location for storing the resurrected databases for the current <see
        /// cref="Index"/>. A value of <c>null</c> or <c>""</c> specifies that 
        /// the resurrected databases should be stored at the default location.
        /// </param>
        /// <param name="attributes">
        /// Attributes to set on the newly created index.
        /// </param>
        /// <returns></returns>
        public async Task CreateAsync(string coldPath = null, string homePath = null, string thawedPath = null, 
            IndexAttributes attributes = null)
        {
            var resourceName = IndexCollection.ClassResourceName;

            var args = new CreationArgs() 
            { 
                Name = this.Name, ColdPath = coldPath, HomePath = homePath, ThawedPath = thawedPath 
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously disables the current <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the POST data/indexes/{name}/disable </a> endpoint 
        /// to disable the current <see cref="Index"/>.
        /// </remarks>
        public async Task DisableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "disable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously enables the current <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the POST data/indexes/{name}/enable </a> endpoint 
        /// to enable the current <see cref="Index"/>.
        /// </remarks>
        public async Task EnableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "enable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously removes the current <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hCc1xe">DELETE 
        /// data/indexes/{name} </a> endpoint to remove the current
        /// <see cref="Index"/>.
        /// <para>
        /// <b>Caution:</b> This operation fully removes the index, not just 
        /// the data contained in it. The index's data directories and 
        /// <a href="http://goo.gl/e4c81J">indexes.conf</a> stanzas are also 
        /// deleted.</para>
        /// </remarks>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asychronously updates the current <see cref="Index"/> with new <see
        /// cref="IndexAttributes"/>.
        /// </summary>
        /// <param name="attributes">
        /// New attributes for the current <see cref="Index"/> instance.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/n3S22S">POST 
        /// data/indexes/{name} </a> endpoint to update the attributes of the
        /// index represented by this instance.
        /// </remarks>
        public async Task UpdateAsync(IndexAttributes attributes)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("data", "indexes");

        #endregion

        #region Types

        public class CreationArgs : Args<CreationArgs>
        {
            #region Properties

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
            /// should be stored at the default location.</para>
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

            #endregion
        }

        #endregion
    }
}
