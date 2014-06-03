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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port, new Namespace(user: "nobody", app: "search")))
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
            await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);
            Console.WriteLine("Press return to cancel.");

            string searchQuery = "search index=_internal | stats count by method";

            Job realtimeJob = await service.Jobs.CreateAsync(searchQuery, new JobArgs
            {
                SearchMode = SearchMode.Realtime,
                EarliestTime = "rt-1h",
                LatestTime = "rt",
            });

            var tokenSource = new CancellationTokenSource();

            var task = Task.Run(async () =>
            {
                Console.ReadLine();

                await realtimeJob.CancelAsync();
                tokenSource.Cancel();
            });

            while (!tokenSource.IsCancellationRequested)
            {
                SearchResultStream resultStream = await realtimeJob.GetSearchResultsPreviewAsync();

                Console.WriteLine("fieldnames: " + string.Join(";", resultStream.FieldNames));
                Console.WriteLine("fieldname count: " + resultStream.FieldNames.Count);
                Console.WriteLine("final result: " + resultStream.IsFinal);

                foreach (Task<SearchResult> item in resultStream)
                {
                    SearchResult result = await item;
                    Console.WriteLine(result);
                }

                Console.WriteLine("");
                await Task.Delay(2000, tokenSource.Token);
            }
        }
    }
}