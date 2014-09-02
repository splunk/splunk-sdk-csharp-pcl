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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk server message entity.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Entity{Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IServerMessage"/>
    public class ServerMessage : Entity<Resource>, IServerMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessage"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within
        /// <paramref name= "service"/>.<see cref="Namespace"/>.
        /// </param>
        ///
        /// ### <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        protected internal ServerMessage(Service service, string name)
            : this(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessage"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        ///
        /// ### <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// ### <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal ServerMessage(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

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
        /// The name of the <see cref="ServerMessage"/>.
        /// </param>
        ///
        /// ### <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// ### <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// ### <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal ServerMessage(Context context, Namespace ns, string name)
            : base(context, ns, ServerMessageCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ServerMessage"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public ServerMessage()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public ServerMessageSeverity Severity
        {
            get { return this.Content.GetValue("Severity", EnumConverter<ServerMessageSeverity>.Instance); }
        }

        /// <inheritdoc/>
        public string Text
        {
            get { return this.Content.GetValue("Message", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public DateTime TimeCreated
        {
            get { return this.Content.GetValue("TimeCreatedEpochSecs", UnixDateTimeConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Unsupported. This method is not supported by the
        /// <see cref= "ServerMessage"/> class because it is not supported by the
        /// <a href= "http://goo.gl/S13BE0">Splunk messages/{name} endpoint</a>.
        /// </summary>
        /// <param name="arguments">
        /// A variable-length parameters list containing arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        public override async Task<bool> UpdateAsync(params Argument[] arguments)
        {
            return await this.UpdateAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsupported. This method is not supported by the
        /// <see cref= "ServerMessage"/> class because it is not supported by the
        /// <a href= "http://goo.gl/S13BE0">Splunk messages/{name} endpoint</a>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown when the requested operation is not supported.
        /// </exception>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        public override Task<bool> UpdateAsync(IEnumerable<Argument> arguments)
        {
            throw new NotSupportedException("The Splunk messages/{name} endpoint does not provide an update method.")
            {
                HelpLink = "http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTsystem#messages.2F.7Bname.7D"
            };
        }

        #endregion
    }
}
