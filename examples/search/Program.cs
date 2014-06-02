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
    /// Starts a normal search and polls for completion to find out when the search has finished.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port, new Namespace(user: "nobody", app: "search")))
            {
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
            await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);

            // Simple oneshot search
            using (SearchResultStream searchResults = await service.SearchOneshotAsync("search index=_internal | head 5"))
            {
                foreach (SearchResult record in searchResults)
                {
                    Console.WriteLine("===============================");
                    Console.WriteLine(record);
                    Console.WriteLine();
                }
            }
        }
    }
}
