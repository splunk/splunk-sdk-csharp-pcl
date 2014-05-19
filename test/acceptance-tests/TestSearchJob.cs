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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using Splunk.Client;
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
        [Trait("class", "Search")]
        [Fact]
        public async void BadOutputMode()
        {
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                var search = "invalidpart" + Query;

                Job job = null;
                try
                {
                    job = service.CreateJobAsync(search).Result;
                }
                catch (Exception e)
                {
                    if (!e.InnerException.Message.ToLower(CultureInfo.InvariantCulture).Contains("400: bad request"))
                    {
                        throw;
                    }
                }
                finally
                {
                    if (job != null)
                    {
                        job.CancelAsync().Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Tests the result from a search argument.
        /// </summary>
        [Trait("class", "Search")]
        [Fact]
        public async void JobSearchMode()
        {
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                JobArgs jobArgs = new JobArgs();

                jobArgs.SearchMode = SearchMode.Normal;
                Job job = await service.CreateJobAsync(Query, jobArgs);
                Assert.NotNull(job);

                jobArgs.SearchMode = SearchMode.Realtime;
                await job.UpdateJobArgs(jobArgs);
                Assert.NotNull(job);

                await job.CancelAsync();
            }
        }

        /// <summary>
        /// Tests the result from a search argument.
        /// </summary>
        [Trait("class", "Search")]
        [Fact]
        public async void JobExecutionMode()
        {
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                JobArgs jobArgs = new JobArgs();

                jobArgs.ExecutionMode = ExecutionMode.Blocking;

                Job job = service.CreateJobAsync(Query, jobArgs).Result;
                Assert.NotNull(job);

                jobArgs.ExecutionMode = ExecutionMode.Normal;
                await job.UpdateJobArgs(jobArgs);
                Assert.NotNull(job);

                jobArgs.ExecutionMode = ExecutionMode.Oneshot;
                await job.UpdateJobArgs(jobArgs);
                Assert.NotNull(job);

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
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                JobArgs jobArgs = new JobArgs();
                ForEachEnum(
                    enumType,
                    (@enum) =>
                    {
                        var job = service.CreateJobAsync(Query, jobArgs).Result;

                        jobFunction(job, @enum);

                        job.CancelAsync().Wait();
                    });
            }
        }

        /// <summary>
        /// Tests all output modes for Job.Events
        /// </summary>
        [Trait("class", "Search")]
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
        [Trait("class", "Search")]
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
        [Trait("class", "Search")]
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
        [Trait("class", "Search")]
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

        [Trait("class", "Search")]
        [Fact]
        public async void JobRefreshTest()
        {
            string search = "search index=_internal * | head 10 ";
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                var job = await service.CreateJobAsync(search);

                this.CheckJob(job, service);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
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
        [Trait("class", "Search")]
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
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                ForEachEnum(
                    enumType,
                    (@enum) => service.ExportSearchPreviewsAsync(search,
                        getJobExportArgs(@enum)).Wait());
            }
        }

        /// <summary>
        /// Run a job for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="getJobArgs">
        /// The funtion to get arguments to run a job.
        /// </param>
        private async void RunJobForEachEnum(
            Type enumType,
            Func<string, JobArgs> getJobArgs)
        {
            TestHelper.GetInstance();
            using (Service service = await TestHelper.Connect())
            {
                ForEachEnum(
                    enumType,
                    (@enum) =>
                    {
                        var job = service.CreateJobAsync(Query, getJobArgs(@enum)).Result;
                        job.CancelAsync().Wait();
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
        private static void ForEachEnum(Type enumType, Action<string> action)
        {
            var enums = Enum.GetNames(enumType);
            foreach (var @enum in enums)
            {
                action(@enum);
            }
        }
    }
}
