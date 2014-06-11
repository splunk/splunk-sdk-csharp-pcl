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

namespace Splunk.Client.UnitTests
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    using Xunit;

    /// <summary>
    /// Tests the Index class
    /// </summary>
    public class IndexTest
    {
        /// <summary>
        /// Tests the basic getters and setters of index
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Index")]
        [Fact]
        public async Task IndexCollection()
        {
            string indexName = "sdk-tests2_indexaccessors";
            await this.RemoveIndexAsync(indexName);

            using (var service = await SDKHelper.CreateService())
            {
                IndexCollection indexes = service.Indexes;

                Index testIndex = await indexes.CreateAsync(indexName);
                //// TODO: Verify testIndex

                await testIndex.GetAsync();
                //// TODO: Reverify testIndex

                await indexes.GetAllAsync();
                Assert.NotNull(indexes.SingleOrDefault(index => index.Title == indexName));

                foreach (Index index in indexes)
                {
                    int dummyInt;
                    string dummyString;
                    bool dummyBool;
                    DateTime dummyTime;
                    dummyBool = index.AssureUTF8;
                    dummyString = index.BlockSignatureDatabase;
                    dummyInt = index.BlockSignSize;
                    dummyInt = index.BloomFilterTotalSizeKB;
                    dummyString = index.ColdPath;
                    dummyString = index.ColdPathExpanded;
                    dummyString = index.ColdToFrozenDir;
                    dummyString = index.ColdToFrozenScript;
                    dummyBool = index.CompressRawData;
                    long size = index.CurrentDBSizeMB;
                    dummyString = index.DefaultDatabase;
                    dummyBool = index.EnableRealtimeSearch;
                    dummyInt = index.FrozenTimePeriodInSecs;
                    dummyString = index.HomePath;
                    dummyString = index.HomePathExpanded;
                    dummyString = index.IndexThreads;
                    long time = index.LastInitTime;
                    dummyString = index.MaxBloomBackfillBucketAge;
                    dummyInt = index.MaxConcurrentOptimizes;
                    dummyString = index.MaxDataSize;
                    dummyInt = index.MaxHotBuckets;
                    dummyInt = index.MaxHotIdleSecs;
                    dummyInt = index.MaxHotSpanSecs;
                    dummyInt = index.MaxMemMB;
                    dummyInt = index.MaxMetaEntries;
                    dummyInt = index.MaxRunningProcessGroups;
                    dummyTime = index.MaxTime;
                    dummyInt = index.MaxTotalDataSizeMB;
                    dummyInt = index.MaxWarmDBCount;
                    dummyString = index.MemPoolMB;
                    dummyString = index.MinRawFileSyncSecs;
                    dummyTime = index.MinTime;
                    dummyInt = index.NumBloomFilters;
                    dummyInt = index.NumHotBuckets;
                    dummyInt = index.NumWarmBuckets;
                    dummyInt = index.PartialServiceMetaPeriod;
                    dummyInt = index.QuarantineFutureSecs;
                    dummyInt = index.QuarantinePastSecs;
                    dummyInt = index.RawChunkSizeBytes;
                    dummyInt = index.RotatePeriodInSecs;
                    dummyInt = index.ServiceMetaPeriod;
                    dummyString = index.SuppressBannerList;
                    bool sync = index.Sync;
                    dummyBool = index.SyncMeta;
                    dummyString = index.ThawedPath;
                    dummyString = index.ThawedPathExpanded;
                    dummyInt = index.ThrottleCheckPeriod;
                    long eventCount = index.TotalEventCount;
                    dummyBool = index.Disabled;
                    dummyBool = index.IsInternal;
                }

                for (int i = 0; i < indexes.Count; i++)
                {
                    Index index = indexes[i];

                    int dummyInt;
                    string dummyString;
                    bool dummyBool;
                    DateTime dummyTime;
                    dummyBool = index.AssureUTF8;
                    dummyString = index.BlockSignatureDatabase;
                    dummyInt = index.BlockSignSize;
                    dummyInt = index.BloomFilterTotalSizeKB;
                    dummyString = index.ColdPath;
                    dummyString = index.ColdPathExpanded;
                    dummyString = index.ColdToFrozenDir;
                    dummyString = index.ColdToFrozenScript;
                    dummyBool = index.CompressRawData;
                    long size = index.CurrentDBSizeMB;
                    dummyString = index.DefaultDatabase;
                    dummyBool = index.EnableRealtimeSearch;
                    dummyInt = index.FrozenTimePeriodInSecs;
                    dummyString = index.HomePath;
                    dummyString = index.HomePathExpanded;
                    dummyString = index.IndexThreads;
                    long time = index.LastInitTime;
                    dummyString = index.MaxBloomBackfillBucketAge;
                    dummyInt = index.MaxConcurrentOptimizes;
                    dummyString = index.MaxDataSize;
                    dummyInt = index.MaxHotBuckets;
                    dummyInt = index.MaxHotIdleSecs;
                    dummyInt = index.MaxHotSpanSecs;
                    dummyInt = index.MaxMemMB;
                    dummyInt = index.MaxMetaEntries;
                    dummyInt = index.MaxRunningProcessGroups;
                    dummyTime = index.MaxTime;
                    dummyInt = index.MaxTotalDataSizeMB;
                    dummyInt = index.MaxWarmDBCount;
                    dummyString = index.MemPoolMB;
                    dummyString = index.MinRawFileSyncSecs;
                    dummyTime = index.MinTime;
                    dummyInt = index.NumBloomFilters;
                    dummyInt = index.NumHotBuckets;
                    dummyInt = index.NumWarmBuckets;
                    dummyInt = index.PartialServiceMetaPeriod;
                    dummyInt = index.QuarantineFutureSecs;
                    dummyInt = index.QuarantinePastSecs;
                    dummyInt = index.RawChunkSizeBytes;
                    dummyInt = index.RotatePeriodInSecs;
                    dummyInt = index.ServiceMetaPeriod;
                    dummyString = index.SuppressBannerList;
                    bool sync = index.Sync;
                    dummyBool = index.SyncMeta;
                    dummyString = index.ThawedPath;
                    dummyString = index.ThawedPathExpanded;
                    dummyInt = index.ThrottleCheckPeriod;
                    long eventCount = index.TotalEventCount;
                    dummyBool = index.Disabled;
                    dummyBool = index.IsInternal;
                }

                var attributes = GetIndexAttributes(testIndex);
                attributes.BlockSignSize = testIndex.BlockSignSize + 1;

                if (TestHelper.VersionCompare(service, "4.3") > 0)
                {
                    attributes.EnableOnlineBucketRepair = !testIndex.EnableOnlineBucketRepair;
                    attributes.MaxBloomBackfillBucketAge = "20d";
                }

                attributes.FrozenTimePeriodInSecs = testIndex.FrozenTimePeriodInSecs + 1;
                attributes.MaxConcurrentOptimizes = testIndex.MaxConcurrentOptimizes + 1;
                attributes.MaxDataSize = "auto";
                attributes.MaxHotBuckets = testIndex.MaxHotBuckets + 1;
                attributes.MaxHotIdleSecs = testIndex.MaxHotIdleSecs + 1;
                attributes.MaxMemMB = testIndex.MaxMemMB + 1;
                attributes.MaxMetaEntries = testIndex.MaxMetaEntries + 1;
                attributes.MaxTotalDataSizeMB = testIndex.MaxTotalDataSizeMB + 1;
                attributes.MaxWarmDBCount = testIndex.MaxWarmDBCount + 1;
                attributes.MinRawFileSyncSecs = "disable";
                attributes.PartialServiceMetaPeriod = testIndex.PartialServiceMetaPeriod + 1;
                attributes.QuarantineFutureSecs = testIndex.QuarantineFutureSecs + 1;
                attributes.QuarantinePastSecs = testIndex.QuarantinePastSecs + 1;
                attributes.RawChunkSizeBytes = testIndex.RawChunkSizeBytes + 1;
                attributes.RotatePeriodInSecs = testIndex.RotatePeriodInSecs + 1;
                attributes.ServiceMetaPeriod = testIndex.ServiceMetaPeriod + 1;
                attributes.SyncMeta = !testIndex.SyncMeta;
                attributes.ThrottleCheckPeriod = testIndex.ThrottleCheckPeriod + 1;

                bool updatedSnapshot = await testIndex.UpdateAsync(attributes);
                Assert.True(updatedSnapshot);

                await testIndex.DisableAsync();
                Assert.True(testIndex.Disabled); // because DisableAsync returns an updated snapshot

                await service.Server.RestartAsync();
            }

            using (var service = await SDKHelper.CreateService())
            {
                Index index = await service.Indexes.GetAsync(indexName);
                await index.EnableAsync();
                Assert.False(index.Disabled);
                await RemoveIndexAsync(indexName);
            }
        }

        /// <summary>
        /// Tests submitting and streaming events to an index given the indexAttributes argument
        /// and also removing all events from the index
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Transmitter")]
        [Fact]
        public async Task Transmitter1()
        {
            string indexName = "sdk-tests2_indexargs";

            await this.RemoveIndexAsync(indexName);

            using (var service = await SDKHelper.CreateService())
            {
                Index index = await service.Indexes.CreateAsync(indexName);
                Assert.False(index.Disabled);

                await Task.Delay(2000);

                // Submit event using TransmitterArgs

                const string Source = "splunk-sdk-tests";
                const string SourceType = "splunk-sdk-test-event";
                const string Host = "test-host";

                var transmitterArgs = new TransmitterArgs
                {
                    Host = Host,
                    Source = Source,
                    SourceType = SourceType,
                };

                Transmitter transmitter = service.Transmitter;
                SearchResult result;

                result = await transmitter.SendAsync(string.Format("1, {0}, {1}, simple event", DateTime.Now, indexName), 
                    indexName, transmitterArgs);
                
                result = await transmitter.SendAsync(string.Format("2, {0}, {1}, simple event", DateTime.Now, indexName), 
                    indexName, transmitterArgs);

                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 4096, leaveOpen: true))
                    {
                        writer.WriteLine(string.Format("1, {0}, {1}, stream event ", DateTime.Now, indexName));
                        writer.WriteLine(string.Format("2, {0}, {1}, stream event", DateTime.Now, indexName));
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    await transmitter.SendAsync(stream, indexName, transmitterArgs);
                }

                await TestHelper.WaitIndexTotalEventCountUpdated(index, 4);

                var search = string.Format(
                    "search index={0} host={1} source={2} sourcetype={3}",
                    indexName,
                    Host,
                    Source,
                    SourceType);

                using (SearchResultStream stream = await service.SearchOneshotAsync(search))
                {
                    Assert.Equal(14, stream.FieldNames.Count);
                }
            }
        }

        /// <summary>
        /// Test submitting and streaming to a default index given the indexAttributes argument
        /// and also removing all events from the index
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Transmitter")]
        public async Task Transmitter2()
        {
            string indexName = "main";

            using (var service = await SDKHelper.CreateService())
            {
                Index index = await service.Indexes.GetAsync(indexName);
                long currentEventCount = index.TotalEventCount;
                Assert.NotNull(index);

                Transmitter transmitter = service.Transmitter;
                IndexAttributes indexAttributes = GetIndexAttributes(index);
                Assert.NotNull(indexAttributes);

                // Submit event to default index using variable arguments

                await transmitter.SendAsync(string.Format("{0}, DefaultIndexArgs string event Hello World", DateTime.Now), indexName);
                await transmitter.SendAsync(string.Format("{0}, DefaultIndexArgs string event Hello World 2", DateTime.Now), indexName);

                await TestHelper.WaitIndexTotalEventCountUpdated(index, currentEventCount + 2);
                currentEventCount += 2;

                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 4096, leaveOpen: true))
                    {
                        writer.WriteLine(string.Format("{0}, DefaultIndexArgs stream events ", DateTime.Now));
                        writer.WriteLine(string.Format("{0}, DefaultIndexArgs stream events 2", DateTime.Now));
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    await transmitter.SendAsync(stream, indexName);
                }

                await TestHelper.WaitIndexTotalEventCountUpdated(index, currentEventCount + 2);
                currentEventCount += 2;
            }
        }

        /// <summary>
        /// Gets old values from given index, skip saving paths and things we cannot write
        /// </summary>
        /// <param name="index">The Index</param>
        /// <returns>The argument getIndexProperties</returns>
        IndexAttributes GetIndexAttributes(Index index)
        {
            IndexAttributes indexAttributes = new IndexAttributes();

            indexAttributes.BlockSignSize = index.BlockSignSize;
            indexAttributes.FrozenTimePeriodInSecs = index.FrozenTimePeriodInSecs;
            indexAttributes.MaxConcurrentOptimizes = index.MaxConcurrentOptimizes;
            indexAttributes.MaxDataSize = index.MaxDataSize;
            indexAttributes.MaxHotBuckets = index.MaxHotBuckets;
            indexAttributes.MaxHotIdleSecs = index.MaxHotIdleSecs;
            indexAttributes.MaxHotSpanSecs = index.MaxHotSpanSecs;
            indexAttributes.MaxMemMB = index.MaxMemMB;
            indexAttributes.MaxMetaEntries = index.MaxMetaEntries;
            indexAttributes.MaxTotalDataSizeMB = index.MaxTotalDataSizeMB;
            indexAttributes.MaxWarmDBCount = index.MaxWarmDBCount;
            indexAttributes.MinRawFileSyncSecs = index.MinRawFileSyncSecs;
            indexAttributes.PartialServiceMetaPeriod = index.PartialServiceMetaPeriod;
            indexAttributes.QuarantineFutureSecs = index.QuarantineFutureSecs;
            indexAttributes.QuarantinePastSecs = index.QuarantinePastSecs;
            indexAttributes.RawChunkSizeBytes = index.RawChunkSizeBytes;
            indexAttributes.RotatePeriodInSecs = index.RotatePeriodInSecs;
            indexAttributes.ServiceMetaPeriod = index.ServiceMetaPeriod;
            indexAttributes.SyncMeta = index.SyncMeta;
            indexAttributes.ThrottleCheckPeriod = index.ThrottleCheckPeriod;

            return indexAttributes;
        }

        /// <summary>
        /// Clear the index
        /// </summary>
        /// <param name="service">A service</param>
        /// <param name="indexName">The index name</param>
        /// <param name="index">The index object</param>
        async Task RemoveIndexAsync(string indexName)
        {
            using (var service = await SDKHelper.CreateService())
            {
                Index index = await service.Indexes.GetOrNullAsync(indexName);
                
                if (index == null)
                {
                    return;
                }

                if (index.Disabled)
                {
                    await index.EnableAsync();
                }

                await index.RemoveAsync();
            }
        }
    }
}
