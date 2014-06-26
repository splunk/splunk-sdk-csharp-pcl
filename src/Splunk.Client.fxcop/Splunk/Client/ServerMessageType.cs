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
    /// Specifies the severity of a <see cref="ServerMessage"/>.
    /// </summary>
    public enum ServerMessageSeverity
    {
        /// <summary>
        /// A <see cref="ServerMessage"/> is at the error severity level.
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// A <see cref="ServerMessage"/> is at the information severity level.
        /// </summary>
        [EnumMember(Value = "info")]
        Information, 
        
        /// <summary>
        /// A <see cref="ServerMessage"/> is at the warning severity level.
        /// </summary>
        [EnumMember(Value = "warn")]
        Warning
    }
}
