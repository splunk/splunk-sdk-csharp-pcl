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
    /// Specifies the type of a server response <see cref="Message"/>.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Specifies a debug  messages.
        /// </summary>
        [EnumMember(Value = "DEBUG")]
        Debug, 
        
        /// <summary>
        /// Specifies an information message.
        /// </summary>
        [EnumMember(Value = "INFO")]
        Information, 
        
        /// <summary>
        /// Specifies a warning message.
        /// </summary>
        [EnumMember(Value = "WARN")]
        Warning, 
        
        /// <summary>
        /// Specifies an error message.
        /// </summary>
        [EnumMember(Value = "ERROR")]
        Error,

        /// <summary>
        /// Specifies a fatal error message.
        /// </summary>
        [EnumMember(Value = "FATAL")]
        Fatal
    }
}
