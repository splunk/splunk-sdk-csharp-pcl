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
// [O] Documentation
// [O] Properties & Methods

namespace Splunk.Client
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for accessing and updating Splunk configuration files.
    /// </summary>
    /// <remarks>
    /// Splunk's configuration information is stored in configuration files. 
    /// These files are identified by their .conf extension and hold the 
    /// information for different aspects of your Splunk configurations, 
    /// including:
    /// <list type="bullet">
    /// <item><description>
    ///     System settings
    /// </description></item>
    /// <item><description>
    ///     Authentication and authorization information
    /// </description></item>
    /// <item><description>
    ///     Index mappings and setting
    /// </description></item>
    /// <item><description>
    ///     Deployment and cluster configurations
    /// </description></item>
    /// <item><description>
    ///     Knowledge objects and saved searches
    /// </description></item>
    /// <para>
    /// Most configuration files come packaged with your Splunk installation
    /// and can be found in <c>$SPLUNK_HOME/etc/default</c>.
    /// </para>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/h9USqx">Admin Manual: About configuration
    ///     files</a>.
    /// </description></item>
    /// <item><description>
    ///     <a href="http://goo.gl/jNHaBr">Admin Manual: List of configuration
    ///     files</a>.
    /// </description></item>
    /// <item><description>
    ///     <a href="http://goo.gl/cTdaIH">REST API Reference: Accessing and 
    ///     updating Splunk configurations</a>.
    /// </description></item>
    /// <item><description>
    ///     <a href="http://goo.gl/0ELhzV">REST API Reference: Configurations</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class Configuration : EntityCollection<Configuration, ConfigurationStanza>
    {
        #region Constructors

        internal Configuration(Context context, Namespace @namespace, string fileName)
            : base(context, @namespace, new ResourceName(ConfigurationCollection.ClassResourceName, fileName))
        { }

        public Configuration()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates the configuration file represented by this
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7">POST 
        /// properties</a> endpoint to create the configuration file represented
        /// by this instance.
        /// </remarks>
        public async Task CreateAsync()
        {
            var args = new Argument[] { new Argument("__conf", this.ResourceName.Title) };

            using (var response = await this.Context.PostAsync(this.Namespace, ConfigurationCollection.ClassResourceName, args))
            {
                if (response.Message.StatusCode != HttpStatusCode.Created)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a setting from the configuration file 
        /// represented by this instance.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current instance.
        /// </param>
        /// <param name="keyName">
        /// Name of a configuration setting in <see cref="stanzaName"/>.
        /// </param>
        /// <returns>
        /// An object representing the configuration setting identifed by <see 
        /// cref="stanzaName"/> and <see cref="keyName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET 
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint to 
        /// construct the <see cref="ConfigurationSetting"/> it returns.
        /// </remarks>
        public async Task<ConfigurationSetting> GetSettingAsync(string stanzaName, string keyName)
        {
            var resource = new ConfigurationSetting(this.Context, this.Namespace, this.ResourceName.Title, stanzaName, 
                keyName);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously updates a configuration stanza from the configuration
        /// file represented by this instance.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current <see cref=
        /// "Configuration"/>.
        /// </param>
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the 
        /// stanza identified by <see cref="stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationSetting> UpdateSettingAsync(string stanzaName, string keyName, string value)
        {
            var resource = new ConfigurationSetting(this.Context, this.Namespace, this.ResourceName.Title, stanzaName, 
                keyName);
            await resource.UpdateAsync(value);
            return resource;
        }

        /// <summary>
        /// Asynchronously creates a configuration stanza in the configuration
        /// file represented by this instance.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to be created.
        /// </param>
        /// <returns>
        /// An object representing the newly created configuration stanza.
        /// </returns>
        public async Task<ConfigurationStanza> CreateStanzaAsync(string stanzaName)
        {
            var resource = new ConfigurationStanza(this.Context, this.Namespace, this.ResourceName.Title, stanzaName);
            await resource.CreateAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves a configuration stanza from the configuration
        /// file represented by this instance.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to be retrieved.
        /// </param>
        /// <returns>
        /// An object representing the configuration stanza identifed by <see 
        /// cref="stanzaName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">GET 
        /// properties/{file_name}/{stanza_name}</a> endpoint to construct the 
        /// <see cref="ConfigurationStanza"/> identified by <see cref=
        /// "stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationStanza> GetStanzaAsync(string stanzaName)
        {
            var resource = new ConfigurationStanza(this.Context, this.Namespace, this.ResourceName.Title, stanzaName);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously removes a configuration stanza from the configuration
        /// file represented by this instance.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current <see cref=
        /// "Configuration"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// stanza identified by <see cref="stanzaName"/>.
        /// </remarks>
        public async Task RemoveStanzaAsync(string stanzaName)
        {
            var resource = new ConfigurationStanza(this.Context, this.Namespace, this.ResourceName.Title, stanzaName);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates a configuration stanza from the configuration
        /// file represented by this instance.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current <see cref=
        /// "Configuration"/>.
        /// </param>
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the 
        /// stanza identified by <see cref="stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationStanza> UpdateStanzaAsync(string stanzaName)
        {
            var resource = new ConfigurationStanza(this.Context, this.Namespace, this.ResourceName.Title, stanzaName);
            await resource.UpdateAsync();
            return resource;
        }

        #endregion
    }
}
