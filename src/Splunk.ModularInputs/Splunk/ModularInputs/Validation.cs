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
    using System.IO;
    using System.Xml.Serialization;
    using System.Linq;

    /// <summary>
    /// The <see cref="Validation"/> class is used to parse and access the
    /// XML data for a new input definition from Splunk, pending validation.
    /// </summary>    
    /// <remarks>
    /// When a new input of this modular input type is being created, Splunk
    /// runs the modular input script (that is, executable) to validate its
    /// configuration.
    /// <example>
    /// Sample XML</example>
    /// <code>
    /// <![CDATA[
    ///   <?xml version="1.0" encoding="utf-16"?>
    ///   <items xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///     <server_host>tiny</server_host>
    ///     <server_uri>https://127.0.0.1:8089</server_uri>
    ///     <checkpoint_dir>/opt/splunk/var/lib/splunk/modinputs</checkpoint_dir>
    ///     <session_key>123102983109283019283</session_key>
    ///     <item name="aaa">
    ///       <param name="param1">value1</param>
    ///       <param name="param2">value2</param>
    ///       <param name="disabled">0</param>
    ///       <param name="index">default</param>
    ///       <param_list name="multiValue">
    ///         <value>value1</value>
    ///         <value>value2</value>
    ///       </param_list>
    ///       <param_list name="multiValue2">
    ///         <value>value3</value>
    ///         <value>value4</value>
    ///       </param_list>
    ///     </item>
    ///   </items>]]>
    /// </code>
    /// </remarks>
    [XmlRoot("items")]
    public class Validation
    {
        [XmlIgnore]
        public IReadOnlyDictionary<string, Parameter> Parameters {
            get
            {
                return InputDefinitionElement.Parameters.ToDictionary(
                    v => v.Name,
                    v => v
                );
            }
        }

        [XmlIgnore]
        public string Name {
            get
            {
                return InputDefinitionElement.Name;
            }
        }

        public class Item
        {
            [XmlAttribute("name")]
            public string Name { get; set; }
            
            [XmlElement("param", Type=typeof(SingleValueParameter))]
            [XmlElement("param_list", Type=typeof(MultiValueParameter))]
            public List<Parameter> Parameters { get; set; }
        }

        [XmlElement("item")]
        public Item InputDefinitionElement { get; set; }
            

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
