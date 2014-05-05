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

namespace Splunk.Client
{
    using System;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk configuration setting.
    /// </summary>
    public class ConfigurationSetting : Entity<ConfigurationSetting>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSetting"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within <see cref="fileName"/>.
        /// </param>
        /// <param name="keyName">
        /// Name of the setting within <see cref="stanzaName"/> to be represented 
        /// by the current instance.</param>
        internal ConfigurationSetting(Context context, Namespace ns, string fileName, string stanzaName, 
            string keyName)
            : base(context, ns, new ResourceName(ConfigurationCollection.ClassResourceName, fileName, stanzaName), keyName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationSetting"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="ConfigurationStanza"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Configuration.GetSettingAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="ConfigurationSetting"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.UpdateSettingAsync"/></term>
        ///   <description>
        ///   Asynchronously creates or updates a <see cref="ConfigurationSetting"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="ConfigurationStanza.GetSettingAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="ConfigurationSetting"/>
        ///   in the current <see cref="ConfigurationStanza"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="ConfigurationStanza.UpdateSettingAsync"/></term>
        ///   <description>
        ///   Asynchronously creates or updates a <see cref="ConfigurationSetting"/> 
        ///   in the current <see cref="ConfigurationStanza"/>
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetConfigurationSettingAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves a <see cref="ConfigurationSetting"/> 
        ///   identified by configuration file, stanza, and setting name.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.UpdateConfigurationSettingAsync"/></term>
        ///   <description>
        ///   Asynchronously creates or updates a <see cref="ConfigurationSetting"/> 
        ///   identified by configuration file, stanza, and setting name.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ConfigurationSetting()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cached value of the current <see cref="ConfigurationSetting"/>.
        /// </summary>
        public string Value 
        {
            get { return this.GetValue("Value", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously retrieves the value of the current <see cref=
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
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                
                var reader = new StreamReader(response.Stream);
                var content = await reader.ReadToEndAsync();

                this.Snapshot = new EntitySnapshot(this.Snapshot, content.Length == 0 ? null : content);
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
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion
    }
}
