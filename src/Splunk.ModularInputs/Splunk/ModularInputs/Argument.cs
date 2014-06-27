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
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="Argument"/> class represents an XML entity describing
    /// an individual argument of a modular input.
    /// </summary>
    /// <remarks>
    /// It corresponds to one of the keys that can be defined for an instance
    /// of that modular input in a stanza in inputs.conf.
    /// <example>
    /// Sample XML Argument</example>
    /// <code>
    /// <arg name="interval">
    ///   <description>Polling Interval</description>
    ///   <validation>is_pos_int('interval')</validation>
    ///   <data_type>number</data_type>
    ///   <required_on_edit>false</required_on_edit>
    ///   <required_on_create>true</required_on_create>
    /// </arg>
    /// </code>
    /// </remarks>
    [XmlRoot("arg")]
    public class Argument
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument" /> class.
        /// </summary>
        public Argument()
        {
            DataType = DataType.String;
            this.RequiredOnEdit = false;
            this.RequiredOnCreate = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique name for the parameter.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The label for the parameter.
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// The validation rules for arguments passed to an endpoint
        /// create or edit action.
        /// </summary>
        [XmlElement("validation")]
        public string Validation { get; set; }

        public delegate bool ValidationHandler(Parameter parameter, out string errorMessage);

        /// <summary>
        /// Specify a delegate to run on the argument's value during validation (before the overall
        /// Validate method is called).
        /// </summary>
        [XmlIgnore]
        public ValidationHandler ValidationDelegate { get; set; }

        /// <summary>
        /// The value for use with scripts that return data in JSON format.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property defines the data type of the parameter.  
        /// </para>
        /// <para>
        /// The default data type is "string".
        /// </para>
        /// </remarks>
        [XmlElement("data_type")]
        public DataType DataType { get; set; }

        /// <summary>
        /// A value indicating whether the parameter is required for edit.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this property to <c>true</c> to make the parameter required for edit.
        /// </para>
        /// <para>
        /// This property's default value is <c>false</c>.
        /// </para>
        /// </remarks>
        [XmlElement("required_on_edit")]
        public bool RequiredOnEdit { get; set; }

        /// <summary>
        /// A value indicating whether the parameter is required for create.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this property to <c>false</c> to make the parameter optional.
        /// </para>
        /// <para>
        /// This property's default value is <c>true</c>.
        /// </para>
        /// </remarks>
        [XmlElement("required_on_create")]
        public bool RequiredOnCreate { get; set; }

        #endregion
    }
}
