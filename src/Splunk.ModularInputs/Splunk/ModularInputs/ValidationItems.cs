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
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ValidationItems"/> class is used to parse and access the
    /// XML data for a new input definition from Splunk, pending validation.
    /// </summary>    
    /// <remarks>
    /// When a new input of this modular input type is being created, Splunk
    /// runs the modular input script (that is, executable) to validate its
    /// configuration.
    /// </remarks>
    [XmlRoot("items")]
    public class ValidationItems : InputDefinitionBase
    {
        // XML Example:
        // <?xml version="1.0" encoding="utf-16"?>
        //<items xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        //  <server_host>tiny</server_host>
        //  <server_uri>https://127.0.0.1:8089</server_uri>
        //  <checkpoint_dir>/opt/splunk/var/lib/splunk/modinputs</checkpoint_dir>
        //  <session_key>123102983109283019283</session_key>
        //  <item name="aaa">
        //    <param name="param1">value1</param>
        //    <param name="param2">value2</param>
        //    <param name="disabled">0</param>
        //    <param name="index">default</param>
        //    <param_list name="multiValue">
        //      <value>value1</value>
        //      <value>value2</value>
        //    </param_list>
        //    <param_list name="multiValue2">
        //      <value>value3</value>
        //      <value>value4</value>
        //    </param_list>
        //  </item>
        //</items>

        /// <summary>
        /// Gets or sets an instance of this modular input.
        /// </summary>
        /// <remarks>This property is used by unit tests.</remarks>
        [XmlElement("item")]
        public ConfigurationItem Item { get; set; }
    }
}
