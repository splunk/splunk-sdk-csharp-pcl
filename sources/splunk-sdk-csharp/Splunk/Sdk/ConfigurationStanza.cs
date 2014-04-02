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
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;

    public class ConfigurationStanza : EntityCollection<ConfigurationStanza, ConfigurationSetting>
    {
        #region Constructors

        internal ConfigurationStanza(Context context, Namespace @namespace, string fileName, string stanzaName)
            : base(context, @namespace, new ResourceName(ResourceName.Properties, fileName, stanzaName))
        { }

        public ConfigurationStanza()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Adds or updates a list of settings in the current 
        /// <see cref="ConfigurationStanza"/>.
        /// </summary>
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the 
        /// <see cref="settings"/>.
        /// </remarks>
        public void UpdateSettings(params Argument[] settings)
        {
            this.UpdateSettingsAsync(settings).Wait();
        }

        /// <summary>
        /// Asynchronously adds or updates a list of settings in the current 
        /// <see cref="ConfigurationStanza"/>.
        /// </summary>
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the 
        /// <see cref="settings"/>.
        /// </remarks>
        public async Task UpdateSettingsAsync(params Argument[] settings)
        {
            Contract.Requires(settings != null);

            if (settings.Length <= 0)
            {
                return;
            }
            
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, settings))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.OK);
            }
        }

        #endregion
    }
}
