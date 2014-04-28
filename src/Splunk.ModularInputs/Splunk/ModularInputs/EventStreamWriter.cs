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
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="EventStreamWriter"/> class writes events to standard
    /// output using the XML streaming mode.
    /// </summary>
    public sealed class EventStreamWriter : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamWriter"/>
        /// class.
        /// </summary>
        public EventStreamWriter()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Releases the resources used by the current <see cref=
        /// "EventStreamWriter"/>.
        /// </summary>
        /// <remarks>
        /// This method writes the closing <![CDATA[</stream>]]> element
        /// synchronously, if it is outstanding, but best practice is to
        /// write this element asynchronously beforehand by calling <see cref=
        /// "EventStreamWriter.WriteEndAsync"/>.
        /// </remarks>
        public void Dispose()
        {
            //// The XmlTextWriter class's IDisposable implementation writes 
            //// all unwritten end elements and--like all good IDisposable 
            //// implementations--can be called many times. Hence, we let it 
            //// do all the work.
            this.xmlWriter.Dispose();
        }

        public async Task WriteEndAsync()
        {
            await this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes an <see cref="EventElement"/> to the standard output device.
        /// </summary>
        /// <param name="eventElement">
        /// An object representing an event element.
        /// </param>
        /// <remarks>
        /// <example>Sample event stream</example>
        /// <code>
        /// <stream>
        ///   <event>
        ///     <index>sdk-tests2</index>
        ///     <sourcetype>test sourcetype</sourcetype>
        ///     <source>test source</source>
        ///     <host>test host</host>
        ///     <data>Event with all default fields set</data>
        ///   </event>
        ///   <event stanza="modular_input:UnitTest2" unbroken="1">
        ///     <data>Part 1 of channel 2 without a newline </data>
        ///   </event>
        /// </stream>
        /// </code>
        /// </remarks>
        public async Task WriteEventAsync(EventElement eventElement)
        {
            if (this.xmlWriter.WriteState == WriteState.Start)
            {
                await this.xmlWriter.WriteStartElementAsync(prefix: null, localName: "stream", ns: null);
            }

            await this.xmlWriter.WriteStartElementAsync(prefix: null, localName: "event", ns: null);
            var stanza = eventElement.Stanza;

            if (stanza != null)
            {
                await this.xmlWriter.WriteAttributeStringAsync(prefix: null, localName: "stanza", ns: null, value: stanza);
            }

            if (eventElement.Unbroken)
            {
                await this.xmlWriter.WriteAttributeStringAsync(prefix: null, localName: "unbroken", ns: null, value: "1");
            }

            await this.WriteElementIfNotNullAsync("index", eventElement.Index);
            await this.WriteElementIfNotNullAsync("sourcetype", eventElement.SourceType);
            await this.WriteElementIfNotNullAsync("source", eventElement.Source);
            await this.WriteElementIfNotNullAsync("host", eventElement.Host);
            await this.WriteElementIfNotNullAsync("data", eventElement.Data);

            var time = eventElement.Time;

            if (time != null)
            {
                await this.xmlWriter.WriteElementStringAsync(
                    prefix: null, localName: "time", ns: null, value: ConvertTimeToUtcUnixTimestamp(time.Value));
            }

            if (eventElement.Done)
            {
                await this.xmlWriter.WriteStartElementAsync(prefix: null, localName: "done", ns: null);
                await this.xmlWriter.WriteEndElementAsync();
                await this.xmlWriter.FlushAsync();
            }

            await this.xmlWriter.WriteEndElementAsync();
        }

        #endregion

        #region Privates/internals

        internal static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings()
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Fragment
        };

        static readonly DateTime UnixUtcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        XmlWriter xmlWriter = XmlWriter.Create(Console.Out, XmlWriterSettings);

        /// <summary>
        /// Converts a date-time value to Unix UTC timestamp.
        /// </summary>
        /// <param name="dateTime">A date-time value.</param>
        /// <returns>The UTC timestamp.</returns>
        static string ConvertTimeToUtcUnixTimestamp(DateTime dateTime)
        {
            var utcTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);
            return (utcTime - UnixUtcEpoch).TotalSeconds.ToString();
        }

        /// <summary>
        /// Asynchronously writes an element if its content is non <c>null</c>.
        /// </summary>
        /// <param name="tag">
        /// The element name.
        /// </param>
        /// <param name="content">
        /// Content of the element.
        /// </param>
        async Task WriteElementIfNotNullAsync(string tag, string content)
        {
            Debug.Assert(tag != null, "tag == null");

            if (content != null)
            {
                await this.xmlWriter.WriteElementStringAsync(prefix: null, localName: tag, ns: null, value: content);
            }
        }

        #endregion
    }
}
