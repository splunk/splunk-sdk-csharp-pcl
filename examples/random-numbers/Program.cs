using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splunk.ModularInputs;
using System.Threading;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace random_numbers
{
    public class Program : ModularInput
    {
        private static Random rnd = new Random();

        static void Main(string[] args)
        {
            RunAsync<Program>(args).Wait();
        }

        public IObservable<long> schedule = null;

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

        public IObservable<Event> InputDefinitionToObservable(InputDefinition inputDefinition)
        {
            if (schedule == null)
                schedule = Observable.Interval(new TimeSpan(0, 0, 1));

            double min = (double)inputDefinition.Parameters["min"];
            double max = (double)inputDefinition.Parameters["max"];

            var subject = new Subject<Event>();

            schedule.ForEachAsync((long i) => {
                subject.OnNext(new Event
                {
                    Stanza = inputDefinition.Name,
                    Data = "number=" + (rnd.NextDouble()*(max-min) + min)
                });
            });

            return subject;
        }

        public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
        {
            var eventSequence = InputDefinitionToObservable(inputDefinition);
            await eventSequence.ForEachAsync((Event e) => {
                eventWriter.WriteEvent(e);
            });
        }
    }
}
