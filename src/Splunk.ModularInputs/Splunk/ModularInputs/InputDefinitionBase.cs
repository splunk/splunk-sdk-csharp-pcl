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
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for input definition.
    /// </summary>
    [XmlRoot("input")]
    public class InputDefinitionBase
    {
        /// <summary>
        /// The hostname for the Splunk server that runs the modular input.
        /// </summary>
        [XmlElement("server_host")]
        public string ServerHost { get; set; }

        /// <summary>
        /// The management URI for the Splunk server, identified by host, port,
        /// and protocol.
        /// </summary>
        [XmlElement("server_uri")]
        public string ServerUri { get; set; }

        /// <summary>
        /// The directory used for a modular input to save checkpoints.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// This location is where Splunk tracks the input state from sources
        /// it is reading from.
        /// </para>
        /// </remarks>
        [XmlElement("checkpoint_dir")]
        public string CheckpointDirectory { get; set; }

        /// <summary>
        /// The REST API session key for this modular input.
        /// </summary>
        [XmlElement("session_key")]
        public string SessionKey { get; set; }
    }
}
