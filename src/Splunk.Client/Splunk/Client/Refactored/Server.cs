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
//// [ ] Server.RestartAsync should post a restart_required message followed by
////     a request to restart the server. It should then poll the server until
////     the restart_required message goes away.

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
    public class Server : Endpoint
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Receiver"/> class
        /// </summary>
        /// <param name="service">
        /// An object representing a Splunk service endpoint.
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
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a <see cref="ServerMessage"/> on the Splunk 
        /// server represented by the current instance.
        /// </summary>
        /// <param name="name">
        /// Name of the message to create.
        /// </param>
        /// <param name="severity">
        /// Message severity level.
        /// </param>
        /// <param name="text">
        /// Text of the message.
        /// </param>
        /// <returns>
        /// An object representing the message created.
        /// </returns>
        public async Task<ServerMessage> CreateMessageAsync(string name, ServerMessageSeverity severity, string text)
        {
            var resource = new ServerMessage(this.Context, this.Namespace, name);
            await resource.CreateAsync(severity, text);
            return resource;
        }

        /// <summary>
        /// Asynchronously gets <see cref="ServerInfo"/> from the Splunk server
        /// represented by the current instance.
        /// </summary>
        /// <returns>
        /// An object representing information about the Splunk server.
        /// </returns>
        public async Task<ServerInfo> GetInfoAsync()
        {
            var resource = new ServerInfo(this.Context, this.Namespace);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously gets a <see cref="ServerMessage"/> from the Splunk 
        /// server represented by the current instance.
        /// </summary>
        /// <param name="name">
        /// Name of the message to get.
        /// </param>
        /// <returns>
        /// An object representing the server message identified by <paramref name=
        /// "name"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        public async Task<ServerMessage> GetMessageAsync(string name)
        {
            var resource = new ServerMessage(this.Context, this.Namespace, name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="ServerMessageCollection"/> 
        /// from the Splunk server represented by the current instance.
        /// </summary>
        /// <returns>
        /// An object representing the collection of server messages.
        /// </returns>
        public async Task<ServerMessageCollection> GetMessagesAsync()
        {
            var resource = new ServerMessageCollection(this.Context, this.Namespace);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves <see cref="ServerSettings"/> from the 
        /// Splunk server represented by the current instance.
        /// </summary>
        /// <returns>
        /// An object representing the server settings from the Splunk server
        /// represented by the current instance.
        /// </returns>
        public async Task<ServerSettings> GetSettingsAsync()
        {
            var resource = new ServerSettings(this.Context, this.Namespace);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Removes a <see cref="ServerMessage"/> from the Splunk server 
        /// represented by the current instance.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="ServerMessage"/> to remove.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        public async Task RemoveMessageAsync(string name)
        {
            var resource = new ServerMessage(this.Context, this.Namespace, name);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Restarts the Splunk server represented by the current instance 
        /// and then optionally checks for a specified period of time for 
        /// server availability.
        /// </summary>
        /// <param name="millisecondsDelay">
        /// The time to wait before canceling the check for server availability.
        /// The default value is <c>60000</c> specifying that the check for
        /// server avaialability will continue for up to 60 seconds. A value
        /// of <c>0</c> specifices that no check should be made. A value of 
        /// <c>-1</c> specifies an infinite wait time.
        /// </param>
        /// <param name="retryInterval">
        /// The time to wait between checks for server availability in 
        /// milliseconds. The default value is <c>250</c> specifying that the
        /// time between checks for server availability is one quarter of a 
        /// second.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsDelay"/> is less than <c>-1</c>.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The restart operation failed.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// The check for server availability was canceled after <paramref 
        /// name="millisecondsDelay"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Insufficient privileges to restart the current <see cref="Server"/>.
        /// </exception>
        public async Task RestartAsync(int millisecondsDelay = 60000, int retryInterval = 250)
        {
            Contract.Requires<ArgumentOutOfRangeException>(millisecondsDelay >= -1);

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

        /// <summary>
        /// Asynchronously updates <see cref="ServerSettings"/> on the Splunk 
        /// server represented by the current instance.
        /// </summary>
        /// <param name="values">
        /// An object representing the updated server setting values.
        /// </param>
        /// <returns>
        /// An object representing the updated server settings on the Splunk 
        /// server represented by the current instance.
        /// </returns>
        public async Task<ServerSettings> UpdateSettingsAsync(ServerSettingValues values)
        {
            var resource = new ServerSettings(this.Context, this.Namespace);
            await resource.UpdateAsync(values);
            return resource;
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("server", "control");
        internal static readonly ResourceName Restart = new ResourceName(ClassResourceName, "restart");

        #endregion
    }
}
