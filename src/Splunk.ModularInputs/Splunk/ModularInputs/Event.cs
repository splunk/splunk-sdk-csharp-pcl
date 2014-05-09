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

//// TODO: 
//// [ ] Splunk.ModularInputs.EventElement really needs to be a class
////     Issue: Simply renaming it as a class breaks StreamEvents test

namespace Splunk.ModularInputs
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="Event"/> struct represents an event element
    /// for XML event streaming.
    /// </summary>
    [XmlRoot("event")]
    public struct Event
    {
        /// <summary>
        /// Event data.
        /// </summary>
        [XmlElement("data")]
        public string Data { get; set; }

        /// <summary>
        /// The event source.
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// The source type.
        /// </summary>
        [XmlElement("sourcetype")]
        public string SourceType { get; set; }

        /// <summary>
        /// The index.
        /// </summary>
        [XmlElement("index")]
        public string Index { get; set; }

        /// <summary>
        /// The host.
        /// </summary>
        [XmlElement("host")]
        public string Host { get; set; }

        /// <summary>
        /// The timestamp of the event.
        /// </summary>
        /// <remarks>
        /// If this property is null, Splunk will generate a timestamp
        /// according to current time, or in case of "unbroken" event, the
        /// timestamp supplied earlier for the event will be used.
        /// </remarks>
        [XmlElement("time")]            
        public DateTime? Time { get; set; }
        // TODO: Fix UTC timestamp generation

        /// <summary>
        /// A value indicating whether the event stream has
        /// completed a set of events and can be flushed.
        /// </summary>
        [XmlElement("done")]
        public bool Done { get; set; }

        /// <summary>
        /// A value indicating whether the element contains
        /// only a part of an event or multiple events. 
        /// </summary>
        /// <remarks>
        /// If this property is false, the element represents a single, 
        /// whole event.
        /// </remarks>
        [XmlAttribute("unbroken")]
        public bool Unbroken { get; set; }

        /// <summary>
        /// The name of the stanza of the input this event belongs to.
        /// </summary>
        [XmlAttribute("stanza")]
        public string Stanza { get; set; }
    }
}
