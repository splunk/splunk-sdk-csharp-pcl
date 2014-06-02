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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    
    using Xunit;

    /// <summary>
    /// This is the search test class
    /// </summary>
    public class SearchTest
    {
        /// <summary>
        /// Search query which will give 'sg' tags
        /// in output when "segmentation == raw".
        /// </summary>
        private const string Query =
            "search index=_internal GET | head 3";


        /// <summary>
        /// Tests the result from a bad search argument.
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public async Task BadOutputMode()
        {
            using (Service service = await SDKHelper.CreateService())
            {
                var search = "invalidpart" + Query;

                Exception exception = null;
                Job job = null;

                try
                {
                    job = await service.Jobs.CreateAsync(search);
                }
                catch (BadRequestException)
                {
                    return;
                }
                catch (Exception e)
                {
                    exception = e;
                }

                await job.CancelAsync();
                
                Assert.DoesNotThrow(() =>
                {
                    throw exception;
                });
            }
        }

        /// <summary>
        /// Tests the result from a search argument.
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public async Task JobSearchMode()
        {
            
            using (Service service = await SDKHelper.CreateService())
            {
                JobArgs jobArgs = new JobArgs();

                jobArgs.SearchMode = SearchMode.Normal;
                Job job = await service.Jobs.CreateAsync(Query, jobArgs);
                Assert.NotNull(job);

                jobArgs.SearchMode = SearchMode.Realtime;
                bool updatedSnapshot = await job.UpdateAsync(jobArgs);
                Assert.True(updatedSnapshot);

                await job.CancelAsync();
            }
        }

        /// <summary>
        /// Tests the result from a search argument.
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public async Task JobExecutionMode()
        {
            
            using (Service service = await SDKHelper.CreateService())
            {
                JobArgs jobArgs = new JobArgs();

                jobArgs.ExecutionMode = ExecutionMode.Blocking;

                Job job = await service.Jobs.CreateAsync(Query, jobArgs);
                Assert.NotNull(job);

                jobArgs.ExecutionMode = ExecutionMode.Normal;
                await job.UpdateAsync(jobArgs);
                Assert.NotNull(job);

                jobArgs.ExecutionMode = ExecutionMode.Oneshot;
                bool updatedSnapshot = await job.UpdateAsync(jobArgs);
                Assert.True(updatedSnapshot);

                await job.CancelAsync();
            }
        }

        /// <summary>
        /// Run a job and a function on the job 
        /// for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="jobFunction">
        /// A function for a job and an enum value
        /// </param>
        private async void RunJobFuntionForEachEnum(
            Type enumType,
            Func<Job, string, SearchResultStream> jobFunction)
        {
            
            using (Service service = await SDKHelper.CreateService())
            {
                JobArgs jobArgs = new JobArgs();

                await ForEachEnum(enumType, async enumValue =>
                {
                    var job = await service.Jobs.CreateAsync(Query, jobArgs);
                    jobFunction(job, enumValue);
                    await job.CancelAsync();
                });
            }
        }

        /// <summary>
        /// Tests all output modes for Job.Events
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public void JobEventsTruncationModeArgument()
        {
            var type = typeof(TruncationMode);

            RunJobFuntionForEachEnum(
                type,
                (job, mode) =>
                    job.GetSearchResultsEventsAsync(
                        new SearchEventArgs
                        {
                            TruncationMode =
                                (TruncationMode)Enum.Parse(
                                    type,
                                    mode)
                        }).Result);
        }

        /// <summary>
        /// Tests all search modes
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public void JobSearchModeArgument()
        {
            var type = typeof(SearchMode);
            JobArgs jobArgs = new JobArgs();

            RunJobForEachEnum(
                type,
                (mode) => new JobArgs()
                    {
                        SearchMode =
                            (SearchMode)Enum.Parse(
                                    type,
                                    mode)
                    });
        }

        /// <summary>
        /// Tests all search modes for export
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public void ExportSearchModeArgument()
        {
            var type = typeof(SearchMode);

            RunExportForEachEnum(
                Query,
                type,
                (mode) => new SearchExportArgs()
                {
                    SearchMode =
                        (SearchMode)Enum.Parse(
                                type,
                                mode)
                });
        }

        /// <summary>
        /// Tests all search modes for export
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public void ExportTruncationModeArgument()
        {
            var type = typeof(TruncationMode);

            RunExportForEachEnum(
                Query,
                type,
                (mode) => new SearchExportArgs()
                {
                    TruncationMode =
                        (TruncationMode)Enum.Parse(
                                type,
                                mode)
                });
        }

        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public async Task JobRefreshTest()
        {
            string search = "search index=_internal * | head 10 ";
            
            using (Service service = await SDKHelper.CreateService())
            {
                var job = await service.Jobs.CreateAsync(search);

                this.CheckJob(job, service);
                Stopwatch stopwatch = Stopwatch.StartNew();
                TimeSpan max = new TimeSpan(0, 0, 0, 10);

                while (!job.IsDone)
                {
                    Thread.Sleep(1000);

                    //has to call this to get the job.IsCompleted
                    job.GetAsync().Wait();
                    Console.WriteLine("jobUpdated={0}", job.Updated);

                    if (stopwatch.Elapsed > max)
                    {
                        Assert.False(true, string.Format("The job is not finished within expected time {0} seconds", max.TotalSeconds));
                    }
                }

                job.CancelAsync().Wait();
            }
        }

        /// <summary>
        /// Tests RemoteServerList property
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Job")]
        [Fact]
        public void RemoteServerList()
        {
            var array = "first,second";

            var args1 = new SearchExportArgs()
            {
                RemoteServerList = array,
            };

            Assert.Equal("first,second", args1.RemoteServerList);

            var args2 = new JobArgs()
            {
                RemoteServerList = array,
            };

            Assert.Equal("first,second", args2.RemoteServerList);
        }

        /// <summary>
        /// Touches the job after it is queryable.
        /// </summary>
        /// <param name="job">The job</param>
        private void CheckJob(Job job, Service service)
        {
            string dummyString;
            //string[] dummyList;
            long dummyInt;
            bool dummyBool;
            DateTime dummyDateTime;
            double dummyDouble;

            dummyDateTime = job.CursorTime;
            //dummyString = job.Delegate;
            dummyInt = job.DiskUsage;
            DispatchState dummyDispatchState = job.DispatchState;
            dummyDouble = job.DoneProgress;
            dummyInt = job.DropCount;
            dummyDateTime = job.EarliestTime;
            dummyInt = job.EventAvailableCount;
            dummyInt = job.EventCount;
            dummyInt = job.EventFieldCount;
            dummyBool = job.EventIsStreaming;
            dummyBool = job.EventIsTruncated;
            dummyString = job.EventSearch;
            SortDirection sordirection = job.EventSorting;
            long indexEarliestTime = job.IndexEarliestTime;
            long indexLatestTime = job.IndexLatestTime;
            dummyString = job.Keywords;
            //dummyString = job.Label;

            if (TestHelper.VersionCompare(service, "6.0") < 0)
            {
                dummyDateTime = job.LatestTime;
            }

            dummyInt = job.NumPreviews;
            dummyInt = job.Priority;
            dummyString = job.RemoteSearch;
            //dummyString = job.ReportSearch;
            dummyInt = job.ResultCount;
            dummyBool = job.ResultIsStreaming;
            dummyInt = job.ResultPreviewCount;
            dummyDouble = job.RunDuration;
            dummyInt = job.ScanCount;
            dummyString = job.EventSearch;// Search;
            DateTime jobearliestTime = job.EarliestTime;//SearchEarliestTime;
            DateTime joblatestTime = job.LatestTime;
            IReadOnlyList<string> providers = job.SearchProviders;
            dummyString = job.Sid;
            dummyInt = job.StatusBuckets;
            dummyInt = job.Ttl;
            dummyBool = job.IsDone;
            dummyBool = job.IsFailed;
            dummyBool = job.IsFinalized;
            dummyBool = job.IsPaused;
            dummyBool = job.IsPreviewEnabled;
            dummyBool = job.IsRealTimeSearch;
            dummyBool = job.IsRemoteTimeline;
            dummyBool = job.IsSaved;
            dummyBool = job.IsSavedSearch;
            dummyBool = job.IsZombie;
            Assert.Equal(job.Name, job.Sid);
        }

        /// <summary>
        /// Run export for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="getJobExportArgs">
        /// The funtion to get arguments to run a job.
        /// </param>
        private async void RunExportForEachEnum(
            string search,
            Type enumType,
            Func<string, SearchExportArgs> getJobExportArgs)
        {
            
            using (Service service = await SDKHelper.CreateService())
            {
                await ForEachEnum(enumType, async @enum => 
                    await service.ExportSearchPreviewsAsync(search, getJobExportArgs(@enum)));
            }
        }

        /// <summary>
        /// Run a job for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="getJobArgs">
        /// The funtion to get arguments to run a job.
        /// </param>
        async void RunJobForEachEnum(
            Type enumType,
            Func<string, JobArgs> getJobArgs)
        {
            
            using (Service service = await SDKHelper.CreateService())
            {
                await ForEachEnum(enumType, async @enum =>
                {
                    var job = await service.Jobs.CreateAsync(Query, getJobArgs(@enum));
                    await job.CancelAsync();
                });
            }
        }

        /// <summary>
        /// Perform an action for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="action">
        /// The action to perform on an enum value
        /// </param>
        static async Task ForEachEnum(Type enumType, Func<string, Task> action)
        {
            var enums = Enum.GetNames(enumType);

            foreach (var @enum in enums)
            {
                await action(@enum);
            }
        }
    }
}
