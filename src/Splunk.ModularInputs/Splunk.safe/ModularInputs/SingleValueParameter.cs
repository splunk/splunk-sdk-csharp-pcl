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

using System.Xml.Serialization;

namespace Splunk.ModularInputs
{
    /// <summary>
    /// The <see cref="SingleValueParameter"/> class represents a parameter
    /// that contains a single value.
    /// </summary>
    [XmlRoot("param")]
    public class SingleValueParameter : ParameterBase
    {
        // XML Example:
        // <param name="param1">value1</param>
        /// <summary>
        /// The value of the parameter.
        /// </summary>
        /// <remarks>
        /// This value is used for XML serialization and deserialization.
        /// </remarks>
        [XmlText]
        public string ValueXmlText { get; set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        internal override ValueBase ValueAsBaseType
        {
            get { return new Value(ValueXmlText); }
        }

        /// <summary>
        /// The <see cref="Value"/> class represents a single value.
        /// </summary>
        public class Value : ValueBase
        {
            /// <summary>
            /// The single value.
            /// </summary>
            private readonly string stringValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Value"/> class.
            /// </summary>
            /// <param name="value">Value of this type.</param>
            public Value(string value)
            {
                stringValue = value;
            }

            /// <summary>
            /// Converts a value to a string.
            /// </summary>
            /// <param name="singleValue">The value.</param>
            /// <returns>The string value.</returns>
            public static implicit operator string(Value singleValue)
            {
                return singleValue.ToString();
            }

            /// <summary>
            /// Converts to a string.
            /// </summary>
            /// <returns>The string value.</returns>
            public override string ToString()
            {
                return stringValue;
            }
        }
    }
}
