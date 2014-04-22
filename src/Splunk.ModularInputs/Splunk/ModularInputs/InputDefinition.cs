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
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="InputDefinition"/> class is used to parse and access
    /// the XML data that defines the input from Splunk.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When Splunk executes a modular input script to stream events into
    /// Splunk, it reads configuration information from inputs.conf files in
    /// the system. It then passes this configuration in XML format to the
    /// script. The modular input script reads the configuration information
    /// from standard input.
    /// </para>
    /// <example>Sample XML</example>
    /// <code>
    /// <![CDATA[
    ///   <?xml version="1.0" encoding="utf-16"?>
    ///   <input xmlns:xsi="http:www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http:www.w3.org/2001/XMLSchema">
    ///     <server_host>tiny</server_host>
    ///     <server_uri>https:127.0.0.1:8089</server_uri>
    ///     <checkpoint_dir>/opt/splunk/var/lib/splunk/modinputs</checkpoint_dir>
    ///     <session_key>123102983109283019283</session_key>
    ///     <configuration>
    ///       <stanza name="foobar:aaa">
    ///         <param name="param1">value1</param>
    ///         <param name="param2">value2</param>
    ///         <param name="disabled">0</param>
    ///         <param name="index">default</param>
    ///       </stanza>
    ///       <stanza name="foobar:bbb">
    ///         <param name="param1">value11</param>
    ///         <param name="param2">value22</param>
    ///         <param name="disabled">0</param>
    ///         <param name="index">default</param>
    ///         <param_list name="multiValue">
    ///           <value>value1</value>
    ///           <value>value2</value>
    ///         </param_list>
    ///         <param_list name="multiValue2">
    ///           <value>value3</value>
    ///           <value>value4</value>
    ///         </param_list>
    ///       </stanza>
    ///     </configuration>
    ///   </input>]]>
    /// </code>
    /// </remarks>
    [XmlRoot("input")]
    public class InputDefinition : InputDefinitionBase
    {
        /// <summary>
        /// A dictionary of stanzas keyed by stanza name.
        /// </summary>
        Dictionary<string, Stanza> stanzas;

        /// <summary> 
        /// The stanza elements in the configuration element.
        /// </summary>
        [XmlArray("configuration")]
        [XmlArrayItem("stanza")]
        public List<Stanza> StanzaXmlElements { get; set; }

        /// <summary>
        /// Gets a dictionary of stanzas keyed by stanza name.
        /// </summary>
        public IDictionary<string, Stanza> Stanzas
        {
            get
            {
                if (this.stanzas == null)
                {
                    this.stanzas = this.StanzaXmlElements.ToDictionary(p => p.Name);
                }

                return this.stanzas;
            }
        }

        /// <summary>
        /// Gets the stanza in the input definition.
        /// </summary>
        /// <remarks>
        /// This method is provided because it is very common to have only one
        /// stanza.  That is the case when <see cref="UseSingleInstance"/> is 
        /// true. If there is more than one stanza, this property will fail.
        /// </remarks>
        public Stanza Stanza
        {
            get
            {
                if (this.StanzaXmlElements.Count > 1)
                {
                    throw new InvalidOperationException(
                        "There are more than one stanza. Use Stanzas property instead.");
                }

                return this.StanzaXmlElements[0];
            }
        }
    }
}
