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
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <inheritdoc/>
    public class ServerMessageCollection : EntityCollection<ServerMessage, Resource>, 
        IServerMessageCollection<ServerMessage>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessageCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal ServerMessageCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessageCollection"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <see cref="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal ServerMessageCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

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
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal ServerMessageCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
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

        #region Properties

        /// <inheritdoc/>
        public virtual IReadOnlyList<Message> Messages
        {
            get { return this.Snapshot.GetValue("Messages") ?? NoMessages; }
        }

        /// <inheritdoc/>
        public virtual Pagination Pagination
        {
            get { return this.Snapshot.GetValue("Pagination") ?? Pagination.None; }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task<ServerMessage> CreateAsync(string name, ServerMessageSeverity type, string text)
        {
            var args = new CreationArgs
            {
                Name = name,
                Type = type,
                Text = text
            };

            return await this.CreateAsync(args.AsEnumerable());
        }

        /// <inheritdoc/>
        public virtual async Task GetSliceAsync(Filter criteria)
        {
            await this.GetSliceAsync(criteria.AsEnumerable());
        }

        #endregion

        #region Privates/internals

        protected internal static readonly ResourceName ClassResourceName = new ResourceName("messages");

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
        class CreationArgs : Args<CreationArgs>
        {
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
        }

        /// <summary>
        /// Provides arguments for retrieving a <see cref="ServerMessageCollection"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/qVZ6wJ">REST API Reference: GET data/indexes</a>.
        /// </description></item>
        /// </list>
        /// </remarks>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets the maximum number of <see cref="ServerMessage"/>
            /// entries to return.
            /// </summary>
            /// <value>
            /// The maximum number of <see cref="ServerMessage"/> entries to
            /// return.
            /// </value>
            /// <remarks>
            /// If the value of <c>Count</c> is <c>0</c>, then all available
            /// entries are returned. The default is <c>30</c>.
            /// </remarks>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive) 
            /// from which to begin returning <see cref="ServerMessage"/> 
            /// entries.
            /// </summary>
            /// <value>
            /// Index of the first result (inclusive) from which to begin
            /// returning <see cref="ServerMessage"/> entries.
            /// </value>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. 
            /// The default value is zero.
            /// </remarks>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref="ServerMessage"/> 
            /// entries. 
            /// </summary>
            /// <value>
            /// A search expression to filter <see cref="ServerMessage"/> 
            /// entries.
            /// </value>
            /// <remarks>
            /// Use this expression to filter the entries returned based on 
            /// search <see cref="ServerMessage"/> properties. The default is 
            /// <c>null</c>.
            /// </remarks>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort direction for <see
            /// cref="ServerMessage"/> entries.
            /// </summary>
            /// <value>
            /// The sort direction for <see cref="ServerMessage"/> entries.
            /// </value>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Ascending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="ServerMessage"/> property to use 
            /// for sorting entries.
            /// </summary>
            /// <value>
            /// The <see cref="ServerMessage"/> property to use for sorting 
            /// entries.
            /// </value>
            /// <remarks>
            /// The default <see cref="ServerMessage"/> property to use for 
            /// sorting is <c>"name"</c>.
            /// </remarks>
            [DataMember(Name = "sort_key", EmitDefaultValue = false)]
            [DefaultValue("name")]
            public string SortKey
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort mode for <see cref=
            /// "ServerMessage"/> entries.
            /// </summary>
            /// <value>
            /// The sort mode for <see cref="ServerMessage"/> entries.
            /// </value>
            /// <remarks>
            /// The default value is <see cref="SortMode"/>.Automatic.
            /// </remarks>
            [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
            [DefaultValue(SortMode.Automatic)]
            public SortMode SortMode
            { get; set; }
        }

        #endregion
    }
}
