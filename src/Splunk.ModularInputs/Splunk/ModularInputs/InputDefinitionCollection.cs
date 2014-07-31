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
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.Linq;

    /// <summary>
    /// The collection of input definitions passed to this program.
    /// </summary>
    [XmlRoot("input")]
    public class InputDefinitionCollection
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

        /// <summary>
        /// Represents the data for a single instance of the modular input.
        /// </summary>
        public class Stanza
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlElement("param", Type=typeof(SingleValueParameter))]
            [XmlElement("param_list", Type=typeof(MultiValueParameter))]
            public List<Parameter> Parameters { get; set; }
        }

        [XmlArray("configuration")]
        [XmlArrayItem("stanza", Type=typeof(Stanza))]
        public List<Stanza> Stanzas { get; set; }

        public IEnumerator<InputDefinition> GetEnumerator()
        {
            foreach (Stanza stanza in Stanzas)
            {
                yield return new InputDefinition {
                    Name = stanza.Name,
                    Parameters = stanza.Parameters.ToDictionary(
                        v => v.Name,
                        v => v
                    ),
                    ServerHost = this.ServerHost,
                    ServerUri = this.ServerUri,
                    CheckpointDirectory = this.CheckpointDirectory,
                    SessionKey = this.SessionKey
                };
            }
        }
    }
}
