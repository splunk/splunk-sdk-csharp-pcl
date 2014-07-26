using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splunk.ModularInputs;

namespace modular_input
{
    public class Program : ModularInput
    {

        /// <summary>
        ///     Name of the input argument
        /// </summary>
        private const string PollingInterval = "polling_interval";

        public static int Main(string[] args)
        {
            return Run<Program>(args);
        }

        public override Scheme Scheme
        {
            get
            {
                return new Scheme
                {
                    Title = "C# SDK Example: System Environment Variable Monitor",
                    Description = "Monitor changes to a system environment variable. When a change is detected, log the new value.",
                    Arguments = new List<Argument>
                    {
                            new Argument
                            {
                                // 'name' is a built in var. Only its description can be
                                // customized.
                                Name = "name",
                                Description = "Name of the environment variable to monitor",
                            },
                            new Argument
                            {
                               Name = PollingInterval,
                                Description =
                                    "Number of milliseconds to wait before the next check of the environment variable for change. Default is 1000.",
                                DataType = DataType.Number,
                                Validation = "is_pos_int('polling_interval')",
                                RequiredOnCreate = false
                             }
                    }
                };
            }
        }

        public override bool Validate(Validation validation, out string errorMessage)
        {
            var varName = validation.Name;

            var varValue = Environment.GetEnvironmentVariable(
                varName,
                EnvironmentVariableTarget.Machine);

            if (varValue == null)
            {
                errorMessage = string.Format("Environment variable '{0}' is not defined", varName);
                return false;
            }

            errorMessage = null;
            return true;
        }

        public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
        {
            int interval = 1000;
            try
            {
                // if user didn't give value for polling_interval, type conversion to SingleValueParameter will throw
                interval = int.Parse(((SingleValueParameter)inputDefinition.Parameters["polling_interval"]).ToString());
            }
            catch (Exception)
            {
            }

            const string Seperator = @"://";
            int index = inputDefinition.Name.IndexOf(Seperator) + Seperator.Length;
            string varName = inputDefinition.Name.Substring(index);

            string lastVarValue = null;
            while (true)
            {
                await Task.Delay(interval);

                string varValue = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Machine);
                // Event data can't be null for real events.  
                varValue = varValue ?? "(not exist)";
                // Splunk does not record lines with only white spaces.
                varValue = string.IsNullOrWhiteSpace(varValue) ? "(white space)" : varValue;

                if (varValue != lastVarValue)
                {
                    await eventWriter.QueueEventForWriting(new Event
                    {
                        Stanza = varName,
                        Data = string.Format("{0}, interval={1}, inputDefinition.Name={2} , varName={3}",
                           varValue, interval, inputDefinition.Name, varName)
                    });

                    lastVarValue = varValue;
                }
            }
        }
    }
}
