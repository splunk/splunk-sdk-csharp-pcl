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
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Provides an object representation of a Splunk server message resource.
    /// </summary>
    public sealed class ServerSettings : Entity<ServerSettings>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSettings"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/> or <see cref="namespace"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
        /// </exception>
        internal ServerSettings(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ServerSettings"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="ServerSettings"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Server.GetServerSettingsAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves <see cref="ServerSettings"/> from the 
        ///   Splunk server represented by the current instance.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.UpdateSettingsAsync"/></term>
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

        /// <summary>
        /// Gets the path to the default index for Splunk.
        /// </summary>
        public string SplunkDB
        {
            get { return this.GetValue("SplunkDb", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the path to the local installation of Splunk. 
        /// </summary>
        public string SplunkHome
        {
            get { return this.GetValue("SplunkHome", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether Splunk web is enabled for HTTPS
        /// or HTTP.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that Splunk Web is enabled for
        /// HTTPS and SSL. A value of <c>false</c> indicates that Splunk Web is
        /// enabled for HTTP and that SSL is disabled.
        /// </remarks>
        public bool EnableSplunkWebSsl
        {
            get { return this.GetValue("EnableSplunkWebSSL", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the default hostname to use for data inputs that do not 
        /// override this setting.
        /// </summary>
        public string Host
        {
            get { return this.GetValue("Host", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the port on which Splunk Web is listening.
        /// </summary>
        public int HttpPort
        {
            get { return this.GetValue("Httpport", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets or sets the port on which splunkd is listening for management 
        /// operations.
        /// </summary>
        public int ManagementHostPort
        {
            get { return this.GetValue("MgmtHostPort", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the amount of space in megabytes that must exist for splunkd 
        /// to continue operating.
        /// </summary>
        /// <remarks>
        /// This value affects search and indexing. Before attempting to launch
        /// a search, Splunk requires this amount of free space on the file 
        /// system where the dispatch directory is located:
        /// <c> "$SPLUNK_HOME/var/run/splunk/dispatch</c>). It is applied 
        /// similarly to the search quota values in <c>authorize.conf</c> 
        /// and <c>limits.conf</c>. Splunk periodically checks space on all 
        /// partitions that contain indexes as specified by <c>indexes.conf</c>. 
        /// When you need to clear more disk space indexing is paused and 
        /// Splunk posts a UI banner and warning.
        /// </remarks>
        public int MinFreeSpace
        {
            get { return this.GetValue("MinFreeSpace", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets or sets the password string that is prefixed to the Splunk 
        /// symmetric key, generating the final key to sign all traffic between
        /// master/slave licensers.
        /// </summary>
        public string Pass4SymmetricKey
        {
            get { return this.GetValue("Pass4SymmKey", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets or sets an ASCII String to use as the name of a Splunk instance
        /// for features such as distributed search.
        /// </summary>
        /// <remarks>
        /// The default value is <![CDATA[<hostname>-<user-running-splunk>]]>.
        /// </remarks>
        public string ServerName
        {
            get { return this.GetValue("ServerName", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the amount of time before a user session times out.
        /// </summary>
        /// <remarks>
        /// This value is expressed as a search-like time range.
        /// <example>Examples</example>
        /// <code>
        /// args.SessionTimeout = "24h"; // 24 hours
        /// args.SessionTimeout = "3d"; // 3 days
        /// args.SessionTimeout = 7200s; // 7,200 seconds or two hours
        /// </code>
        /// </remarks>
        public string SessionTimeout
        {
            get { return this.GetValue("SessionTimeout", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether Splunk Web is enabled.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that Splunk Web is enabled.
        /// </remarks>
        public bool StartWebServer
        {
            get { return this.GetValue("Startwebserver", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the IP address of the authenticating proxy.
        /// </summary>
        /// <remarks>
        /// If the authentication proxy is disabled, a value of <c>null</c> is
        /// returned.
        /// </remarks>
        public string TrustedIP
        {
            get { return this.GetValue("TrustedIP", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously updates the server settings represented by the 
        /// current instance.
        /// </summary>
        /// <param name="values">
        /// An object representing the updated server setting values.
        /// </param>
        /// <returns>
        /// </returns>
        public async Task UpdateAsync(ServerSettingValues values)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, values))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        #endregion

        #region Privates/internals

        static readonly ResourceName ClassResourceName = new ResourceName("server", "settings", "settings");

        #endregion
    }
}
