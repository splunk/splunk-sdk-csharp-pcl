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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an object representation of a Splunk configuration setting.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Resource"/>
    /// <seealso cref="T:Splunk.Client.IConfigurationSetting"/>
    public class ConfigurationSetting : Resource, IConfigurationSetting
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSetting"/>
        /// class.
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

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ConfigurationSetting"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public ConfigurationSetting()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.GetValue("Links"); }
        }

        /// <inheritdoc/>
        public string Value 
        {
            get { return this.GetValue("Value", StringConverter.Instance); }
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ConfigurationSetting"/> class.
        /// </summary>
        /// <param name="resource">
        /// An object representing a configuration setting.
        /// </param>
        internal ConfigurationSetting(BaseResource resource)
        {
            this.Object = resource.Object;
        }

        #endregion
    }
}
