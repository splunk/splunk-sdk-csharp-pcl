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

namespace Splunk.ModularInputs.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Splunk.ModularInputs;

    using Xunit;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.Linq;
    using System.Threading;

    /// <summary>
    /// Test classes in Splunk.ModularInputs namespace
    /// </summary>
    public class TestModularInputs
    {
        /// <summary>
        /// Input file folder
        /// </summary>
        static readonly string TestDataFolder = Path.Combine("Data", "ModularInputs");

        /// <summary>
        /// Input file containing input definition
        /// </summary>
        const string InputDefinitionFilePath = "InputDefinition.xml";

        /// <summary>
        /// Input file containing validation items
        /// </summary>
        const string ValidationItemsFilePath = "ValidationItems.xml";

        /// <summary>
        /// Input file containing expected validation error message.
        /// </summary>
        const string ValidationErrorMessageFilePath = "ValidationErrorMessage.xml";

        /// <summary>
        /// Input file containing scheme
        /// </summary>
        const string SchemeFilePath = "Scheme.xml";

        /// <summary>
        /// Input file containing events
        /// </summary>
        const string EventsFilePath = "Events.xml";

        [Trait("class", "Splunk.ModularInputs.InputDefinition")]
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

        [Trait("class", "Splunk.ModularInputs.InputDefinition")]
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

        [Trait("class", "Splunk.ModularInputs.SingleValueParameter")]
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

        [Trait("class", "Splunk.ModularInputs.SingleValueParameter")]
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

        [Trait("class", "Splunk.ModularInputs.MultiValueParameter")]
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

        [Trait("class", "Splunk.ModularInputs.MultiValueParameter")]
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

        public static IEnumerable<Task> TriggeredSchedule(ManualResetEvent triggerEvent)
        {
            while (true)
            {
                yield return Task.Run(() =>
                {
                    triggerEvent.WaitOne();
                    triggerEvent.Reset();
                });
            }
        }

        class TestInput : ModularInput
        {
            public List<ManualResetEvent> TriggerEvents { get; set; }

            public override async Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter)
            {
                ManualResetEvent e = new ManualResetEvent(false);
                TriggerEvents.Add(e);
                int i = 0;
                foreach (Task t in TriggeredSchedule(e))
                {
                    await t;
                    eventWriter.WriteEvent(new Event {
                        Data = "Test " + i
                    });
                }
            }

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


        }

        [Trait("class", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task StreamEventsCorrectly()
        {
            // Generate stdin with an input definition

            using (StringReader stdin = new StringReader(""))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = {};

                
                await ModularInput.RunAsync<TestInput>(args, stdin, stdout, stderr);

              
            }
        }

        [Trait("class", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task GeneratesSchemeCorrectly()
        {
            
            using (StringReader stdin = new StringReader(""))
            using (StringWriter stdout = new StringWriter())
            using (StringWriter stderr = new StringWriter())
            {
                string[] args = { "--scheme" };
                await ModularInput.RunAsync<TestInput>(args, stdin, stdout, stderr);

                XDocument doc = XDocument.Parse(stdout.ToString());
                Assert.Equal("Random numbers", doc.Element("scheme").Element("title").Value);
                Assert.Equal("Generate random numbers in the specified range", 
                    doc.Element("scheme").Element("description").Value);
                Assert.NotNull(doc.Element("scheme").Element("endpoint").Element("args"));
                Assert.Equal(String.Empty, stderr.ToString());
            }
        }

        [Trait("class", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task WorkingValidation()
        {
        }

        [Trait("class", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationFails()
        {
        }

        [Trait("class", "Splunk.ModularInputs.ModularInput")]
        [Fact]
        public async Task ValidationThrows()
        {
        }

        [Trait("class", "Splunk.ModularInputs.Event")]
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


        [Trait("class", "Splunk.ModularInputs.Event")]
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

        /// <summary>
        /// Assert equal with expected file content.
        /// </summary>
        /// <param name="expectedFilePath">Relative file path</param>
        /// <param name="actual">Data to check</param>
        static void AssertEqualWithExpectedFile(
            string expectedFilePath,
            string actual)
        {
            var expected = ReadFileFromDataFolderAsString(expectedFilePath);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Read file from data directory as a string
        /// </summary>
        /// <param name="relativePath">Relative path to the resource</param>
        /// <returns>Resource content</returns>
        static string ReadFileFromDataFolderAsString(string relativePath)
        {
            return File.ReadAllText(GetDataFilePath(relativePath));
        }

        /// <summary>
        /// Read file from data directory as a test reader
        /// </summary>
        /// <param name="relativePath">Relative path to the resource</param>
        /// <returns>Resource content</returns>
        static TextReader ReadFileFromDataFolderAsReader(string relativePath)
        {
            return File.OpenText(GetDataFilePath(relativePath));
        }

        /// <summary>
        /// Get full path to the data file.
        /// </summary>
        /// <param name="relativePath">Relative path to the data folder.</param>
        /// <returns>A full path</returns>
        static string GetDataFilePath(string relativePath)
        {
            return Path.Combine(TestDataFolder, relativePath);
        }



       
        /// <summary>
        /// Redirect console in
        /// </summary>
        /// <param name="target">Destination of the redirection</param>
        static void SetConsoleIn(TextReader target)
        {
            // Must set Console encoding to be UTF8. Otherwise, Script.Run
            // will call the setter of OutputEncoding which results in
            // resetting Console.In (which should be a System.Console bug).
            Console.InputEncoding = Encoding.UTF8;
            Console.SetIn(target);
        }
    }
}