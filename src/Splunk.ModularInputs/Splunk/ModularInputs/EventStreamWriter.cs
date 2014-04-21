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
    using System.Xml;

    /// <summary>
    /// The <see cref="EventStreamWriter"/> class writes an event to stdout
 	/// using the XML streaming mode.
    /// </summary>
    public class EventStreamWriter : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamWriter"/>
 		/// class.
        /// </summary>
        public EventStreamWriter()
        {
            xmlWriter.WriteStartElement("stream");
        }

        /// <summary>
        /// Writes the last end tag and releases resources.
        /// </summary>
        public void Dispose()
        {
            // TODO: Correct this bad practice. 
            // 1. Dispose must never throw.
            //    xmlWriter may throw.

            if (xmlWriter == null)
            {
                return;
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Dispose();
            xmlWriter = null;
        }

        /// <summary>
        /// Writes the event element.
        /// </summary>
        /// <param name="eventElement">An event element.</param>
        public void Write(EventElement eventElement)
        {
            //XML Example:
            //<stream>
            //  <event>
            //      <index>sdk-tests2</index>
            //      <sourcetype>test sourcetype</sourcetype>
            //      <source>test source</source>
            //      <host>test host</host>
            //      <data>Event with all default fields set</data>
            //  </event>
            //  <event stanza="modular_input://UnitTest2" unbroken="1">
            //      <data>Part 1 of channel 2 without a newline </data>
            //  </event>
            //</stream>

            xmlWriter.WriteStartElement("event");

            var stanza = eventElement.Stanza;
            if (stanza != null)
            {
                xmlWriter.WriteAttributeString("stanza", stanza);
            }

            if (eventElement.Unbroken)
            {
                xmlWriter.WriteAttributeString("unbroken", "1");
            }

            WriteElementIfNotNull(eventElement.Index, "index");
            WriteElementIfNotNull(eventElement.SourceType, "sourcetype");
            WriteElementIfNotNull(eventElement.Source, "source");
            WriteElementIfNotNull(eventElement.Host, "host");

            WriteElementIfNotNull(eventElement.Data, "data");

            var time = eventElement.Time;
            if (time != null)
            {
                xmlWriter.WriteElementString(
                    "time",
                    ConvertTimeToUtcUnixTimestamp(time.Value));
            }

            if (eventElement.Done)
            {
                xmlWriter.WriteStartElement("done");
                xmlWriter.WriteEndElement();
                Console.Out.Flush();
            }

            xmlWriter.WriteEndElement();
        }

        #region Privates

        /// <summary>
        /// Used to write XML to stdout.
        /// </summary>
        XmlWriter xmlWriter = new XmlTextWriter(Console.Out);

        /// <summary>
        /// Writes the element if its content is not null.
        /// </summary>
        /// <param name="content">Content of the element.</param>
        /// <param name="tag">The tag name.</param>
        private void WriteElementIfNotNull(string content, string tag)
        {
            if (content != null)
            {
                xmlWriter.WriteElementString(tag, content);
            }
        }

        /// <summary>
        /// Converts a date-time value to Unix UTC timestamp.
        /// </summary>
        /// <param name="dateTime">A date-time value.</param>
        /// <returns>The UTC timestamp.</returns>
        private static string ConvertTimeToUtcUnixTimestamp(DateTime dateTime)
        {
            // Unit timestamp is seconds after a fixed date, known as 
            // "unix timestamp epoch".
            var unixUtcEpoch =
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var utcTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);

            return (utcTime - unixUtcEpoch).TotalSeconds.ToString();
        }

        #endregion
    }
}
