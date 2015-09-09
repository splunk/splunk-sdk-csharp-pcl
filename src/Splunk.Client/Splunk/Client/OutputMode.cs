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

namespace Splunk.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the format for the output from a search command.
    /// </summary>
    public enum OutputMode
    {
        /// <summary>
        /// 
        /// </summary>
        Default,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "atom")]
        Atom,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "csv")]
        Csv,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "json")]
        Json,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "json_cols")]
        JsonColumns,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "json_rows")]
        JsonRows,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "raw")]
        Raw,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "xml")]
        Xml
    }
}
