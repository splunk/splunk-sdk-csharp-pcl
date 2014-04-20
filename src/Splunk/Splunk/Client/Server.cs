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

namespace Splunk.Client
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class Server : Entity<Server>
    {
        #region Constructors

        internal Server(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

        public Server()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ServerMessage> CreateMessageAsync(string name, ServerMessageType type, string text)
        {
            var resource = new ServerMessage(this.Context, this.Namespace, name);
            await resource.CreateAsync(type, text);
            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public async Task<ServerInfo> GetInfoAsync()
        {
            var resource = new ServerInfo(this.Context, this.Namespace);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ServerMessage> GetMessageAsync(string name)
        {
            var resource = new ServerMessage(this.Context, this.Namespace, name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<ServerMessageCollection> GetMessagesAsync()
        {
            var resource = new ServerMessageCollection(this.Context, this.Namespace);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task RemoveMessageAsync(string name)
        {
            var resource = new ServerMessage(this.Context, this.Namespace, name);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsDelay">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="AuthenticationFailureException">
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// </exception>
        public async Task RestartAsync(int millisecondsDelay = 0)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, new ResourceName(this.ResourceName, "restart")))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
            
            if (millisecondsDelay <= 0)
            {
                return;
            }

            //// Wait until the server goes down

            for (int i = 0; ; i++)
            {
                try
                {
                    using (var response = await this.Context.GetAsync(this.Namespace, ClassResourceName))
                    {
                        await Task.Delay(millisecondsDelay: 500);
                    }
                }
                catch (HttpRequestException)
                {
                    break;
                }
            }

            //// Wait for millisecondsDelay for the server to come up

            this.Context.SessionKey = null; // We're no longer authenticated

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(millisecondsDelay);
                var token = cancellationTokenSource.Token;

                for (int i = 0; ; i++)
                {
                    try
                    {
                        using (var response = await this.Context.GetAsync(this.Namespace, ClassResourceName, token))
                        {
                            await response.EnsureStatusCodeAsync(HttpStatusCode.Unauthorized);
                            break;
                        }
                    }
                    catch (HttpRequestException)
                    {
                        continue;
                    }
                }
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("server", "control");

        #endregion
    }
}
