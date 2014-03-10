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
            SearchResultsReader searchResultsReader;
            SearchResults searchResults;
            Job job;

            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            service.LoginAsync("admin", "changeme").Wait();

            // Saved search

            Console.WriteLine("Begin: Service.DispatchSavedSearch: Syncrhonous use case");
            job = service.DispatchSavedSearchAsync("Splunk errors last 24 hours", dispatchArgs: new SavedSearchDispatchArgs()).Result;
            
            using (searchResults = job.GetSearchResultsAsync(new SearchResultArgs() { Count = 0 }).Result)
            {
                Console.WriteLine(string.Format("searchResults.ArePreview: {0}", searchResults.ArePreview));
                int recordNumber = 0;

                foreach (var record in searchResults)
                {
                    Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                }
            }

            // Export search

            using (searchResultsReader = service.SearchExportAsync("search index=_internal | head 100").Result)
            {
                Console.WriteLine("Begin: Service.SearchExportAsnyc: Syncrhonous use case");
                int recordNumber = 0;
                int setNumber = 0;

                foreach (var searchResultSet in searchResultsReader)
                {
                    Console.WriteLine(string.Format("Result set {0}", ++setNumber));

                    foreach (var record in searchResultSet)
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                    }
                }

                Console.WriteLine("End: Service.SearchExportAsync: Syncrhonous use case");
            }

            using (searchResultsReader = service.SearchExportAsync("search index=_internal | head 100000").Result)
            {
                Console.WriteLine("Begin: Service.SearchExportAsync: Asyncrhonous use case");
                int recordNumber = 0;
                int setNumber = 0;

                foreach (var resultSet in searchResultsReader)
                {
                    Console.WriteLine(string.Format("Result set {0}", ++setNumber));
                    var manualResetEvent = new ManualResetEvent(true);

                    resultSet.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe
                    (
                        onNext: (record) =>
                        {
                            Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                        },
                        onError: (exception) =>
                        {
                            Console.WriteLine(string.Format("SearchResults error: {0}", exception.Message));
                            manualResetEvent.Set();
                        },
                        onCompleted: () =>
                        {
                            Console.WriteLine("Service.SearchExportAsync: Asyncrhonous use case");
                            manualResetEvent.Set();
                        }
                    );

                    manualResetEvent.Reset(); manualResetEvent.WaitOne();
                }
            }

            // Oneshot search

            using (searchResults = service.SearchOneshotAsync("search index=_internal | head 10").Result)
            {
                Console.WriteLine("Begin: Service.SearchOneshotAsnyc: Syncrhonous use case");

                foreach (var record in searchResults)
                {
                    Console.WriteLine(record);
                }

                Console.WriteLine("End: Service.SearchOneshotAsnyc: Syncrhonous use case");
            }

            // Search Job

            Console.WriteLine("Normal search: Job.GetSearchResultsAsync");
            job = service.SearchAsync("search index=_internal | head 10").Result;

            using (searchResults = job.GetSearchResultsAsync().Result)
            {
                int recordNumber = 0;

                foreach (var record in searchResults)
                {
                    Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                }
            }                

            Console.WriteLine("Normal search: Job.GetSearchResultsPreviewAsync");
            job = service.SearchAsync("search index=_internal | head 10000").Result;

            do
            {
                using (searchResults = job.GetSearchResultsPreviewAsync(new SearchResultArgs() { Count = 0 }).Result)
                {
                    Console.WriteLine(string.Format("searchResults.ArePreview: {0}", searchResults.ArePreview));
                    int recordNumber = 0;

                    foreach (var record in searchResults)
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                    }
                }
            }
            while (searchResults.ArePreview);

            Console.WriteLine("Blocking search");
            job = service.SearchAsync("search index=_internal | head 10", ExecutionMode.Blocking).Result;

            using (searchResults = job.GetSearchResultsAsync().Result)
            {
                foreach (var record in searchResults)
                {
                    Console.WriteLine(record);
                }
            }
        }
    }
}
