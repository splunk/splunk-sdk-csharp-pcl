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
        /// An object identifying a Splunk service namespace.
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
        /// </exception>
        internal ServerMessage(Context context, Namespace @namespace, string name)
            : base(context, @namespace, ServerMessageCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ServerMessage"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public ServerMessage()
        { }

        #endregion

        #region Properties

        public ServerMessageSeverity Severity
        {
            get { return this.GetValue("Severity", EnumConverter<ServerMessageSeverity>.Instance); }
        }

        public string Text
        {
            get { return this.GetValue("Message", StringConverter.Instance); }
        }

        public long TimeCreatedEpochSecs
        {
            get { return this.GetValue("TimeCreatedEpochSecs", Int64Converter.Instance); }
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
            var resourceName = ServerMessageCollection.ClassResourceName;

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, name, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateSnapshotAsync(response);
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
    }
}
