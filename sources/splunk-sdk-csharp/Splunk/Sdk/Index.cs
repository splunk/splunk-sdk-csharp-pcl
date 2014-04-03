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
// [ ] Contracts
// [ ] Documentation
// [ ] Properties & Methods

namespace Splunk.Sdk
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for manipulating Splunk configuration files.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/cTdaIH">REST API Reference: Accessing and 
    ///     updating Splunk configurations</a>
    /// </description></item>
    /// <item><description>
    ///     <a href="http://goo.gl/0ELhzV">REST API Reference: Indexs</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class Index : Entity<Index>
    {
        #region Constructors

        internal Index(Context context, Namespace @namespace, string name)
            : base(context, @namespace, ResourceName.DataIndexes, name)
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

        public bool Sync
        {
            get { return this.Content.GetValue("Sync", BooleanConverter.Instance); }
        }

        public bool SyncMeta
        {
            get { return this.Content.GetValue("SyncMeta", BooleanConverter.Instance); }
        }

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

        public async Task CreateAsync(string coldPath, string homePath, string thawedPath, IndexAttributes attributes = null)
        {
            await this.CreateAsync(new IndexArgs(coldPath, homePath, thawedPath), attributes);
        }

        public async Task CreateAsync(IndexArgs create, IndexAttributes attributes)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, ResourceName.DataIndexes,
                new Argument[] { new Argument("name", this.Title) },
                create, attributes))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.Created);
                AtomFeed feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count > 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }

                this.Data = new DataObject(feed.Entries[0]);
            }
        }

        /// <summary>
        /// Removes a configuration stanza.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current <see cref=
        /// "Configuration"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE 
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration 
        /// stanza identified by <see cref="stanzaName"/>.
        /// </remarks>
        public void Remove()
        {
            this.RemoveAsync().Wait();
        }

        /// <summary>
        /// Asynchronously removes a configuration stanza.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current <see cref=
        /// "Configuration"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// stanza identified by <see cref="stanzaName"/>.
        /// </remarks>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.OK);
            }
        }

        public void Update(IndexAttributes attributes)
        {
            this.UpdateAsync(attributes).Wait();
        }

        public async Task UpdateAsync(IndexAttributes attributes)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, attributes))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.Created);
                AtomFeed feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count > 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }

                this.Data = new DataObject(feed.Entries[0]);
            }
        }

        #endregion
    }
}
