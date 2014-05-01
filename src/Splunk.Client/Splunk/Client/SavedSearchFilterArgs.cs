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

// TODO:
// [O] Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving <see cref="SavedSearch"/>
    /// entries.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/bKrRK0">REST API: GET saved/searches</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class SavedSearchFilterArgs : Args<SavedSearchFilterArgs>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the lower bound of the time window for which saved 
        /// search schedules should be returned.
        /// </summary>
        /// <remarks>
        /// This property specifies that all the scheduled times starting from 
        /// this time (not just the next run time) should be returned.
        /// </remarks>
        [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string EarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets the upper bound of the time window for which saved 
        /// search schedules should be returned.
        /// </summary>
        /// <remarks>
        /// This property specifies that all the scheduled times ending with 
        /// this time (not just the next run time) should be returned.
        /// </remarks>
        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string LatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to list default actions for
        /// <see cref="SavedSearch"/> entries.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "listDefaultActionArgs", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ListDefaultActions
        { get; set; }

        #endregion
    }
}
