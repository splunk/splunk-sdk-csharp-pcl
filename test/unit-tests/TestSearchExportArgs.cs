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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Xunit;

    public class TestSearchExportArgs
    {
        [Trait("unit-test", "Splunk.Client.SearchExportArgs")]
        [Fact]
        void CanConstructSearchExportArgs()
        {
            var args = new SearchExportArgs();
            
            Assert.Equal(
                "auto_cancel=0; " +
                "auto_finalize_ec=0; " +
                "auto_pause=0; " +
                "count=100; " +
                "earliest_time=null; " +
                "enable_lookups=1; " +
                "f=null; " +
                "force_bundle_replication=0; " +
                "id=null; " +
                "index_earliest=null; " +
                "index_latest=null; " +
                "latest_time=null; " +
                "max_lines=0; " +
                "max_time=0; " +
                "namespace=null; " +
                "now=null; " +
                "offset=0; " +
                "output_time_format=null; " +
                "reduce_freq=0; " +
                "reload_macros=1; " +
                "remote_server_list=null; " +
                "rf=null; " +
                "rt_blocking=0; " +
                "rt_indexfilter=0; " +
                "rt_maxblocksecs=60; " +
                "rt_queue_size=10000; " +
                "search=null; " +
                "search_listener=null; " +
                "search_mode=normal; " +
                "segmentation=raw; " +
                "sync_bundle_replication=0; " +
                "time_format=null; " +
                "timeout=86400; " +
                "truncation_mode=abstract",
                args.ToString());

            Assert.Equal(0, args.ToArray().Length);
        }

        [Trait("unit-test", "Splunk.Client.SearchExportArgs")]
        [Fact]
        void CanSetEveryValue()
        {
            var args = new SearchExportArgs()
            {
                AutoCancel = 1,
                AutoFinalizeEventCount = 2,
                AutoPause = 3,
                Count = 10000,
                EarliestTime = "some_unchecked_string",
                EnableLookups = false,
                FieldList = new string[] { "some_unchecked_string", "some_other_unchecked_string" },
                ForceBundleReplication = true,
                Id = "some_unchecked_string",
                IndexEarliest = "some_unchecked_string",
                IndexLatest = "some_unchecked_string",
                LatestTime = "some_unchecked_string",
                MaxLines = 4,
                MaxTime = 5,
                Namespace = "some_unchecked_string",
                Now = "some_unchecked_string",
                Offset = 6,
                OutputTimeFormat = "some_unchecked_string",
                ReduceFrequency = 9,
                RealTimeBlocking = true,
                RealTimeIndexFilter = true,
                RealTimeMaxBlockSeconds = 7,
                RealTimeQueueSize = 8,
                ReloadMacros = false,
                RemoteServerList = "server1,server2",
                RequiredFieldList = new string[] { "some_unchecked_string", "some_other_uncheck_string" },
                Search = "some_unchecked_string",
                SearchListener = "some_unchecked_string",
                SearchMode = SearchMode.Realtime,
                Segmentation = "some_unchecked_string",
                SyncBundleReplication = true,
                TimeFormat = "some_unchecked_string",
                Timeout = 11,
                TruncationMode = TruncationMode.Truncate
            };

            Assert.Equal(36, args.ToArray().Length); // includes two lists with two members each

            Assert.Equal(
                "auto_cancel=1; " +
                "auto_finalize_ec=2; " +
                "auto_pause=3; " +
                "count=10000; " +
                "earliest_time=some_unchecked_string; " +
                "enable_lookups=0; " +
                "f=some_unchecked_string; " +
                "f=some_other_unchecked_string; " +
                "force_bundle_replication=1; " +
                "id=some_unchecked_string; " +
                "index_earliest=some_unchecked_string; " +
                "index_latest=some_unchecked_string; " +
                "latest_time=some_unchecked_string; " +
                "max_lines=4; " +
                "max_time=5; " +
                "namespace=some_unchecked_string; " +
                "now=some_unchecked_string; " +
                "offset=6; " +
                "output_time_format=some_unchecked_string; " +
                "reduce_freq=9; " +
                "reload_macros=0; " +
                "remote_server_list=server1,server2; " +
                "rf=some_unchecked_string; " +
                "rf=some_other_uncheck_string; " +
                "rt_blocking=1; " +
                "rt_indexfilter=1; " +
                "rt_maxblocksecs=7; " +
                "rt_queue_size=8; " +
                "search=some_unchecked_string; " +
                "search_listener=some_unchecked_string; " +
                "search_mode=realtime; " +
                "segmentation=some_unchecked_string; " +
                "sync_bundle_replication=1; " +
                "time_format=some_unchecked_string; " +
                "timeout=11; " +
                "truncation_mode=truncate",
                args.ToString());

            Assert.Equal(new Argument[]
                {
                    new Argument("auto_cancel", "1"),
                    new Argument("auto_finalize_ec", "2"),
                    new Argument("auto_pause", "3"),
                    new Argument("count", "10000"),
                    new Argument("earliest_time", "some_unchecked_string"),
                    new Argument("enable_lookups", 0),
                    new Argument("f", "some_unchecked_string"),
                    new Argument("f", "some_other_unchecked_string"),
                    new Argument("force_bundle_replication", 1),
                    new Argument("id", "some_unchecked_string"),
                    new Argument("index_earliest", "some_unchecked_string"),
                    new Argument("index_latest", "some_unchecked_string"),
                    new Argument("latest_time", "some_unchecked_string"),
                    new Argument("max_lines", "4"),
                    new Argument("max_time", "5"),
                    new Argument("namespace", "some_unchecked_string"),
                    new Argument("now", "some_unchecked_string"),
                    new Argument("offset", "6"),
                    new Argument("output_time_format", "some_unchecked_string"),
                    new Argument("reduce_freq", "9"),
                    new Argument("reload_macros", "0"),
                    new Argument("remote_server_list", "server1,server2"),
                    new Argument("rf", "some_unchecked_string"),
                    new Argument("rf", "some_other_uncheck_string"),
                    new Argument("rt_blocking", 1),
                    new Argument("rt_indexfilter", 1),
                    new Argument("rt_maxblocksecs", "7"),
                    new Argument("rt_queue_size", "8"),
                    new Argument("search", "some_unchecked_string"),
                    new Argument("search_listener", "some_unchecked_string"),
                    new Argument("search_mode", "realtime"),
                    new Argument("segmentation", "some_unchecked_string"),
                    new Argument("sync_bundle_replication", 1),
                    new Argument("time_format", "some_unchecked_string"),
                    new Argument("timeout", "11"),
                    new Argument("truncation_mode", "truncate")
                },
                args.ToArray());
        }
    }
}
