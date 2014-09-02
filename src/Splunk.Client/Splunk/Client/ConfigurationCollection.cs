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
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk configuration
    /// files.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.EntityCollection{Splunk.Client.Configuration,Splunk.Client.ResourceCollection}"/>
    /// <seealso cref="T:Splunk.Client.IConfigurationCollection{Splunk.Client.Configuration,Splunk.Client.ConfigurationStanza}"/>
    public class ConfigurationCollection : EntityCollection<Configuration, ResourceCollection>, 
        IConfigurationCollection<Configuration, ConfigurationStanza>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal ConfigurationCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/>
        /// class.
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
        /// <exception cref="System.IO.InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal ConfigurationCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal ConfigurationCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ConfigurationCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public ConfigurationCollection()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a configuration file.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7">POST properties</a>
        /// endpoint to create the configuration file represented by this instance.
        /// </remarks>
        /// <param name="arguments">
        /// A sequence of arguments to the configuration endpoint.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public override async Task<Configuration> CreateAsync(IEnumerable<Argument> arguments)
        {
            //// We override this method because the "POST properties" endpoint returns nothing.

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created).ConfigureAwait(false);
                
                var fileName = arguments.First(arg => arg.Name == "__conf").Value;
                var configuration = new Configuration(this.Context, this.Namespace, fileName);

                return configuration;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<Configuration> CreateAsync(string fileName)
        {
            var arguments = new Argument[] { new Argument("__conf", fileName) };
            return await this.CreateAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<ConfigurationStanza> GetAsync(string fileName, string stanzaName)
        {
            var stanza = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await stanza.GetAsync().ConfigureAwait(false);
            return stanza;
        }

        /// <summary>
        /// Unsupported. This method is not supported by the
        /// <see cref= "ConfigurationCollection"/> class because it is not supported
        /// by the <a href="http://goo.gl/Unj6fs">Splunk properties endpoint</a>.
        /// </summary>
        /// <param name="arguments">
        /// A variable-length parameters list containing arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public override async Task GetSliceAsync(params Argument[] arguments)
        {
            await this.GetSliceAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsupported. This method is not supported by the
        /// <see cref= "ConfigurationCollection"/> class because it is not supported
        /// by the <a href="http://goo.gl/Unj6fs">Splunk properties endpoint</a>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown when the requested operation is not supported.
        /// </exception>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public override Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            throw new NotSupportedException("The Splunk properties endpoint can only return the full list of configuration files.")
            {
                HelpLink = "http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTconfig#GET_properties"
            };
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Name of the class resource.
        /// </summary>
        internal static readonly ResourceName ClassResourceName = new ResourceName("properties");

        /// <summary>
        /// The configs.
        /// </summary>
        internal static readonly ResourceName Configs = new ResourceName("configs");

        #endregion
    }
}
