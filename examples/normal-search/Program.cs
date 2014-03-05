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
    using System.Net;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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

            SearchResultsReader searchResultsReader;

            using (searchResultsReader = service.SearchOneshotAsync("search index=_internal | head 10").Result)
            {
                Console.WriteLine("Syncrhonous user case");

                foreach (var searchResults in searchResultsReader)
                {
                    foreach (var record in searchResults)
                    {
                        Console.WriteLine(record.ToString());
                    }
                }
                Console.WriteLine("End of search results");
            }

            using (searchResultsReader = service.SearchOneshotAsync("search index=_internal | head 10").Result)
            {
                Console.WriteLine("Asyncrhonous user case");

                var manualResetEvent = new ManualResetEvent(true);

                searchResultsReader.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe(
                    onNext: (searchResults) =>
                    {
                        // We use SearchResults.ToEnumerable--which buffers all
                        // input before returning an enumerator--to verify that we
                        // work with Rx operations. In practice you would be 
                        // advised to use the SearchResults enumerator instead.

                        // Requirements: 
                        //
                        // + You must process all records before returning from this
                        //   method.
                        //   Reason: The SearchResultsReader will skip past any 
                        //   records you do not process.
                        //
                        // + You should process records in an asynchronous manner.
                        //   Reactive operations are asynchrononous in the same way
                        //   that the SearchResults enumerator is asynchronous. They 
                        //   each await data to ensure their execution context is 
                        //   time shared.

                        // TODO: Verify all of the above statements.

                        foreach (var record in searchResults.ToEnumerable())
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
                        Console.WriteLine("End of search results");
                        manualResetEvent.Set();
                    }
                );

                manualResetEvent.Reset();
                manualResetEvent.WaitOne();
            }

            // Search Job

            Job job;
            
            Console.WriteLine("Normal search");

            using (job = service.SearchAsync("search index=_internal | head 10").Result)
            {
                var searchResults = job.GetSearchResults().Result;
            }

            Console.WriteLine("Blocking search");

            using (job = service.SearchAsync("search index=_internal | head 10", ExecutionMode.Blocking).Result)
            {
                var searchResults = job.GetSearchResults().Result;
            }
        }
    }
}
