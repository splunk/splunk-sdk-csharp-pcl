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
//// [ ] Documentation
//// [X] Property accessors should not throw, but return default value if the underlying field is undefined (?)
////     Guaranteed across the code base by ExpandoAdapter.GetValue.

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Provides an object representation of a Splunk server message resource.
    /// </summary>
    public sealed class ServerMessage : Entity<ServerMessage>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessage"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="name">
        /// The name of the <see cref="ServerMessage"/>
        /// </param>
        /// <exception cref="ArgumentException">
        /// <see cref="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/> or <see cref="namespace"/> are <c>null</c>.
        /// </exception>
        public ServerMessage(Context context, Namespace @namespace, string name)
            : base(context, @namespace, new ResourceName(ClassResourceName, name))
        { }

        public ServerMessage()
        { }

        #endregion

        #region Properties

        public ServerMessageSeverity Severity
        {
            get { return this.Content.GetValue("Severity", EnumConverter<ServerMessageSeverity>.Instance); }
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

        public async Task CreateAsync(ServerMessageSeverity type, string text)
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
