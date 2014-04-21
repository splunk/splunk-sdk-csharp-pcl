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
        /// <summary>
        /// Parameter in the input definition item.
        /// </summary>
        private Dictionary<string, ParameterBase.ValueBase> parameters;

        /// <summary>
        /// Single value parameters keyed 
        /// by name in the input definition item.
        /// </summary>
        private Dictionary<string, string> singleValueParameters;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ConfigurationItemBase" /> class.
        /// </summary>
        internal ConfigurationItemBase()
        {
            SingleValueParameterXmlElements = new List<SingleValueParameter>();
            MultiValueParameterXmlElements = new List<MultiValueParameter>();
        }

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

        /// <summary>
        /// Single value parameters keyed by name in the item.
        /// </summary>
        // This method is provided to make it easier to retrieve single value
        // parameters. It is a much more common case than multi value 
        // parameters. Splunk auto-generated modular input UI does not
        // support multi value parameters.
        public IDictionary<string, string> SingleValueParameters
        {
            get
            {
                if (singleValueParameters == null)
                {
                    singleValueParameters = SingleValueParameterXmlElements
                        .ToDictionary(
                        p => p.Name, 
                        p => (string) (SingleValueParameter.Value) p.ValueAsBaseType);
                }
                return singleValueParameters;
            }
        }

        /// <summary>
        /// Parameters keyed by name in the item.
        /// </summary>
        public IDictionary<string, ParameterBase.ValueBase> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    var list = new List<ParameterBase>();

                    list.AddRange(
                        SingleValueParameterXmlElements.Select(
                            p => (ParameterBase) p));

                    list.AddRange(
                        MultiValueParameterXmlElements.Select(
                            p => (ParameterBase) p));

                        parameters = list.ToDictionary(
                            p => p.Name, p => p.ValueAsBaseType);
                }
                return parameters;
            }
        }
    }
}
