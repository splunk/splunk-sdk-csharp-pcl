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
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
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
    /// Most configuration files come packaged with your Splunk installation and
    /// can be found in <c>$SPLUNK_HOME/etc/default</c>.
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
    /// <seealso cref="T:Splunk.Client.EntityCollection{Splunk.Client.ConfigurationStanza,Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IConfiguration{Splunk.Client.ConfigurationStanza}"/>
    public class Configuration : EntityCollection<ConfigurationStanza, Resource>, IConfiguration<ConfigurationStanza>
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
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

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
        /// Name of the configuration file to be represented by the current instance.
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
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal Configuration(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "Configuration"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public Configuration()
        { }

        #endregion

        #region Methods

        /// <inheritdoc/>
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
                    await base.CreateAsync(arguments).ConfigureAwait(false);
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

        /// <inheritdoc/>
        public virtual async Task<ConfigurationStanza> CreateAsync(string stanzaName)
        {
            var arguments = new Argument[] { new Argument("__stanza", stanzaName) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created).ConfigureAwait(false);
            }

            return new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
        }

        /// <inheritdoc/>
        public virtual async Task<string> GetSettingAsync(string stanzaName, string keyName)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            var value = await resourceEndpoint.GetAsync(keyName).ConfigureAwait(false);
            return value;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAsync(string stanzaName)
        {
            var resource = new ConfigurationStanza(this.Context, this.Namespace, this.ResourceName.Title, stanzaName);
            await resource.RemoveAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<ConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            await resourceEndpoint.UpdateAsync(settings).ConfigureAwait(false);
            return resourceEndpoint;
        }

        /// <inheritdoc/>
        public virtual async Task UpdateSettingAsync(string stanzaName, string keyName, object value)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            await resourceEndpoint.UpdateAsync(keyName, value).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task UpdateSettingAsync(string stanzaName, string keyName, string value)
        {
            var resourceEndpoint = new ConfigurationStanza(this.Context, this.Namespace, this.Name, stanzaName);
            await resourceEndpoint.UpdateAsync(keyName, value).ConfigureAwait(false);
        }

        #endregion
    }
}
