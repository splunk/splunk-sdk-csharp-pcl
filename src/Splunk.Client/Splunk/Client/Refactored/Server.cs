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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Provides an object representation of a Splunk server.
    /// </summary>
    public class Server : Endpoint, IServer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Receiver"/> class
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        protected internal Server(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
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
        internal Server(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        {
            this.messages = new ServerMessageCollection(context, ns);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual ServerMessageCollection Messages
        {
            get { return this.messages; }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task<ServerInfo> GetInfoAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, Info))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);
                var info = new ServerInfo(feed);
                
                return info;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ServerSettings> GetSettingsAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, Settings))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);
                var settings = new ServerSettings(feed);

                return settings;
            }
        }

        /// <inheritdoc/>
        public virtual async Task RestartAsync(int millisecondsDelay = 60000, int retryInterval = 250)
        {
            var info = await this.GetInfoAsync();
            var startupTime = info.StartupTime;

            using (var response = await this.Context.PostAsync(this.Namespace, Restart))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }

            if (millisecondsDelay == 0)
            {
                return;
            }

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(millisecondsDelay);
                var token = cancellationTokenSource.Token;

                for (int i = 0; ; i++)
                {
                    try
                    {
                        info = await this.GetInfoAsync();

                        if (startupTime < info.StartupTime)
                        {
                            this.Context.SessionKey = null;
                            return;
                        }
                    }
                    catch (RequestException)
                    {
                        // because the server may return a failure code on the way up or down
                    }
                    catch (HttpRequestException e)
                    {
                        var innerException = e.InnerException as WebException;

                        if (innerException == null || innerException.Status != WebExceptionStatus.ConnectFailure)
                        {
                            throw;
                        }
                    }

                    await Task.Delay(millisecondsDelay: retryInterval);
                }
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ServerSettings> UpdateSettingsAsync(ServerSettingValues values)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, Settings, values))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);
                var settings = new ServerSettings(feed);

                return settings;
            }
        }

        #endregion

        #region Privates/internals

        protected internal static readonly ResourceName ClassResourceName = new ResourceName("server");
        protected internal static readonly ResourceName Info = new ResourceName("server", "info");
        protected internal static readonly ResourceName Settings = new ResourceName("server", "settings");
        protected internal static readonly ResourceName Restart = new ResourceName("server", "control", "restart");

        ServerMessageCollection messages;

        #endregion
    }
}
