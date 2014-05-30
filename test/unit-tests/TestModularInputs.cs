// 
// Copyright 2013 Splunk, Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License"): you may
// not use this file except in compliance with the License. You may obtain
// a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations
// under the License.

namespace Splunk.ModularInputs.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Splunk.ModularInputs;

    using Xunit;
    using System.Xml.Serialization;
    using System.Xml.Linq;

    public class AwaitableProgress<T> : IProgress<T>
    {
        private event Action<T> Handler = (T x) => { };

        public void Report(T value)
        {
            this.Handler(value);
        }

        public async Task<T> AwaitProgressAsync()
        {
            var source = new TaskCompletionSource<T>();
            Action<T> onReport = null;
            onReport = (T x) =>
            {
                Handler -= onReport;
                source.SetResult(x);
            };
            Handler += onReport;
            return await source.Task;
        }
    }

    /// <summary>
    /// Test classes in Splunk.ModularInputs namespace
    /// </summary>
    public class TestModularInputs
    {
        [Trait("unit-test", "AwaitableProgress")]
        [Fact]
        public async Task AwaitProgressWorks()
        {
            AwaitableProgress<bool> progress = new AwaitableProgress<bool>();
            Task<bool> triggered = progress.AwaitProgressAsync();
            progress.Report(true);
            Assert.Equal(true, await triggered);
        }

        [Trait("unit-test", "Splunk.ModularInputs.InputDefinition")]
        [Fact]
        public void InputDefinitionServiceWorks()
        {
            InputDefinition inputDefinition = new InputDefinition
            {
                Name = "input_definition",
                Parameters = null,
                ServerHost = "boris",
                ServerUri = "https://localhost:8089",
                CheckpointDirectory = "",
                SessionKey = "abcdefg"
            };

            Splunk.Client.Service service = inputDefinition.Service;
            Assert.Equal(Splunk.Client.Scheme.Https, service.Context.Scheme);
            Assert.Equal("localhost", service.Context.Host);
            Assert.Equal(8089, service.Context.Port);
            Assert.Equal("abcdefg", service.Context.SessionKey);
        }

        [Trait("unit-test", "Splunk.ModularInputs.InputDefinition")]
        [Fact]
        public void InputDefinitionServiceFailsWithInvalidUri()
        {
            InputDefinition inputDefinition = new InputDefinition
            {
                Name = "input_definition",
                Parameters = null,
                ServerHost = "boris",
                ServerUri = "gopher://localhost:8089",
                CheckpointDirectory = "",
                SessionKey = "abcdefg"
            };
            Assert.Throws<FormatException>(() => inputDefinition.Service);
        }

        [Trait("unit-test", "Splunk.ModularInputs.SingleValueParameter")]
        [Fact]
        public void SingleValueParameterConversions()
        {
            SingleValueParameter parameter = new SingleValueParameter();

            parameter.Value = "abc";
            Assert.Equal("abc", (string)parameter);

            parameter.Value = "52";
            Assert.Equal(52, (int)parameter);

            parameter.Value = "52";
            Assert.Equal((double)52, (double)parameter);

            parameter.Value = "1";
            Assert.True((bool)parameter);

            parameter.Value = "52";
            Assert.Equal((long)52, (long)parameter);
        }

        [Trait("unit-test", "Splunk.ModularInputs.SingleValueParameter")]
        [Fact]
        public void SingleValueParameterParsing()
        {
            using (TextReader reader = new StringReader("<param name=\"param1\">value1</param>"))
            {
                SingleValueParameter parameter =
                    (SingleValueParameter)new XmlSerializer(typeof(SingleValueParameter)).Deserialize(reader);
                Assert.Equal("param1", parameter.Name);
                Assert.Equal("value1", parameter.Value);
            }

        }

        [Trait("unit-test", "Splunk.ModularInputs.MultiValueParameter")]
        [Fact]
        public void MultiValueParameterParsing()
        {
            using (TextReader reader = new StringReader("<param_list name=\"multiValue\"><value>value3</value>" +
                "<value>value4</value></param_list>"))
            {
                MultiValueParameter parameter =
                    (MultiValueParameter)new XmlSerializer(typeof(MultiValueParameter)).Deserialize(reader);
                Assert.Equal("multiValue", parameter.Name);
                Assert.Equal(new List<string> { "value3", "value4" }, parameter.Values);
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.MultiValueParameter")]
        [Fact]
        public void MultiValueParameterConversions()
        {
            MultiValueParameter parameter = new MultiValueParameter
            {
                Name = "some_name",
                Values = new List<string> { "abc", "def" }
            };
            Assert.Equal(new List<string> { "abc", "def" }, (List<string>)parameter);

            parameter.Values = new List<string> { "true", "0" };
            Assert.Equal(new List<bool> { true, false }, (List<bool>)parameter);

            parameter.Values = new List<string> { "52", "42" };
            Assert.Equal(new List<double> { 52.0, 42.0 }, (List<double>)parameter);

            parameter.Values = new List<string> { "52", "42" };
            Assert.Equal(new List<float> { (float)52, (float)42 }, (List<float>)parameter);

            parameter.Values = new List<string> { "52", "42" };
            Assert.Equal(new List<int> { 52, 42 }, (List<int>)parameter);

            parameter.Values = new List<string> { "52", "42" };
            Assert.Equal(new List<long> { 52, 42 }, (List<long>)parameter);
        }

        class TestInput : ModularInput
        {
            public override Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter) { return Task.FromResult(false); }

            public override Scheme Scheme
            {
                get
                {
                    return new Scheme
                    {
                        Title = "Random numbers",
                        Description = "Generate random numbers in the specified range",
                        Arguments = new List<Argument> {
                            new Argument
                            {
                                Name = "min",
                                Description = "Generated value should be at least min",
                                DataType = DataType.Number,
                                RequiredOnCreate = true,
                                ValidationDelegate = delegate (Parameter param, out string errorMessage) {
                                    bool isDouble;
                                    try { double _ = (double)param; isDouble = true; }
                                    catch (Exception) { isDouble = false; }
                                    if (isDouble)
                                    {
                                        errorMessage = "";
                                        return true;
                                    }
                                    else
                                    {
                                        errorMessage = "min should be a floating point number.";
                                        return false;
                                    }
                                }
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

            public override bool Validate(Validation validationItems, out string errorMessage)
            {
                double min = (double)validationItems.Parameters["min"];
                double max = (double)validationItems.Parameters["max"];

                if (min >= max)
                {
                    errorMessage = "Max must be greater than min.";
                    return false;
                }
                else
                {
                    errorMessage = "";
                    return true;
                }
            }


        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task GeneratesSchemeCorrectly()
        {
            
            using (StringReader stdin = new StringReader(""))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--scheme" };
                await new TestInput().RunAsync(args, stdin, stdout, stderr);
 
                XDocument doc = XDocument.Parse(stdout.ToString());
                Assert.Equal("Random numbers", doc.Element("scheme").Element("title").Value);
                Assert.Equal("Generate random numbers in the specified range", 
                    doc.Element("scheme").Element("description").Value);
                Assert.NotNull(doc.Element("scheme").Element("endpoint").Element("args"));
                Assert.Equal(String.Empty, stderr.ToString());
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task WorkingValidation()
        {
            XDocument doc = new XDocument(
                new XElement("items",
                    new XElement("server_host", "tiny"),
                    new XElement("server_uri", "https://127.0.0.1:8089"),
                    new XElement("checkpoint_dir", "/somewhere"),
                    new XElement("session_key", "abcd"),
                    new XElement("item",
                        new XAttribute("name", "aaa"),
                        new XElement("param", new XAttribute("name", "min"), 0),
                        new XElement("param", new XAttribute("name", "max"), 12))));
            using (StringReader stdin = new StringReader(doc.ToString()))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--validate-arguments" };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.Equal(0, exitCode);
                Assert.Equal("", stdout.ToString());
                Assert.Equal("", stderr.ToString());
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationFails()
        {
             XDocument doc = new XDocument(
                new XElement("items",
                    new XElement("server_host", "tiny"),
                    new XElement("server_uri", "https://127.0.0.1:8089"),
                    new XElement("checkpoint_dir", "/somewhere"),
                    new XElement("session_key", "abcd"),
                    new XElement("item",
                        new XAttribute("name", "aaa"),
                        new XElement("param", new XAttribute("name", "min"), 48),
                        new XElement("param", new XAttribute("name", "max"), 12))));
            using (StringReader stdin = new StringReader(doc.ToString()))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--validate-arguments" };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.NotEqual(0, exitCode);
                Assert.Equal(
                    "<error><message>Max must be greater than min.</message></error>",
                    stdout.ToString().Trim()
                );
                Assert.Equal("", stderr.ToString());
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationFailsOnSingleParameterDelegate()
        {
            XDocument doc = new XDocument(
               new XElement("items",
                   new XElement("server_host", "tiny"),
                   new XElement("server_uri", "https://127.0.0.1:8089"),
                   new XElement("checkpoint_dir", "/somewhere"),
                   new XElement("session_key", "abcd"),
                   new XElement("item",
                       new XAttribute("name", "aaa"),
                       new XElement("param", new XAttribute("name", "min"), "boris"),
                       new XElement("param", new XAttribute("name", "max"), 12))));
            using (StringReader stdin = new StringReader(doc.ToString()))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--validate-arguments" };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.NotEqual(0, exitCode);
                Assert.Equal(
                    "<error><message>min should be a floating point number.</message></error>",
                    stdout.ToString().Trim()
                );
                Assert.Equal("", stderr.ToString());
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationThrows()
        {
            using (StringReader stdin = new StringReader("blargh!"))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--validate-arguments" };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);
  
                Assert.NotEqual(0, exitCode);
                Assert.NotEqual("", stderr.ToString());
                Assert.Equal("", stdout.ToString());
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.Event")]
        [Fact]
        public void SerializeEventWithoutDone()
        {
            DateTime timestamp = System.DateTime.Now;
            Event e = new Event {
                Time = timestamp,
                Source = "hilda",
                SourceType = "misc",
                Index = "main",
                Host = "localhost",
                Data = "This is a test of the emergency broadcast system.",
                Done = false,
                Unbroken = true
            };
            string serialized;
            using (StringWriter writer = new StringWriter()) {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                serializer.Serialize(writer, e);
                serialized = writer.ToString();
            }

            XDocument doc = XDocument.Parse(serialized);
            Assert.False(serialized.Contains("<done"));
            Assert.Equal("hilda", doc.Element("event").Element("source").Value);
            Assert.Equal("misc", doc.Element("event").Element("sourcetype").Value);
            Assert.Equal("main", doc.Element("event").Element("index").Value);
            Assert.Equal("localhost", doc.Element("event").Element("host").Value);
            Assert.Equal("This is a test of the emergency broadcast system.",
                doc.Element("event").Element("data").Value);
            Assert.Equal("1", doc.Element("event").Attribute("unbroken").Value);
            long utcTimestamp = timestamp.Ticks - new DateTime(1970, 1, 1).Ticks;
            utcTimestamp /= TimeSpan.TicksPerSecond;
            Assert.Equal(utcTimestamp, long.Parse(doc.Element("event").Element("time").Value));
        }


        [Trait("unit-test", "Splunk.ModularInputs.Event")]
        [Fact]
        public void SerializeEventWithDone()
        {
            Event e = new Event
            {
                Time = System.DateTime.Now,
                Source = "hilda",
                SourceType = "misc",
                Index = "main",
                Host = "localhost",
                Data = "This is a test of the emergency broadcast system.",
                Done = true,
                Unbroken = false
            };
            string serialized;
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                serializer.Serialize(writer, e);
                serialized = writer.ToString();
            }

            Assert.True(serialized.Contains("<done />"));

            XDocument doc = XDocument.Parse(serialized);
            Assert.NotNull(doc.Element("event").Element("done"));
            Assert.Equal("", doc.Element("event").Element("done").Value);
            Assert.Equal("0", doc.Element("event").Attribute("unbroken").Value);
        }

       

        [Trait("unit-test", "Splunk.ModularInputs.EventWriter")]
        [Fact]
        public async Task EventWriterReportsOnDispose()
        {
            var progress = new AwaitableProgress<EventWrittenProgressReport>();
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            EventWriter eventWriter = new EventWriter(
                stdout: stdout,
                stderr: stderr,
                progress: progress
            );

            Task<EventWrittenProgressReport> t =
                progress.AwaitProgressAsync();
            eventWriter.Dispose();
            EventWrittenProgressReport r = await t;

            Assert.Equal(new Event(), r.WrittenEvent);
            Assert.Equal("", stderr.ToString());
            Assert.Equal("", stdout.ToString());
        }

        
        [Trait("unit-test", "Splunk.ModularInputs.EventWriter")]
        [Fact]
        public async Task EventWriterReportsOnWrite()
        {
            var progress = new AwaitableProgress<EventWrittenProgressReport>();
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            EventWriter eventWriter = new EventWriter(
                stdout: stdout,
                stderr: stderr,
                progress: progress
            );

            try
            {
                
                var writtenTask = progress.AwaitProgressAsync();
                await eventWriter.QueueEventForWriting(new Event
                {
                    Time = DateTime.FromFileTime(0),
                    Data = "Boris the mad baboon"
                });
                var report = await writtenTask;
                
                Assert.Equal("Boris the mad baboon", report.WrittenEvent.Data);
                
                //the expect string head may vary from env to env, so we will only verify the <data> section
                string expectedXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><stream>" +
                    "<event xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=" +
                    "\"http://www.w3.org/2001/XMLSchema\" unbroken=\"0\"><data>Boris the mad " +
                    "baboon</data><time>-11644502400</time></event>";
                               
                Console.WriteLine("actual   :"+stdout.ToString().Trim());
                Console.WriteLine("expected :"+expectedXml);
                Assert.True(stdout.ToString().Trim().Contains("<data>Boris the mad baboon</data><time>-11644502400</time></event>"));
                Assert.True(stdout.ToString().Trim().StartsWith("<?xml version=\"1.0\" encoding=\"utf-16\"?><stream><event"));
                Assert.Equal("", stderr.ToString());
                
                var completedTask = progress.AwaitProgressAsync();
                eventWriter.Dispose();
                report = await completedTask;

                Assert.Equal("", stderr.ToString());
                Assert.True(stdout.ToString().EndsWith("</stream>"));
            }
            finally
            {
                // EventWriter.Dispose() is idempotent, so there is no problem if this is invoked twice.
                eventWriter.Dispose(); 
            }
        }
         
    }
}