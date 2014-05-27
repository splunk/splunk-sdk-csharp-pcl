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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;
    using System.Linq;

    /// <summary>
    /// The <see cref="MultiValueParameter"/> class represents a parameter that
    /// contains multiple values.
    /// </summary>
    /// <remarks>
    /// <example>Sample XML</example>
    /// <code>
    /// <param_list name="multiValue">
    ///   <value>value1</value>
    ///   <value>value2</value>
    /// </param_list>
    /// </code>
    /// </remarks>
    [XmlRoot("param_list")]
    public class MultiValueParameter : Parameter
    {
        #region Properties

        /// <summary>
        /// The values in this parameter.
        /// </summary>
        [XmlElement("value")]
        public List<string> Values;

        #endregion

        #region Methods

        public static explicit operator List<string>(MultiValueParameter parameter)
        {
            return new List<string>(parameter.Values);
        }

        public static explicit operator List<bool>(MultiValueParameter parameter)
        {
            return (from x in parameter.Values select Util.ParseSplunkBoolean(x)).ToList();
        }

        public static explicit operator List<double>(MultiValueParameter parameter)
        {
            return (from x in parameter.Values select double.Parse(x)).ToList();
        }

        public static explicit operator List<float>(MultiValueParameter parameter)
        {
            return (from x in parameter.Values select float.Parse(x)).ToList();
        }

        public static explicit operator List<int>(MultiValueParameter parameter)
        {
            return (from x in parameter.Values select int.Parse(x)).ToList();
        }

        public static explicit operator List<long>(MultiValueParameter parameter)
        {
            return (from x in parameter.Values select long.Parse(x)).ToList();
        }
        
        #endregion
    }
}
