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

namespace Splunk.Examples.saved_searches
{
    using Splunk.Client.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Splunk.Client;
    using System.Net;

    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }

            Console.Write("Press return to exit: ");
            Console.ReadLine();
        }

        private static async Task Run(Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

            string savedSearchName = "example_search";
            string savedSearchQuery = "search index=_internal | head 10";

            // Delete the saved search if it exists before we start.
            SavedSearch savedSearch = await service.SavedSearches.GetOrNullAsync(savedSearchName);

            if (savedSearch != null)
            {
                await savedSearch.RemoveAsync();
            }

            savedSearch = await service.SavedSearches.CreateAsync(savedSearchName, savedSearchQuery);
            Job savedSearchJob = await savedSearch.DispatchAsync();

            using (SearchResultStream stream = await savedSearchJob.GetSearchResultsAsync())
            {
                foreach (SearchResult result in stream)
                {
                    Console.WriteLine(result);
                }
            }

            await savedSearch.RemoveAsync();
        }
    }
}
