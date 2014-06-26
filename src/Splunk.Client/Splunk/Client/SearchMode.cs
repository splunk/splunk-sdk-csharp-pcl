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
    /// Specifies the mode for a search job.
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Indicates that a search job should run over historical data.
        /// </summary>
        [EnumMember(Value = "normal")]
        Normal, 
        
        /// <summary>
        /// Indicates taht a search job should run over realtime data.
        /// </summary>
        [EnumMember(Value = "realtime")]
        RealTime
    }
}
