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

    /// <summary>
    /// Provides a class for accessing Splunk system messages.
    /// </summary>
    /// <remarks>
    /// Most messages are created by splunkd to inform the user of system 
    /// problems. Splunk Web typically displays these as bulletin board 
    /// messages.
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/w3Rmjp">REST API: messages</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class ServerMessageCollection : EntityCollection<ServerMessageCollection, ServerMessage>
    {
        #region Constructors

        /// <summary>
        /// Intializes a new instance of the <see cref="ServerMessageCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="args">
        /// Optional arguments for retrieving the <see cref="ServerMessageCollection"/>.
        /// </param>
        internal ServerMessageCollection(Context context, Namespace ns, ServerMessageCollectionArgs args = null)
            : base(context, ns, ClassResourceName, args)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ServerMessageCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Server.GetMessagesAsync"/> to asynchronously retrieve the collection
        /// of messages from a Splunk server.
        /// </remarks>
        public ServerMessageCollection()
        { }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("messages");

        #endregion
    }
}
