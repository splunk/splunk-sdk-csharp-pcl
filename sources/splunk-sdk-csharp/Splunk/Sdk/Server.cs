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

namespace Splunk.Sdk
{
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public struct Server
    {
        #region Constructors

        internal Server(Service service)
        {
            this.service = service;
        }

        #endregion

        #region Methods

        public ServerInfo GetInfo()
        {
            return this.GetInfoAsync().Result;
        }

        public async Task<ServerInfo> GetInfoAsync()
        {
            var serverInfo = new ServerInfo(this.service);
            await serverInfo.GetAsync();
            return serverInfo;
        }

        public void Restart()
        {
            this.RestartAsync().Wait();
        }

        public async Task RestartAsync()
        {
            using (var response = await this.service.Context.PostAsync(this.service.Namespace, ServerControlRestart))
            {
                if (!response.Message.IsSuccessStatusCode)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        #endregion

        #region Privates

        static readonly ResourceName ServerControlRestart = new ResourceName("server", "control", "restart");
        readonly Service service;

        #endregion
    }
}
