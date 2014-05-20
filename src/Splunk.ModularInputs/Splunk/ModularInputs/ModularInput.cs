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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    public static class ExtensionMethods
    {
        public static async Task WaitForCancellationAsync(this CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => source.SetResult(true));
            await source.Task;
            return;
        }
    }

    public class AwaitableProgress<T> : IProgress<T>
    {
        private event Action<T> handler = (T x) => { };

        public void Report(T value)
        {
            this.handler(value);
        }

        public async Task<T> AwaitProgressAsync()
        {
            TaskCompletionSource<T> source = new TaskCompletionSource<T>();
            Action<T> onReport = null;
            onReport = (T x) =>
            {
                handler -= onReport;
                source.SetResult(x);
            };
            handler += onReport;
            return await source.Task;
        }
    }

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
    /// 

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
        /// Performs the action specified by the <paramref name="args"/> parameter.
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
        /// If the <paramref name="args"/> are not in the supported set of values,
        /// the method will do nothing and return a non-zero value. Any 
        /// exceptions and internal progress messages encountered during 
        /// execution are written to the splunkd log file.
        /// </remarks>
        public async static Task<int> RunAsync<T>(string[] args,
            TextReader stdin = null,
            TextWriter stdout = null,
            TextWriter stderr = null,
            CancellationToken cancellationToken = default(CancellationToken),
            IProgress<EventWrittenProgressReport> progress = null) where T : ModularInput, new()
        {
            if (cancellationToken.IsCancellationRequested)
                return -1;

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

            using (EventWriter writer = new EventWriter(stdout, stderr, progress))
            {
                try
                {
                    T script = new T();
                    if (args.Length == 0)
                    {
                        List<Task> instances = new List<Task>();

                        InputDefinitionCollection inputDefinitions =
                            (InputDefinitionCollection)new XmlSerializer(typeof(InputDefinitionCollection)).
                            Deserialize(stdin);
                        foreach (InputDefinition inputDefinition in inputDefinitions)
                        {
                            instances.Add(script.StreamEventsAsync(inputDefinition, writer, cancellationToken));
                        }

                        await Task.WhenAll(instances.ToArray());
                        return 0;
                    }
                    else if (args[0].ToLower().Equals("--scheme"))
                    {
                        Scheme scheme = script.Scheme;
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

                        try
                        {
                            Validation validation = (Validation)new XmlSerializer(typeof(Validation)).
                                Deserialize(stdin);
                            if (script.Validate(validation, out errorMessage))
                            {
                                return 0; // Validation succeeded
                            }
                        }
                        catch (Exception e)
                        {
                            if (errorMessage == null)
                            {
                                errorMessage = e.ToString().Replace(Environment.NewLine, " | ");
                            }
                        }

                        using (var xmlWriter = XmlWriter.Create(stdout, XmlWriterSettings))
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
            }
        }



        /// <summary>
        /// Streams events to Splunk through standard output.
        /// </summary>
        /// <param name="inputDefinition">
        /// Input definition from Splunk for this input.
        /// </param>
        public abstract Task StreamEventsAsync(InputDefinition inputDefinition, EventWriter eventWriter,
            CancellationToken cancellationToken);

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
        public virtual bool Validate(Validation validationItems, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        #endregion

        #region Privates/internals

        private static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings()
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Fragment
        };

        #endregion

        
    }
}
