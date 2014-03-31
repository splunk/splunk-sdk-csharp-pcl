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

// TODO:
// [ ] Contracts
// [ ] Documentation
// [ ] Properties & Methods

namespace Splunk.Sdk
{
    using System.Net;
    using System.Threading.Tasks;

    public class StanzaInfo : Entity<StanzaInfo>
    {
        #region Constructors

        public StanzaInfo()
        { }

        #endregion

        #region Methods

        public void Create()
        {
            this.CreateAsync().Wait();
        }

        public async Task CreateAsync()
        {
            var args = new Argument[] { new Argument("__stanza", this.Title) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.Collection, args))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        public Stanza Get()
        {
            return this.GetAsync().Result;
        }

        public async Task<Stanza> GetAsync()
        {
            var stanza = new Stanza(this.Context, this.Namespace, this.ResourceName);
            await stanza.UpdateAsync();
            return stanza;
        }

        public void Remove()
        {
            this.RemoveAsync().Wait();
        }

        public async Task RemoveAsync()
        {
            var resource = new ResourceName(ResourceName.Configs, string.Join("-", "conf", this.Collection.Name));

            using (var response = await this.Context.DeleteAsync(this.Namespace, resource))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        public void UpdateSettings(params Argument[] arguments)
        {
            this.UpdateSettingsAsync(arguments).Wait();
        }

        public async Task UpdateSettingsAsync(params Argument[] arguments)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        #endregion
    }
}
