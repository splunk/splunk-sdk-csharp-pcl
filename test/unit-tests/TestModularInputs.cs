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

        /// <summary>
        /// Test returning scheme through stdout
        /// </summary>
        [Trait("class", "Splunk.ModularInputs")]
        [Fact]
        public async Task OutputScheme()
        {
            using (var consoleOut = new StringWriter())
            {
                Console.SetOut(consoleOut);
                await ModularInput.RunAsync<TestScript>(new[] { "--scheme" });
                AssertEqualWithExpectedFile(SchemeFilePath, consoleOut.ToString());
            }
        }

        /// <summary>
        /// Test getting validation info from stdin and return validation error through stdout.
        /// </summary>
        [Trait("class", "Splunk.ModularInputs")]
        [Fact]
        public async Task ExternalValidation()
        {
            using (var consoleIn = ReadFileFromDataFolderAsReader(ValidationItemsFilePath))
            using (var consoleOut = new StringWriter())
            {
                SetConsoleIn(consoleIn);
                Console.SetOut(consoleOut);

                int exitCode = await ModularInput.RunAsync<TestScript>(new[] { "--validate-arguments" });

                AssertEqualWithExpectedFile(ValidationErrorMessageFilePath, consoleOut.ToString());
                Assert.NotEqual(0, exitCode);
            }
        }

        /// <summary>
        /// Test getting validation info from stdin
        /// </summary>
        [Trait("class", "Splunk.ModularInputs")]
        [Fact]
        public async Task StreamEvents()
        {
            using (var consoleIn = ReadFileFromDataFolderAsReader(InputDefinitionFilePath))
            {
                using (var consoleOut = new StringWriter())
                {
                    SetConsoleIn(consoleIn);
                    Console.SetOut(consoleOut);
                    await ModularInput.RunAsync<TestScript>(new string[] { });
                    AssertEqualWithExpectedFile(EventsFilePath, consoleOut.ToString());
                }
            }
        }

        /// <summary>
        /// Test error handling and logging
        /// </summary>
        [Trait("class", "Splunk.ModularInputs")]
        [Fact]
        public async Task ErrorHandling()
        {
            using (var consoleIn = new StringReader(string.Empty))
            {
                using (var consoleError = new StringWriter())
                {
                    SetConsoleIn(consoleIn);
                    Console.SetError(consoleError);
                    int exitCode = await ModularInput.RunAsync<TestScript>(new string[] { });

                    // There will be an exception due to missing input definition in 
                    // (redirected) console stdin.      
                    var error = consoleError.ToString();

                    // Verify that an exception is logged with level FATAL.
                    Assert.Contains("FATAL Script.Run: Unhandled exception:", error);

                    // Verify that the exception is what we expect.
                    Assert.Contains("No input definitions could be read from the standard input stream.", error);

                    // Verify that an info level message is logged properly.
                    Assert.Contains("INFO Script.Run: Reading input definitions.", error);

                    // Verify that the logged exception does not span more than one line
                    // Splunk breaks up events using new lines for splunkd log.
                    var lines = error.Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries);

                    Assert.Equal(2, lines.Length);
                    Assert.NotEqual(0, exitCode);
                }
            }
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
        /// Write events using EventStreamWriter
        /// </summary>
        // This method can be used by manual testing thus is public 
        public static void WriteEvents()
        {
            var writer = new EventStream();

            var eventTemplate = new EventElement
                {
                    Index = "sdk-tests2",
                    Host = "test host",
                    SourceType = "test sourcetype",
                    Source = "test source",
                };

            WriteEventData(
                writer,
                eventTemplate,
                "Event with all default fields set");

            WriteEventData(
                writer,
                eventTemplate,
                "Letter O with double acute: \u0150");

            eventTemplate.Unbroken = true;

            WriteEventData(
                writer,
                eventTemplate,
                "Part 1 of an unbroken event ");

            WriteEventData(
                writer,
                eventTemplate,
                "Part 2 of an unbroken event ending with newline" + Environment.NewLine);

            WriteEventDone(
                writer,
                eventTemplate);

            eventTemplate.Unbroken = false;

            WriteEventData(
                writer,
                eventTemplate,
                "Event after done key");

            var timedEvent = eventTemplate;
            timedEvent.Time = new DateTime(2013, 1, 1, 0, 0, 0, 1, DateTimeKind.Utc);
            timedEvent.Data = "Event with fixed time";
            writer.Write(timedEvent);

            WriteMultiplex(writer);
        }

        /// <summary>
        /// Write for multiple stanzas
        /// </summary>
        /// <param name="writer">An event writer</param>
        static void WriteMultiplex(EventStream writer)
        {
            var eventTemplate1 = new EventElement
                {
                    Stanza = "modular_input://UnitTest1",
                    Unbroken = true,
                };

            var eventTemplate2 = new EventElement
                {
                    Stanza = "modular_input://UnitTest2",
                    Unbroken = true,
                };

            WriteEventDataLine(writer, eventTemplate1, "Part 1 of channel 1 with a newline");
            WriteEventData(writer, eventTemplate2, "Part 1 of channel 2 without a newline ");

            // Mark the first channel done.
            WriteEventDone(writer, eventTemplate1);

            WriteEventDataLine(writer, eventTemplate1, "Part 2 of channel 1 with a newline");
            WriteEventDataLine(writer, eventTemplate2, "Part 2 of channel 2 with a newline");

            // Mark the second channel done.
            WriteEventDone(writer, eventTemplate2);
        }

        /// <summary>
        /// Write a done key
        /// </summary>
        /// <param name="writer">An event writer</param>
        /// <param name="eventTemplate">An event template</param>
        static void WriteEventDone(EventStream writer, EventElement eventTemplate)
        {
            var @event = eventTemplate;
            @event.Unbroken = false;
            @event.Done = true;
            writer.Write(@event);
        }

        /// <summary>
        /// Write an event data line.
        /// </summary>
        /// <param name="writer">An event writer</param>
        /// <param name="eventTemplate">An event template</param>
        /// <param name="eventData">Event data</param>
        static void WriteEventDataLine(
            EventStream writer,
            EventElement eventTemplate,
            string eventData)
        {
            WriteEventData(
                writer,
                eventTemplate,
                eventData + Environment.NewLine);
        }

        /// <summary>
        /// Write event data without appending a newline seperator.
        /// </summary>
        /// <param name="writer">An event writer</param>
        /// <param name="eventTemplate">An event template</param>
        /// <param name="eventData">Event data</param>
        static void WriteEventData(EventStream writer, EventElement eventTemplate, string eventData)
        {
            var @event = eventTemplate;
            @event.Data = eventData;
            writer.Write(@event);
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

        /// <summary>
        /// Run the scripts and validate the results.
        /// </summary>
        class TestScript : ModularInput
        {
            /// <summary>
            /// Scheme used by the test
            /// </summary>
            public override Scheme Scheme
            {
                get
                {
                    return new Scheme
                    {
                        Title = "Test Example",
                        Description = "This is a test modular input that handles all the appropriate functionality",
                        StreamingMode = StreamingMode.Xml,
                        Endpoint =
                        {
                            Arguments = new List<Argument>
                            {
                                new Argument
                                {
                                    Name = "interval",
                                    Description = "Polling Interval",
                                    DataType = DataType.Number,
                                    Validation = "is_pos_int('interval')"
                                },
                                new Argument
                                {
                                    Name = "username",
                                    Description = "Admin Username",
                                    DataType = DataType.String,
                                    RequiredOnCreate = false
                                },
                                new Argument
                                {
                                    Name = "password",
                                    Description = "Admin Password",
                                    DataType = DataType.String,
                                    RequiredOnEdit = true
                                }
                            }
                        }
                    };
                }
            }

            /// <summary>
            /// Perform test verifications and stream events.
            /// </summary>
            /// <param name="inputDefinition">Input definition</param>
            public override Task StreamEventsAsync(InputDefinition inputDefinition)
            {
                // Verify every part of the input definition is received 
                // parsed, and later recontructed correctly.
                var reconstructed = Serialize(inputDefinition);
                AssertEqualWithExpectedFile(InputDefinitionFilePath, reconstructed);

                // Test the dictionary for single value parameter.
                var stanza = inputDefinition.Stanzas["foobar://bbb"];
                var parameterName = "param2";

                // Test full parameter dictionary.
                var parameterValue = stanza.Parameters[parameterName];
                var singleValue = (SingleValueParameter.Value)parameterValue;
                Assert.Equal("value22", singleValue);

                // Test single value parameter dictionary.
                var stringValue = stanza.SingleValueParameters[parameterName];
                Assert.Equal("value22", stringValue);

                // Test the dictionary for multi value parameter.
                stanza = inputDefinition.Stanzas["foobar://bbb"];
                parameterValue = stanza.Parameters["multiValue2"];

                var multiValue = (MultiValueParameter.Value)parameterValue;
                var elementInMultiValue = multiValue[1];
                Assert.Equal("value4", elementInMultiValue);

                // Stanza property can't be used since there are more than one.
                stanza = null;
                try
                {
                    stanza = inputDefinition.Stanza;
                }
                catch (InvalidOperationException e)
                {
                    Assert.True(e.Message.Contains("Use Stanzas property instead"));
                }

                Assert.Null(stanza);
                WriteEvents();

                return Task.FromResult(false);
            }

            /// <summary>
            /// Validate and return an error message.
            /// </summary>
            /// <param name="validationItems">Configuration data to validate</param>
            /// <param name="errorMessage">Message to display in UI when validation fails</param>
            /// <returns>Whether the validation succeeded</returns>
            public override bool Validate(ValidationItems validationItems, out string errorMessage)
            {
                // Test the dictionary for single value parameter.
                var item = validationItems.Item;
                string stringParamValue = (SingleValueParameter.Value)item.Parameters["disabled"];
                Assert.Equal("0", stringParamValue);

                var reconstructed = Serialize(validationItems);
                AssertEqualWithExpectedFile(ValidationItemsFilePath, reconstructed);

                errorMessage = "test message";
                return false;
            }
        }
    }
}