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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for sending events to a Splunk <see cref="Transmitter"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/OWTUws">REST API Reference: POST
    ///     search/jobs</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.TransmitterArgs}"/>
    public sealed class TransmitterArgs : Args<TransmitterArgs>
    {
        /// <summary>
        /// Gets or sets the value to populate in the host field for events from this
        /// data input.
        /// </summary>
        /// <value>
        /// The value to populate in the host field for events from this data input.
        /// </value>
        [DataMember(Name = "host", EmitDefaultValue = false)]
        public string Host
        { get; set; }

        /// <summary>
        /// Gets or sets a regular expression used to extract the host value from
        /// each event.
        /// </summary>
        /// <value>
        /// A regular expression used to extract the host value from each event.
        /// </value>
        [DataMember(Name = "host_regex", EmitDefaultValue = false)]
        public string HostRegex
        { get; set; }

        /// <summary>
        /// Gets or sets the source value to fill in the metadata for this input's
        /// events.
        /// </summary>
        /// <value>
        /// The source value to fill in the metadata for this input's events.
        /// </value>
        [DataMember(Name = "source", EmitDefaultValue = false)]
        public string Source
        { get; set; }

        /// <summary>
        /// Gets or sets the source type to apply to events from this input.
        /// </summary>
        /// <value>
        /// The source type to apply to events from this input.
        /// </value>
        [DataMember(Name = "sourcetype", EmitDefaultValue = false)]
        public string SourceType
        { get; set; }
    }
}