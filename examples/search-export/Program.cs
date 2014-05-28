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

        public class ResultsHandler : IObserver<SearchPreview>
        {
            private int previewNumber = 0;
            private TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();

            public void OnNext(SearchPreview searchPreview)
            {
                Console.WriteLine("Preview {0:D8}: {1}", ++previewNumber, searchPreview.IsFinal ? "final" : "partial");
                int recordNumber = 0;

                foreach (var result in searchPreview.SearchResults)
                {
                    Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, result));
                }
            }

            public void OnError(Exception e)
            {
                source.SetException(e);
            }

            public void OnCompleted()
            {
                source.SetResult(true);
            }

            public Task AwaitCompletionAsync()
            {
                return (Task)source.Task;
            }
        }

        public static async Task Run(Service service)
        {
            await service.LoginAsync("admin", "changeme");

            //// Search : Export Previews
            using (var searchPreviewStream = service.ExportSearchPreviewsAsync("search index=_internal | head 100").Result)
            {
                ResultsHandler handler = new ResultsHandler();
                searchPreviewStream.Subscribe(handler);
                await handler.AwaitCompletionAsync();
            }
        }
    }
}
