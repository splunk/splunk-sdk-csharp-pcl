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

        public static explicit operator string(Parameter parameter)
        {
            if (parameter is SingleValueParameter)
                return (string)(SingleValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to string");
        }

        public static explicit operator int(Parameter parameter)
        {
            if (parameter is SingleValueParameter)
                return (int)(SingleValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to int");
        }

        public static explicit operator double(Parameter parameter)
        {
            if (parameter is SingleValueParameter)
                return (double)(SingleValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to double");
        }

        public static explicit operator float(Parameter parameter)
        {
            if (parameter is SingleValueParameter)
                return (float)(SingleValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to float");
        }

        public static explicit operator long(Parameter parameter)
        {
            if (parameter is SingleValueParameter)
                return (long)(SingleValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to long");
        }

        public static explicit operator bool(Parameter parameter)
        {
            if (parameter is SingleValueParameter)
                return (bool)(SingleValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert multivalued parameter to bool");
        }

        public static explicit operator List<string>(Parameter parameter)
        {
            if (parameter is MultiValueParameter)
                return (List<string>)(MultiValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<string>");
        }

        public static explicit operator List<bool>(Parameter parameter)
        {
            if (parameter is MultiValueParameter)
                return (List<bool>)(MultiValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<bool>");
        }

        public static explicit operator List<double>(Parameter parameter)
        {
            if (parameter is MultiValueParameter)
                return (List<double>)(MultiValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<double>");
        }

        public static explicit operator List<float>(Parameter parameter)
        {
            if (parameter is MultiValueParameter)
                return (List<float>)(MultiValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<float>");
        }

        public static explicit operator List<int>(Parameter parameter)
        {
            if (parameter is MultiValueParameter)
                return (List<int>)(MultiValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<int>");
        }

        public static explicit operator List<long>(Parameter parameter)
        {
            if (parameter is MultiValueParameter)
                return (List<long>)(MultiValueParameter)parameter;
            else
                throw new InvalidCastException("Could not convert single valued parameter to List<long>");
        }
    }
}
