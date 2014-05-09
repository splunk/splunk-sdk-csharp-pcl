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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    public class EventWriter : IDisposable
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(Event));

        private BlockingCollection<Event> eventQueue;
        private XmlWriter writer;
        Task eventQueueMonitor;
        TextWriter stderr;
        
        
        /// <summary>
        /// 
        /// </summary>
        public EventWriter() : this(Console.Out, Console.Error) {}

        public EventWriter(TextWriter stdout, TextWriter stderr)
        {
            this.stderr = stderr;
            eventQueue = new BlockingCollection<Event>();
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            writer = XmlWriter.Create(stdout, settings);
            eventQueueMonitor = WriteEventElementsAsync();
        }

        public void WriteEvent(Event e)
        {
            eventQueue.Add(e);
        }
    
        #region Privates/internals

        static readonly DateTime UnixUtcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Dispose()
        {
            eventQueue.CompleteAdding();
            eventQueueMonitor.Wait();
        }

        public async Task LogAsync(string severity, string message)
        {
            await this.stderr.WriteAsync(severity);
            await this.stderr.WriteAsync(" ");
            await this.stderr.WriteAsync(message);
            await this.stderr.WriteAsync(this.stderr.NewLine);
        }

        static string ConvertDateTimeToUnixTimestamp(DateTime value)
        {
            var utcTime = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Utc);
            return (utcTime - UnixUtcEpoch).TotalSeconds.ToString();
        }

        private async Task WriteEventElementsAsync()
        {
            await this.writer.WriteStartElementAsync(prefix: null, localName: "stream", ns: null);

            foreach (Event e in this.eventQueue.GetConsumingEnumerable())
            {
                serializer.Serialize(writer, e);
                await this.writer.FlushAsync();
            }

            await writer.WriteEndElementAsync();
            await writer.FlushAsync();
            writer.Close();
        }
        
        #endregion
    }
}
