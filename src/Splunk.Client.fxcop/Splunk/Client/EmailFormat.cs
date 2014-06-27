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

//// TODO:
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the format of text in an e-mail.
    /// </summary>
    /// <remarks>
    /// This value also applies to any attachments.
    /// </remarks>
    public enum EmailFormat
    {
        /// <summary>
        /// Specifies that an e-mail is formatted as comma-separated values.
        /// </summary>
        [EnumMember(Value = "csv")]
        Csv,
        
        /// <summary>
        /// Specifies that an e-mail is formatted as HTML text.
        /// </summary>
        [EnumMember(Value = "html")]
        Html,

        /// <summary>
        /// Specifies that an e-mail is formatted as plain text.
        /// </summary>
        [EnumMember(Value = "plain")]
        Plain,

        /// <summary>
        /// Specifies that an e-mail contains raw data.
        /// </summary>
        [EnumMember(Value = "raw")]
        Raw
    }
}
