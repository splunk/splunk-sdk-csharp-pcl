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

        public static int Run<T>(string[] args) where T : ModularInput, new()
        {
            T script = new T();
            Task<int> run = script.RunAsync(args);
            run.Wait();
            if (run.IsCompleted)
                return run.Result;
            else
                return -1;
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
            IProgress<EventWrittenProgressReport> progress = null)
        {
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
