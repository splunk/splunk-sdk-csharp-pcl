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
//// [X] Contracts
//// [O] Documentation
//// [ ] Seal all Args types

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk storage
    /// passwords.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.EntityCollection{Splunk.Client.StoragePassword,Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IStoragePasswordCollection{Splunk.Client.StoragePassword}"/>
    public class StoragePasswordCollection : EntityCollection<StoragePassword, Resource>, 
        IStoragePasswordCollection<StoragePassword>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePasswordCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal StoragePasswordCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePasswordCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        ///
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal StoragePasswordCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Intializes an new instance of the <see cref="StoragePasswordCollection"/>
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
        protected internal StoragePasswordCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "StoragePasswordCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public StoragePasswordCollection()
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
        public virtual async Task<StoragePassword> CreateAsync(string password, string username, string realm = null)
        {
            var arguments = new CreationArgs
            {
                Password = password,
                Username = username,
                Realm = realm
            };

            return await this.CreateAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<StoragePassword> GetAsync(string username, string realm = null)
        {
            var passwordName = StoragePassword.CreateNameFromRealmAndUsername(realm, username);
            var password = await base.GetAsync(passwordName).ConfigureAwait(false);

            return password;
        }

        /// <inheritdoc/>
        public virtual async Task<StoragePassword> GetOrNullAsync(string username, string realm = null)
        {
            var passwordName = StoragePassword.CreateNameFromRealmAndUsername(realm, username);
            var password = await base.GetOrNullAsync(passwordName).ConfigureAwait(false);

            return password;
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
        internal static readonly ResourceName ClassResourceName = new ResourceName("storage", "passwords");

        #endregion

        #region Types

        /// <summary>
        /// Provides the arguments required for creating <see cref="StoragePassword"/>
        /// resources.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/JgyIeN">REST API Reference: POST
        ///   storage/passwords</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.StoragePasswordCollection.CreationArgs}"/>
        class CreationArgs : Args<CreationArgs>
        {
            /// <summary>
            /// Gets or sets the password for a <see cref="StoragePassword"/>.
            /// </summary>
            /// <value>
            /// The password.
            /// </value>
            [DataMember(Name = "password", IsRequired = true)]
            public string Password
            { get; set; }

            /// <summary>
            /// Gets or sets the realm in which a <see cref="StoragePassword"/> is valid.
            /// </summary>
            /// <value>
            /// The realm.
            /// </value>
            [DataMember(Name = "realm", EmitDefaultValue = false)]
            public string Realm
            { get; set; }

            /// <summary>
            /// Gets or sets the username for a <see cref="StoragePassword"/>.
            /// </summary>
            /// <value>
            /// The username.
            /// </value>
            [DataMember(Name = "name", IsRequired = true)]
            public string Username
            { get; set; }
        }

        /// <summary>
        /// Provides arguments for retrieving a
        /// <see cref="StoragePasswordCollection"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///     <a href="http://goo.gl/pqZJco">REST API: GET apps/local</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.StoragePasswordCollection.Filter}"/>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets a value specifying the maximum number of
            /// <see cref="StoragePassword"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is set to zero, then all
            /// <see cref="StoragePassword"/> entries are returned. The default value is
            /// <c>30</c>.
            /// </remarks>
            /// <value>
            /// A value specifying the maximum number of <see cref= "StoragePassword"/>
            /// entries to return.
            /// </value>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive)
            /// from which to begin returning entries.
            /// </summary>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. The
            /// default value is zero.
            /// </remarks>
            /// <value>
            /// A value specifying the first result (inclusive) from which to begin
            /// returning entries.
            /// </value>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref= "StoragePassword"/>
            /// entries.
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on
            /// <see cref="StoragePassword"/> properties.
            /// </remarks>
            /// <value>
            /// A search expression to filter <see cref="StoragePassword"/>
            /// entries.
            /// </value>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search // TODO: Good search example
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort direction for
            /// <see cref="StoragePassword"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            /// <value>
            /// The sort direction for <see cref="StoragePassword"/> entries.
            /// </value>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Ascending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="StoragePassword"/> property to use for
            /// sorting entries.
            /// </summary>
            /// <remarks>
            /// The default <see cref="StoragePassword"/> property to use for sorting is
            /// <c>"name"</c>.
            /// </remarks>
            /// <value>
            /// The <see cref="StoragePassword"/> property to use for sorting entries.
            /// </value>
            [DataMember(Name = "sort_key", EmitDefaultValue = false)]
            [DefaultValue("name")]
            public string SortKey
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the sort mode for
            /// <see cref= "StoragePassword"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortMode"/>.Automatic.
            /// </remarks>
            /// <value>
            /// The sort mode for <see cref="StoragePassword"/> entries.
            /// </value>
            [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
            [DefaultValue(SortMode.Automatic)]
            public SortMode SortMode
            { get; set; }
        }

        #endregion
    }
}
