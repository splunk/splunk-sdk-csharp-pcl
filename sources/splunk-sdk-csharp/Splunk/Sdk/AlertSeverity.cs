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

namespace Splunk.Sdk
{
    using System.Runtime.Serialization;

    /// <summary>
    /// 
    /// </summary>
    public enum AlertSeverity
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "1")]
        Debug, 
        
        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "2")]
        Information, 
        
        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "3")]
        Warning, 
        
        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "4")]
        Error,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "5")]
        Severe,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "6")]
        Fatal
    }
}
