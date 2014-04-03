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

        string EaiAttributes
        {
            get { return this.Content.GetValue("EaiAttributes", StringConverter.Instance); }
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

        public int IndexThreads
        {
            get { return this.Content.GetValue("IndexThreads", Int32Converter.Instance); }
        }

        public bool IsInternal
        {
            get { return this.Content.GetValue("IsInternal", BooleanConverter.Instance); }
        }

        public DateTime LastInitTime
        {
            get { return this.Content.GetValue("LastInitTime", DateTimeConverter.Instance); }
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

        public long MaxTime
        {
            get { return this.Content.GetValue("MaxTime", Int64Converter.Instance); }
        }

        public string MaxTotalDataSizeMB
        {
            get { return this.Content.GetValue("MaxTotalDataSizeMB", StringConverter.Instance); }
        }

        public string MaxWarmDBCount
        {
            get { return this.Content.GetValue("MaxWarmDBCount", StringConverter.Instance); }
        }

        public string MemPoolMB
        {
            get { return this.Content.GetValue("MemPoolMB", StringConverter.Instance); }
        }

        public string MinRawFileSyncSecs
        {
            get { return this.Content.GetValue("MinRawFileSyncSecs", StringConverter.Instance); }
        }

        public string MinTime
        {
            get { return this.Content.GetValue("MinTime", StringConverter.Instance); }
        }

        public string NumBloomfilters
        {
            get { return this.Content.GetValue("NumBloomfilters", StringConverter.Instance); }
        }

        public string NumHotBuckets
        {
            get { return this.Content.GetValue("NumHotBuckets", StringConverter.Instance); }
        }

        public string NumWarmBuckets
        {
            get { return this.Content.GetValue("NumWarmBuckets", StringConverter.Instance); }
        }

        public string PartialServiceMetaPeriod
        {
            get { return this.Content.GetValue("PartialServiceMetaPeriod", StringConverter.Instance); }
        }

        public string QuarantineFutureSecs
        {
            get { return this.Content.GetValue("QuarantineFutureSecs", StringConverter.Instance); }
        }

        public string QuarantinePastSecs
        {
            get { return this.Content.GetValue("QuarantinePastSecs", StringConverter.Instance); }
        }

        public string RawChunkSizeBytes
        {
            get { return this.Content.GetValue("RawChunkSizeBytes", StringConverter.Instance); }
        }

        public string RotatePeriodInSecs
        {
            get { return this.Content.GetValue("RotatePeriodInSecs", StringConverter.Instance); }
        }

        public string ServiceMetaPeriod
        {
            get { return this.Content.GetValue("ServiceMetaPeriod", StringConverter.Instance); }
        }

        public string Summarize
        {
            get { return this.Content.GetValue("Summarize", StringConverter.Instance); }
        }

        public string SuppressBannerList
        {
            get { return this.Content.GetValue("SuppressBannerList", StringConverter.Instance); }
        }

        public string Sync
        {
            get { return this.Content.GetValue("Sync", StringConverter.Instance); }
        }

        public string SyncMeta
        {
            get { return this.Content.GetValue("SyncMeta", StringConverter.Instance); }
        }

        public string ThawedPath
        {
            get { return this.Content.GetValue("ThawedPath", StringConverter.Instance); }
        }

        public string ThawedPathExpanded
        {
            get { return this.Content.GetValue("ThawedPathExpanded", StringConverter.Instance); }
        }

        public string ThrottleCheckPeriod
        {
            get { return this.Content.GetValue("ThrottleCheckPeriod", StringConverter.Instance); }
        }

        public string TotalEventCount
        {
            get { return this.Content.GetValue("TotalEventCount", StringConverter.Instance); }
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
