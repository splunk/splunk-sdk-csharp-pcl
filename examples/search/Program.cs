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
    using Splunk.Client.Helpers;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Executes a oneshot search and prints the results.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }

            Console.WriteLine("Press return to continue: ");
            Console.ReadLine();
        }

        /// <summary>
        /// Called when [search].
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        static async Task Run(Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

            //// Simple oneshot search

            using (SearchResultStream stream = await service.SearchOneShotAsync("search index=_internal | head 5"))
            {
                foreach (SearchResult result in stream)
                {
                    Console.WriteLine(result);
                }
            }
        }
    }
}
