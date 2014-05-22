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

namespace Splunk.Client.UnitTesting
{
    using Splunk.Client;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using SDKHelper;
    using Xunit;

    /// <summary>
    /// Tests the Index class
    /// </summary>
    public class IndexTest
    {
        /// <summary>
        /// Tests the basic getters and setters of index
        /// </summary>
        [Trait("class", "Index")]
        [Fact]
        public async void IndexAccessors()
        {
            string indexName = "sdk-tests2_indexaccessors";

            await this.RemoveIndex(indexName);
            using (Service service = await SDKHelper.CreateService())
            {
                await service.CreateIndexAsync(indexName);
                var x = service.GetIndexesAsync().Result;

                IndexCollection indexes = await service.GetIndexesAsync();
                foreach (Index idx in indexes)
                {
                    int dummyInt;
                    string dummyString;
                    bool dummyBool;
                    DateTime dummyTime;
                    dummyBool = idx.AssureUTF8;
                    dummyString = idx.BlockSignatureDatabase;
                    dummyInt = idx.BlockSignSize;
                    dummyInt = idx.BloomFilterTotalSizeKB;
                    dummyString = idx.ColdPath;
                    dummyString = idx.ColdPathExpanded;
                    dummyString = idx.ColdToFrozenDir;
                    dummyString = idx.ColdToFrozenScript;
                    dummyBool = idx.CompressRawData;
                    long size = idx.CurrentDBSizeMB;
                    dummyString = idx.DefaultDatabase;
                    dummyBool = idx.EnableRealtimeSearch;
                    dummyInt = idx.FrozenTimePeriodInSecs;
                    dummyString = idx.HomePath;
                    dummyString = idx.HomePathExpanded;
                    dummyString = idx.IndexThreads;
                    long time = idx.LastInitTime;
                    dummyString = idx.MaxBloomBackfillBucketAge;
                    dummyInt = idx.MaxConcurrentOptimizes;
                    dummyString = idx.MaxDataSize;
                    dummyInt = idx.MaxHotBuckets;
                    dummyInt = idx.MaxHotIdleSecs;
                    dummyInt = idx.MaxHotSpanSecs;
                    dummyInt = idx.MaxMemMB;
                    dummyInt = idx.MaxMetaEntries;
                    dummyInt = idx.MaxRunningProcessGroups;
                    dummyTime = idx.MaxTime;
                    dummyInt = idx.MaxTotalDataSizeMB;
                    dummyInt = idx.MaxWarmDBCount;
                    dummyString = idx.MemPoolMB;
                    dummyString = idx.MinRawFileSyncSecs;
                    dummyTime = idx.MinTime;
                    dummyInt = idx.NumBloomFilters;
                    dummyInt = idx.NumHotBuckets;
                    dummyInt = idx.NumWarmBuckets;
                    dummyInt = idx.PartialServiceMetaPeriod;
                    dummyInt = idx.QuarantineFutureSecs;
                    dummyInt = idx.QuarantinePastSecs;
                    dummyInt = idx.RawChunkSizeBytes;
                    dummyInt = idx.RotatePeriodInSecs;
                    dummyInt = idx.ServiceMetaPeriod;
                    dummyString = idx.SuppressBannerList;
                    bool sync = idx.Sync;
                    dummyBool = idx.SyncMeta;
                    dummyString = idx.ThawedPath;
                    dummyString = idx.ThawedPathExpanded;
                    dummyInt = idx.ThrottleCheckPeriod;
                    long eventCount = idx.TotalEventCount;
                    dummyBool = idx.Disabled;
                    dummyBool = idx.IsInternal;
                }

                await this.RemoveIndex(indexName);

                await service.CreateIndexAsync(indexName);
                Index index = await service.GetIndexAsync(indexName);
                var indexAttributes = GetIndexAttributes(index);

                // use setters to update most
                indexAttributes.BlockSignSize = index.BlockSignSize + 1;
                if (TestHelper.VersionCompare(service, "4.3") > 0)
                {
                    indexAttributes.EnableOnlineBucketRepair = !index.EnableOnlineBucketRepair;
                    indexAttributes.MaxBloomBackfillBucketAge = "20d";
                }

                indexAttributes.FrozenTimePeriodInSecs = index.FrozenTimePeriodInSecs + 1;
                indexAttributes.MaxConcurrentOptimizes = index.MaxConcurrentOptimizes + 1;
                indexAttributes.MaxDataSize = "auto";
                indexAttributes.MaxHotBuckets = index.MaxHotBuckets + 1;
                indexAttributes.MaxHotIdleSecs = index.MaxHotIdleSecs + 1;
                indexAttributes.MaxMemMB = index.MaxMemMB + 1;
                indexAttributes.MaxMetaEntries = index.MaxMetaEntries + 1;
                indexAttributes.MaxTotalDataSizeMB = index.MaxTotalDataSizeMB + 1;
                indexAttributes.MaxWarmDBCount = index.MaxWarmDBCount + 1;
                indexAttributes.MinRawFileSyncSecs = "disable";
                indexAttributes.PartialServiceMetaPeriod = index.PartialServiceMetaPeriod + 1;
                indexAttributes.QuarantineFutureSecs = index.QuarantineFutureSecs + 1;
                indexAttributes.QuarantinePastSecs = index.QuarantinePastSecs + 1;
                indexAttributes.RawChunkSizeBytes = index.RawChunkSizeBytes + 1;
                indexAttributes.RotatePeriodInSecs = index.RotatePeriodInSecs + 1;
                indexAttributes.ServiceMetaPeriod = index.ServiceMetaPeriod + 1;
                indexAttributes.SyncMeta = !index.SyncMeta;
                indexAttributes.ThrottleCheckPeriod = index.ThrottleCheckPeriod + 1;

                await index.UpdateAsync(indexAttributes);
                await index.DisableAsync();
                Assert.True(index.Disabled);

                await TestHelper.RestartServer();
            }

            using (Service service = await SDKHelper.CreateService())
            {
                Index index = await service.GetIndexAsync(indexName);
                await index.EnableAsync();
                Assert.False(index.Disabled);
                await RemoveIndex(indexName);
            }
        }

        /// <summary>
        /// Tests submitting and streaming events to an index given the indexAttributes argument
        /// and also removing all events from the index
        /// </summary>
        [Trait("class", "Index")]
        [Fact]
        public async void IndexArgs()
        {
            string indexName = "sdk-tests2_indexargs";

            await this.RemoveIndex(indexName);

            using (Service service = await SDKHelper.CreateService())
            {
                await service.CreateIndexAsync(indexName);
                Index index = await service.GetIndexAsync(indexName);
                Assert.False(index.Disabled);

                // submit event using ReceiverSubmitArgs
                const string Source = "splunk-sdk-tests";
                const string SourceType = "splunk-sdk-test-event";
                const string Host = "test-host";
                var args = new ReceiverArgs
                {
                    Index = indexName,
                    Host = Host,
                    Source = Source,
                    SourceType = SourceType,
                };

                Receiver receiver = service.Receiver;
                await receiver.SendAsync(string.Format("{0}, {1}, Hello World.", DateTime.Now, indexName), args);
                await receiver.SendAsync(string.Format("{0}, {1}, Hello World.", DateTime.Now, indexName), args);
                await TestHelper.WaitIndexTotalEventCountUpdated(index, 2);

                SearchResultStream result = await service.SearchOneshotAsync(
                        string.Format(
                            "search index={0} host={1} source={2} sourcetype={3}",
                            indexName,
                            Host,
                            Source,
                            SourceType));

                Assert.Equal(14, result.FieldNames.Count);
            }
        }

        /// <summary>
        /// Test submitting and streaming to a default index given the indexAttributes argument
        /// and also removing all events from the index
        /// </summary>
        [Trait("class", "Index")]
        [Fact]
        public async void DefaultIndexArgs()
        {
            string indexName = "main";

            using (Service service = await SDKHelper.CreateService())
            {
                Index index = await service.GetIndexAsync(indexName);
                long currentEventCount = index.TotalEventCount;
                Assert.NotNull(index);

                //Receiver receiver = service.GetReceiver();
                Receiver receiver = service.Receiver;
                IndexAttributes indexAttributes = GetIndexAttributes(index);
                ReceiverArgs receiverArgs = new ReceiverArgs() { Index = index.Name, };

                // submit event to default index using variable arguments
                await receiver.SendAsync(string.Format("{0}, DefaultIndexArgs string event Hello World", DateTime.Now), receiverArgs);
                await receiver.SendAsync(string.Format("{0}, DefaultIndexArgs string event Hello World 2", DateTime.Now), receiverArgs);

                await TestHelper.WaitIndexTotalEventCountUpdated(index, currentEventCount + 2);

                currentEventCount = currentEventCount + 2;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 4096, leaveOpen: true))
                    {
                        writer.Write(string.Format("{0}, DefaultIndexArgs stream events ", DateTime.Now));
                        writer.Write(string.Format("{0}, DefaultIndexArgs stream events 2", DateTime.Now));
                    }

                    stream.Seek(0, SeekOrigin.Begin);
                    await receiver.SendAsync(stream, receiverArgs);
                }

                await TestHelper.WaitIndexTotalEventCountUpdated(index, currentEventCount + 1);
            }
        }

        /// <summary>
        /// Gets old values from given index, skip saving paths and things we cannot write
        /// </summary>
        /// <param name="index">The Index</param>
        /// <returns>The argument getIndexProperties</returns>
        private IndexAttributes GetIndexAttributes(Index index)
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
        private async Task RemoveIndex(string indexName)
        {
            using (Service service = SDKHelper.CreateService().Result)
            {
                try
                {
                    await service.RemoveIndexAsync(indexName);
                }
                catch (RequestException e)
                {
                    if (e.Message.Contains("is disabled"))
                    {
                        Index index = service.GetIndexAsync(indexName).Result;
                        index.EnableAsync().Wait();
                        TestHelper.RestartServer().Wait();
                        Service service1 = SDKHelper.CreateService().Result;
                        service1.RemoveIndexAsync(indexName).Wait();
                    }
                    else if (!e.Message.Contains("Not found"))
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}
