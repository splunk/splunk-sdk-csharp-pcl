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

namespace Splunk.Client.Examples
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
            // 3. Instantiate a Splunk.Client.Context with the WebRequestHandler

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        static void Main(string[] args)
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }
        }

        static async Task Run(Service service)
        {
            //// Login

            await service.LoginAsync("admin", "changeme");

            //// Search : Pull model (foreach loop => IEnumerable)

            Job job = await service.CreateJobAsync("search index=_internal | head 10");
            SearchResultStream searchResultStream;

            using (searchResultStream = await job.GetSearchResultsAsync())
            {
                int recordNumber = 0;
                try
                {
                    foreach (var record in searchResultStream.ToEnumerable())
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("SearchResults error: {0}", e.Message));
                }
            }

            //// Search : Push model (by way of subscription to search result records => IObservable)

            job = await service.CreateJobAsync("search index=_internal | head 10");

            using (searchResultStream = await job.GetSearchResultsAsync())
            {
                var manualResetEvent = new ManualResetEvent(true);
                int recordNumber = 0;

                searchResultStream.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe(
                    onNext: (record) =>
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                    },
                    onError: (e) =>
                    {
                        Console.WriteLine(string.Format("SearchResults error: {0}", e.Message));
                        manualResetEvent.Set();
                    },
                    onCompleted: () =>
                    {
                        manualResetEvent.Set();
                    });

                manualResetEvent.Reset();
                manualResetEvent.WaitOne();
            }

            //// Search : Export

            SearchExportStream searchExportStream;

            using (searchExportStream = service.StartSearchExportAsync("search index=_internal | head 100").Result)
            {
                int recordNumber = 0;
                int setNumber = 0;

                foreach (var resultStream in searchExportStream.ToEnumerable())
                {
                    Console.WriteLine(string.Format("Result set {0}", ++setNumber));

                    foreach (var record in resultStream.ToEnumerable())
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                    }
                }
            }

            //// Search : Oneshot

            using (searchResultStream = await service.SearchOneshotAsync("search index=_internal | head 10"))
            {
                foreach (var result in searchResultStream.ToEnumerable())
                {
                    Console.WriteLine(result);
                }
            }

            //// Search : Results preview

            job = await service.CreateJobAsync("search index=_internal | head 10000");
            do
            {
                using (searchResultStream = await job.GetSearchResultsPreviewAsync(new SearchResultsArgs() { Count = 0 }))
                {
                    int recordNumber = 0;

                    foreach (var record in searchResultStream.ToEnumerable())
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, record));
                    }
                }
            }
            while (searchResultStream.ArePreview);

            //// Search : Saved search

            job = await service.DispatchSavedSearchAsync("Splunk errors last 24 hours", dispatchArgs: new SavedSearchDispatchArgs());

            using (searchResultStream = await job.GetSearchResultsAsync(new SearchResultsArgs() { Count = 0 }))
            {
                int recordNumber = 0;

                foreach (var result in searchResultStream.ToEnumerable())
                {
                    Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, result));
                }
            }
        }

        static async void Other()
        {
            SearchResultStream searchResultStream;
            Job job;

            //// Login

            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            Console.WriteLine("Blocking search");
            job = await service.CreateJobAsync("search index=_internal | head 10", new JobArgs() { ExecutionMode = ExecutionMode.Blocking });

            using (searchResultStream = await job.GetSearchResultsAsync())
            {
                foreach (var result in searchResultStream.ToEnumerable())
                {
                    Console.WriteLine(result);
                }
            }

            using (var searchExportStream = await service.StartSearchExportAsync("search index=_internal | head 100000"))
            {
                Console.WriteLine("Begin: Service.SearchExportAsync: Asyncrhonous use case");
                int recordNumber = 0;
                int setNumber = 0;

                foreach (var resultStream in searchExportStream.ToEnumerable())
                {
                    Console.WriteLine(string.Format("Result set {0}", ++setNumber));
                    var manualResetEvent = new ManualResetEvent(true);

                    resultStream.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe(
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
                        });

                    manualResetEvent.Reset(); 
                    manualResetEvent.WaitOne();
                }
            }
        }
    }
}
