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
    /// </list>
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
    public class Configuration : EntityCollection<ConfigurationStanza>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal Configuration(Service service, string fileName)
            : this(service.Context, service.Namespace, fileName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="fileName">
        /// Name of the configuration file to be represented by the current
        /// instance.
        /// </param>
        protected internal Configuration(Context context, Namespace ns, string fileName)
            : base(context, ns, new ResourceName(ConfigurationCollection.ClassResourceName, fileName))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <see cref="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal Configuration(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Configuration"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="Configuration"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Service.CreateConfigurationAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetConfigurationAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="Configuration"/>
        ///   file.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public Configuration()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new configuration stanza in the current 
        /// <see cref="Configuration"/>.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST 
        /// properties/{file_name}</a> endpoint to create the configuration
        /// stanza identified by <paramref name="stanzaName"/>.
        /// </remarks>
        public override async Task<ConfigurationStanza> CreateAsync(IEnumerable<Argument> arguments)
        {
            //// These gymnastics are required because:
            //// * The POST properties/{file_name} endpoint returns nothing in the
            ////   response body. 
            //// * This method is obliged to return a ConfigurationStanza and we can
            ////   do that, if we can do that without an extra round trip by probing
            ////   arguments for the stanza name.

            foreach (var argument in arguments)
            {
                if (argument.Name == "__stanza")
                {
                    await base.CreateAsync(arguments);
                    return new ConfigurationStanza(this.Context, this.Namespace, this.Name, argument.Name);
                }
            }

            //// We throw this exception instead of sending a request we know will fail due to the missing __stanza
            //// argument.

            throw new ArgumentException("The arguments list does not include '__stanza' as required by the POST properties/{file_name} endpoint.")
            {
                HelpLink = "http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTconfig#POST_properties.2F.7Bfile_name.7D"
            };
        }

        /// <summary>
        /// Asynchronously creates a new configuration stanza in the current 
        /// <see cref="Configuration"/>.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST 
        /// properties/{file_name}</a> endpoint to create the configuration
        /// stanza identified by <paramref name="stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationStanza> CreateAsync(string stanzaName)
        {
            var arguments = new Argument[] { new Argument("__stanza", stanzaName) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
            }

            return new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
        }

        /// <summary>
        /// Asynchronously retrieves a setting from a configuration stanza in
        /// the current <see cref="Configuration"/>.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in the current instance.
        /// </param>
        /// <param name="keyName">
        /// Name of a setting in the <see cref="ConfigurationStanza"/> identified
        /// by <paramref name="stanzaName"/>.
        /// </param>
        /// <returns>
        /// An object representing the configuration setting identified by <paramref 
        /// name="stanzaName"/> and <paramref name="keyName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET 
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint to 
        /// construct the <see cref="ConfigurationSetting"/> it returns.
        /// </remarks>
        public async Task<string> GetSettingAsync(string stanzaName, string keyName)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            var value = await resourceEndpoint.GetAsync(keyName);
            return value;
        }

        /// <summary>
        /// Asynchronously removes a configuration stanza from the current <see
        /// cref="Configuration">.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a configuration stanza to remove.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// stanza identified by <paramref name="stanzaName"/>.
        /// </remarks>
        public async Task RemoveAsync(string stanzaName)
        {
            var resource = new ConfigurationStanza(this.Context, this.Namespace, this.ResourceName.Title, stanzaName);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates a configuration stanza from the configuration
        /// file represented by the current instance.
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
        /// stanza identified by <paramref name="stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            await resourceEndpoint.UpdateAsync(settings);
            return resourceEndpoint;
        }

        /// <summary>
        /// Asynchronously updates a configuration stanza from the configuration
        /// file represented by the current <see cref="Configuration">.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of a <see cref="ConfigurationStanza"/> in the current <see 
        /// cref="Configuration"/>.
        /// </param>
        /// <param name="keyName">
        /// Name of a setting in the <see cref="ConfigurationStanza"/> identified
        /// by <paramref name="stanzaName"/>.
        /// </param>
        /// <param name="value">
        /// A string value for <paramref name="keyName"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the 
        /// setting identified by <paramref name="stanzaName"/> and <paramref 
        /// name="keyName"/>.
        /// </remarks>
        public async Task UpdateSettingAsync(string stanzaName, string keyName, string value)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            await resourceEndpoint.UpdateAsync(keyName, value);
        }

        #endregion
    }
}
