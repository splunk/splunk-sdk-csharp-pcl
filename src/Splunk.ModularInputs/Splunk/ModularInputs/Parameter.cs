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
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="Parameter"/> class is the base class for different
    /// types of parameters in an input configuration.
    /// </summary>
    public abstract class Parameter
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        public string ToString()
        {
            if (this is SingleValueParameter)
                return ((SingleValueParameter)this).ToString();
            else
                throw new InvalidCastException("Could not convert multivalued parameter to string");
        }

        public int ToInt()
        {
            if (this is SingleValueParameter)
                return ((SingleValueParameter)this).ToInt();
            else
                throw new InvalidCastException("Could not convert multivalued parameter to int");
        }

        public double ToDouble()
        {
            if (this is SingleValueParameter)
                return ((SingleValueParameter)this).ToDouble();
            else
                throw new InvalidCastException("Could not convert multivalued parameter to double");
        }

        public float ToFloat()
        {
            if (this is SingleValueParameter)
                return ((SingleValueParameter)this).ToFloat();
            else
                throw new InvalidCastException("Could not convert multivalued parameter to float");
        }

        public long ToLong()
        {
            if (this is SingleValueParameter)
                return ((SingleValueParameter)this).ToLong();
            else
                throw new InvalidCastException("Could not convert multivalued parameter to long");
        }

        public bool ToBool()
        {
            if (this is SingleValueParameter)
                return ((SingleValueParameter)this).ToBool();
            else
                throw new InvalidCastException("Could not convert multivalued parameter to bool");
        }

        public List<string> ToListOfString()
        {
            if (this is MultiValueParameter)
                return ((MultiValueParameter)this).ToListOfString();
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<string>");
        }

        public List<bool> ToListOfBool()
        {
            if (this is MultiValueParameter)
                return ((MultiValueParameter)this).ToListOfBool();
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<bool>");
        }

        public List<double> ToListOfDouble()
        {
            if (this is MultiValueParameter)
                return ((MultiValueParameter)this).ToListOfDouble();
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<double>");
        }

        public List<float> ToListOfFloat()
        {
            if (this is MultiValueParameter)
                return ((MultiValueParameter)this).ToListOfFloat();
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<float>");
        }

        public List<int> ToListOfInt()
        {
            if (this is MultiValueParameter)
                return ((MultiValueParameter)this).ToListOfInt();
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<int>");
        }

        public List<long> ToListOfLong()
        {
            if (this is MultiValueParameter)
                return ((MultiValueParameter)this).ToListOfLong();
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<long>");
        }
    }
}
