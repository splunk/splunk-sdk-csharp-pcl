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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk data indexes.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.EntityCollection{Splunk.Client.Index,Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IIndexCollection{Splunk.Client.Index}"/>
    public class IndexCollection : EntityCollection<Index, Resource>, IIndexCollection<Index>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal IndexCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal IndexCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal IndexCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "IndexCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public IndexCollection()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual ReadOnlyCollection<Message> Messages
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
        public virtual async Task<Index> CreateAsync(string name, IndexAttributes attributes = null, 
            string coldPath = null, string homePath = null, string thawedPath = null)
        {
            var arguments = new CreationArgs()
            {
                Name = name,
                ColdPath = coldPath,
                HomePath = homePath,
                ThawedPath = thawedPath
            }
            .AsEnumerable();

            if (attributes != null)
            {
                arguments = arguments.Concat(attributes);
            }

            return await this.CreateAsync(arguments).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task GetSliceAsync(Filter criteria)
        {
            await this.GetSliceAsync(criteria.AsEnumerable()).ConfigureAwait(false);
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Name of the class resource.
        /// </summary>
        internal static readonly ResourceName ClassResourceName = new ResourceName("data", "indexes");

        #endregion

        #region Types

        class CreationArgs : Args<CreationArgs>
        {
            [DataMember(Name = "name", IsRequired = true)]
            public string Name
            { get; set; }

            [DataMember(Name = "coldPath")]
            [DefaultValue("")]
            public string ColdPath
            { get; set; }

            [DataMember(Name = "homePath")]
            [DefaultValue("")]
            public string HomePath
            { get; set; }

            [DataMember(Name = "thawedPath")]
            [DefaultValue("")]
            public string ThawedPath
            { get; set; }
        }

        /// <summary>
        /// Provides arguments for retrieving an <see cref="IndexCollection"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/qVZ6wJ">REST API Reference: GET data/indexes</a>.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.IndexCollection.Filter}"/>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets the maximum number of <see cref="Index"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is <c>0</c>, then all available entries are
            /// returned. The default is <c>30</c>.
            /// </remarks>
            /// <value>
            /// The maximum number of <see cref="Index"/> entries to return.
            /// </value>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive)
            /// from which to begin returning <see cref="Index"/> entries.
            /// </summary>
            /// <remarks>
            /// This value is zero-based and cannot be negative. The default value is
            /// zero.
            /// </remarks>
            /// <value>
            /// Index of the first result (inclusive) from which to begin returning
            /// <see cref="Index"/> entries.
            /// </value>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref="Index"/>
            /// entries.
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on search
            /// <see cref="Index"/> properties. The default is <c>null</c>.
            /// </remarks>
            /// <value>
            /// A search expression to filter <see cref="Index"/> entries.
            /// </value>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort direction for
            /// <see cref="Index"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            /// <value>
            /// The sort direction for <see cref="Index"/> entries.
            /// </value>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Ascending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Index"/> property to use for sorting entries.
            /// </summary>
            /// <remarks>
            /// The default <see cref="Index"/> property to use for sorting is
            /// <c>"name"</c>.
            /// </remarks>
            /// <value>
            /// The <see cref="Index"/> property to use for sorting entries.
            /// </value>
            [DataMember(Name = "sort_key", EmitDefaultValue = false)]
            [DefaultValue("name")]
            public string SortKey
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort mode for <see cref= "Index"/>
            /// entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortMode"/>.Automatic.
            /// </remarks>
            /// <value>
            /// The sort mode for <see cref="Index"/> entries.
            /// </value>
            [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
            [DefaultValue(SortMode.Automatic)]
            public SortMode SortMode
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to leave out certain
            /// <see cref="Index"/> details in order to provide a faster
            /// response.
            /// </summary>
            /// <remarks>
            /// The default value is <c>false</c>.
            /// </remarks>
            /// <value>
            /// <c>true</c>, if certain <see cref="Index"/> details should be left out in
            /// order to provide a faster response.
            /// </value>
            [DataMember(Name = "summarize", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool Summarize
            { get; set; }
        }

        #endregion
    }
}
