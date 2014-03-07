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

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Xunit;

    public class TestJobArgs
    {
        [Trait("class", "JobArgs")]
        [Fact]
        void CanConstruct()
        {
            var expectedKeyValuePairs = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("search", "some unchecked search string")
            };
            string[] expectedString = new string[] {
                "auto_cancel=0; auto_finalize_ec=0; auto_pause=0; earliest_time=null; enable_lookups=t; exec_mode=normal; force_bundle_replication=f; id=null; index_earliest=null; index_latest=null; latest_time=null; max_count=10000; max_time=0; namespace=null; now=null; reduce_freq=0; reload_macros=t; remote_server_list=null; reuse_max_seconds_ago=0; rf=null; rt_blocking=f; rt_indexfilter=f; rt_maxblocksecs=60; rt_queue_size=10000; search=null; search_listener=null; search_mode=normal; spawn_process=t; status_buckets=0; sync_bundle_replication=f; time_format=null; timeout=86400",
                "auto_cancel=0; auto_finalize_ec=0; auto_pause=0; earliest_time=null; enable_lookups=t; exec_mode=normal; force_bundle_replication=f; id=null; index_earliest=null; index_latest=null; latest_time=null; max_count=10000; max_time=0; namespace=null; now=null; reduce_freq=0; reload_macros=t; remote_server_list=null; reuse_max_seconds_ago=0; rf=null; rt_blocking=f; rt_indexfilter=f; rt_maxblocksecs=60; rt_queue_size=10000; search=some unchecked search string; search_listener=null; search_mode=normal; spawn_process=t; status_buckets=0; sync_bundle_replication=f; time_format=null; timeout=86400"
            };
            string search = "some unchecked search string";

            JobArgs args;

            args = new JobArgs();
            Assert.Equal(expectedString[0], args.ToString());
            Assert.Throws(typeof(SerializationException), () => args.ToArray());
            args.Search = search;
            Assert.Equal(expectedString[1], args.ToString());
            Assert.Equal(expectedKeyValuePairs, args.ToArray());

            args = new JobArgs(search);
            Assert.Equal(expectedString[1], args.ToString());
            Assert.Equal(expectedKeyValuePairs, args.ToArray());
            args.Search = null;
            Assert.Equal(expectedString[0], args.ToString());
            Assert.Throws(typeof(SerializationException), () => args.ToArray());
        }

        [Trait("class", "JobArgs")]
        [Fact]
        void HasCorrectDefaults()
        {
            var defaults = new List<KeyValuePair<string, Func<JobArgs, bool>>>()
            {
                new KeyValuePair<string, Func<JobArgs, bool>>("Search",
                    (jobArgs) => jobArgs.Search == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("AutoCancel", 
                    (jobArgs) => jobArgs.AutoCancel == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("AutoFinalizeEventCount",
                    (jobArgs) => jobArgs.AutoFinalizeEventCount == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("AutoPause",
                    (jobArgs) => jobArgs.AutoPause == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("EarliestTime", 
                    (jobArgs) => jobArgs.EarliestTime == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("EnableLookups",
                    (jobArgs) => jobArgs.EnableLookups == true),
                new KeyValuePair<string, Func<JobArgs, bool>>("ExecutionMode",
                    (jobArgs) => jobArgs.ExecutionMode == ExecutionMode.Normal),
                new KeyValuePair<string, Func<JobArgs, bool>>("ForceBundleReplication",
                    (jobArgs) => jobArgs.ForceBundleReplication == false),
                new KeyValuePair<string, Func<JobArgs, bool>>("ForceBundleReplication",
                    (jobArgs) => jobArgs.Id == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("ForceBundleReplication",
                    (jobArgs) => jobArgs.ForceBundleReplication == false),
                new KeyValuePair<string, Func<JobArgs, bool>>("IndexEarliest",
                    (jobArgs) => jobArgs.IndexEarliest == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("IndexLatest",
                    (jobArgs) => jobArgs.IndexLatest == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("LatestTime",
                    (jobArgs) => jobArgs.LatestTime == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("MaxCount",
                    (jobArgs) => jobArgs.MaxCount == 10000),
                new KeyValuePair<string, Func<JobArgs, bool>>("MaxTime",
                    (jobArgs) => jobArgs.MaxTime == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("Namespace",
                    (jobArgs) => jobArgs.Namespace == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("Now",
                    (jobArgs) => jobArgs.Now == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("ReduceFrequency",
                    (jobArgs) => jobArgs.ReduceFrequency == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("ReloadMacros",
                    (jobArgs) => jobArgs.ReloadMacros == true),
                new KeyValuePair<string, Func<JobArgs, bool>>("RemoteServerList",
                    (jobArgs) => jobArgs.RemoteServerList == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("RequiredFieldList",
                    (jobArgs) => jobArgs.RequiredFieldList == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("ReuseMaxSecondsAgo",
                    (jobArgs) => jobArgs.ReuseMaxSecondsAgo == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("RealTimeBlocking",
                    (jobArgs) => jobArgs.RealTimeBlocking == false),
                new KeyValuePair<string, Func<JobArgs, bool>>("RealTimeIndexFilter",
                    (jobArgs) => jobArgs.RealTimeIndexFilter == false),
                new KeyValuePair<string, Func<JobArgs, bool>>("RealTimeMaxBlockSeconds",
                    (jobArgs) => jobArgs.RealTimeMaxBlockSeconds == 60),
                new KeyValuePair<string, Func<JobArgs, bool>>("RealTimeQueueSize",
                    (jobArgs) => jobArgs.RealTimeQueueSize == 10000),
                new KeyValuePair<string, Func<JobArgs, bool>>("SearchListener",
                    (jobArgs) => jobArgs.SearchListener == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("SearchMode",
                    (jobArgs) => jobArgs.SearchMode == SearchMode.Normal),
                new KeyValuePair<string, Func<JobArgs, bool>>("SpawnProcess",
                    (jobArgs) => jobArgs.SpawnProcess == true),
                new KeyValuePair<string, Func<JobArgs, bool>>("StatusBuckets",
                    (jobArgs) => jobArgs.StatusBuckets == 0),
                new KeyValuePair<string, Func<JobArgs, bool>>("SyncBundleReplication",
                    (jobArgs) => jobArgs.SyncBundleReplication == false),
                new KeyValuePair<string, Func<JobArgs, bool>>("TimeFormat",
                    (jobArgs) => jobArgs.TimeFormat == null),
                new KeyValuePair<string, Func<JobArgs, bool>>("Timeout",
                    (jobArgs) => jobArgs.Timeout == 86400),
            };

            var args = new JobArgs();

            foreach (var item in defaults)
            {
                Assert.True(item.Value(args), string.Format("Incorrect default value: JobArgs.{0}", item.Key));
            }
        }
    }
}
