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

namespace search_realtime
{
    using Splunk.Client;
    using Splunk.Client.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, new Namespace(user: "nobody", app: "search")))
            {
                Task task = Run(service);

                while (!task.IsCanceled)
                {
                    Task.Delay(500).Wait();
                }
            }

            Console.Write("Press return to exit: ");
            Console.ReadLine();
        }

        static async Task Run(Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);
            Console.WriteLine("Press return to cancel.");

            string searchQuery = "search index=_internal | stats count by method";

            Job realtimeJob = await service.Jobs.CreateAsync(searchQuery, args: new JobArgs
            {
                SearchMode = SearchMode.RealTime,
                EarliestTime = "rt-1h",
                LatestTime = "rt",
            });

            var tokenSource = new CancellationTokenSource();

            #pragma warning disable 4014
            //// Because this call is not awaited, execution of the current 
            //// method continues before the call is completed. Consider 
            //// applying the 'await' operator to the result of the call.

            Task.Run(async () =>
            {
                Console.ReadLine();

                await realtimeJob.CancelAsync();
                tokenSource.Cancel();
            });

            #pragma warning restore 4014

            while (!tokenSource.IsCancellationRequested)
            {
                using (SearchResultStream stream = await realtimeJob.GetSearchPreviewAsync())
                {
                    Console.WriteLine("fieldnames: " + string.Join(";", stream.FieldNames));
                    Console.WriteLine("fieldname count: " + stream.FieldNames.Count);
                    Console.WriteLine("final result: " + stream.IsFinal);

                    foreach (SearchResult result in stream)
                    {
                        Console.WriteLine(result);
                    }

                    Console.WriteLine("");
                    await Task.Delay(2000, tokenSource.Token);
                }
            }
        }
    }
}