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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the XML output when a modular input is called by Splunk for 
    /// introspection.
    /// </summary>
    /// <remarks>
    /// The modular input script (that is, executable) returns the XML output
    /// through standard output to Splunk.
    /// <example>
    /// Sample XML Output Document</example>
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <scheme xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///   <title>Test Example</title>
    ///   <description>This is a test modular input that handles all the appropriate functionality</description>
    ///   <use_external_validation>true</use_external_validation>
    ///   <use_single_instance>false</use_single_instance>
    ///   <streaming_mode>xml</streaming_mode>
    ///   <endpoint>
    ///     <args>
    ///       <arg name="interval">
    ///         <description>Polling Interval</description>
    ///         <validation>is_pos_int('interval')</validation>
    ///         <data_type>number</data_type>
    ///         <required_on_edit>false</required_on_edit>
    ///         <required_on_create>true</required_on_create>
    ///       </arg>
    ///       <arg name="username">
    ///         <description>Admin Username</description>
    ///         <data_type>string</data_type>
    ///         <required_on_edit>false</required_on_edit>
    ///         <required_on_create>false</required_on_create>
    ///       </arg>
    ///       <arg name="password">
    ///         <description>Admin Password</description>
    ///         <data_type>string</data_type>
    ///         <required_on_edit>true</required_on_edit>
    ///         <required_on_create>true</required_on_create>
    ///       </arg>
    ///     </args>
    ///   </endpoint>
    /// </scheme>]]>
    /// </code>
    /// </remarks>
    [XmlRoot("scheme")]
    public class Scheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scheme" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up default values for this scheme.
        /// </remarks>
        public Scheme()
        {
            this.UseExternalValidation = true;
            this.UseSingleInstance = false;
            this.Endpoint = new EndpointElement();
        }

        /// <summary>
        /// Gets or sets the title of the modular input.
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the modular input.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether external validation 
        /// is enabled for this modular input.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Override <see cref="ModularInput.Validate"/> to perform the validation.
        /// </para>
        /// <para>
        /// This property's default value is <c>true</c>.
        /// </para>
        /// </remarks>
        [XmlElement("use_external_validation")]
        public bool UseExternalValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to launch a single instance
        /// of the script or one script instance for each input stanza.  
        /// </summary>
        /// <remarks>
        /// This property's default value is <c>false</c>.
        /// </remarks>
        [XmlElement("use_single_instance")]
        public bool UseSingleInstance { get; set; }

        /// <summary>
        /// Gest or sets the streaming mode for this modular input (Simple or Xml).
        /// </summary>
        /// <remarks>
        /// This property's default value is "Xml".
        /// </remarks>
        [XmlElement("streaming_mode")]
        public StreamingMode StreamingMode { get; set; }

        /// <summary>
        /// Gets or sets the endpoint element for this scheme.
        /// </summary>
        [XmlElement("endpoint")]
        public EndpointElement Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the list of arguments specified by this Scheme.
        /// </summary>
        [XmlIgnore]
        public List<Argument> Arguments
        {
            get { return Endpoint.Arguments; }
            set { Endpoint.Arguments = value; }
        }

        public class EndpointElement
        {
            internal EndpointElement()
            {
                this.Arguments = new List<Argument>();
            }

            [XmlArray("args")]
            [XmlArrayItem("arg")]
            public List<Argument> Arguments { get; set; }
        }
    }
}
