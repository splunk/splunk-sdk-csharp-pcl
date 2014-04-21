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
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="Script"/> class represents the functionality of a
    /// modular input script (that is, an executable).
    /// </summary>
    /// <remarks>
    /// <para>
    /// An application derives from this class to define a modular input. It
    /// must override the <see cref="Scheme"/> and <see cref="StreamEvents"/>
    /// methods. It can optionally override the <see cref="Validate"/> method.
    /// </para>
    /// </remarks>
    public abstract class Script
    {
        /// <summary>
        /// Gets the <see cref="Scheme" /> that will be returned to Splunk for
        /// introspection.
        /// </summary>
        public abstract Scheme Scheme { get; }

        /// <summary>
        /// Performs the action specified by the <c>args</c> parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <c>args</c> are not in the supported set of values,
        /// the method will do nothing and return a non-zero code (for
        /// instance, "1"), without raising an exception.
        /// </para>
        /// <para>
        /// Any exceptions and internal progress messages when executing this
        /// method will be logged to the splunkd log.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">
        /// The application-derived type of the <see cref="Script"/>. It must
        /// have a constructor without a parameter.
        /// </typeparam>
        /// <param name="args">
        /// Command-line arguments provided by Splunk when it invokes the
        /// modular input script (that is, executable). An application should
        /// pass the <c>args</c> of the <c>Main</c> method (that
        /// is, the executable entry point) into this method.
        /// </param>
        /// <returns>
        /// Exit code, which should be used as the return value of the 
        /// <c>Main</c> method. A value of <c>0</c> indicates success.
        /// </returns>
        public static int Run<T>(string[] args)
            where T : Script, new()
        {
            try
            {
                var utf8 = new UTF8Encoding();

                // Console default is OEM text encoding, which is not handled by Splunk,
                // resulting in loss of chars such as O with double acute (\u0150)
                // Splunk's default is UTF8.

                // Avoid setting InputEncoding unnecessarily because 
                // it will cause a reset of Console.In 
                // (which should be a System.Console bug), 
                // losing the redirection unit tests depend on.
                if (!(Console.InputEncoding is UTF8Encoding))
                {
                    Console.InputEncoding = utf8;
                }

                // Below will set both stdout and stderr.
                Console.OutputEncoding = utf8;

                var script = new T();

                if (args.Length == 0)
                {
                    Log("Reading input definition");
                    var inputDefinition = (InputDefinition)Read(
                        typeof(InputDefinition));
                    Log("Calling StreamEvents");
                    script.StreamEvents(inputDefinition);
                    return 0;
                }

                if (args[0].ToLower().Equals("--scheme"))
                {
                    if (script.Scheme != null)
                    {
                        Log("Writing introspection streme");
                        Console.WriteLine(Serialize(script.Scheme));
                    }
                    return 0;
                }

                if (args[0].ToLower().Equals("--validate-arguments"))
                {
                    string errorMessage = null;

                    try
                    {
                        Log("Reading validation items");
                        var validationItems = (ValidationItems)Read(
                            typeof(ValidationItems));

                        Log("Calling Validate");
                        if (script.Validate(validationItems, out errorMessage))
                        {
                            // Validation succeeded.
                            return 0;
                        }
                    }
                    catch (Exception e)
                    {
                        LogException(e);

                        if (errorMessage == null)
                        {
                            errorMessage = e.Message;
                            Log("Using exception message as validation error message");
                        }
                    }

                    // Validation failed.
                    WriteValidationError(errorMessage);
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }

            // Return code indicating a failure.
            // '1' has non special meaning other than it is non zero.
            return 1;
        }

        /// <summary>
        /// Writes a validation error to stdout during external validation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The validation error will also be displayed in the Splunk UI.
        /// </para>
        /// <para>
        /// Normally an application does not need to call this method.
        /// It will be called by <see cref="Script.Run{T}"/> automatically.
        /// </remarks>
        /// <param name="errorMessage">The error message.</param>
        public static void WriteValidationError(string errorMessage)
        {
            // XML Example:
            // <error><message>test message</message></error>
            using (var xmlWriter = new XmlTextWriter(Console.Out))
            {
                xmlWriter.WriteStartElement("error");
                xmlWriter.WriteElementString("message", errorMessage);
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Reads stdin and returns the parsed XML input.
        /// </summary>
        /// <param name="type">Type of object to parse.</param>
        /// <returns>An object.</returns>
        internal static object Read(Type type)
        {
            var x = new XmlSerializer(type);
            return x.Deserialize(Console.In);
        }

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
        /// Writes an exception as a <c>LogLevel.Info</c> event into the
        /// splunkd log.
        /// </summary>
        /// <param name="e">An exception.</param>
        private static void LogException(Exception e)
        {
            // splunkd log breaks up events with newlines, which will split
            // stack trace. Replace them with double space.
            var full = e.ToString();
            full = full.Replace(Environment.NewLine, "  ");

            Log(
                string.Format("Unhandled exception: {0}", full),
                LogLevel.Fatal);
        }

        /// <summary>
        /// Writes a message into the splunkd log.
        /// </summary>
        /// <param name="msg">A message.</param>
        /// <param name="level">Log level. The default value is 
        /// <c>LogLevel.Info</c>.
        /// </param>
        private static void Log(string msg, LogLevel level = LogLevel.Info)
        {
            SystemLogger.Write(level, "Script.Run: " + msg);
        }

        /// <summary>
        /// Streams events to Splunk through stdout.
        /// </summary>
        /// <param name="inputDefinition">
        /// Input definition from Splunk for this input.
        /// </param>
        public abstract void StreamEvents(InputDefinition inputDefinition);

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
    }
}
