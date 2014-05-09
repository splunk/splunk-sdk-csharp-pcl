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
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ConfigurationItemBase"/> class is the base class for
    /// input definition that Splunk sends to modular input to start event
    /// streaming.
    /// </summary>
    public class ConfigurationItemBase
    {
        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItemBase"/> class.
        /// </summary>
        internal ConfigurationItemBase()
        {
            this.SingleValueParameterXmlElements = new List<SingleValueParameter>();
            this.MultiValueParameterXmlElements = new List<MultiValueParameter>();
        }

        #endregion

        /// <summary>
        /// The name of this item.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The list of parameters for defining this item.
        /// </summary>
        [XmlElement("param")]
        public List<SingleValueParameter> SingleValueParameterXmlElements { get; set; }

        /// <summary>
        /// The list of multi value parameters for defining this stanza.
        /// </summary>
        [XmlElement("param_list")]
        public List<MultiValueParameter> MultiValueParameterXmlElements { get; set; }

     

        #region Privates/internals

        /// <summary>
        /// Single value parameters keyed by name in the input definition item.
        /// </summary>
        Dictionary<string, string> singleValueParameters;

        #endregion
    }
}
