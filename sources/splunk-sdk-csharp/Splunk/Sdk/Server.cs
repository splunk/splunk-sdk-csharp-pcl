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

    public class Server : Resource<Server>
    {
        #region Constructors

        internal Server(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

        public Server()
        { }

        #endregion

        #region Methods

        public async Task<ServerInfo> GetInfoAsync()
        {
            var serverInfo = new ServerInfo(this.Context, this.Namespace);
            await serverInfo.GetAsync();
            return serverInfo;
        }

        public async Task<ServerMessageCollection> GetMessagesAsync()
        {
            var serverMessages = new ServerMessageCollection(this.Context, this.Namespace);
            await serverMessages.GetAsync();
            return serverMessages;
        }

        public async Task RestartAsync()
        {
            using (var response = await this.Context.PostAsync(this.Namespace, new ResourceName(this.ResourceName, "restart")))
            {
                if (!response.Message.IsSuccessStatusCode)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("server", "control");

        #endregion
    }
}
