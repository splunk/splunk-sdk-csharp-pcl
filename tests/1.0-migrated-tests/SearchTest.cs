/*
 * Copyright 2013 Splunk, Inc.
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

namespace Splunk.Sdk.UnitTesting
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Splunk.Sdk;

    /// <summary>
    /// This is the search test class
    /// </summary>
    [TestClass]
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
        /// <param name="args">The args</param>
        /// <returns>The job</returns>
        private void RunWait(Service service, JobArgs args)
        {
            service.StartJobAsync(args).Wait();

        }

        /// <summary>
        /// Tests the basic create job, wait for it to finish, close the stream
        /// and cancel (clean up) the job on the server. Try with optional args too.
        /// </summary>
        [TestMethod]
        public void Search()
        {
            Service service = Connect();

            //Job job;
            JobArgs jobArgs = new JobArgs();

            jobArgs.Search = Query;
            service.StartJobAsync(jobArgs).Wait();

            jobArgs.Search = Query;
            //jobArgs.outputMode="csv"
            service.StartJobAsync(jobArgs).Wait();


            //this.RunWait(service, Query);
            //job.Results(new Args("output_mode", "json")).Close();
            //job.Cancel();
        }

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[TestMethod]
        //public void SegmentationWithExport()
        //{
        //    VerifySegmentation(
        //        (service, query, args) => service.Export(query, args));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[TestMethod]
        //public void SegmentationWithOneshot()
        //{
        //    VerifySegmentation(
        //        (service, query, args) => service.Oneshot(query, args));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[TestMethod]
        //public void SegmentationWithJobResults()
        //{
        //    SegmentationWithJob(
        //        (job, resultsArgs) => job.Results(resultsArgs));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[TestMethod]
        //public void SegmentationWithJobResultsPreview()
        //{
        //    SegmentationWithJob(
        //        (job, resultsArgs) => job.ResultsPreview(resultsArgs));
        //}

        ///// <summary>
        ///// Verify that segmentation is defaulted to 'none' and can be changed.
        ///// </summary>
        //[TestMethod]
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
        //    var segmentationDefaultEffective = true;//service.VersionCompare("5.0") >= 0;

        //    var countSgWithDefault = CountSgIn(
        //        () => getResults(
        //            service,
        //            Query,
        //            null));

        //    if (segmentationDefaultEffective)
        //    {
        //        Assert.AreEqual(0, countSgWithDefault);
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
        //        Assert.AreEqual(
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
        [TestMethod]
        public void BadOutputMode()
        {
            var service = Connect();
            JobArgs jobArgs = new JobArgs();
            jobArgs.Search = "invalidpart" + Query;
            //jobArgs.outuputMode = badOutputMode;

            Job job = null;
            try
            {
                job = service.StartJob(jobArgs);
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
        //[TestMethod]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadTruncateMode()
        //{
        //    var service = Connect();

        //    var job = this.RunWait(service, Query);
        //    job.Events(badTruncationMode).Close();
        //    job.Cancel();
        //}

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadSearchMode()
        //{
        //    var service = Connect();
        //    JobArgs jobArgs = new JobArgs();
        //    jobArgs.Search = Query;
        //    jobArgs.SearchMode = SearchMode.None;

        //    Job job = null;
        //    try
        //    {
        //        job = service.StartJob(jobArgs);
        //    }
        //    catch (Exception e)
        //    {
        //        if (!e.InnerException.Message.ToLower().Contains("400: bad request"))
        //        {
        //            throw;
        //        }
        //    }
        //    finally
        //    {
        //        if (job != null)
        //        {
        //            job.CancelAsync().Wait();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadExecutionMode()
        //{
        //    var service = Connect();

        //    var job = this.RunWait(
        //        service,
        //        Query,
        //        badExecutionMode);
        //    job.Cancel();
        //}

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadSearchModeExport()
        //{
        //    var service = Connect();

        //    service.Export(Query, badSearchMode);
        //}

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadOutputModeExport()
        //{
        //    var service = Connect();

        //    service.Export(Query, badOutputMode);
        //}

        ///// <summary>
        ///// Tests the result from a bad search argument.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(WebException),
        //  "Bad argument should cause Splunk to return http 400: Bad Request")]
        //public void BadTruncationModeExport()
        //{
        //    var service = Connect();

        //    service.Export(Query, badTruncationMode);
        //}

        ///// <summary>
        ///// Tests all output modes for Job.Results
        ///// </summary>
        //[TestMethod]
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
        //[TestMethod]
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
        //[TestMethod]
        //public void JobResultsPreviewOutputModeArgument()
        //{
        //    var type = typeof(JobResultsPreviewArgs.OutputModeEnum);

        //    RunJobFuntionForEachEnum(
        //        type,
        //        (job, mode) =>
        //            job.ResultsPreview(
        //                new JobResultsPreviewArgs
        //                {
        //                    OutputMode =
        //                        (JobResultsPreviewArgs.OutputModeEnum)Enum.Parse(
        //                            type,
        //                            mode)
        //                }));
        //}

        ///// <summary>
        ///// Tests all output modes for Job.Events
        ///// </summary>
        //[TestMethod]
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

        ///// <summary>
        ///// Tests all output modes for Job.Events
        ///// </summary>
        //[TestMethod]
        //public void JobEventsTruncationModeArgument()
        //{
        //    var type = typeof(JobEventsArgs.TruncationModeEnum);

        //    RunJobFuntionForEachEnum(
        //        type,
        //        (job, mode) =>
        //            job.Events(
        //                new JobEventsArgs
        //                {
        //                    TruncationMode =
        //                        (JobEventsArgs.TruncationModeEnum)Enum.Parse(
        //                            type,
        //                            mode)
        //                }));
        //}

        ///// <summary>
        ///// Run a job and a function on the job 
        ///// for each enum value in an enum type.
        ///// </summary>
        ///// <param name="enumType">The enum type</param>
        ///// <param name="jobFunction">
        ///// A function for a job and an enum value
        ///// </param>
        //private void RunJobFuntionForEachEnum(
        //    Type enumType,
        //    Func<Job, string, Stream> jobFunction)
        //{
        //    var service = Connect();

        //    ForEachEnum(
        //        enumType,
        //        (@enum) =>
        //        {
        //            var job = this.RunWait(service, Query);

        //            jobFunction(job, @enum).Close();

        //            job.Cancel();
        //        });
        //}

        ///// <summary>
        ///// Tests all search modes
        ///// </summary>
        //[TestMethod]
        //public void JobSearchModeArgument()
        //{
        //    var type = typeof(JobArgs.SearchModeEnum);

        //    RunJobForEachEnum(
        //        type,
        //        (mode) => new JobArgs
        //            {
        //                SearchMode =
        //                    (JobArgs.SearchModeEnum)Enum.Parse(
        //                            type,
        //                            mode)
        //            });
        //}

        ///// <summary>
        ///// Tests all search modes for export
        ///// </summary>
        //[TestMethod]
        //public void ExportSearchModeArgument()
        //{
        //    var type = typeof(JobExportArgs.SearchModeEnum);

        //    RunExportForEachEnum(
        //        type,
        //        (mode) => new JobExportArgs
        //        {
        //            SearchMode =
        //                (JobExportArgs.SearchModeEnum)Enum.Parse(
        //                        type,
        //                        mode)
        //        });
        //}

        ///// <summary>
        ///// Tests all search modes for export
        ///// </summary>
        //[TestMethod]
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

        ///// <summary>
        ///// Tests all search modes for export
        ///// </summary>
        //[TestMethod]
        //public void ExportTruncationModeArgument()
        //{
        //    var type = typeof(JobExportArgs.TruncationModeEnum);

        //    RunExportForEachEnum(
        //        type,
        //        (mode) => new JobExportArgs
        //        {
        //            TruncationMode =
        //                (JobExportArgs.TruncationModeEnum)Enum.Parse(
        //                        type,
        //                        mode)
        //        });
        //}

        [TestMethod]
        public void JobRefreshTest()
        {
            var cli = SplunkSDKHelper.Command.Splunk("search");
            cli.AddRule("search", typeof (string), "search string");
            cli.Opts["search"] = "search index=_internal * | head 10 ";

            var service = new Service(Scheme.Https, "localhost", 8089);
            service.LoginAsync("admin", "changeme").Wait();
            var job = service.StartJob((string) cli.Opts["search"]);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            TimeSpan max = new TimeSpan(0, 0, 0, 10);

            while (!job.IsDone)
            {
                Thread.Sleep(1000);

                //has to call this to get the job.IsCompleted
                job.GetAsync();
                Console.WriteLine("jobUpdated={0}", job.Updated);

                if (stopwatch.Elapsed > max)
                {
                    Assert.Fail("the job is not finished within expected time {0} seconds", max.TotalSeconds);
                }
            }

            job.CancelAsync().Wait();
        }

        ///// <summary>
        ///// Run export for each enum value in an enum type.
        ///// </summary>
        ///// <param name="enumType">The enum type</param>
        ///// <param name="getJobExportArgs">
        ///// The funtion to get arguments to run a job.
        ///// </param>
        //private void RunExportForEachEnum(
        //    Type enumType,
        //    Func<string, JobExportArgs> getJobExportArgs)
        //{
        //    var service = Connect();

        //    ForEachEnum(
        //        enumType,
        //        (@enum) => service.Export(
        //            Query,
        //            getJobExportArgs(@enum)));
        //}

        ///// <summary>
        ///// Run a job for each enum value in an enum type.
        ///// </summary>
        ///// <param name="enumType">The enum type</param>
        ///// <param name="getJobArgs">
        ///// The funtion to get arguments to run a job.
        ///// </param>
        //private void RunJobForEachEnum(
        //    Type enumType,
        //    Func<string, JobArgs> getJobArgs)
        //{
        //    var service = Connect();

        //    ForEachEnum(
        //        enumType,
        //        (@enum) =>
        //        {
        //            var job = this.Run(
        //                service,
        //                Query,
        //                getJobArgs(@enum));

        //            job.Cancel();
        //        });
        //}

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

        ///// <summary>
        ///// Tests RemoteServerList property
        ///// </summary>
        //[TestMethod]
        //public void RemoteServerList()
        //{
        //    const string ParamName = "remote_server_list";
        //    var array = new string[] { "first", "second" };

        //    var args1 = new JobArgs
        //        {
        //            RemoteServerList = array,
        //        };

        //    Assert.AreEqual("first,second", args1[ParamName]);

        //    var args2 = new JobExportArgs
        //    {
        //        RemoteServerList = array,
        //    };

        //    Assert.AreEqual("first,second", args2[ParamName]);
        //}
    }
}
