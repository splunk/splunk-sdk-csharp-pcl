using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splunk.ModularInputs;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using System.IO;

namespace random_numbers
{
    public static class ExtensionMethods
    {
        public static async Task WaitForEventAsync(this EventHandler eventToAwait, CancellationToken token = default(CancellationToken))
        {
            if (token.IsCancellationRequested)
                return;

            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            EventHandler callback = (object sender, EventArgs eventArgs) => source.SetResult(true);
            eventToAwait += callback;
            token.Register(() => source.SetResult(false));
            await source.Task;
            eventToAwait -= callback;
            return;
        }
    }

    public class Program : ModularInput
    {
        private static Random rnd = new Random();

        public static void Main(string[] args)
        {
            Program p = new Program();
            XDocument doc = new XDocument(
                new XElement("input",
                    new XElement("server_host", "tiny"),
                    new XElement("server_uri", "https://127.0.0.1:8089"),
                    new XElement("checkpoint_dir", "/some/dir"),
                    new XElement("session_key", "1234567890"),
                    new XElement("configuration",
                        new XElement("stanza",
                            new XAttribute("name", "foobar://aaa"),
                            new XElement("param",
                                new XAttribute("name", "min"),
                                0),
                            new XElement("param",
                                new XAttribute("name", "max"),
                                12)))));
            StringReader stdinReader = new StringReader(doc.ToString());
            p.RunAsync(args, stdin: stdinReader).Wait();
            return;
        }

        public override Scheme Scheme
        {
            get {
                return new Scheme
                {
                    Title = "Random numbers",
                    Description = "Generate random numbers in the specified range",
                    Arguments = new List<Argument>
                    {
                            new Argument
                            {
                                Name = "min",
                                Description = "Generated value should be at least min",
                                DataType = DataType.Number,
                                RequiredOnCreate = true
                            },
                            new Argument
                            {
                                Name = "max",
                                Description = "Generated value should be less than max",
                                DataType = DataType.Number,
                                RequiredOnCreate = true
                            }
                    }
                };

            }
        }
            
        public override bool Validate(Validation validation, out string errorMessage)
        {
            double min = (double)validation.Parameters["min"];
            double max = (double)validation.Parameters["max"];

            if (min >= max) {
                errorMessage = "min must be less than max.";
                return false;
            }
            else
            {
                errorMessage = "";
                return true;
            }
        }

        public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            double min = (double)inputDefinition.Parameters["min"];
            double max = (double)inputDefinition.Parameters["max"];

            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
                await eventWriter.QueueEventForWriting(new Event
                {
                    Stanza = inputDefinition.Name,
                    Data = "number=" + (rnd.NextDouble() * (max - min) + min)
                });
            }
        }
    }
}
