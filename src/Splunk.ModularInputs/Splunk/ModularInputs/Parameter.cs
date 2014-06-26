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
                return (string)(SingleValueParameter)this;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to string");
        }

        public int ToInt()
        {
            if (this is SingleValueParameter)
                return (int)(SingleValueParameter)this;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to int");
        }

        public double ToDouble()
        {
            if (this is SingleValueParameter)
                return (double)(SingleValueParameter)this;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to double");
        }

        public float ToFloat()
        {
            if (this is SingleValueParameter)
                return (float)(SingleValueParameter)this;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to float");
        }

        public long ToLong()
        {
            if (this is SingleValueParameter)
                return (long)(SingleValueParameter)this;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to long");
        }

        public bool ToBool()
        {
            if (this is SingleValueParameter)
                return (bool)(SingleValueParameter)this;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to bool");
        }

        public List<string> ToListOfString()
        {
            if (this is MultiValueParameter)
                return (List<string>)(MultiValueParameter)this;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<string>");
        }

        public List<bool> ToListOfBool()
        {
            if (this is MultiValueParameter)
                return (List<bool>)(MultiValueParameter)this;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<bool>");
        }

        public List<double> ToListOfDouble()
        {
            if (this is MultiValueParameter)
                return (List<double>)(MultiValueParameter)this;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<double>");
        }

        public List<float> ToListOfFloat()
        {
            if (this is MultiValueParameter)
                return (List<float>)(MultiValueParameter)this;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<float>");
        }

        public List<int> ToListOfInt()
        {
            if (this is MultiValueParameter)
                return (List<int>)(MultiValueParameter)this;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<int>");
        }

        public List<long> ToListOfLong()
        {
            if (this is MultiValueParameter)
                return (List<long>)(MultiValueParameter)this;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<long>");
        }
    }
}
