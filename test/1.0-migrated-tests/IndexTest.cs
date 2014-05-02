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
    using Xunit;

    /// <summary>
    /// Tests the Index class
    /// </summary>
    public class IndexTest : TestHelper
    {
        /// <summary>
        /// Polls the index until wither time runs down, or the event count
        /// matches the desired value.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="value">The desired event count value</param>
        /// <param name="seconds">The number seconds to poll</param>
        private async static void WaitUntilEventCount(Index index, int value, int seconds)
        {
            while (seconds > 0)
            {
                await Task.Delay(1000); // 1000ms (1 second sleep)
                seconds = seconds - 1;
                //index.Refresh();
                var count = index.TotalEventCount;
                if (count == value)
                {
                    return;
                }
            }

            Assert.True(false, "Count did not reach the expected in alloted time.");
        }

        /// <summary>
        /// Tests the basic getters and setters of index
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public async Task IndexAccessors()
        {
            string indexName = "sdk-tests2";
            Service service = Connect();
            DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss") +
                         string.Format("{0}{1} ", offset.Offset.Hours.ToString("D2"),
                             offset.Offset.Minutes.ToString("D2"));

            ServerInfo info = await service.Server.GetInfoAsync();

            if ((await service.GetIndexesAsync()).Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
                await service.CreateIndexAsync(indexName);
            }

            //// set can_delete if not set, so we can delete events from the index.
            //User user = service.GetUsers().Get("admin");
            //string[] roles = user.Roles;
            //if (!this.Contains(roles, "can_delete"))
            //{
            //    string[] newRoles = new string[roles.Length + 1];
            //    roles.CopyTo(newRoles, 0);
            //    newRoles[roles.Length] = "can_delete";
            //    user.Roles = newRoles;
            //    user.Update();
            //}

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
                dummyInt = idx.CurrentDBSizeMB;
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

            Index index = null;

            try
            {
                index = await service.GetIndexAsync(indexName);
            }
            catch (Splunk.Client.ResourceNotFoundException)
            { }

            if (index == null)
            {
                index = await service.CreateIndexAsync(indexName);
            }

            await service.GetIndexAsync(indexName);

            var indexAttributes = GetIndexAttributes(index);

            // use setters to update most

            indexAttributes.BlockSignSize = index.BlockSignSize + 1;


            if (this.VersionCompare(service, "4.3") > 0)
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

            // check, then restore using map method
            //index.Refresh();
            await ClearIndex(service, indexName, index);

            await index.DisableAsync();
            Assert.True(index.Disabled);

            this.SplunkRestart();

            service = this.Connect();
            index = await service.GetIndexAsync(indexName);
            //user = service.GetUsers().Get("admin");

            await index.EnableAsync();
            Assert.False(index.Disabled);

            //// Restore original roles
            //user.Roles = roles;
            //user.Update();
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
        /// Tests submitting and streaming events to an index 
        /// and also removing all events from the index
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public async Task IndexEvents()
        {
            string indexName = "sdk-tests2";
            DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss") +
                         string.Format("{0}{1} ", offset.Offset.Hours.ToString("D2"),
                             offset.Offset.Minutes.ToString("D2"));

            Service service = this.Connect();
            ServerInfo info = await service.Server.GetInfoAsync();

            if ((await service.GetIndexesAsync()).Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
                await service.CreateIndexAsync(indexName);
            }

            Index index = await service.GetIndexAsync(indexName);

            //if (index.TotalEventCount > 0)
            //{
            //    //index.Clean(20);
            //}

            Assert.Equal(0, index.TotalEventCount);

            await ClearIndex(service, indexName, index);

            // submit events to index
            //index.Submit(now + " Hello World. \u0150");
            //index.Submit(now + " Goodbye world. \u0150");
            //WaitUntilEventCount(index, 2, 45);

            //await ClearIndex(service, indexName, index);

            //// stream events to index
            //Stream stream = index.Attach();
            //stream.Write(Encoding.UTF8.GetBytes(now + " Hello World again. \u0150\r\n"));
            //stream.Write(Encoding.UTF8.GetBytes(now + " Goodbye World again.\u0150\r\n"));
            //stream.Close();
            //WaitUntilEventCount(index, 2, 45);

            //await ClearIndex(service, indexName, index);
            //index.Clean(180);
            Assert.Equal(0, index.TotalEventCount);

            ////TODO : should be data/inputs/oneshot endpoint tests
            //string filename;
            //if (info.OSName.Equals("Windows"))
            //{
            //    filename = "C:\\Windows\\WindowsUpdate.log"; // normally here
            //}
            //else if (info.OSName.Equals("Linux"))
            //{
            //    filename = "/var/log/syslog";
            //}
            //else if (info.OSName.Equals("Darwin"))
            //{
            //    filename = "/var/log/system.log";
            //}
            //else
            //{
            //    throw new Exception("OS: " + info.OSName + " not supported");
            //}

            //try
            //{
            //    index.Upload(filename);
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("File " + filename + "failed to upload: Exception -> " + e.Message);
            //}

            //await ClearIndex(service, indexName, index);
        }

        /// <summary>
        /// Tests submitting and streaming events to a default index 
        /// and also removing all events from the index
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public async Task DefaultIndex()
        {
            string indexName = "main";
            DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss") +
                         string.Format("{0}{1} ", offset.Offset.Hours.ToString("D2"),
                             offset.Offset.Minutes.ToString("D2"));

            Service service = this.Connect();
            Receiver receiver = service.Receiver;

            if ((await service.GetIndexesAsync()).Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
                await service.CreateIndexAsync(indexName);
            }

            Index index = await service.GetIndexAsync(indexName);
            await index.EnableAsync();
            Assert.False(index.Disabled);

            // submit events to default index
            await receiver.SendAsync(now + " Hello World. \u0150");
            await receiver.SendAsync(now + " Goodbye World. \u0150");

            //// stream event to default index
            //Stream streamDefaultIndex = receiver.Attach();
            //streamDefaultIndex.Write(Encoding.UTF8.GetBytes(now + " Hello World again. \u0150\r\n"));
            //streamDefaultIndex.Write(Encoding.UTF8.GetBytes(now + " Goodbye World again.\u0150\r\n"));
            //streamDefaultIndex.Close();

            UnicodeEncoding uniEncoding = new UnicodeEncoding();
            byte[] inputString = uniEncoding.GetBytes("stream hello world ");
            MemoryStream stream = new MemoryStream();
            stream.Write(inputString, 0, inputString.Length);
            await receiver.SendAsync(stream);
            stream.Close();
        }

        /// <summary>
        /// Clear the index
        /// </summary>
        /// <param name="service">A service</param>
        /// <param name="indexName">The index name</param>
        /// <param name="index">The index object</param>
        private async Task ClearIndex(Service service, string indexName, Index index)
        {
            SearchResults result = await service.SearchOneshotAsync(string.Format("search index={0} * | delete", indexName));

            //StreamReader reader = new StreamReader(stream);
            //string message = reader.ReadToEnd();

            //if (message.Contains("msg type=\"FATAL\""))
            //{
            //    throw new ApplicationException(string.Format("web reqest return error: {0}", message));
            //}

            WaitUntilEventCount(index, 0, 45);
        }

        /// <summary>
        /// Tests submitting and streaming events to an index given the indexAttributes argument
        /// and also removing all events from the index
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public async Task IndexArgs()
        {
            string indexName = "sdk-tests2";
            DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss") +
                         string.Format("{0}{1} ", offset.Offset.Hours.ToString("D2"),
                             offset.Offset.Minutes.ToString("D2"));

            Service service = this.Connect();

            if ((await service.GetIndexesAsync()).Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
                await service.CreateIndexAsync(indexName);
            }

            Index index = await service.GetIndexAsync(indexName);

            //index.Enable();
            Assert.False(index.Disabled);

            IndexAttributes indexAttributes = GetIndexAttributes(index);

            //ClearIndex(service, indexName, index);

            // submit event to index using variable arguments
            //index.Submit(indexAttributes, now + " Hello World. \u0150");
            //index.Submit(indexAttributes, now + " Goodbye World. \u0150");
            //WaitUntilEventCount(index, 2, 45);

            await ClearIndex(service, indexName, index);

            // stream event to index with variable arguments
            //Stream streamArgs = index.Attach(indexAttributes);
            //streamArgs.Write(Encoding.UTF8.GetBytes(now + " Hello World again. \u0150\r\n"));
            //streamArgs.Write(Encoding.UTF8.GetBytes(now + " Goodbye World again.\u0150\r\n"));
            //streamArgs.Close();
            //WaitUntilEventCount(index, 2, 45);

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

            //var receiver = service.GetReceiver();
            Receiver receiver = service.Receiver;
            //receiver.Submit(args, "Hello World.");
            //receiver.Submit(args, "Goodbye world.");
            await receiver.SendAsync("Hello World.", args);
            await receiver.SendAsync("Goodbye world.", args);
            //WaitUntilEventCount(index, 4, 45);
            // verify the fields of events in the index matching the args.
            //using (var stream =
            //    service.Oneshot(
            //        string.Format(
            //            "search index={0} host={1} source={2} sourcetype={3}",
            //            indexName,
            //            Host,
            //            Source,
            //            SourceType)))

            SearchResults result = await service.SearchOneshotAsync(
                    string.Format(
                        "search index={0} host={1} source={2} sourcetype={3}",
                        indexName,
                        Host,
                        Source,
                        SourceType));

            //using (var reader = new ResultsReaderXml(stream))
            //{
            //    Assert.Equal(2, reader.Count());
            //}
            Assert.Equal(14, result.FieldNames.Count);

            Stopwatch watch=new Stopwatch();
            watch.Start();

            while (watch.Elapsed < new TimeSpan(0, 0, 60) || index.TotalEventCount < 2)
            {
                await Task.Delay(1000);
                await index.GetAsync();
            }

            Console.WriteLine("sleep {0}s to wait index.TotalEventCount got updated", watch.Elapsed);
            Assert.Equal(2, index.TotalEventCount);

            await ClearIndex(service, indexName, index);
            //index.Clean(180);
            //Assert.Equal(0, index.TotalEventCount, "Expected the total event count to be 0");
        }

        /// <summary>
        /// Test submitting and streaming to a default index given the indexAttributes argument
        /// and also removing all events from the index
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public async Task DefaultIndexArgs()
        {
            string indexName = "main";
            DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss") +
                         string.Format("{0}{1} ", offset.Offset.Hours.ToString("D2"),
                             offset.Offset.Minutes.ToString("D2"));

            Service service = this.Connect();

            if ((await service.GetIndexesAsync()).Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
                await service.CreateIndexAsync(indexName);
            }

            //Receiver receiver = service.GetReceiver();
            Receiver receiver = new Receiver();
            if ((await service.GetIndexesAsync()).Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
                await service.CreateIndexAsync(indexName);
            }

            Index index = await service.GetIndexAsync(indexName);


            //index.Enable();
            Assert.False(index.Disabled);

            IndexAttributes indexAttributes = GetIndexAttributes(index);
            ReceiverArgs receiverArgs = new ReceiverArgs() { Index = index.Name, };
            // submit event to default index using variable arguments
            //receiver.Log(indexAttributes, "Hello World. \u0150");
            //receiver.Log(indexAttributes, "Goodbye World. \u0150");
            await receiver.SendAsync("Hello World", receiverArgs);
            await receiver.SendAsync("Hello World 2", receiverArgs);

            // stream event to default index with variable arguments
            //Stream streamArgs = receiver.Attach(indexAttributes);
            //streamArgs.Write(Encoding.UTF8.GetBytes(" Hello World again. \u0150\r\n"));
            //streamArgs.Write(Encoding.UTF8.GetBytes(" Goodbye World again.\u0150\r\n"));
            //streamArgs.Close();

            UnicodeEncoding uniEncoding = new UnicodeEncoding();
            byte[] inputString = uniEncoding.GetBytes("stream hello world ");
            MemoryStream stream = new MemoryStream();
            stream.Write(inputString, 0, inputString.Length);
            await receiver.SendAsync(stream, receiverArgs);
            stream.Close();
        }
    }
}
