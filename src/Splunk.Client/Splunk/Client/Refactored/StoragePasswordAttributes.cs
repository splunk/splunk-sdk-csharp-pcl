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
//// [X] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for creating <see cref="StoragePassword"/> 
    /// resources.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/JgyIeN">REST API Reference: POST storage/passwords</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    sealed class StoragePasswordAttributes : Args<StoragePasswordAttributes>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the password for a <see cref="StoragePassword"/>.
        /// </summary>
        [DataMember(Name = "password", IsRequired = true)]
        public string Password
        { get; set; }

        /// <summary>
        /// Gets or sets the realm in which a <see cref="StoragePassword"/> is
        /// valid.
        /// </summary>
        [DataMember(Name = "realm", EmitDefaultValue = false)]
        public string Realm
        { get; set; }

        /// <summary>
        /// Gets or sets the username for a <see cref="StoragePassword"/>.
        /// </summary>
        [DataMember(Name = "name", IsRequired = true)]
        public string Username
        { get; set; }

        #endregion
    }
}
