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
    /// Used with a threshold value to trigger the alert actions of a <see 
    /// cref="SavedSearch"/>.
    /// </summary>
    public enum AlertComparator
    {
        /// <summary>
        /// Specifies that there is no alert comparator.
        /// </summary>
        /// <remarks>
        /// This value unsets the alert comparator for a <see cref="SavedSearch"/>.
        /// </remarks>
        [EnumMember(Value = "")]
        None,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "drops by")]
        DropsBy,
        
        /// <summary>
        /// </summary>
        [EnumMember(Value = "drops by perc")]
        DropsByPercent,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "equal to")]
        Equal,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "greater than")]
        GreaterThan,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "less than")]
        LessThan,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "rises by")]
        RisesBy,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "rises by perc")]
        RisesByPercent
    }
}
