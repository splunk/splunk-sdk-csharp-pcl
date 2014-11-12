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

namespace search_export
{
    using Splunk.Client;
    using Splunk.Client.Helpers;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Reactive;

    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }

            Console.Write("Press return to exit: ");
            Console.ReadLine();
        }

        public static async Task Run(Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

            //// Search : Export Previews
            var stream = await service.ExportSearchResultsAsync("search index=_internal error", new SearchExportArgs { EarliestTime="rt", LatestTime = "rt" });
            
            var observer = Observer.Create<dynamic>(
                r =>
                {
                    Console.WriteLine(r._raw);
                }
            );
            stream.Subscribe(observer);
        }
    }
}
