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
    /// Specifies the condition under which the alert actions of a
    /// <see cref= "SavedSearch"/> are triggered.
    /// </summary>
    public enum AlertType
    {
        /// <summary>
        /// Specifies that alert actions are triggered whenever the <see cref=
        /// "SavedSearch"/> runs.
        /// </summary>
        /// <remarks>
        /// This is the default value.
        /// </remarks>
        [EnumMember(Value = "always")]
        Always,
        
        /// <summary>
        /// Specifies that alert actions are triggered based on a custom 
        /// condition on the results produced by a <see cref="SavedSearch"/>.
        /// </summary>
        [EnumMember(Value = "custom")]
        Custom,

        /// <summary>
        /// Specifies that alert actions are triggered based on the number of 
        /// events produced by a <see cref="SavedSearch"/>.
        /// </summary>
        [EnumMember(Value = "number of events")]
        NumberOfEvents,

        /// <summary>
        /// Specifies that alert actions are triggered based on the number of 
        /// hosts appearing in the results produced by a <see cref="SavedSearch"/>.
        /// </summary>
        [EnumMember(Value = "number of hosts")]
        NumberOfHosts,

        /// <summary>
        /// Specifies that alert actions are triggered based on the number of 
        /// sources appearing in the results produced by a <see cref=
        /// "SavedSearch"/>.
        /// </summary>
        [EnumMember(Value = "number of sources")]
        NumberOfSources
    }
}
