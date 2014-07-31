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
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="Event"/> struct represents an event element
    /// for XML event streaming.
    /// </summary>
    public struct Event
    {
        private static long ticksSinceEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public string Data { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }
        public string Index { get; set; }
        public string Host { get; set; }
        public DateTime? Time { get; set; }
        
        [System.ComponentModel.DefaultValue(false)]
        public bool Done { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool Unbroken { get; set; }
        public string Stanza { get; set; }

        private void WriteTextElement(XmlWriter writer, string name, string content)
        {
            writer.WriteStartElement(name);
            writer.WriteString(content);
            writer.WriteEndElement();
        }

        // Originally this class used the XML attributes to serialize itself
        // via an XmlSerializer, but that turns out not the work (see the comments
        // in EventWriter). So instead we have localized all the XML related code
        // in this one method. It turns out to be simpler to understand, too.
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("event");
            if (Stanza != null) writer.WriteAttributeString("stanza", Stanza);
            if (Unbroken) writer.WriteAttributeString("unbroken", "1");

            if (Data != null)   WriteTextElement(writer, "data", Data);
            if (Source != null) WriteTextElement(writer, "source", Source);
            if (SourceType != null) WriteTextElement(writer, "sourcetype", SourceType);
            if (Index != null) WriteTextElement(writer, "index", Index);
            if (Host != null) WriteTextElement(writer, "host", Host);

            if (Time.HasValue)
            {
                double timestamp = (Time.Value.Ticks - ticksSinceEpoch) / TimeSpan.TicksPerSecond;
                writer.WriteStartElement("time");
                writer.WriteString(timestamp.ToString());
                writer.WriteEndElement();
            }

            if (Done)
            {
                writer.WriteStartElement("done");
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
