using System;
using System.Net.Http;
using System.Threading.Tasks;
using Splunk.Client;
using System.Security.Authentication;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls,
                ServerCertificateCustomValidationCallback = delegate { return true; }
            };
            var context = new Context(Scheme.Https, "localhost", 8089, TimeSpan.FromMinutes(1), handler);
            var service = new Service(context);

            Run(service).Wait();
        }

        static async Task Run(Service s)
        {
            await s.LogOnAsync("admin", "changeme");
            try
            {
                await s.SearchAsync("search index=_internal | head 10");
                await s.Applications.GetAllAsync();
                foreach (var app in s.Applications)
                {
                    Console.WriteLine(app.Name);
                }
                throw new Exception("test"); // TOOD: just here so the process doesn't exit while debugging
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ERROR " + e.Message);
            }
        }
    }
}