/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

namespace Splunk.ModularInputs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ModularInput"/> class represents the functionality of a
    /// modular input script (that is, an executable).
    /// </summary>
    /// <remarks>
    /// <para>
    /// An application derives from this class to define a modular input. It
    /// must override the <see cref="Scheme"/> and <see cref="StreamEvents"/>
    /// methods. It can optionally override the <see cref="Validate"/> method.
    /// </para>
    /// </remarks>
    public abstract class ModularInput
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="Scheme" /> that will be returned to Splunk for
        /// introspection.
        /// </summary>
        public abstract Scheme Scheme { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the action specified by the <see cref="args"/> parameter.
        /// </summary>
        /// <typeparam name="T">
        /// The application-derived type of the <see cref="ModularInput"/>. It
        /// must have a parameterless constructor.
        /// </typeparam>
        /// <param name="args">
        /// Command-line arguments provided by Splunk when it invokes the
        /// modular input program. Implementers should pass the arguments to
        /// the main method of the program as the value of this parameter.
        /// </param>
        /// <returns>
        /// A value which should be used as the exit code from the modular
        /// input program. A value of <c>0</c> indicates success. A non-zero
        /// value indicates failure.
        /// </returns>
        /// <remarks>
        /// If the <see cref="args"/> are not in the supported set of values,
        /// the method will do nothing and return a non-zero value. Any 
        /// exceptions and internal progress messages encountered during 
        /// execution are written to the splunkd log file.
        /// </remarks>
        public static async Task<int> RunAsync<T>(string[] args) where T : ModularInput, new()
        {
            try
            {
                //// Console default is OEM text encoding, which is not handled by Splunk,
                //// resulting in loss of chars such as O with double acute (\u0150)
                //// Splunk's default is UTF8.

                //// Avoid setting InputEncoding unnecessarily because 
                //// it will cause a reset of Console.In (which should be a 
                //// System.Console bug), 
                //// losing the redirection unit tests depend on.

                if (!(Console.InputEncoding is UTF8Encoding))
                {
                    Console.InputEncoding = Encoding.UTF8;
                }

                Console.OutputEncoding = Encoding.UTF8; // sets stderr and stdout
                var script = new T();

                if (args.Length == 0)
                {
                    await LogAsync("Reading input definitions.");
                    var eventStreams = new List<Task>();

                    for (int i = 1; Console.In.Peek() != -1; i++)
                    {
                        var inputDefinition = (InputDefinition)Read(typeof(InputDefinition));
                        await LogAsync(string.Format("Starting event stream {0}.", i));
                        eventStreams.Add(script.StreamEventsAsync(inputDefinition));
                    }

                    if (eventStreams.Count == 0)
                    {
                        var message = "No input definitions could be read from the standard input stream.";
                        throw new InvalidDataException(message);
                    }

                    await Task.Factory.ContinueWhenAll(eventStreams.ToArray(), 
                        (tasks) => EventWriter.CompleteAdding(),
                        TaskContinuationOptions.LongRunning);

                    return 0;
                }

                if (args[0].ToLower().Equals("--scheme"))
                {
                    if (script.Scheme != null)
                    {
                        await LogAsync("Writing introspection scheme");
                        Console.WriteLine(Serialize(script.Scheme));
                    }

                    return 0;
                }

                if (args[0].ToLower().Equals("--validate-arguments"))
                {
                    string errorMessage = null;

                    try
                    {
                        await LogAsync("Reading validation items");
                        var validationItems = (ValidationItems)Read(typeof(ValidationItems));
                        await LogAsync("Calling Validate");

                        if (script.Validate(validationItems, out errorMessage))
                        {
                            return 0; // Validation succeeded
                        }
                    }
                    catch (Exception e)
                    {
                        LogExceptionAsync(e).Wait();

                        if (errorMessage == null)
                        {
                            errorMessage = e.Message;
                            LogAsync("Using exception message as validation error message").Wait();
                        }
                    }

                    await WriteValidationErrorAsync(errorMessage);
                }
            }
            catch (Exception e)
            {
                LogExceptionAsync(e).Wait();
            }

            // The value one indicates failure, but has no specific meaning
            return 1;
        }

        /// <summary>
        /// Streams events to Splunk through standard output.
        /// </summary>
        /// <param name="inputDefinition">
        /// Input definition from Splunk for this input.
        /// </param>
        public abstract Task StreamEventsAsync(InputDefinition inputDefinition);

        /// <summary>
        /// Writes a validation error to standard output during external 
        /// validation.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <remarks>
        /// <para>
        /// The validation error will also be displayed in the Splunk UI.
        /// Normally an application does not need to call this method.
        /// It will be called by <see cref="ModularInput.Run{T}"/> automatically.
        /// </para>
        /// <example>Sample error message</example>
        /// <code>
        /// <error>
        ///   <message>test message</message>
        /// </error>
        /// </code>
        /// </remarks>
        public static async Task WriteValidationErrorAsync(string message)
        {
            using (var xmlWriter = XmlWriter.Create(Console.Out, XmlWriterSettings))
            {
                await xmlWriter.WriteStartElementAsync(prefix: null, localName: "error", ns: null);
                await xmlWriter.WriteElementStringAsync(prefix: null, localName: "message", ns: null, value: message);
                await xmlWriter.WriteEndElementAsync();
            }
        }

        /// <summary>
        /// Performs validation for configurations of a new input being
        /// created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// An application can override this method to perform custom
        /// validation logic.
        /// </para>
        /// </remarks>
        /// <param name="validationItems">Configuration data to validate.
        /// </param>
        /// <param name="errorMessage">Message to display in UI when validation
        /// fails.</param>
        /// <returns>A value indicating whether the validation
        /// succeeded.</returns>
        public virtual bool Validate(ValidationItems validationItems, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        #endregion

        #region Privates/internals

        static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings() 
        { 
            Async = true, 
            ConformanceLevel = ConformanceLevel.Fragment 
        };

        /// <summary>
        /// Serializes this object to XML output. Used by unit tests.
        /// </summary>
        /// <param name="object">An object to serialize.</param>
        /// <returns>The XML string.</returns>
        internal static string Serialize(object @object)
        {
            var x = new XmlSerializer(@object.GetType());
            var sw = new StringWriter();
            x.Serialize(sw, @object);
            return sw.ToString();
        }

        /// <summary>
        /// Writes an exception as a <see cref="LogLevel.Info"/> event to the
        /// splunkd log.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Exception"/> to write.
        /// </param>
        static async Task LogExceptionAsync(Exception e)
        {
            //// The splunkd logger identifies each line of text as a new log entry, but the stack trace is a
            //// multi-line entry. Hence, we replace newlines with the string " | ".
            var full = e.ToString();
            full = full.Replace(Environment.NewLine, " | ");
            await LogAsync(string.Format("Unhandled exception: {0}", full), LogLevel.Fatal);
        }

        /// <summary>
        /// Writes a message to the splunkd log.
        /// </summary>
        /// <param name="message">
        /// A message.
        /// </param>
        /// <param name="level">Log level. The default value is 
        /// <c>LogLevel.Info</c>.
        /// </param>
        static async Task LogAsync(string message, LogLevel level = LogLevel.Info)
        {
            await SystemLogger.WriteAsync(level, "Script.Run: " + message);
        }

        /// <summary>
        /// Reads standard input and returns the parsed XML input.
        /// </summary>
        /// <param name="type">Type of object to parse.</param>
        /// <returns>An object.</returns>
        static object Read(Type type)
        {
            var x = new XmlSerializer(type);
            return x.Deserialize(Console.In);
        }

        #endregion
    }
}
