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

    /// <summary>
    /// Provides an object representation of the settings of a Splunk server.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Resource"/>
    /// <seealso cref="T:Splunk.Client.IServerSettings"/>
    public class ServerSettings : Resource, IServerSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSettings"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal ServerSettings(AtomEntry entry, Version generatorVersion)
            : base(entry, generatorVersion)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSettings"/> class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal ServerSettings(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ServerSettings"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code. Use one of these methods to obtain a
        /// <see cref="ServerSettings"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Server.GetSettingsAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves <see cref="ServerSettings"/> from the Splunk
        ///   server represented by the current instance.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Server.UpdateSettingsAsync"/></term>
        ///   <description>
        ///   Asynchronously updates <see cref="ServerSettings"/> on the Splunk
        ///   server represented by the current instance.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ServerSettings()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string SplunkDB
        {
            get { return this.Content.GetValue("SPLUNKDB", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string SplunkHome
        {
            get { return this.Content.GetValue("SPLUNKHOME", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool EnableSplunkWebSsl
        {
            get { return this.Content.GetValue("EnableSplunkWebSSL", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Host
        {
            get { return this.Content.GetValue("Host", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual int HttpPort
        {
            get { return this.Content.GetValue("Httpport", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual int ManagementHostPort
        {
            get { return this.Content.GetValue("MgmtHostPort", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual int MinFreeSpace
        {
            get { return this.Content.GetValue("MinFreeSpace", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Pass4SymmetricKey
        {
            get { return this.Content.GetValue("Pass4SymmKey", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string ServerName
        {
            get { return this.Content.GetValue("ServerName", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string SessionTimeout
        {
            get { return this.Content.GetValue("SessionTimeout", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool StartWebServer
        {
            get { return this.Content.GetValue("Startwebserver", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string TrustedIP
        {
            get { return this.Content.GetValue("TrustedIP", StringConverter.Instance); }
        }

        #endregion
    }
}
