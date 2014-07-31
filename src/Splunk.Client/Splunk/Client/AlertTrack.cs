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
    /// Specifies whether to track the actions triggered by a scheduled search.
    /// </summary>
    public enum AlertTrack
    {
        /// <summary>
        /// Determine whether to track based on the tracking setting of each 
        /// action. Do not track scheduled searches that always trigger 
        /// actions.
        /// </summary>
        [EnumMember(Value = "auto")]
        Automatic,

        /// <summary>
        /// Force tracking.
        /// </summary>
        [EnumMember(Value = "1")]
        True, 
        
        /// <summary>
        /// Disable tracking.
        /// </summary>
        [EnumMember(Value = "0")]
        False 
    }
}
