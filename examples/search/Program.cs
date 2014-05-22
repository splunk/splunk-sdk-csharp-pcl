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

namespace Splunk.Client.Examples.Search
{
    using System;
    using System.Net;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SDKHelper;
    using System.Diagnostics;

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
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port, new Namespace(user: "nobody", app: "search")))
            {
                SearchRealTime(service).Wait();
                OneshotSearch(service).Wait();
            }
        }


        /// <summary>
        /// Called when [search].
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        static async Task OneshotSearch(Service service)
        {
            //// Login
            await service.LoginAsync("admin", "changeme");

            // Simple oneshot search
            using (SearchResultStream searchResults = await service.SearchOneshotAsync("search index=_internal | head 5"))
            {
                foreach (SearchResult record in searchResults.ToEnumerable())
                {
                    Console.WriteLine(record);
                }
            }

            //Use JobArgs to define the search
            JobArgs args = new JobArgs
            {
                AutoCancel = 0
            };

            using (SearchResultStream searchResults = await service.SearchOneshotAsync("search index=_internal | head 5", args))
            {
                foreach (SearchResult record in searchResults.ToEnumerable())
                {
                    Console.WriteLine(record);
                }
            }

            await service.LogoffAsync();
        }

        /// <summary>
        /// Do real time seach
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        static async Task SearchRealTime(Service service)
        {

            await service.LoginAsync("admin", "changeme");

            // Realtime window is 5 minutes
            string query = "search index=_internal";

            var jobArgs = new JobArgs
            {
                SearchMode = SearchMode.Realtime,
                EarliestTime = "rt-5m",
                LatestTime = "rt",
            };

            Job job = await service.CreateJobAsync(query, jobArgs);

            int fieldCount = -1;

            Stopwatch watch = Stopwatch.StartNew();
            while (true)
            {

                using (SearchResultStream searchResults = await job.GetSearchResultsPreviewAsync())
                {
                    System.Diagnostics.Debug.Assert(!searchResults.IsFinal);

                    if (searchResults.FieldNames.Count != fieldCount)
                    {
                        System.Console.WriteLine("============================= Snapshot {0}=================================", watch.Elapsed);

                        System.Console.WriteLine("searchResults isFinal:{0}", searchResults.IsFinal);
                        System.Console.WriteLine("searchResults FieldNames.Count:{0}", searchResults.FieldNames.Count);

                        int i = 0;

                        foreach (var x in searchResults.ToEnumerable())
                        {
                            Console.WriteLine(x);
                        }
                        Console.WriteLine(i);
                    }

                    if (watch.Elapsed > new TimeSpan(0, 0, 10))
                    {
                        break;
                    }

                    fieldCount = searchResults.FieldNames.Count;
                }
            }

            await job.CancelAsync();

            await service.LogoffAsync();
        }
    }
}
