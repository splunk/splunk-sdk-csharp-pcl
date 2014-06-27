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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;
    using System.Linq;

    /// <summary>
    /// The <see cref="MultiValueParameter"/> class represents a this that
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
        #region Constructors

        public MultiValueParameter(string name, params string[] values)
            : this(name, values.AsEnumerable())
        { }

        public MultiValueParameter(string name, IEnumerable<string> values)
        {
            this.Values = new Collection<string>();

            foreach (var value in values)
            {
                this.Values.Add(value);
            }
        }


        #endregion

        #region Properties

        /// <summary>
        /// The values in this this.
        /// </summary>
        [XmlElement("value")]
        public Collection<string> Values
        { get; private set; }

        #endregion

        #region Methods

        public override Boolean ToBoolean()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to a boolean collection.
        /// </summary>
        /// <returns>
        /// This object as a Collection&lt;Boolean&gt;
        /// </returns>
        public override Collection<Boolean> ToBooleanCollection()
        {
            var collection = new Collection<bool>();
            
            foreach (var value in this.Values)
            {
                collection.Add( Util.ParseSplunkBoolean(value));
            }

            return collection;
        }

        public override Double ToDouble()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to a double collection.
        /// </summary>
        /// <returns>
        /// This object as a Collection&lt;Double&gt;
        /// </returns>
        public override Collection<Double> ToDoubleCollection()
        {
            var collection = new Collection<double>();

            foreach (var value in this.Values)
            {
                collection.Add(double.Parse(value));
            }

            return collection;
        }


        public override Int32 ToInt32()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to an int 32 collection.
        /// </summary>
        /// <returns>
        /// This object as a Collection&lt;Int32&gt;
        /// </returns>
        public override Collection<Int32> ToInt32Collection()
        {
            var collection = new Collection<int>();

            foreach (var value in this.Values)
            {
                collection.Add(int.Parse(value));
            }

            return collection;
        }

        public override Int64 ToInt64()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to an int 64 collection.
        /// </summary>
        /// <returns>
        /// This object as a Collection&lt;Int64&gt;
        /// </returns>
        public override Collection<Int64> ToInt64Collection()
        {
            var collection = new Collection<long>();

            foreach (var value in this.Values)
            {
                collection.Add(long.Parse(value));
            }

            return collection;
        }

        public override Single ToSingle()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts this object to a single collection.
        /// </summary>
        /// <returns>
        /// This object as a Collection&lt;Single&gt;
        /// </returns>
        public override Collection<Single> ToSingleCollection()
        {
            var collection = new Collection<float>();

            foreach (var value in this.Values)
            {
                collection.Add(float.Parse(value));
            }

            return collection;
        }

        /// <summary>
        /// Converts this object to a string collection.
        /// </summary>
        /// <returns>
        /// This object as a Collection&lt;String&gt;
        /// </returns>
        public override Collection<String> ToStringCollection()
        {
            return new Collection<string>(this.Values);
        }

        #endregion
    }
}
