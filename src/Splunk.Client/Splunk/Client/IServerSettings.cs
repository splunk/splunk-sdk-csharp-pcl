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

//// TODO
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    /// <summary>
    /// Provides an operational interface the Splunk server settings resource.
    /// </summary>
    /// <seealso cref="T:IBaseResource"/>
    public interface IServerSettings : IBaseResource
    {
        /// <summary>
        /// Gets the extensible administration interface properties for the Splunk
        /// server instance.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the path to the default index for Splunk.
        /// </summary>
        /// <value>
        /// The splunk database.
        /// </value>
        string SplunkDB
        { get; }

        /// <summary>
        /// Gets the path to the local installation of Splunk.
        /// </summary>
        /// <value>
        /// The splunk home.
        /// </value>
        string SplunkHome
        { get; }

        /// <summary>
        /// Gets a value that indicates whether Splunk web is enabled for HTTPS or
        /// HTTP.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that Splunk Web is enabled for HTTPS and
        /// SSL. A value of <c>false</c> indicates that Splunk Web is enabled for
        /// HTTP and that SSL is disabled.
        /// </remarks>
        /// <value>
        /// <c>true</c> if enable splunk web ssl, <c>false</c> if not.
        /// </value>
        bool EnableSplunkWebSsl
        { get; }

        /// <summary>
        /// Gets the default hostname to use for data inputs that do not override
        /// this setting.
        /// </summary>
        /// <value>
        /// The default hostname.
        /// </value>
        string Host
        { get; }

        /// <summary>
        /// Gets the port on which Splunk Web is listening.
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        int HttpPort
        { get; }

        /// <summary>
        /// Gets the port on which splunkd is listening for management operations.
        /// </summary>
        /// <value>
        /// The management host port.
        /// </value>
        int ManagementHostPort
        { get; }

        /// <summary>
        /// Gets the amount of space in megabytes that must exist for splunkd to
        /// continue operating.
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
        int MinFreeSpace
        { get; }

        /// <summary>
        /// Gets the password string that is prefixed to the Splunk symmetric key,
        /// generating the final key to sign all traffic between master/slave
        /// licensers.
        /// </summary>
        /// <value>
        /// The pass 4 symmetric key.
        /// </value>
        string Pass4SymmetricKey
        { get; }

        /// <summary>
        /// Gets an ASCII String to use as the name of a Splunk instance for features
        /// such as distributed search.
        /// </summary>
        /// <remarks>
        /// The default value is &lt;![CDATA[&lt;hostname&gt;-&lt;user-running-
        /// splunk&gt;]]&gt;.
        /// </remarks>
        /// <value>
        /// The name of the server.
        /// </value>
        string ServerName
        { get; }

        /// <summary>
        /// Gets the amount of time before a user session times out.
        /// </summary>
        /// <value>
        /// The session timeout.
        /// </value>
        /// <remarks>
        /// This value is expressed as a search-like time range.
        /// <example>Examples:</example>
        /// <code>
        /// args.SessionTimeout = "24h"; // 24 hours
        /// args.SessionTimeout = "3d";  // 3 days
        /// args.SessionTimeout = 7200s; // 7,200 seconds or two hours
        /// </code>
        /// </remarks>
        string SessionTimeout
        { get; }

        /// <summary>
        /// Gets a value indicating whether Splunk Web is enabled.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that Splunk Web is enabled.
        /// </remarks>
        /// <value>
        /// <c>true</c> if start web server, <c>false</c> if not.
        /// </value>
        bool StartWebServer
        { get; }

        /// <summary>
        /// Gets the IP address of the authenticating proxy.
        /// </summary>
        /// <remarks>
        /// If the authentication proxy is disabled, a value of <c>null</c> is
        /// returned.
        /// </remarks>
        /// <value>
        /// The trusted IP.
        /// </value>
        string TrustedIP
        { get; }
    }
}
