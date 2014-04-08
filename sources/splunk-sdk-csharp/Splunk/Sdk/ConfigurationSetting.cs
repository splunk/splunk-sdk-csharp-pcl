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
// [ ] Contracts
// [ ] Documentation
// [ ] Properties & Methods

namespace Splunk.Sdk
{
    using System;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationSetting : Entity<ConfigurationSetting>
    {
        #region Constructors

        internal ConfigurationSetting(Context context, Namespace @namespace, string fileName, string stanzaName, 
            string keyName)
            : base(context, @namespace, new ResourceName(ResourceName.Properties, fileName, stanzaName), keyName)
        { }

        public ConfigurationSetting()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cached value of the current <see cref="ConfigurationSetting"/>.
        /// </summary>
        public string Value 
        {
            get { return this.Content.GetValue("Value", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchrononously retrieves the value of the current <see cref=
        /// "ConfigurationSetting"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET 
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint to 
        /// retrieve the configuration setting represented by this instance.
        /// </remarks>
        public override async Task GetAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.OK);
                var reader = new StreamReader(response.Stream);
                var content = await reader.ReadToEndAsync();

                this.Data = new DataCache(new AtomEntry(this.Data.Entry, content.Length == 0 ? null : content));
            }
        }

        /// <summary>
        /// Asynchronously updates the value of the current <see cref=
        /// "ConfigurationSetting"/>.
        /// </summary>
        /// <param name="value">
        /// A new value for the current <see cref="ConfigurationSetting"/>.
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sSzcMy">POST 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// update the current <see cref="ConfigurationSetting"/> value.
        /// </remarks>
        public async Task UpdateAsync(string value)
        {
            var args = new Argument[] { new Argument("value", value) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, args))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.OK);
            }
        }

        #endregion
    }
}
