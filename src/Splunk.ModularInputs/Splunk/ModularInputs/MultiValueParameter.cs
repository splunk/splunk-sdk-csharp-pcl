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

    /// <summary>
    /// The <see cref="MultiValueParameter"/> class represents a parameter that
    /// contains a multivalue.
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
    public class MultiValueParameter : ParameterBase
    {
        #region Properties

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        [XmlElement("value")]
        public Value ValueXmlElements { get; set; }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        internal override ValueBase ValueAsBaseType
        {
            get { return this.ValueXmlElements; }
        }

        #endregion

        #region Types

        /// <summary>
        /// The <see cref="Value"/> class represents a multivalue.
        /// </summary>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1600:ElementsMustBeDocumented",
            Justification = "Internal class. Pure passthrough.")]
        public class Value : ValueBase, IList<string>
        {
            readonly List<string> value = new List<string>();

            public int Count
            {
                get { return this.value.Count; }
            }

            public bool IsReadOnly
            {
                get { return ((ICollection<string>)this.value).IsReadOnly; }
            }

            public string this[int index]
            {
                get { return this.value[index]; }
                set { this.value[index] = value; }
            }

            public int IndexOf(string item)
            {
                return this.value.IndexOf(item);
            }

            public void Insert(int index, string item)
            {
                this.value.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                this.value.RemoveAt(index);
            }

            public void Add(string item)
            {
                this.value.Add(item);
            }

            public void Clear()
            {
                this.value.Clear();
            }

            public bool Contains(string item)
            {
                return this.value.Contains(item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                this.value.CopyTo(array, arrayIndex);
            }

            public bool Remove(string item)
            {
                return this.value.Remove(item);
            }

            public IEnumerator<string> GetEnumerator()
            {
                return this.value.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// The <see cref="ValueXmlElement"/> class represents the <b>value</b> 
        /// XML element.
        /// </summary>
        /// <remarks>
        /// This class is used for serializing and deserializing the
        /// <b>value</b> XML element for a multivalue parameter.
        /// </remarks>
        [XmlRoot("value")]
        public class ValueXmlElement
        {
            /// <summary>
            /// The value of the parameter.
            /// </summary>
            [XmlText]
            public string Text { get; set; }

            /// <summary>
            /// Returns the string value.
            /// </summary>
            /// <returns>
            /// The string value.
            /// </returns>
            public override string ToString()
            {
                return this.Text;
            }

            /// <summary>
            /// Converts a <see cref="ValueXmlElement"/> to a <c>string</c>.
            /// </summary>
            /// <param name="value">Field value.</param>
            /// <returns>
            /// The string value delimiter.
            /// </returns>
            /// <remarks>
            /// This method is the same as <see cref="ToString" />.
            /// </remarks>
            public static implicit operator string(ValueXmlElement value)
            {
                return value.ToString();
            }
        }

        #endregion
    }
}
