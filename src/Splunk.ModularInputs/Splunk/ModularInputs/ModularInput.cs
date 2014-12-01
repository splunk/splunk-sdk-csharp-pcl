﻿/*
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

using Splunk.Client;

namespace Splunk.ModularInputs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Threading;

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

        static ModularInput()
        {
            _isAttached = () => Debugger.IsAttached;
        }

        public static int Run<T>(string[] args) where T : ModularInput, new()
        {
            return Run<T>(args, DebuggerAttachPoints.None, 0);
        }

        public static int Run<T>(string[] args, DebuggerAttachPoints attachPoints, uint timeout = 30) where T : ModularInput, new()
        {
            if (!IsAttachPointNone(attachPoints) && timeout == 0)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Timeout parameter must be greater than or equal to 1 second");
            }

            T script = new T();
            Task<int> run = script.RunAsync(args, _stdin, _stdout, _stderr, null, attachPoints, timeout);
            run.Wait();
            if (run.IsCompleted)
                return run.Result;
            
            return -1;
        }

        internal static Func<bool> _isAttached;
        internal static TextReader _stdin;
        internal static TextWriter _stdout;
        internal static TextWriter _stderr;

        internal static void WaitForAttach(uint timeout)
        {
            var start = DateTime.Now;

            while (!_isAttached() && ((DateTime.Now - start).Seconds < timeout))
            {
                Thread.Sleep(100);
            }
        }

        internal static bool ShouldWaitForDebuggerToAttach(string[] args, DebuggerAttachPoints attachPoints)
        {
            if (IsAttachPointNone(attachPoints))
            {
                return false;
            }

            if (IsAttachPointAll(attachPoints))
            {
                return true;
            }

            if (args.Length > 0)
            {
                if (IsScheme(args, attachPoints) || IsValidateArguments(args, attachPoints))
                {
                    return true;
                }
            }
            else if (IsStreamEvents(attachPoints))
            {
                return true;
            }

            return false;
        }

        private static bool IsAttachPointAll(DebuggerAttachPoints attachPoints)
        {
            return attachPoints == DebuggerAttachPoints.All;
        }

        private static bool IsAttachPointNone(DebuggerAttachPoints attachPoints)
        {
            return attachPoints == DebuggerAttachPoints.None;
        }

        private static bool IsStreamEvents(DebuggerAttachPoints attachPoints)
        {
            return (attachPoints & DebuggerAttachPoints.StreamEvents) == DebuggerAttachPoints.StreamEvents;
        }

        private static bool IsValidateArguments(string[] args, DebuggerAttachPoints attachPoints)
        {
            return (args[0].ToLower().Equals("--validate-arguments") &&
                    ((attachPoints & DebuggerAttachPoints.ValidateArguments) == DebuggerAttachPoints.ValidateArguments));
        }

        private static bool IsScheme(string[] args, DebuggerAttachPoints attachPoints)
        {
            return (args[0].ToLower().Equals("--scheme") &&
                    ((attachPoints & DebuggerAttachPoints.Scheme) == DebuggerAttachPoints.Scheme));
        }

        /// <summary>
        /// Performs the action specified by the <paramref name="args"/> parameter.
        /// </summary>
        /// <param name="args">
        /// Command-line arguments provided by Splunk when it invokes the
        /// modular input program. Implementers should pass the arguments to
        /// the main method of the program as the value of this parameter.
        /// </param>
        /// <returns>
        /// <param name="stdin">
        /// Reader to use for the stdin stream
        /// </param>
        /// <param name="stdout">
        /// Writer to use for the stdout stream
        /// </param>
        /// <param name="stderr">
        /// Writer to use for the stderr stream
        /// </param>
        /// <param name="progress">
        /// Reports back progress as events are written to the <see cref="EventWriter"/>
        /// </param>
        /// <param name="attachPoints">
        /// Defines the <see cref="DebuggerAttachPoints"/> for this input
        /// </param>
        /// <param name="timeout">
        /// Number of seconds to wait for a debugger to attach before continuing processing.
        /// </param>
        /// A value which should be used as the exit code from the modular
        /// input program. A value of <c>0</c> indicates success. A non-zero
        /// value indicates failure.
        /// </returns>
        /// <remarks>
        /// If the <paramref name="args"/> are not in the supported set of values,
        /// the method will do nothing and return a non-zero value. Any 
        /// exceptions and internal progress messages encountered during 
        /// execution are written to the splunkd log file.
        /// </remarks>
        public async Task<int> RunAsync(string[] args,
            TextReader stdin = null,
            TextWriter stdout = null,
            TextWriter stderr = null,
            IProgress<EventWrittenProgressReport> progress = null,
            DebuggerAttachPoints attachPoints = DebuggerAttachPoints.None,
            uint timeout = 0
            )
        {
            // Check if the developer has specified they want to attach a debugger
            bool wait = ShouldWaitForDebuggerToAttach(args, attachPoints);

            // If a debugger is going to attach
            if (wait)
            {
                WaitForAttach(timeout);
            }

            if (progress == null)            
                progress = new Progress<EventWrittenProgressReport>();
            

            /// Console default is OEM text encoding, which is not handled by Splunk,
            //// resulting in loss of chars such as O with an umlaut (\u0150)
            //// Splunk's default is UTF8.
            if (stdin == null)
            {
                stdin = Console.In;
                Console.InputEncoding = Encoding.UTF8;
            }
            if (stdout == null)
            {
                stdout = Console.Out;
                Console.OutputEncoding = Encoding.UTF8;
            }
            if (stderr == null)
            {
                stderr = Console.Error;
                Console.OutputEncoding = Encoding.UTF8;
            }

            EventWriter writer = new EventWriter(stdout, stderr, progress);
            try
            {
                if (args.Length == 0)
                {
                    List<Task> instances = new List<Task>();

                    InputDefinitionCollection inputDefinitions =
                        (InputDefinitionCollection)new XmlSerializer(typeof(InputDefinitionCollection)).
                        Deserialize(stdin);
                    foreach (InputDefinition inputDefinition in inputDefinitions)
                    {
                        instances.Add(this.StreamEventsAsync(inputDefinition, writer));
                    }

                    await Task.WhenAll(instances.ToArray());
                    return 0;
                }
                else if (args[0].ToLower().Equals("--scheme"))
                {
                    Scheme scheme = this.Scheme;
                    if (scheme != null)
                    {
                        StringWriter stringWriter = new StringWriter();
                        new XmlSerializer(typeof(Scheme)).Serialize(stringWriter, scheme);
                        stdout.WriteLine(stringWriter.ToString());
                        return 0;
                    }
                    else
                    {
                        throw new NullReferenceException("Scheme was null; could not serialize.");
                    }
                }
                else if (args[0].ToLower().Equals("--validate-arguments"))
                {
                    string errorMessage = null;

                    Validation validation = (Validation)new XmlSerializer(typeof(Validation)).
                        Deserialize(stdin);

                    try
                    {
                        bool validationSuccessful = true;
                        Scheme scheme = this.Scheme;
                        foreach (Argument arg in scheme.Arguments)
                        {
                            if (arg.ValidationDelegate != null)
                            {
                                if (!arg.ValidationDelegate(validation.Parameters[arg.Name], out errorMessage))
                                {
                                    validationSuccessful = false;
                                    break;
                                }
                            }
                        }

                        if (validationSuccessful && this.Validate(validation, out errorMessage))
                        {
                            return 0; // Validation succeeded
                        }
                    }
                    catch (Exception) { }

                    using (var xmlWriter = XmlWriter.Create(stdout, new XmlWriterSettings
                    {
                        Async = true,
                        ConformanceLevel = ConformanceLevel.Fragment
                    }))
                    {
                        await xmlWriter.WriteStartElementAsync(prefix: null, localName: "error", ns: null);
                        await xmlWriter.WriteElementStringAsync(prefix: null, localName: "message", ns: null, value: errorMessage);
                        await xmlWriter.WriteEndElementAsync();
                    }
                    return -1;
                }
                else
                {
                    await writer.LogAsync("ERROR", "Invalid arguments to modular input.");
                    return -1;
                }
            }
            catch (Exception e)
            {
                var full = e.ToString().Replace(Environment.NewLine, " | ");
                writer.LogAsync(string.Format("Unhandled exception: {0}", full), "FATAL").Wait();
                return -1;
            }
            finally
            {
                writer.CompleteAsync().Wait();
            }
        }



        /// <summary>
        /// Streams events to Splunk through the provided EventWriter.
        /// </summary>
        /// <param name="inputDefinition">
        /// Input definition from Splunk for this input.
        /// </param>
        /// <param name="eventWriter">
        /// An object encapsulating writing events and log messages to Splunk.
        /// </param>
        public abstract Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter);

        /// <summary>
        /// Performs validation for configurations of a new input being
        /// created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// An application can override this method to perform custom
        /// validation logic. The default is to accept anything as valid.
        /// </para>
        /// </remarks>
        /// <param name="validationItems">Configuration data to validate.
        /// </param>
        /// <param name="errorMessage">Message to display in UI when validation
        /// fails.</param>
        /// <returns>A value indicating whether the validation
        /// succeeded.</returns>
        public virtual bool Validate(Validation validationItems, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        #endregion
        
    }
}
