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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Xunit;

    public class TestJobArgs
    {
        [Trait("class", "Args")]
        [Fact]
        void CanConstruct()
        {
            var expectedArguments = new Argument[]
            {
                new Argument("search", "some unchecked search string")
            };
            string[] expectedString = new string[] {
                "auto_cancel=0; auto_finalize_ec=0; auto_pause=0; earliest_time=null; enable_lookups=t; exec_mode=normal; force_bundle_replication=f; id=null; index_earliest=null; index_latest=null; latest_time=null; max_count=10000; max_time=0; namespace=null; now=null; reduce_freq=0; reload_macros=t; remote_server_list=null; reuse_max_seconds_ago=0; rf=null; rt_blocking=f; rt_indexfilter=f; rt_maxblocksecs=60; rt_queue_size=10000; search=null; search_listener=null; search_mode=normal; spawn_process=t; status_buckets=0; sync_bundle_replication=f; time_format=null; timeout=86400",
                "auto_cancel=0; auto_finalize_ec=0; auto_pause=0; earliest_time=null; enable_lookups=t; exec_mode=normal; force_bundle_replication=f; id=null; index_earliest=null; index_latest=null; latest_time=null; max_count=10000; max_time=0; namespace=null; now=null; reduce_freq=0; reload_macros=t; remote_server_list=null; reuse_max_seconds_ago=0; rf=null; rt_blocking=f; rt_indexfilter=f; rt_maxblocksecs=60; rt_queue_size=10000; search=some unchecked search string; search_listener=null; search_mode=normal; spawn_process=t; status_buckets=0; sync_bundle_replication=f; time_format=null; timeout=86400"
            };
            string search = "some unchecked search string";

            var args = new JobArgs(search);
            Assert.Equal(expectedString[1], args.ToString());
            Assert.Equal(expectedArguments, args);
        }

        [Trait("class", "Args")]
        [Fact]
        void CanSetEveryValue()
        {
            var args = new JobArgs("some_unchecked_string")
            {
                AutoCancel = 1,
                AutoFinalizeEventCount = 2,
                AutoPause = 3,
                EarliestTime = "some_unchecked_string",
                EnableLookups = false,
                ExecutionMode = ExecutionMode.Blocking,
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
                SearchMode = SearchMode.Realtime,
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
                "enable_lookups=f; " +
                "exec_mode=blocking; " +
                "force_bundle_replication=t; " +
                "id=some_unchecked_string; " +
                "index_earliest=some_unchecked_string; " +
                "index_latest=some_unchecked_string; " +
                "latest_time=some_unchecked_string; " +
                "max_count=4; " +
                "max_time=5; " +
                "namespace=some_unchecked_string; " +
                "now=some_unchecked_string; " +
                "reduce_freq=8; " +
                "reload_macros=f; " +
                "remote_server_list=some_unchecked_string; " +
                "reuse_max_seconds_ago=9; " +
                "rf=some_unchecked_string; " +
                "rf=some_other_uncheck_string; " +
                "rt_blocking=t; " +
                "rt_indexfilter=t; " +
                "rt_maxblocksecs=6; " +
                "rt_queue_size=7; " +
                "search=some_unchecked_string; " +
                "search_listener=some_unchecked_string; " +
                "search_mode=realtime; " +
                "spawn_process=f; " +
                "status_buckets=10; " +
                "sync_bundle_replication=t; " +
                "time_format=some_unchecked_string; " +
                "timeout=11", 
                args.ToString());

            Assert.Equal(new List<Argument>()
                {
                    new Argument("auto_cancel", "1"),
                    new Argument("auto_finalize_ec", "2"),
                    new Argument("auto_pause", "3"),
                    new Argument("earliest_time", "some_unchecked_string"),
                    new Argument("enable_lookups", "f"),
                    new Argument("exec_mode", "blocking"),
                    new Argument("force_bundle_replication", "t"),
                    new Argument("id", "some_unchecked_string"),
                    new Argument("index_earliest", "some_unchecked_string"),
                    new Argument("index_latest", "some_unchecked_string"),
                    new Argument("latest_time", "some_unchecked_string"),
                    new Argument("max_count", "4"),
                    new Argument("max_time", "5"),
                    new Argument("namespace", "some_unchecked_string"),
                    new Argument("now", "some_unchecked_string"),
                    new Argument("reduce_freq", "8"),
                    new Argument("reload_macros", "f"),
                    new Argument("remote_server_list", "some_unchecked_string"),
                    new Argument("reuse_max_seconds_ago", "9"),
                    new Argument("rf", "some_unchecked_string"),
                    new Argument("rf", "some_other_uncheck_string"),
                    new Argument("rt_blocking", "t"),
                    new Argument("rt_indexfilter", "t"),
                    new Argument("rt_maxblocksecs", "6"),
                    new Argument("rt_queue_size", "7"),
                    new Argument("search", "some_unchecked_string"),
                    new Argument("search_listener", "some_unchecked_string"),
                    new Argument("search_mode", "realtime"),
                    new Argument("spawn_process", "f"),
                    new Argument("status_buckets", "10"),
                    new Argument("sync_bundle_replication", "t"),
                    new Argument("time_format", "some_unchecked_string"),
                    new Argument("timeout", "11")
                },
                args);
        }
    }
}
