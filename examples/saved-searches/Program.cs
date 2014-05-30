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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Splunk.Client;
    using SDKHelper;

    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }
        }

        private static async Task Run(Service service)
        {
            await service.LoginAsync("admin", "changeme");

            string savedSearchName = "example_search";
            string savedSearchQuery = "search index=_internal | head 10";

            // Delete the saved search if it exists before we start.
            try
            {
                SavedSearch preexistingSearch = await service.GetSavedSearchAsync(savedSearchName);
                await preexistingSearch.RemoveAsync();
            }
            catch (KeyNotFoundException re) { }

            SavedSearch savedSearch = await service.CreateSavedSearchAsync(savedSearchName, savedSearchQuery);
            Job savedSearchJob = await savedSearch.DispatchAsync();

            using (SearchResultStream searchResults = await savedSearchJob.GetSearchResultsAsync())
            {
       

            }

            await savedSearch.RemoveAsync();
        }
    }
}
