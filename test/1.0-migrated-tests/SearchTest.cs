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
    public class SearchTest : TestHelper
    {
        /// <summary>
        /// Search query which will give 'sg' tags
        /// in output when "segmentation == raw".
        /// </summary>
        private const string Query =
            "search index=_internal GET | head 3";

        ///// <summary>
        ///// Invalid argument
        ///// </summary>
        //private readonly Args<SearchOption> badOutputMode =
        //    new Args("output_mode", "invalid_arg_value");

        ///// <summary>
        ///// Invalid argument
        ///// </summary>
        //private readonly Args<SearchMode> badSearchMode =
        //    new Args("search_mode", "invalid_arg_value");

        ///// <summary>
        ///// Invalid argument
        ///// </summary>
        //private readonly Args badTruncationMode =
        //    new Args("truncation_mode", "invalid_arg_value");

        ///// <summary>
        ///// Invalid argument
        ///// </summary>
        //private readonly Args badExecutionMode =
        //    new Args("exec_mode", "invalid_arg_value");

        ///// <summary>
        ///// Run the given query.
        ///// </summary>
        ///// <param name="service">The service</param>
        ///// <param name="query">The search query</param>
        ///// <returns>The job</returns>
        //private Job Run(Service service, string query)
        //{
        //    return this.Run(service, query, null);
        //}

        ///// <summary>
        ///// Run the given query with the given query args.
        ///// </summary>
        ///// <param name="service">The service</param>
        ///// <param name="query">The search query</param>
        ///// <param name="args">The args</param>
        ///// <returns>The job</returns>
        //private Job Run(Service service, string query, JobArgs args)
        //{
        //    args.Search = query;
        //    return service.StartJob(args);
        //}

        ///// <summary>
        ///// Run the given query and wait for the job to complete.
        ///// </summary>
        ///// <param name="service">The service</param>
        ///// <param name="query">The search query</param>
        ///// <returns>The job</returns>
        //private Job RunWait(Service service, string query)
        //{
        //    return this.RunWait(service, query, null);
        //}

        /// <summary>
        /// Run the given query with the given query args and wait for the job to
        /// complete.
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="jobArgs">The args</param>
        /// <returns>The job</returns>
        private Job RunWait(Service service, JobArgs jobArgs)
        {

            return service.StartJobAsync(jobArgs).Result;
        }

        /// <summary>
        /// Tests the basic create job, wait for it to finish, close the stream
        /// and cancel (clean up) the job on the server. Try with optional args too.
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void Search()
        {
            Service service = Connect();

            //Job job;
            JobArgs jobArgs = new JobArgs(Query);

            service.StartJobAsync(jobArgs).Wait();
            //jobArgs.e="csv"
            service.StartJobAsync(jobArgs).Wait();


            //this.RunWait(service, Query);
            //job.Results(new Args("output_mode", "json")).Close();
            //job.Cancel();
        }

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void SegmentationWithExport()
        //{
        //    VerifySegmentation(
        //        (service, query, args) => service.Export(query, args));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void SegmentationWithOneshot()
        //{
        //    VerifySegmentation(
        //        (service, query, args) => service.Oneshot(query, args));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void SegmentationWithJobResults()
        //{
        //    SegmentationWithJob(
        //        (job, resultsArgs) => job.Results(resultsArgs));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void SegmentationWithJobResultsPreview()
        //{
        //    SegmentationWithJob(
        //        (job, resultsArgs) => job.ResultsPreview(resultsArgs));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void SegmentationWithJobEvents()
        //{
        //    SegmentationWithJob(
        //        (job, resultsArgs) => job.Events(resultsArgs));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        ///// <param name="results">
        ///// Get results stream from a Job object.
        ///// </param>
        //private void SegmentationWithJob(
        //    Func<Job, Args, Stream> results)
        //{
        //    VerifySegmentation(
        //        (service, query, resultsArgs) =>
        //        {
        //            var job = this.RunWait(service, query);
        //            return results(job, resultsArgs);
        //        });
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        ///// <param name="getResults">
        ///// Function to get a results stream.
        ///// </param>
        //private void VerifySegmentation(
        //    Func<Service, string, Args, Stream> getResults)
        //{
        //    var service = Connect();

        //    // SDK's segmentation default has no impact on Splunk 4.3.5 (or earlier).
        //    var segmentationDefaultEffective = true;//this.VersionCompare(service,"5.0") >= 0;

        //    var countSgWithDefault = CountSgIn(
        //        () => getResults(
        //            service,
        //            Query,
        //            null));

        //    if (segmentationDefaultEffective)
        //    {
        //        Assert.Equal(0, countSgWithDefault);
        //    }

        //    var args = new Args
        //        {
        //            { "segmentation", "raw" }
        //        };

        //    var countSgWithSegmentationRaw = CountSgIn(
        //        () => getResults(
        //            service,
        //            Query,
        //            args));

        //    Assert.AreNotEqual(0, countSgWithSegmentationRaw);

        //    if (!segmentationDefaultEffective)
        //    {
        //        Assert.Equal(
        //            countSgWithDefault,
        //            countSgWithSegmentationRaw);
        //    }
        //}

        /// <summary>
        /// Count the number of sg tags in a stream
        /// </summary>
        /// <param name="getStream">Function to return a stream</param>
        /// <returns>The count</returns>
        private static int CountSgIn(
            Func<Stream> getStream)
        {
            const string SgTag = "<sg";

            using (var input = getStream())
            using (var reader = new StreamReader(input))
            {
                var data = reader.ReadToEnd();
                return Regex.Matches(data, SgTag).Count;
            }
        }

        /// <summary>
        /// Tests the result from a bad search argument.
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void BadOutputMode()
        {
            var service = Connect();
            JobArgs jobArgs = new JobArgs( "invalidpart" + Query);
            //jobArgs.outuputMode = badOutputMode;

            Job job = null;
            try
            {
                job = service.StartJobAsync(jobArgs).Result;
            }
            catch (Exception e)
            {
                if (!e.InnerException.Message.ToLower().Contains("400: bad request"))
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

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadTruncateMode()
        //{
        //    var service = Connect();

        //    var job = this.RunWait(service, Query);
        //    job.Events(badTruncationMode).Close();
        //    job.Cancel();
        //}


        /// <summary>
        /// Tests the result from a search argument.
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void JobSearchMode()
        {
            var service = Connect();
            JobArgs jobArgs = new JobArgs(Query);

            jobArgs.SearchMode = SearchMode.Normal;
            Job job = service.StartJobAsync(jobArgs).Result;
            Assert.NotNull(job);

            jobArgs.SearchMode = SearchMode.Realtime;
            job.UpdateJobArgs(jobArgs).Wait();
            Assert.NotNull(job);

            //try
            //{

            //    jobArgs.SearchMode = SearchMode.None;
            //    job.UpdateJobArgs(jobArgs).Wait();
            //    Assert.Fail("SearchMode.None should return 400 invalid query error");
            //}
            //catch (Exception e)
            //{
            //    Assert.True(e.InnerException.Message.Contains("400"));
            //}

            job.CancelAsync().Wait();
        }

        /// <summary>
        /// Tests the result from a search argument.
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void JobExecutionMode()
        {
            var service = Connect();
            JobArgs jobArgs = new JobArgs(Query);
            
            jobArgs.ExecutionMode = ExecutionMode.Blocking;

            Job job = service.StartJobAsync(jobArgs).Result;
            Assert.NotNull(job);

            jobArgs.ExecutionMode = ExecutionMode.None;
            job.UpdateJobArgs(jobArgs).Wait();
            Assert.NotNull(job);

            jobArgs.ExecutionMode = ExecutionMode.Normal;
            job.UpdateJobArgs(jobArgs).Wait();
            Assert.NotNull(job);

            jobArgs.ExecutionMode = ExecutionMode.Oneshot;
            job.UpdateJobArgs(jobArgs).Wait();
            Assert.NotNull(job);

            job.CancelAsync().Wait();
        }

        /// <summary>
        /// Tests the result from a bad search argument.
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void BadSearchModeExport()
        {
            var service = Connect();
            JobArgs jobArgs = new JobArgs(Query);
            
            //jobArgs.SearchMode = SearchMode.Realtime;

            Job job = service.StartJobAsync(jobArgs).Result;
            Assert.NotNull(job);

            job.CancelAsync().Wait();
        }

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadOutputModeExport()
        //{
        //    var service = Connect();

        //    service.Export(Query, badOutputMode);
        //}

        ///// <summary>
        ///// Tests all output modes for Job.Results
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void JobResultsOutputModeArgument()
        //{
        //    var type = typeof(JobResultsArgs.OutputModeEnum);

        //    RunJobFuntionForEachEnum(
        //        type,
        //        (job, mode) =>
        //            job.Results(
        //                new JobResultsArgs
        //                {
        //                    OutputMode =
        //                        (JobResultsArgs.OutputModeEnum)Enum.Parse(
        //                            type,
        //                            mode)
        //                }));
        //}

        ///// <summary>
        ///// Unittest for DVPL-2678, make sure the result stream can be read through.
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void JobResultStream()
        //{
        //    var cli = SplunkSDKHelper.Command.Splunk("search");
        //    cli.AddRule("search", typeof(string), "search string");
        //    cli.Opts["search"] = "search index=_internal * | head 10 ";

        //    var service = Service.Connect(cli.Opts);
        //    var jobs = service.GetJobs();
        //    var job = jobs.Create((string)cli.Opts["search"]);

        //    while (!job.IsDone)
        //    {
        //        System.Threading.Thread.Sleep(1000);
        //    }

        //    var outArgs = new JobResultsArgs
        //    {
        //        OutputMode = JobResultsArgs.OutputModeEnum.Xml,
        //        Count = 0
        //    };

        //    try
        //    {
        //        using (var stream = job.Results(outArgs))
        //        {
        //            using (var rr = new ResultsReaderXml(stream))
        //            {
        //                foreach (var @event in rr)
        //                {
        //                    System.Console.WriteLine("EVENT:");
        //                    GC.Collect();

        //                    foreach (string key in @event.Keys)
        //                    {
        //                        System.Console.WriteLine("   " + key + " -> " + @event[key]);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail(string.Format("Reading Job result throw exception : {0} ", e));
        //    }


        //    try
        //    {
        //        using (var stream = service.Export((string)cli.Opts["search"]))
        //        {
        //            using (var rr = new ResultsReaderXml(stream))
        //            {
        //                foreach (var @event in rr)
        //                {
        //                    System.Console.WriteLine("EVENT:");
        //                    GC.Collect();

        //                    foreach (string key in @event.Keys)
        //                    {
        //                        System.Console.WriteLine("   " + key + " -> " + @event[key]);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail(string.Format("Export result throw exception : {0} ", e));
        //    }
        //}

        ///// <summary>
        ///// Tests all output modes for Job.ResultsPreview
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void JobResultsPreviewOutputModeArgument()
        //{
        //    var type = typeof(SearchResultsArgs);

        //    RunJobFuntionForEachEnum(
        //        type,
        //        (job, mode) =>
        //            job.GetSearchResultsPreviewAsync(
        //                new SearchResultsArgs
        //                {
        //                    OutputMode =
        //                        (SearchResultsArgs)Enum.Parse(
        //                            type,
        //                            mode)
        //                }));
        //}

        ///// <summary>
        ///// Tests all output modes for Job.Events
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void JobEventsOutputModeArgument()
        //{
        //    var type = typeof(JobEventsArgs.OutputModeEnum);

        //    RunJobFuntionForEachEnum(
        //        type,
        //        (job, mode) =>
        //        job.Events(
        //            new JobEventsArgs
        //                {
        //                    OutputMode =
        //                        (JobEventsArgs.OutputModeEnum)Enum.Parse(
        //                            type,
        //                            mode)
        //                }));
        //}

        /// <summary>
        /// Run a job and a function on the job 
        /// for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="jobFunction">
        /// A function for a job and an enum value
        /// </param>
        private void RunJobFuntionForEachEnum(
            Type enumType,
            Func<Job, string, SearchResults> jobFunction)
        {
            var service = Connect();

            JobArgs jobArgs = new JobArgs(Query);
            ForEachEnum(
                enumType,
                (@enum) =>
                {
                    var job = this.RunWait(service, jobArgs);

                    jobFunction(job, @enum);

                    job.CancelAsync().Wait();
                });
        }

        /// <summary>
        /// Tests all output modes for Job.Events
        /// </summary>
        [Trait("class", "Service")]
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
        [Trait("class", "Service")]
        [Fact]
        public void JobSearchModeArgument()
        {
            var type = typeof(SearchMode);
            JobArgs jobArgs = new JobArgs(Query);
            
            RunJobForEachEnum(
                type,
                (mode) => new JobArgs(Query)
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
        [Trait("class", "Service")]
        [Fact]
        public void ExportSearchModeArgument()
        {
            var type = typeof(SearchMode);

            RunExportForEachEnum(
                type,
                (mode) => new SearchExportArgs(Query)
                {                    
                    SearchMode =
                        (SearchMode)Enum.Parse(
                                type,
                                mode)
                });
        }

        ///// <summary>
        ///// Tests all search modes for export
        ///// </summary>
        //[Trait("class", "Service")]
        //[Fact]
        //public void ExportOutputModeArgument()
        //{
        //    var type = typeof(JobExportArgs.OutputModeEnum);

        //    RunExportForEachEnum(
        //        type,
        //        (mode) => new JobExportArgs
        //        {
        //            OutputMode =
        //                (JobExportArgs.OutputModeEnum)Enum.Parse(
        //                        type,
        //                        mode)
        //        });
        //}

        /// <summary>
        /// Tests all search modes for export
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void ExportTruncationModeArgument()
        {
            var type = typeof(TruncationMode);

            RunExportForEachEnum(
                type,
                (mode) => new SearchExportArgs(Query)
                {
                    TruncationMode =
                        (TruncationMode)Enum.Parse(
                                type,
                                mode)
                });
        }

        [Trait("class", "Service")]
        [Fact]
        public void JobRefreshTest()
        {
            var cli = Command.Splunk("search");
            cli.AddRule("search", typeof(string), "search string");
            cli.Opts["search"] = "search index=_internal * | head 10 ";

            var service = new Service(Scheme.Https, "localhost", 8089);
            service.LoginAsync("admin", "changeme").Wait();
            var job = service.StartJobAsync((string)cli.Opts["search"]).Result;

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

        /// <summary>
        /// Run export for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="getJobExportArgs">
        /// The funtion to get arguments to run a job.
        /// </param>
        private void RunExportForEachEnum(
            Type enumType,
            Func<string, SearchExportArgs> getJobExportArgs)
        {
            var service = Connect();

            ForEachEnum(
                enumType,
                (@enum) => service.SearchExportAsync(
                    getJobExportArgs(@enum)).Wait());
        }

        /// <summary>
        /// Run a job for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="getJobArgs">
        /// The funtion to get arguments to run a job.
        /// </param>
        private void RunJobForEachEnum(
            Type enumType,
            Func<string, JobArgs> getJobArgs)
        {
            var service = Connect();

            ForEachEnum(
                enumType,
                (@enum) =>
                {
                    var job = this.RunWait(
                        service,
                        getJobArgs(@enum));

                    job.CancelAsync().Wait();
                });
        }

        /// <summary>
        /// Perform an action for each enum value in an enum type.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="action">
        /// The action to perform on an enum value
        /// </param>
        private static
            void ForEachEnum(
            Type enumType,
            Action<string> action)
        {
            var enums = Enum.GetNames(enumType);
            foreach (var @enum in enums)
            {
                action(@enum);
            }
        }

        /// <summary>
        /// Tests RemoteServerList property
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void RemoteServerList()
        {
            var array = "first,second";// new string[] { "first", "second" };

            var args1 = new SearchExportArgs("")
                {
                    RemoteServerList = array,
                };

            Assert.Equal("first,second", args1.RemoteServerList);

            var args2 = new JobArgs("")
            {
                RemoteServerList = array,
            };

            Assert.Equal("first,second", args2.RemoteServerList);
        }
    }
}
