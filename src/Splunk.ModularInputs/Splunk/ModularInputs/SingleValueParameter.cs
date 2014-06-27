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
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="SingleValueParameter"/> class represents a parameter
    /// that contains a single value.
    /// </summary>
    [XmlRoot("param")]
    public class SingleValueParameter : Parameter
    {
        #region Constructors

        public SingleValueParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        /// <remarks>
        /// This value is used for XML serialization and deserialization.
        /// <example>
        /// Sample XML</example>
        /// <code>
        /// <param name="param1">value1</param>
        /// </code>
        /// </remarks>
        [XmlText]
        public string Value 
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts this object to a Boolean.
        /// </summary>
        /// <returns>
        /// This object as a Boolean.
        /// </returns>
        public override Boolean ToBoolean()
        {
            return Util.ParseSplunkBoolean(this.Value);
        }

        public override Collection<Boolean> ToBooleanCollection()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to a double.
        /// </summary>
        /// <returns>
        /// This object as a Double.
        /// </returns>
        public override Double ToDouble()
        {
            return double.Parse(this.Value);
        }

        public override Collection<double> ToDoubleCollection()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to an int 32.
        /// </summary>
        /// <returns>
        /// This object as an Int32.
        /// </returns>
        public override Int32 ToInt32()
        {
            return int.Parse(this.Value);
        }

        public override Collection<int> ToInt32Collection()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to an int 64.
        /// </summary>
        /// <returns>
        /// This object as an Int64.
        /// </returns>
        public override Int64 ToInt64()
        {
            return long.Parse(this.Value);
        }

        public override Collection<long> ToInt64Collection()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to a single.
        /// </summary>
        /// <returns>
        /// This object as a Single.
        /// </returns>
        public override Single ToSingle()
        {
            return float.Parse(this.Value);
        }

        public override Collection<float> ToSingleCollection()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            return this.Value;
        }

        public override Collection<string> ToStringCollection()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
