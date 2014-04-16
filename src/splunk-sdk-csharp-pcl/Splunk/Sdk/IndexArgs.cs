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
// [ ]  Documentation

namespace Splunk.Sdk
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for starting a new search job.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/OWTUws">REST API Reference: POST search/jobs</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class IndexArgs : Args<IndexArgs>
    {
        #region Constructors

        public IndexArgs(string coldPath = "", string homePath = "", string thawedPath = "")
        {
            this.ColdPath = coldPath;
            this.HomePath = homePath;
            this.ThawedPath = thawedPath;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an absolute path that contains the cold databases for 
        /// an index.
        /// </summary>
        /// <remarks>
        /// The path must be readable and writable. Cold databases are opened 
        /// as needed when searching. The path may be defined in terms of a 
        /// volume definition. Splunk will not start if an index lacks a valid 
        /// cold path. This value is required.
        /// </remarks>
        [DataMember(Name = "coldPath", IsRequired = true)]
        public string ColdPath
        { get; private set; }

        /// <summary>
        /// Gets or sets an absolute path that contains the hot and warm 
        /// buckets for an index.
        /// </summary>
        /// <remarks>
        /// The path must be readable and writable. This value is required.
        /// </remarks>
        [DataMember(Name = "homePath", IsRequired = true)]
        public string HomePath
        { get; private set; }

        /// <summary>
        /// Gets or sets an absolute path that contains the thawed (resurrected)
        /// databases for an index.
        /// </summary>
        /// <remarks>
        /// The path must be readable and writable. The path cannot be defined 
        /// in terms of a volume definition. Splunk will not start if an index 
        /// lacks a valid thawed path. This value is required.
        /// </remarks>
        [DataMember(Name = "thawedPath", IsRequired = true)]
        public string ThawedPath
        { get; private set; }

        #endregion
    }
}