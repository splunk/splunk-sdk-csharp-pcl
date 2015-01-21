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
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk server.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Endpoint"/>
    /// <seealso cref="T:Splunk.Client.IServer"/>
    public class Server : Endpoint, IServer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal Server(Service service)
            : this(service.Context, service.Namespace)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

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

        /// <inheritdoc/>
        Context IServer.Context { get; set; }
        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task<ServerInfo> GetInfoAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, Info).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader).ConfigureAwait(false);
                var info = new ServerInfo(feed);
                
                return info;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ServerSettings> GetSettingsAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, Settings).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader).ConfigureAwait(false);
                var settings = new ServerSettings(feed);

                return settings;
            }
        }

        /// <inheritdoc/>
        public virtual async Task RestartAsync(int millisecondsDelay = 60000, int retryInterval = 250)
        {
            var info = await this.GetInfoAsync().ConfigureAwait(false);
            var startupTime = info.StartupTime;

            using (var response = await this.Context.PostAsync(this.Namespace, Restart).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
            }

            this.Context.SessionKey = null; // because this session is now or shortly will be gone

            if (millisecondsDelay == 0)
            {
                return;
            }

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(millisecondsDelay);
                var token = cancellationTokenSource.Token;

                for (int i = 0; !token.IsCancellationRequested ; i++)
                {
                    try
                    {
                        info = await this.GetInfoAsync().ConfigureAwait(false);

                        if (startupTime < info.StartupTime)
                        {
                            return;
                        }
                    }
                    catch (RequestException)
                    {
                        //// Because the server may return a failure code on the way up or down
                    }
                    catch (WebException e) 
                    {
                        //// Because the HttpClient that Mono 3.4 depends on is known to throw a WebException on 
                        //// connection failure

                        if (e.Status != WebExceptionStatus.ConnectFailure) 
                        {
                            throw new HttpRequestException(e.Message, e);
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        //// Because Microsoft's HttpClient code always throws an HttpRequestException

                        var innerException = e.InnerException as WebException;

                        if (innerException == null || innerException.Status != WebExceptionStatus.ConnectFailure)
                        {
                            throw;
                        }
                    }

                    await Task.Delay(millisecondsDelay: retryInterval).ConfigureAwait(false);
                }

                throw new OperationCanceledException();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ServerSettings> UpdateSettingsAsync(ServerSettingValues values)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, Settings, values).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader).ConfigureAwait(false);
                var settings = new ServerSettings(feed);

                return settings;
            }
        }

        #endregion

        #region Privates/internals

        #pragma warning disable 1591

        protected internal static readonly ResourceName ClassResourceName = new ResourceName("server");
        protected internal static readonly ResourceName Info = new ResourceName("server", "info");
        protected internal static readonly ResourceName Restart = new ResourceName("server", "control", "restart");
        protected internal static readonly ResourceName Settings = new ResourceName("server", "settings", "settings");

        #pragma warning restore 1591

        ServerMessageCollection messages;

        #endregion
    }
}
