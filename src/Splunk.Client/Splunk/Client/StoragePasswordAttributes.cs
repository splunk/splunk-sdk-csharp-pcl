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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving <see cref="StoragePassword"/> 
    /// entries.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/pqZJco">REST API: GET apps/local</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    internal sealed class StoragePasswordAttributes : Args<StoragePasswordAttributes>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePasswordAttributes"/> 
        /// class.
        /// </summary>
        public StoragePasswordAttributes()
        { }

        #endregion

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

        #endregion

        /// <summary>
        /// Gets or sets the username for a <see cref="StoragePasssword"/>.
        /// </summary>
        [DataMember(Name = "name", IsRequired = true)]
        public string Username
        { get; set; }

    }
}
