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

    public class TestJobArgs
    {
        [Trait("unit-test", "Splunk.Client.JobArgs")]
        [Fact]
        void CanConstructJobArgs()
        {
            var args = new JobArgs();
            
            Assert.Equal(
                "auto_cancel=null; " +
                "auto_finalize_ec=null; " +
                "auto_pause=null; " +
                "earliest_time=null; " +
                "enable_lookups=null; " +
                "force_bundle_replication=null; " +
                "id=null; " +
                "index_earliest=null; " +
                "index_latest=null; " +
                "latest_time=null; " +
                "max_count=null; " +
                "max_time=null; " +
                "namespace=null; " +
                "now=null; " +
                "reduce_freq=null; " +
                "reload_macros=null; " +
                "remote_server_list=null; " +
                "reuse_max_seconds_ago=null; " +
                "rf=null; " +
                "rt_blocking=null; " +
                "rt_indexfilter=null; " +
                "rt_maxblocksecs=null; " +
                "rt_queue_size=null; " +
                "search_listener=null; " +
                "search_mode=null; " +
                "spawn_process=null; " +
                "status_buckets=null; " +
                "sync_bundle_replication=null; " +
                "time_format=null; " +
                "timeout=null", 
                args.ToString());

            Assert.Equal(0, args.Count());
        }

        [Trait("unit-test", "Splunk.Client.JobArgs")]
        [Fact]
        void CanSetEveryValue()
        {
            var args = new JobArgs()
            {
                AutoCancel = 1,
                AutoFinalizeEventCount = 2,
                AutoPause = 3,
                EarliestTime = "some_unchecked_string",
                EnableLookups = false,
                ForceBundleReplication = true,
                Id = "some_unchecked_string",
                IndexEarliest = "some_unchecked_string",
                IndexLatest = "some_unchecked_string",
                LatestTime = "some_unchecked_string",
                MaxCount = 4,
                MaxTime = 5,
                Namespace = "some_unchecked_string",
                Now = "some_unchecked_string",
                RealTimeBlocking = true,
                RealTimeIndexFilter = true,
                RealTimeMaxBlockSeconds = 6,
                RealTimeQueueSize = 7,
                ReduceFrequency = 8,
                ReloadMacros = false,
                RemoteServerList = "some_unchecked_string",
                RequiredFieldList = new List<string>() { "some_unchecked_string", "some_other_uncheck_string" },
                ReuseMaxSecondsAgo = 9,
                SearchListener = "some_unchecked_string",
                SearchMode = SearchMode.RealTime,
                SpawnProcess = false,
                StatusBuckets = 10,
                SyncBundleReplication = true,
                TimeFormat = "some_unchecked_string",
                Timeout = 11
            };

            Assert.Equal(
                "auto_cancel=1; " +
                "auto_finalize_ec=2; " +
                "auto_pause=3; " + 
                "earliest_time=some_unchecked_string; " + 
                "enable_lookups=0; " +
                "force_bundle_replication=1; " +
                "id=some_unchecked_string; " +
                "index_earliest=some_unchecked_string; " +
                "index_latest=some_unchecked_string; " +
                "latest_time=some_unchecked_string; " +
                "max_count=4; " +
                "max_time=5; " +
                "namespace=some_unchecked_string; " +
                "now=some_unchecked_string; " +
                "reduce_freq=8; " +
                "reload_macros=0; " +
                "remote_server_list=some_unchecked_string; " +
                "reuse_max_seconds_ago=9; " +
                "rf=some_unchecked_string; " +
                "rf=some_other_uncheck_string; " +
                "rt_blocking=1; " +
                "rt_indexfilter=1; " +
                "rt_maxblocksecs=6; " +
                "rt_queue_size=7; " +
                "search_listener=some_unchecked_string; " +
                "search_mode=realtime; " +
                "spawn_process=0; " +
                "status_buckets=10; " +
                "sync_bundle_replication=1; " +
                "time_format=some_unchecked_string; " +
                "timeout=11", 
                args.ToString());

            Assert.Equal(new List<Argument>()
                {
                    new Argument("auto_cancel", "1"),
                    new Argument("auto_finalize_ec", "2"),
                    new Argument("auto_pause", "3"),
                    new Argument("earliest_time", "some_unchecked_string"),
                    new Argument("enable_lookups", 0),
                    new Argument("force_bundle_replication", 1),
                    new Argument("id", "some_unchecked_string"),
                    new Argument("index_earliest", "some_unchecked_string"),
                    new Argument("index_latest", "some_unchecked_string"),
                    new Argument("latest_time", "some_unchecked_string"),
                    new Argument("max_count", "4"),
                    new Argument("max_time", "5"),
                    new Argument("namespace", "some_unchecked_string"),
                    new Argument("now", "some_unchecked_string"),
                    new Argument("reduce_freq", "8"),
                    new Argument("reload_macros", 0),
                    new Argument("remote_server_list", "some_unchecked_string"),
                    new Argument("reuse_max_seconds_ago", "9"),
                    new Argument("rf", "some_unchecked_string"),
                    new Argument("rf", "some_other_uncheck_string"),
                    new Argument("rt_blocking", 1),
                    new Argument("rt_indexfilter", 1),
                    new Argument("rt_maxblocksecs", "6"),
                    new Argument("rt_queue_size", "7"),
                    new Argument("search_listener", "some_unchecked_string"),
                    new Argument("search_mode", "realtime"),
                    new Argument("spawn_process", 0),
                    new Argument("status_buckets", "10"),
                    new Argument("sync_bundle_replication", 1),
                    new Argument("time_format", "some_unchecked_string"),
                    new Argument("timeout", "11")
                },
                args);
        }
    }
}
