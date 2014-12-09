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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;

    using Splunk.ModularInputs;

    using Xunit;
    using System.Xml.Serialization;
    using System.Xml.Linq;
    using System.Xml;

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

            using (var service = inputDefinition.Service)
            {
                Assert.Equal(Splunk.Client.Scheme.Https, service.Context.Scheme);
                Assert.Equal("localhost", service.Context.Host);
                Assert.Equal(8089, service.Context.Port);
                Assert.Equal("abcdefg", service.Context.SessionKey);
            }
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
            SingleValueParameter parameter;

            parameter = new SingleValueParameter("some_name", "abc");
            Assert.Equal("abc", parameter.ToString());

            parameter = new SingleValueParameter("some_name", "52");
            Assert.Equal(52, parameter.ToInt32());

            parameter = new SingleValueParameter("some_name", "52");
            Assert.Equal((double)52, parameter.ToDouble());

            parameter = new SingleValueParameter("some_name", "1");
            Assert.True(parameter.ToBoolean());

            parameter = new SingleValueParameter("some_name", "52");
            Assert.Equal((long)52, parameter.ToInt64());
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
            Collection<string> values;
            MultiValueParameter parameter;

            values = new Collection<string> { "abc", "def" };
            parameter = new MultiValueParameter("some_name", values);

            Assert.Equal(values, parameter.ToStringCollection());

            values = new Collection<string> { "true", "0" };
            parameter = new MultiValueParameter("some_name", values);

            Assert.Equal(new Collection<bool> { true, false }, parameter.ToBooleanCollection());

            values = new Collection<string> { "52", "42" };
            parameter = new MultiValueParameter("some_name", values);

            Assert.Equal(new Collection<double> { (double)52, (double)42 }, parameter.ToDoubleCollection());
            Assert.Equal(new Collection<float> { (float)52, (float)42 }, parameter.ToSingleCollection());
            Assert.Equal(new Collection<int> { 52, 42 }, parameter.ToInt32Collection());
            Assert.Equal(new Collection<long> { 52, 42 }, parameter.ToInt64Collection());
        }

        class TestInput : ModularInput
        {
            private readonly bool _throwOnScheme;

            public TestInput(bool throwOnScheme = false)
            {
                _throwOnScheme = throwOnScheme;
                _isAttached = () => false;
            }

            public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
            {
                var min = ((SingleValueParameter)inputDefinition.Parameters["min"]).ToDouble();
                if (min == 1)
                {
                    throw new InvalidOperationException();
                }

                await eventWriter.QueueEventForWriting(new Event
                {
                    Data = "Boris!"
                });
            }

            

            public override Scheme Scheme
            {
                get
                {
                    if (_throwOnScheme)
                    {
                        throw new InvalidOperationException();
                    }

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
                                RequiredOnCreate = true,
                                
                                ValidationDelegate = delegate (Parameter param, out string errorMessage) 
                                {
                                    bool isDouble;
                                    
                                    try 
                                    { 
                                        double _ = ((SingleValueParameter)param).ToDouble(); 
                                        isDouble = true; 
                                    }
                                    catch (FormatException)
                                    {
                                        isDouble = false;
                                    }
                                    catch (InvalidCastException)
                                    { 
                                        isDouble = false; 
                                    }
                                    catch (OverflowException)
                                    {
                                        isDouble = false;
                                    }

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
                double min = ((SingleValueParameter)validationItems.Parameters["min"]).ToDouble();
                double max = ((SingleValueParameter)validationItems.Parameters["max"]).ToDouble();

                if (min == 1)
                {
                    throw new InvalidOperationException();
                }

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
                Assert.Equal("xml", doc.Element("scheme").Element("streaming_mode").Value);
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
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationFailureLogsErrror()
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
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.Event")]
        [Fact]
        public void SerializeEventWithoutDone()
        {
            DateTime timestamp = System.DateTime.Now;
            Event e = new Event
            {
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
            using (StringWriter writer = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment }))
                    e.ToXml(xmlWriter);
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
            using (var writer = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment }))
                    e.ToXml(xmlWriter);
                serialized = writer.ToString();
            }

            Assert.True(serialized.Contains("<done />"));

            XDocument doc = XDocument.Parse(serialized);
            Assert.NotNull(doc.Element("event").Element("done"));
            Assert.Equal("", doc.Element("event").Element("done").Value);
            Assert.False(doc.Element("event").HasAttributes);
        }

        [Trait("unit-test", "Splunk.ModularInputs.EventWriter")]
        [Fact]
        public async Task EventWriterConvertsSeverityEnumValueToName()
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            var writer = new EventWriter(stdout, stderr, null);
            await writer.LogAsync(Severity.Info, "Test");
            await writer.LogAsync(Severity.Warning, "Test");
            await writer.LogAsync(Severity.Error, "Test");
            var events = stderr.GetStringBuilder().ToString();
            Assert.Contains("INFO Test", events);
            Assert.Contains("WARNING Test", events);
            Assert.Contains("ERROR Test", events);
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

            Task<EventWrittenProgressReport> t = progress.AwaitProgressAsync();

            #pragma warning disable 4014
            //// Because this call is not awaited, execution of the current 
            //// method continues before the call is completed. Consider 
            //// applying the 'await' operator to the result of the call.
            eventWriter.CompleteAsync();
            #pragma warning restore 4014

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
                Assert.True(stdout.ToString().Trim().Contains("<data>Boris the mad baboon</data>"));
                Assert.True(stdout.ToString().Trim().Contains("<time>-11644502400</time>"));
                Assert.Equal("", stderr.ToString());

                var completedTask = progress.AwaitProgressAsync();
                await eventWriter.CompleteAsync();
                report = await completedTask;

                Assert.Equal("", stderr.ToString());
                Assert.True(stdout.ToString().Trim().EndsWith("</stream>"));
            }
            finally
            {
                // EventWriter.CompleteAsync() is idempotent, so there is no problem if this is invoked twice.
                eventWriter.CompleteAsync().Wait();
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ModularInputWritesEvents()
        {
            using (StringReader stdin = new StringReader(@"<?xml version=""1.0"" encoding=""utf-16""?>
                <input xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <server_host>tiny</server_host>
                    <server_uri>https://127.0.0.1:8089</server_uri>
                    <checkpoint_dir>/some/dir</checkpoint_dir>
                    <session_key>123102983109283019283</session_key>
                    <configuration>
                        <stanza name=""random_numbers://aaa"">
                            <param name=""min"">0</param>
                            <param name=""max"">5</param>
                        </stanza>
                    </configuration>
                </input>"))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.Equal(0, exitCode);
                Assert.Equal("", stderr.ToString());
                Assert.False(stdout.ToString().Contains("xmlns:xsi"));
                Assert.True(stdout.ToString().Contains("<data>Boris!</data>"));
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task StreamingLogsExceptions()
        {
            using (StringReader stdin = new StringReader(@"<?xml version=""1.0"" encoding=""utf-16""?>
                <input xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <server_host>tiny</server_host>
                    <server_uri>https://127.0.0.1:8089</server_uri>
                    <checkpoint_dir>/some/dir</checkpoint_dir>
                    <session_key>123102983109283019283</session_key>
                    <configuration>
                        <stanza name=""random_numbers://aaa"">
                            <param name=""min"">0</param>
                            <param name=""max"">5</param>
                        </stanza>
                        <stanza name=""random_numbers://bbb"">
                            <param name=""min"">1</param>
                            <param name=""max"">5</param>
                        </stanza>
                        <stanza name=""random_numbers://ccc"">
                            <param name=""min"">1</param>
                            <param name=""max"">5</param>
                        </stanza>
                    </configuration>
                </input>"))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.Equal(0, exitCode);
                var err = stderr.ToString();
                Assert.DoesNotContain("FATAL Exception during streaming: name=random_numbers://aaa | System.InvalidOperationException: Operation is not valid due to the current state of the object.", err);
                Assert.Contains("FATAL Exception during streaming: name=random_numbers://bbb | System.InvalidOperationException: Operation is not valid due to the current state of the object.", err);
                Assert.Contains("FATAL Exception during streaming: name=random_numbers://ccc | System.InvalidOperationException: Operation is not valid due to the current state of the object.", err);
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationLogsExceptions()
        {
            XDocument doc = new XDocument(
               new XElement("items",
                   new XElement("server_host", "tiny"),
                   new XElement("server_uri", "https://127.0.0.1:8089"),
                   new XElement("checkpoint_dir", "/somewhere"),
                   new XElement("session_key", "abcd"),
                   new XElement("item",
                       new XAttribute("name", "random_numbers://aaa"),
                       new XElement("param", new XAttribute("name", "min"), 1),
                       new XElement("param", new XAttribute("name", "max"), 12))));
            using (StringReader stdin = new StringReader(doc.ToString()))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = {"--validate-arguments"};
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.NotEqual(0, exitCode);
                var err = stderr.ToString();
                Assert.Contains("FATAL Exception during validation: name=random_numbers://aaa | System.InvalidOperationException: Operation is not valid due to the current state of the object.", err);
                Assert.Contains("<error><message>Operation is not valid due to the current state of the object.</message></error>", stdout.ToString());
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationLogsInputDefinition()
        {
            XDocument doc = new XDocument(
              new XElement("items",
                  new XElement("server_host", "tiny"),
                  new XElement("server_uri", "https://127.0.0.1:8089"),
                  new XElement("checkpoint_dir", "/somewhere"),
                  new XElement("session_key", "abcd"),
                  new XElement("item",
                      new XAttribute("name", "random_numbers://aaa"),
                      new XElement("param", new XAttribute("name", "min"), 0),
                      new XElement("param", new XAttribute("name", "max"), 12))));
            using (StringReader stdin = new StringReader(doc.ToString()))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--validate-arguments" };
                TestInput testInput = new TestInput();
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                var err = stderr.ToString();
                Assert.Contains("DEBUG <items>\r\n  <server_host>tiny</server_host>\r\n  <server_uri>https://127.0.0.1:8089</server_uri>\r\n  <checkpoint_dir>/somewhere</checkpoint_dir>\r\n  <session_key>abcd</session_key>\r\n  <item name=\"random_numbers://aaa\">\r\n    <param name=\"min\">0</param>\r\n    <param name=\"max\">12</param>\r\n  </item>\r\n</items>\r\n", err);
            }           
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task SchemeLogsExceptions()
        {
            using (StringReader stdin = new StringReader(""))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--scheme" };
                TestInput testInput = new TestInput(true);
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.NotEqual(0, exitCode);
                var err = stderr.ToString();
                Assert.Contains("name=" + typeof(TestInput).FullName, err);
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task StreamingLogsSerializationException()
        {
            using (StringReader stdin = new StringReader("{{}}"))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { };
                TestInput testInput = new TestInput(true);
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.NotEqual(0, exitCode);
                var err = stderr.ToString();
                Assert.Contains("FATAL Exception during streaming: name=Splunk.ModularInputs.UnitTests.TestModularInputs+TestInput | System.InvalidOperationException: There is an error in XML document (1, 1)",err);
            }
        }

        [Trait("unit-test", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidateLogsSerializationException()
        {
            using (StringReader stdin = new StringReader("{{}}"))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--validate-arguments" };
                TestInput testInput = new TestInput(true);
                int exitCode = await testInput.RunAsync(args, stdin, stdout, stderr);

                Assert.NotEqual(0, exitCode);
                var err = stderr.ToString();
                Assert.Contains("FATAL Exception during validation: name=Splunk.ModularInputs.UnitTests.TestModularInputs+TestInput | System.InvalidOperationException: There is an error in XML document (1, 1)", err);
            }
        }
    }
}