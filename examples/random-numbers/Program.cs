using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splunk.ModularInputs;

namespace random_numbers
{
    public class Program : ModularInput
    {
        private static Random rnd = new Random();

        public static int Main(string[] args)
        {
            Task<int> t = new Program().RunAsync(args);
            t.Wait();
            return t.Result;
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

        public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
        {
            double min = (double)inputDefinition.Parameters["min"];
            double max = (double)inputDefinition.Parameters["max"];

            while (true)
            {
                await Task.Delay(1000);
                await eventWriter.QueueEventForWriting(new Event
                {
                    Stanza = inputDefinition.Name,
                    Data = "number=" + (rnd.NextDouble() * (max - min) + min)
                });
            }
        }
    }
}
