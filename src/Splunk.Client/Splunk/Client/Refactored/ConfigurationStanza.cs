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
//// [X] Properties & Methods

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk configuration stanza.
    /// </summary>
    public class ConfigurationStanza : Entity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within <paramref name="fileName"/> containing the 
        /// configuration stanza to be represented by the current instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/>, or <paramref name="stanzaName"/> are 
        /// <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal ConfigurationStanza(Service service, string fileName, string stanzaName)
            : this(service.Context, service.Namespace, fileName, stanzaName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/>
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
        /// Name of a stanza within <paramref name="fileName"/> containing the 
        /// configuration stanza to be represented by the current instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> or <paramref name="stanzaName"/> are 
        /// <c>null</c> or <paramref name="ns"/> is non-specific. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        protected internal ConfigurationStanza(Context context, Namespace ns, string fileName, string stanzaName)
            : base(context, ns, new ResourceName(ConfigurationCollection.ClassResourceName, fileName, stanzaName))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/> 
        /// class.
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
        protected internal ConfigurationStanza(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationStanza"/> class.
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
        ///   <term><see cref="Configuration.CreateStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.GetStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.UpdateStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously updates an existing <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.CreateConfigurationStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a <see cref="ConfigurationStanza"/> 
        ///   identified by configuration file and stanza name.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetConfigurationStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves a <see cref="ConfigurationStanza"/> 
        ///   identified by configuration file and stanza name.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ConfigurationStanza()
        { }

        #endregion

        #region Methods

        #region Operational interface

        /// <summary>
        /// Asynchronously retrieves a configuration setting value from the 
        /// current <see cref="ConfigurationStanza"/>
        /// </summary>
        /// <param name="keyName">
        /// The name of a configuration setting.
        /// </param>
        /// <returns>
        /// The string value of <paramref name="keyName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// construct the <see cref="ConfigurationSetting"/> identified by 
        /// <paramref name="keyName"/>.
        /// </remarks>
        public async Task<string> GetAsync(string keyName)
        {
            var resourceName = new ResourceName(this.ResourceName, keyName);

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var reader = new StreamReader(response.Stream);
                var value = await reader.ReadToEndAsync();

                return value;
            }
        }

        /// <summary>
        /// Asynchronously updates the value of an existing setting in the 
        /// current <see cref="ConfigurationStanza"/>.
        /// </summary>
        /// <param name="keyName">
        /// The name of a configuration setting in the current <see cref=
        /// "ConfigurationStanza"/>.
        /// </param>
        /// <param name="value">
        /// A new value for the setting identified by <paramref name="keyName"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sSzcMy">POST 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// update the <see cref="ConfigurationSetting"/> identified by <paramref 
        /// name="keyName"/>.
        /// </remarks>
        public async Task UpdateAsync(string keyName, string value)
        {
            var resourceName = new ResourceName(this.ResourceName, keyName);
            var arguments = new Argument[] { new Argument("value", value) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Infrastructure methods

        /// <inheritdoc/>
        protected override void ReconstructSnapshot(AtomFeed feed)
        {
            this.Snapshot = new Resource(feed);
        }

        /// <inheritdoc/>
        protected override void ReconstructSnapshot(Resource resource)
        {
            this.Snapshot = resource;
        }

        #endregion
        
        #endregion
    }
}
