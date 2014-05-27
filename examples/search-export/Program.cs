using System;
using System.Threading.Tasks;
using Splunk.Client;

namespace search_export
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


        public static async Task Run(Service service)
        {
            await service.LoginAsync("admin", "changeme");

            //// Search : Export Previews

            using (var earchPreviewStream = service.ExportSearchPreviewsAsync("search index=_internal | head 100").Result)
            {
                int previewNumber = 0;

                //foreach (var searchPreview in searchPreviewStream)
                //{
                //    Console.WriteLine("Preview {0:D8}: {1}", ++previewNumber, searchPreview.IsFinal ? "final" : "partial");
                //    int recordNumber = 0;

                //    foreach (var result in searchPreview.SearchResults)
                //    {
                //        Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, result));
                //    }
                //}
            }
        }
    }
}
