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
            return Run<Program>(args);
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
            double min = ((SingleValueParameter)validation.Parameters["min"]).ToDouble();
            double max = ((SingleValueParameter)validation.Parameters["max"]).ToDouble();

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
            double min = ((SingleValueParameter)inputDefinition.Parameters["min"]).ToDouble();
            double max = ((SingleValueParameter)inputDefinition.Parameters["max"]).ToDouble();

            while (true)
            {
                await Task.Delay(100);
                await eventWriter.QueueEventForWriting(new Event
                {
                    Stanza = inputDefinition.Name,
                    Data = "number=" + (rnd.NextDouble() * (max - min) + min)
                });
            }
        }
    }
}
