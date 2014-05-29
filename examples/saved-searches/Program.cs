using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splunk.Client;

namespace saved_searches
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
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
