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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    static class EventWriter
    {
        /// <summary>
        /// 
        /// </summary>
        static EventWriter()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            eventCollection = new BlockingCollection<EventElement>();
            eventWriter = XmlWriter.Create(Console.Out, settings);

            writeEventElements = Task.Factory.StartNew(WriteEventElementsAsync);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventElement">
        /// </param>
        public static void Add(EventElement item)
        {
            eventCollection.Add(item);
        }

        #region Privates/internals

        static readonly DateTime UnixUtcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly BlockingCollection<EventElement> eventCollection;
        static readonly XmlWriter eventWriter;
        static Task writeEventElements;

        internal static void CompleteAdding()
        {
            eventCollection.CompleteAdding();
            writeEventElements.Wait();
        }

        static string ConvertDateTimeToUnixTimestamp(DateTime value)
        {
            var utcTime = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Utc);
            return (utcTime - UnixUtcEpoch).TotalSeconds.ToString();
        }

        static async Task WriteEventElementAsync(EventElement eventElement)
        {
            if (eventWriter.WriteState == WriteState.Start)
            {
                await eventWriter.WriteStartElementAsync(prefix: null, localName: "stream", ns: null);
            }

            await eventWriter.WriteStartElementAsync(prefix: null, localName: "event", ns: null);
            var stanza = eventElement.Stanza;

            if (stanza != null)
            {
                await eventWriter.WriteAttributeStringAsync(prefix: null, localName: "stanza", ns: null, value: stanza);
            }

            if (eventElement.Unbroken)
            {
                await eventWriter.WriteAttributeStringAsync(prefix: null, localName: "unbroken", ns: null, value: "1");
            }

            if (eventElement.Index != null)
            {
                await eventWriter.WriteElementStringAsync(prefix: null, localName: "index", ns: null, value: eventElement.Index);
            }
            
            if (eventElement.SourceType != null)
            {
                await eventWriter.WriteElementStringAsync(prefix: null, localName: "sourcetype", ns: null, value: eventElement.SourceType);
            }
            
            if (eventElement.Source != null)
            {
                await eventWriter.WriteElementStringAsync(prefix: null, localName: "source", ns: null, value: eventElement.Source);
            }
            
            if (eventElement.Host != null)
            {
                await eventWriter.WriteElementStringAsync(prefix: null, localName: "host", ns: null, value: eventElement.Host);
            }

            if (eventElement.Data != null)
            {
                await eventWriter.WriteElementStringAsync(prefix: null, localName: "data", ns: null, value: eventElement.Data);
            }

            var time = eventElement.Time;

            if (time != null)
            {
                await eventWriter.WriteElementStringAsync(
                    prefix: null, localName: "time", ns: null, value: ConvertDateTimeToUnixTimestamp(time.Value));
            }

            if (eventElement.Done)
            {
                await eventWriter.WriteStartElementAsync(prefix: null, localName: "done", ns: null);
                await eventWriter.WriteEndElementAsync();
                await eventWriter.FlushAsync();
            }

            await eventWriter.WriteEndElementAsync();
        }

        static async void WriteEventElementsAsync()
        {
            foreach (var eventElement in eventCollection.GetConsumingEnumerable())
            {
                await WriteEventElementAsync(eventElement);
            }

            eventWriter.Close();
        }
        
        #endregion
    }
}
