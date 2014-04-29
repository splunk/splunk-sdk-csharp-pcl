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
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class that represents a Splunk data index.
    /// </summary>
    public class Index : Entity<Index>
    {
        #region Constructors

        internal Index(Context context, Namespace @namespace, string name)
            : base(context, @namespace, ClassResourceName, name)
        { }

        public Index()
        { }

        #endregion

        #region Properties

        public bool AssureUTF8
        {
            get { return this.Content.GetValue("AssureUTF8", BooleanConverter.Instance); }
        }

        public int BlockSignSize
        {
            get { return this.Content.GetValue("BlockSignSize", Int32Converter.Instance); }
        }

        public string BlockSignatureDatabase
        {
            get { return this.Content.GetValue("BlockSignatureDatabase", StringConverter.Instance); }
        }

        public int BloomFilterTotalSizeKB
        {
            get { return this.Content.GetValue("BloomfilterTotalSizeKB", Int32Converter.Instance); }
        }

        public string BucketRebuildMemoryHint
        {
            get { return this.Content.GetValue("BucketRebuildMemoryHint", StringConverter.Instance); }
        }

        public string ColdPath
        {
            get { return this.Content.GetValue("ColdPath", StringConverter.Instance); }
        }

        public string ColdPathExpanded
        {
            get { return this.Content.GetValue("ColdPathExpanded", StringConverter.Instance); }
        }

        public string ColdToFrozenDir
        {
            get { return this.Content.GetValue("ColdToFrozenDir", StringConverter.Instance); }
        }

        public string ColdToFrozenScript
        {
            get { return this.Content.GetValue("ColdToFrozenScript", StringConverter.Instance); }
        }

        public bool CompressRawData
        {
            get { return this.Content.GetValue("CompressRawdata", BooleanConverter.Instance); }
        }

        public int CurrentDBSizeMB
        {
            get { return this.Content.GetValue("CurrentDBSizeMB", Int32Converter.Instance); }
        }

        public string DefaultDatabase
        {
            get { return this.Content.GetValue("DefaultDatabase", StringConverter.Instance); }
        }

        public bool Disabled
        {
            get { return this.Content.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the current <see cref="Index"/>.
        /// </summary>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        public bool EnableOnlineBucketRepair
        {
            get { return this.Content.GetValue("EnableOnlineBucketRepair", BooleanConverter.Instance); }
        }

        public bool EnableRealtimeSearch
        {
            get { return this.Content.GetValue("EnableRealtimeSearch", BooleanConverter.Instance); }
        }

        public int FrozenTimePeriodInSecs
        {
            get { return this.Content.GetValue("FrozenTimePeriodInSecs", Int32Converter.Instance); }
        }

        public string HomePath
        {
            get { return this.Content.GetValue("HomePath", StringConverter.Instance); }
        }

        public string HomePathExpanded
        {
            get { return this.Content.GetValue("HomePathExpanded", StringConverter.Instance); }
        }

        public string IndexThreads
        {
            get { return this.Content.GetValue("IndexThreads", StringConverter.Instance); }
        }

        public bool IsInternal
        {
            get { return this.Content.GetValue("IsInternal", BooleanConverter.Instance); }
        }

        public bool IsReady
        {
            get { return this.Content.GetValue("IsReady", BooleanConverter.Instance); }
        }

        public bool IsVirtual
        {
            get { return this.Content.GetValue("IsVirtual", BooleanConverter.Instance); }
        }

        public long LastInitSequenceNumber
        {
            get { return this.Content.GetValue("LastInitSequenceNumber", Int64Converter.Instance); }
        }

        public long LastInitTime
        {
            get { return this.Content.GetValue("LastInitTime", Int64Converter.Instance); }
        }

        public string MaxBloomBackfillBucketAge
        {
            get { return this.Content.GetValue("MaxBloomBackfillBucketAge", StringConverter.Instance); }
        }

        public int MaxBucketSizeCacheEntries
        {
            get { return this.Content.GetValue("MaxBucketSizeCacheEntries", Int32Converter.Instance); }
        }

        public int MaxConcurrentOptimizes
        {
            get { return this.Content.GetValue("MaxConcurrentOptimizes", Int32Converter.Instance); }
        }

        public string MaxDataSize
        {
            get { return this.Content.GetValue("MaxDataSize", StringConverter.Instance); }
        }

        public int MaxHotBuckets
        {
            get { return this.Content.GetValue("MaxHotBuckets", Int32Converter.Instance); }
        }

        public int MaxHotIdleSecs
        {
            get { return this.Content.GetValue("MaxHotIdleSecs", Int32Converter.Instance); }
        }

        public int MaxHotSpanSecs
        {
            get { return this.Content.GetValue("MaxHotSpanSecs", Int32Converter.Instance); }
        }

        public int MaxMemMB
        {
            get { return this.Content.GetValue("MaxMemMB", Int32Converter.Instance); }
        }

        public int MaxMetaEntries
        {
            get { return this.Content.GetValue("MaxMetaEntries", Int32Converter.Instance); }
        }

        public int MaxRunningProcessGroups
        {
            get { return this.Content.GetValue("MaxRunningProcessGroups", Int32Converter.Instance); }
        }

        public int MaxRunningProcessGroupsLowPriority
        {
            get { return this.Content.GetValue("MaxRunningProcessGroupsLowPriority", Int32Converter.Instance); }
        }

        public DateTime MaxTime
        {
            get { return this.Content.GetValue("MaxTime", DateTimeConverter.Instance); }
        }

        public int MaxTimeUnreplicatedNoAcks
        {
            get { return this.Content.GetValue("MaxTimeUnreplicatedNoAcks", Int32Converter.Instance); }
        }

        public int MaxTimeUnreplicatedWithAcks
        {
            get { return this.Content.GetValue("MaxTimeUnreplicatedWithAcks", Int32Converter.Instance); }
        }

        public int MaxTotalDataSizeMB
        {
            get { return this.Content.GetValue("MaxTotalDataSizeMB", Int32Converter.Instance); }
        }

        public int MaxWarmDBCount
        {
            get { return this.Content.GetValue("MaxWarmDBCount", Int32Converter.Instance); }
        }

        public string MemPoolMB
        {
            get { return this.Content.GetValue("MemPoolMB", StringConverter.Instance); }
        }

        public string MinRawFileSyncSecs
        {
            get { return this.Content.GetValue("MinRawFileSyncSecs", StringConverter.Instance); }
        }

        public DateTime MinTime
        {
            get { return this.Content.GetValue("MinTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of bloom filters that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        public int NumBloomFilters
        {
            get { return this.Content.GetValue("NumBloomfilters", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of hot buckets that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        public int NumHotBuckets
        {
            get { return this.Content.GetValue("NumHotBuckets", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of warm buckets that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        public int NumWarmBuckets
        {
            get { return this.Content.GetValue("NumWarmBuckets", Int32Converter.Instance); }
        }

        public int PartialServiceMetaPeriod
        {
            get { return this.Content.GetValue("PartialServiceMetaPeriod", Int32Converter.Instance); }
        }

        public int ProcessTrackerServiceInterval
        {
            get { return this.Content.GetValue("ProcessTrackerServiceInterval", Int32Converter.Instance); }
        }

        public int QuarantineFutureSecs
        {
            get { return this.Content.GetValue("QuarantineFutureSecs", Int32Converter.Instance); }
        }

        public int QuarantinePastSecs
        {
            get { return this.Content.GetValue("QuarantinePastSecs", Int32Converter.Instance); }
        }

        public int RawChunkSizeBytes
        {
            get { return this.Content.GetValue("RawChunkSizeBytes", Int32Converter.Instance); }
        }

        public int RepFactor
        {
            get { return this.Content.GetValue("RepFactor", Int32Converter.Instance); }
        }

        public int RotatePeriodInSecs
        {
            get { return this.Content.GetValue("RotatePeriodInSecs", Int32Converter.Instance); }
        }

        public int ServiceMetaPeriod
        {
            get { return this.Content.GetValue("ServiceMetaPeriod", Int32Converter.Instance); }
        }

        public bool ServiceOnlyAsNeeded
        {
            get { return this.Content.GetValue("ServiceOnlyAsNeeded", BooleanConverter.Instance); }
        }

        public int ServiceSubtaskTimingPeriod
        {
            get { return this.Content.GetValue("ServiceSubtaskTimingPeriod", Int32Converter.Instance); }
        }

        public string SummaryHomePathExpanded
        {
            get { return this.Content.GetValue("SummaryHomePathExpanded", StringConverter.Instance); }
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
            get { return this.Content.GetValue("SuppressBannerList", StringConverter.Instance); }
        }

        public bool Sync
        {
            get { return this.Content.GetValue("Sync", BooleanConverter.Instance); }
        }

        public bool SyncMeta
        {
            get { return this.Content.GetValue("SyncMeta", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the path to the resurrected databases for the current <see 
        /// cref="Index"/>.
        /// </summary>
        public string ThawedPath
        {
            get { return this.Content.GetValue("ThawedPath", StringConverter.Instance); }
        }

        public string ThawedPathExpanded
        {
            get { return this.Content.GetValue("ThawedPathExpanded", StringConverter.Instance); }
        }

        public int ThrottleCheckPeriod
        {
            get { return this.Content.GetValue("ThrottleCheckPeriod", Int32Converter.Instance); }
        }

        public long TotalEventCount
        {
            get { return this.Content.GetValue("TotalEventCount", Int64Converter.Instance); }
        }

        public string TStatsHomePath
        {
            get { return this.Content.GetValue("TstatsHomePath", StringConverter.Instance); }
        }

        public string TStatsHomePathExpanded
        {
            get { return this.Content.GetValue("TstatsHomePathExpanded", StringConverter.Instance); }
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
                await this.UpdateDataAsync(response);
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
                await this.UpdateDataAsync(response);
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
                await this.UpdateDataAsync(response);
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
                AtomFeed feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count > 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }

                this.Data = new DataCache(feed.Entries[0]);
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
