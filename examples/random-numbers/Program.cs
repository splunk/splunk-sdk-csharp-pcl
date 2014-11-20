using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splunk.ModularInputs;

namespace random_numbers
{
    /// <summary>
    /// This example shows how to create a modular input with C# 2.0 SDK.
    /// After build, it will generate a folder @ bin\Debug\app\random-numbers (or bin\Release\app\random-numbers).
    /// if to install this app on splunk hosted on Windows server, zip the folder of bin\Debug\app\random-numbers, and name it as random-numbers.spl, then install the app.
    /// if to install this app on splunk hosted on Linux/OSX server, use tar command to targiz the folder of bin\Debug\app\random-numbers, and name it as random-numbers.tar.gz, then install the app; 
    /// Noted this will need your install MONO Make sure the app can run on non-windows environment; make sure the random-numbers.sh file contain the right path to your MONO path
    /// </summary>
    public class Program : ModularInput
    {
        private static Random rnd = new Random();

        public static int Main(string[] args)
        {
            // Run in debug attach mode. This will force the input to pause when streaming, for the default timeout of 30 seconds, to wait for a debugger to attach. 
            // If no debugger attaches, it will continue processing afer the timeout. 
            // To change the timeout, pass a value for the 3rd parameter.
            return Run<Program>(args, DebuggerAttachPoints.StreamEvents);
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
