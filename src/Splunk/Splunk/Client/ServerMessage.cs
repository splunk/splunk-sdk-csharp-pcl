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
// [O] Property accessors should not throw, but return default value if the underlying field is undefined (?)

namespace Splunk.Client
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class ServerMessage : Entity<ServerMessage>
    {
        #region Constructors

        public ServerMessage(Context context, Namespace @namespace, string name)
            : base(context, @namespace, new ResourceName(ClassResourceName, name))
        { }

        public ServerMessage()
        { }

        #endregion

        #region Properties

        public ServerMessageType Severity
        {
            get { return this.Content.GetValue("Severity", EnumConverter<ServerMessageType>.Instance); }
        }

        public string Text
        {
            get { return this.Content.GetValue("Message", StringConverter.Instance); }
        }

        public long TimeCreatedEpochSecs
        {
            get { return this.Content.GetValue("TimeCreatedEpochSecs", Int64Converter.Instance); }
        }

        #endregion

        #region Methods

        public async Task CreateAsync(ServerMessageType type, string text)
        {
            await CreateAsync(new ServerMessageArgs(type, text));
        }

        public async Task CreateAsync(ServerMessageArgs args)
        {
            var name = new Argument[] { new Argument("name", this.Name) };

            using (var response = await this.Context.PostAsync(this.Namespace, ClassResourceName, name, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count != 1)
                {
                    throw new InvalidDataException();  // TODO: Diagnostics
                }

                this.Data = new DataCache(feed.Entries[0]);
            }
        }

        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("messages");

        #endregion
    }
}
