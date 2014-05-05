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
//
// [ ] Port DispatchTimeFormat and use it wherever appropriate. Can we create
//     a relative as well as an absolute time formatter?
// [O] Documentation

namespace Splunk.Client
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving transformed search 
    /// results.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/AfzBJO">REST API Reference: POST 
    ///   saved/searches/{name}/dispatch</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class SavedSearchDispatchArgs : Args<SavedSearchDispatchArgs>
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether a search should be 
        /// dispatched immediately.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.now", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchNow
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of timeline buckets for a search.
        /// </summary>
        /// <remarks>
        /// This default value is <c>0</c>.
        /// </remarks>        
        [DataMember(Name = "dispatch.buckets", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the earliest time to begin a search.
        /// </summary>
        /// <remarks>
        /// The value can be a relative or absolute time. If it is an absolute 
        /// time, use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        [DataMember(Name = "dispatch.earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string DispatchEarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the latest time to begin a search.
        /// </summary>
        /// <remarks>
        /// The value can be a relative or absolute time. If it is an absolute
        /// time, use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        [DataMember(Name = "dispatch.latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string DispatchLatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lookups are enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.lookups", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool DispatchLookups
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to produce before 
        /// finalizing a search.
        /// </summary>
        [DataMember(Name = "dispatch.max_count", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchMaxCount
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of time to run before finalizing a
        /// search.
        /// </summary>
        [DataMember(Name = "dispatch.max_time", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying how frequently Splunk runs the 
        /// map/reduce phase on accumulated map values.
        /// </summary>
        /// <remarks>
        /// The default value is <c>10</c>.
        /// </para>
        /// </remarks>
        [DataMember(Name = "dispatch.reduce_freq", EmitDefaultValue = false)]
        [DefaultValue(10)]
        public int DispatchReduceFrequency
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to back fill the real-time
        /// window for a search.
        /// </summary>
        /// <remarks>
        /// This value only applies to real-time searches.
        /// </remarks>
        [DataMember(Name = "dispatch.rt_backfill", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchRealTimeBackfill
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search should run in its
        /// own process.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>. A searches against an index must 
        /// run in its own process.
        /// </remarks>
        [DataMember(Name = "dispatch.spawn_process", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchSpawnProcess
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the time format Splunk should use to
        /// for the earliest and latest times of a search.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"%FT%T.%Q%:z"</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue("%FT%T.%Q%:z")]
        public string DispatchTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets the time to live (in seconds) for the artifacts of a
        /// search, if no actions are triggered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>"2p"</c> (two scheduled search periods).
        /// </para><para>
        /// If an action is triggered, Splunk changes the time to live (TTL) to
        /// that action's TTL. If multiple actions are triggered, Splunk applies
        /// the maximum TTL to the artifacts. To set an action's TTL, refer to
        /// <a href="http://goo.gl/6YNR2X">alert_actions.conf.spec</a> in the
        /// Splunk Admin Manual.</para>
        /// </remarks>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue("2p")]
        public string DispatchTtl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to start a new search even 
        /// if another instance of the <see cref="SavedSearch"/> is already 
        /// running.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>
        /// </remarks>
        [DataMember(Name = "force_dispatch", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ForceDispatch
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trigger alert actions.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "trigger_actions", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool TriggerActions
        { get; set; }

        #endregion
    }
}