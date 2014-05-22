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
        static void Main(string[] args)
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }
        }

        static async Task Run(Service service)
        {
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
                searchResultStream.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe(
                    onNext: (record) =>
                    {
                        Console.WriteLine(record.ToString());
                    },
                    onError: (e) =>
                    {
                        Console.WriteLine(string.Format("SearchResults error: {0}", e.Message));
                    },
                    onCompleted: () =>
                    {
                        Console.WriteLine("End of search results");
                    });
                await searchResultStream; // Awaiting an IObservable completes when the observable completes.
            }
        }
    }
}
