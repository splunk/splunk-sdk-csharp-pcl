namespace search_realtime
{
    using Splunk.Client;
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
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }
        }

        private static async Task Run(Service service)
        {
            await service.LoginAsync("admin", "changeme");

            Console.WriteLine("Type enter to cancel.");

            string searchQuery = "search index=_internal | stats count by method";

            Job realtimeJob = await service.CreateJobAsync(searchQuery, new JobArgs
            {
                SearchMode = SearchMode.Realtime,
                EarliestTime = "rt-1h",
                LatestTime = "rt",
            });

            var tokenSource = new CancellationTokenSource();

            SearchResultStream searchResults;
            while (!tokenSource.IsCancellationRequested)
            {
                searchResults = await realtimeJob.GetSearchResultsPreviewAsync();
                foreach (var result in searchResults)
                {
                    Console.WriteLine(result.ToString());
                }
                Console.WriteLine("");
                await Task.Delay(500, tokenSource.Token);
            }

            Task.Run(() =>
            {
                Console.ReadLine();
                tokenSource.Cancel();
                realtimeJob.CancelAsync().Wait();
            });
        }
    }
}
