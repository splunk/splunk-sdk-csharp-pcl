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

namespace Splunk.Sdk.Examples
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Starts a normal search and polls for completion to find out when the search has finished.
    /// </summary>
    class Program
    {
        static Program()
        {
            // TODO: Use WebRequestHandler.ServerCertificateValidationCallback instead
            // 1. Instantiate a WebRequestHandler
            // 2. Set its ServerCertificateValidationCallback
            // 3. Instantiate a Splunk.Sdk.Context with the WebRequestHandler

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        static void Main(string[] args)
        {
            var service = new Service(new Context(Scheme.Https, "localhost", 8089), Namespace.Default);
            service.LoginAsync("admin", "changeme").Wait();

            // Oneshot search

            SearchResultsReader reader;

            // ...Syncrhonous use case

            using (reader = service.SearchOneshotAsync("search index=_internal | head 10").Result)
            {
                foreach (var searchResults in reader)
                {
                    foreach (var record in searchResults.ReadRecords())
                    {
                        Console.WriteLine(record.ToString());
                    }
                }
            }

            // ...Asyncrhonous use case

            reader = service.SearchOneshotAsync("search index=_internal | head 10").Result;
            var manualResetEvent = new ManualResetEvent(true);

            IDisposable subscription = reader.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe(
                onNext: (searchResults) =>
                {
                    foreach (var record in searchResults.ToEnumerable<Record>())
                    {
                        Console.WriteLine(record.ToString());
                    }
                },
                onError: (exception) =>
                {
                    Console.WriteLine(string.Format("SearchResultsReader error: {0}", exception.Message));
                    manualResetEvent.Set();
                },
                onCompleted: () =>
                {
                    Console.WriteLine("end-of-searchResults");
                    manualResetEvent.Set();
                }
            );

            manualResetEvent.Reset();
            manualResetEvent.WaitOne();

            reader.Dispose();
#if false
            // Normal asynchronous search with pollback for completion

            var job = service.SearchAsync("search index=_internal | head 10").Result;

            while (!job.IsCompleted)
            {
                try
                {
                    Thread.Sleep(500);
                }
                catch (ThreadInterruptedException e)
                {
                    // TODO Auto-generated catch block
                    System.Console.WriteLine(e.StackTrace);
                }
                Task updateTask = job.UpdateAsync();
                updateTask.Wait();
            }

            // Blocking search

            job = service.SearchAsync("search index=_internal | head 10", ExecutionMode.Blocking).Result;

            // Get the search results and use the built-in XML parser to display them
            var outArgs = new JobResultsArgs
            {
                OutputMode = JobResultsArgs.OutputModeEnum.Xml,
                Count = 0 // Return all entries.
            };

            using (var stream = job.Results(outArgs))
            {
                using (var rr = new ResultsReaderXml(stream))
                {
                    foreach (var @event in rr)
                    {
                        System.Console.WriteLine("EVENT:");
                        foreach (string key in @event.Keys)
                        {
                            System.Console.WriteLine("   " + key + " -> " + @event[key]);
                        }
                    }
                }
            }

            // Get properties of the job
            System.Console.WriteLine("Search job properties:\n---------------------");
            System.Console.WriteLine("Search job ID:         " + job.Sid);
            System.Console.WriteLine("The number of events:  " + job.EventCount);
            System.Console.WriteLine("The number of results: " + job.ResultCount);
            System.Console.WriteLine("Search duration:       " + job.RunDuration + " seconds");
            System.Console.WriteLine("This job expires in:   " + job.Ttl + " seconds");
#endif
        }
    }
}
