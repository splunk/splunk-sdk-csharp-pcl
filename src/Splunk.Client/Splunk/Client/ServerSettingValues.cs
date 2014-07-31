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
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for updating Splunk <see cref="Server"/> setting
    /// values.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/OWTUws">REST API Reference: POST search/jobs</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.ServerSettingValues}"/>
    public sealed class ServerSettingValues : Args<ServerSettingValues>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to the default index for this instance of Splunk.
        /// </summary>
        /// <remarks>
        /// The default location is
        /// <c>"$SPLUNK_HOME/var/lib/splunk/defaultdb/db/"</c>.
        /// </remarks>
        /// <value>
        /// The splunk database.
        /// </value>
        [DataMember(Name = "SPLUNK_DB", EmitDefaultValue = false)]
        public string SplunkDB
        { get; set; }

        /// <summary>
        /// Gets or sets a toggles between HTTPS and HTTP.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> enables https and SSL for Splunk Web. A value of
        /// <c>false</c> enables http and disables SSL for Splunk Web.
        /// </remarks>
        /// <value>
        /// The enable splunk web ssl.
        /// </value>
        [DataMember(Name = "enableSplunkWebSSL", EmitDefaultValue = false)]
        public bool? EnableSplunkWebSsl
        { get; set; }

        /// <summary>
        /// Gets or sets the default hostname for data inputs that do not
        /// provide one.
        /// </summary>
        /// <value>
        /// The default hostname to use for data inputs that do not provide 
        /// one.
        /// </value>
        [DataMember(Name = "host", EmitDefaultValue = false)]
        public string Host
        { get; set; }

        /// <summary>
        /// Gets or sets the Specifies the port on which Splunk Web is listening for
        /// this instance of Splunk.
        /// </summary>
        /// <remarks>
        /// The default value is <c>8000</c>. Use the HTTPS port number if
        /// <see cref="EnableSplunkWebSsl"/> is <c>true</c>.
        /// </remarks>
        /// <value>
        /// The HTTP port.
        /// </value>
        [DataMember(Name = "httpport", EmitDefaultValue = false)]
        public int? HttpPort
        { get; set; }

        /// <summary>
        /// Gets or sets the port on which Splunk is listening for management
        /// operations.
        /// </summary>
        /// <remarks>
        /// The default value is <c>8089</c>.
        /// </remarks>
        /// <value>
        /// The management host port.
        /// </value>
        [DataMember(Name = "mgmtHostPort", EmitDefaultValue = false)]
        public int? ManagementHostPort
        { get; set; }

        /// <summary>
        /// Gets or sets a safe amount of space in megabytes that must exist for
        /// splunkd to continue operating.
        /// </summary>
        /// <remarks>
        /// This value affects search and indexing. Before attempting to launch a
        /// search, Splunk requires this amount of free space on the file system
        /// where the dispatch directory is located:
        /// <c> "$SPLUNK_HOME/var/run/splunk/dispatch</c>). It is applied
        /// similarly to the search quota values in <c>authorize.conf</c>
        /// and <c>limits.conf</c>. Splunk periodically checks space on all
        /// partitions that contain indexes as specified by <c>indexes.conf</c>. When
        /// you need to clear more disk space indexing is paused and Splunk posts a
        /// UI banner and warning.
        /// </remarks>
        /// <value>
        /// The minimum free space.
        /// </value>
        [DataMember(Name = "minFreeSpace", EmitDefaultValue = false)]
        public int MinFreeSpace
        { get; set; }

        /// <summary>
        /// Gets or sets the password string that is prefixed to the Splunk symmetric
        /// key, generating the final key to sign all traffic between master/slave
        /// licensers.
        /// </summary>
        /// <value>
        /// The pass 4 symmetric key.
        /// </value>
        [DataMember(Name = "pass4SymmKey", EmitDefaultValue = false)]
        public string Pass4SymmetricKey
        { get; set; }

        /// <summary>
        /// Gets or sets an ASCII String to use as the name of a Splunk instance for
        /// features such as distributed search.
        /// </summary>
        /// <remarks>
        /// The default value is &lt;![CDATA[&lt;hostname&gt;-&lt;user-running-
        /// splunk&gt;]]&gt;.
        /// </remarks>
        /// <value>
        /// The name of the server.
        /// </value>
        [DataMember(Name = "serverName", EmitDefaultValue = false)]
        public string ServerName
        { get; set; }

        /// <summary>
        /// Gets or sets a time range string to set the amount of time before a user
        /// session times out.
        /// </summary>
        /// <remarks>
        /// Express this value as a search-like time range. The default is <c>
        /// "1h"</c> (one hour).
        /// </remarks>
        /// <value>
        /// The session timeout.
        /// </value>
        /// <remarks>
        /// <example>Examples:</example>
        /// <code>
        /// args.SessionTimeout = "24h"; // 24 hours
        /// args.SessionTimeout = "3d";  // 3 days
        /// args.SessionTimeout = 7200s; // 7,200 seconds or two hours
        /// </code>        /// </remarks>
        [DataMember(Name = "sessionTimeout", EmitDefaultValue = false)]
        public string SessionTimeout
        { get; set; }

        /// <summary>
        /// Gets or sets a value that enables or disables Splunk Web.
        /// </summary>
        /// <remarks>
        /// Specify a value of <c>true</c> to enable Splunk Web.
        /// </remarks>
        /// <value>
        /// The start web server.
        /// </value>
        [DataMember(Name = "startwebserver", EmitDefaultValue = false)]
        public bool? StartWebServer
        { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the authenticating proxy.
        /// </summary>
        /// <remarks>
        /// The authenticating proxy is disabled by default. Set to a valid IP
        /// address to enable single signo on (SSO). A normal value is
        /// <c>"127.0.0.1"</c>.
        /// </remarks>
        /// <value>
        /// The trusted IP.
        /// </value>
        [DataMember(Name = "trustedIP", EmitDefaultValue = false)]
        public string TrustedIP
        { get; set; }
        
        #endregion
    }
}