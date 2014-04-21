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

using System;
using System.Diagnostics;

namespace Splunk.ModularInputs
{
    /// <summary>
    /// List of log levels for logging functions.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug messages.
        /// </summary>
        Debug,

        /// <summary>
        /// Informational messages.
        /// </summary>
        Info,

        /// <summary>
        /// Warning messages.
        /// </summary>
        Warn,

        /// <summary>
        /// Error messages.
        /// </summary>
        Error,

        /// <summary>
        /// Fatal error messages.
        /// </summary>
        Fatal
    }

    /// <summary>
    /// The <see cref="SystemLogger"/> class holds information for logging 
    /// in a Splunk modular input.
    /// </summary>
    public static class SystemLogger
    {
        /// <summary>
        /// Converts a log level into a string.
        /// </summary>
        /// <param name="level">The log level to convert.</param>
        /// <returns>The string representation of the log level.</returns>
        private static string GetLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Info:
                    return "INFO";
                case LogLevel.Warn:
                    return "WARN";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Fatal:
                    return "FATAL";
                default:
                    Debug.Assert(false, "Unexpected trace level.");
                    return "ERROR";
            }
        }

        /// <summary>
        /// Convenience method to write log messages to splunkd.log.
        /// </summary>
        /// <param name="msg">The message.</param>
        public static void Write(string msg)
        {
            Write(LogLevel.Info, msg);
        }

        /// <summary>
        /// Convenience method to write log messages to splunkd.log.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="msg">The message.</param>
        public static void Write(LogLevel level, string msg)
        {
            // Message example:
            // INFO Script.Run: Reading input definition FATAL Script.Run: Unhandled exception: System.InvalidOperationException: There is an error in XML document (0, 0). ---> System.Xml.XmlException: Root element is missing.     at System.Xml.XmlTextReaderImpl.Throw(Exception e)     at System.Xml.XmlTextReaderImpl.ParseDocumentContent()     at System.Xml.XmlTextReaderImpl.Read()     at System.Xml.XmlTextReader.Read()     at System.Xml.XmlReader.MoveToContent()     at Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationReaderInputDefinition.Read9_input()     --- End of inner exception stack trace ---     at System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader xmlReader, String encodingStyle, XmlDeserializationEvents events)     at System.Xml.Serialization.XmlSerializer.Deserialize(TextReader textReader)     at Splunk.ModularInputs.Script.Read(Type type) in c:\Users\Andy\Documents\GitHub\splunk-sdk-csharp\SplunkSDK\ModularInputs\Script.cs:line 187     at Splunk.ModularInputs.Script.Run[T](String[] args) in c:\Users\Andy\Documents\GitHub\splunk-sdk-csharp\SplunkSDK\ModularInputs\Script.cs:line 98
            var line = string.Format("{0} {1}", GetLevelString(level), msg);
            Console.Error.WriteLine(line);
            Console.Error.Flush();
        }
    }
}
