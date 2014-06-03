namespace search_export
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }

            Console.Write("Press return to exit: ");
            Console.ReadLine();
        }

        public static async Task Run(Service service)
        {
            await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);

            //// Search : Export Previews

            using (SearchPreviewStream previewStream = await service.ExportSearchPreviewsAsync("search index=_internal | head 100"))
            {
                int previewNumber = 0;

                foreach (Task<SearchPreview> task in previewStream)
                {
                    SearchPreview preview = await task;
                    int resultNumber = 0;

                    Console.WriteLine("Preview {0:D8}: {1}", ++previewNumber, preview.IsFinal ? "final" : "partial");

                    foreach (var result in preview.Results)
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++resultNumber, result));
                    }
                }
            }
        }
    }
}
