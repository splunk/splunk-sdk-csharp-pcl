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
    using Splunk;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk configuration setting.
    /// </summary>
    public class ConfigurationSetting : BaseResource
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSetting"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal ConfigurationSetting(AtomEntry entry, Version generatorVersion)
        {
            this.Initialize(entry, generatorVersion);
        }

        protected internal ConfigurationSetting(BaseResource resource)
            : base(resource)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationSetting"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public ConfigurationSetting()
        { }

        #endregion

        #region Properties

        public IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.GetValue("Links"); }
        }

        /// <summary>
        /// Gets the cached value of the current <see cref="ConfigurationSetting"/>.
        /// </summary>
        public string Value 
        {
            get { return this.GetValue("Value", StringConverter.Instance); }
        }

        #endregion
    }
}
