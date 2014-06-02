namespace search_export
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Threading.Tasks;
    using System.Linq;
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }
        }


        public static async Task Run(Service service)
        {
            await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);

            //// Search : Export Previews
            using (SearchPreviewStream searchPreviewStream = service.ExportSearchPreviewsAsync("search index=_internal | head 100").Result)
            {
                int previewNumber = 0;

                //Console.WriteLine("*************" + searchPreviewStream.Count());
                foreach (Task<SearchPreview> task in searchPreviewStream)
                {
                    previewNumber++;
                    Console.WriteLine("==================== {0}=================", previewNumber);
                    SearchPreview preview = await task;
                    int recordNumber = 0;
                    foreach (SearchResult result in preview.SearchResults)
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, result));
                        Console.WriteLine("===============" + result.ToString().Substring(0, 200));
                    }
                }
            }
        }
    }
}

