namespace Splunk.Sdk.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Starts a normal search and polls for completion to find out when the search has finished.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var service = new Service(new Context(Scheme.Https, "localhost", 8089), Namespace.Default);
            Task task = service.LoginAsync("admin", "changeme");
            task.Wait();

            Task<Job> jobTask = service.SearchAsync(command: "* | head 100");
            jobTask.Wait();

            Job job = jobTask.Result;

            while (!job.IsCompleted)
            {
                try
                {
                    Thread.Sleep(500);
                }
                catch (ThreadInterruptedException e)
                {
                    // TODO Auto-generated catch block
                    System.Console.WriteLine(e.StackTrace);
                }
                task = job.UpdateAsync<Job>();
                task.Wait();
            }

#if false
            // Get the search results and use the built-in XML parser to display them
            var outArgs = new JobResultsArgs
            {
                OutputMode = JobResultsArgs.OutputModeEnum.Xml,
                Count = 0 // Return all entries.
            };

            using (var stream = job.Results(outArgs))
            {
                using (var rr = new ResultsReaderXml(stream))
                {
                    foreach (var @event in rr)
                    {
                        System.Console.WriteLine("EVENT:");
                        foreach (string key in @event.Keys)
                        {
                            System.Console.WriteLine("   " + key + " -> " + @event[key]);
                        }
                    }
                }
            }

            // Get properties of the job
            System.Console.WriteLine("Search job properties:\n---------------------");
            System.Console.WriteLine("Search job ID:         " + job.Sid);
            System.Console.WriteLine("The number of events:  " + job.EventCount);
            System.Console.WriteLine("The number of results: " + job.ResultCount);
            System.Console.WriteLine("Search duration:       " + job.RunDuration + " seconds");
            System.Console.WriteLine("This job expires in:   " + job.Ttl + " seconds");
#endif
        }
    }
}
