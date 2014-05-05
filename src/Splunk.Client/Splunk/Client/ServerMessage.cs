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
    using System.Runtime.Serialization;
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
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
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
        internal ServerMessage(Context context, Namespace ns, string name)
            : base(context, ns, ServerMessageCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ServerMessage"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="ServerMessage"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Server.CreateMessageAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="ServerMessage"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Server.GetMessageAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="ServerMessage"/>.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ServerMessage()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ServerMessageSeverity Severity
        {
            get { return this.GetValue("Severity", EnumConverter<ServerMessageSeverity>.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return this.GetValue("Message", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long TimeCreatedEpochSecs
        {
            get { return this.GetValue("TimeCreatedEpochSecs", Int64Converter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task CreateAsync(ServerMessageSeverity type, string text)
        {
            var resourceName = ServerMessageCollection.ClassResourceName;

            var args = new CreationArgs
            {
                Name = this.Name,
                Type = type,
                Text = text
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Types

        /// <summary>
        /// Provides arguments for creating a new <see cref="ServerMessage"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/WlDoZx">REST API Reference: POST messages</a>.
        /// </description></item>
        /// </list>
        /// </remarks>
        public class CreationArgs : Args<CreationArgs>
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name of a <see cref="ServerMessage"/>.
            /// </summary>
            [DataMember(Name = "name", IsRequired = true)]
            public string Name
            { get; set; }

            /// <summary>
            /// Gets or sets the type of a <see cref="ServerMessage"/>.
            /// </summary>
            [DataMember(Name = "severity", IsRequired = true)]
            public ServerMessageSeverity Type
            { get; set; }

            /// <summary>
            /// Gets or sets the text of a <see cref="ServerMessage"/>.
            /// </summary>
            [DataMember(Name = "value", IsRequired = true)]
            public string Text
            { get; set; }

            #endregion
        }

        #endregion
    }
}
