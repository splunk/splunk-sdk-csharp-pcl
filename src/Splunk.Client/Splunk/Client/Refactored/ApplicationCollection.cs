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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk applications.
    /// </summary>
    public class ApplicationCollection : EntityCollection<Application>, IApplicationCollection<Application>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal ApplicationCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollection"/> 
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
        protected internal ApplicationCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollection"/> 
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
        protected internal ApplicationCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ApplicationCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetApplicationsAsync"/> to asynchronously retrieve a 
        /// collection of installed Splunk applications.
        /// </remarks>
        public ApplicationCollection()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IReadOnlyList<Message> Messages
        {
            get { return this.Snapshot.GetValue("Messages") ?? NoMessages; }
        }

        /// <inheritdoc/>
        public Pagination Pagination
        {
            get { return this.Snapshot.GetValue("Pagination") ?? Pagination.None; }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public async Task<Application> CreateAsync(string template, ApplicationAttributes attributes = null)
        {
            var resourceName = ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = this.Title,
                Filename = false,
                Name = this.Title,
                Template = template
            };

            var resourceEndpoint = await this.CreateAsync(attributes == null ? args : args.Concat(attributes));
            return resourceEndpoint;
        }

        /// <inheritdoc/>
        public virtual async Task GetSliceAsync(Filter criteria)
        {
            await this.GetSliceAsync(criteria.AsEnumerable());
        }

        /// <inheritdoc/>
        public async Task<Application> InstallAsync(string path, string name = null, bool update = false)
        {
            var resourceName = ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = this.Title,
                Filename = true,
                Name = path,
                Update = update
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                
                var resourceEndpoint = await ResourceEndpoint.CreateAsync<Application>(this.Context, response);
                return resourceEndpoint;
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("apps", "local");

        #endregion

        #region Types

        class CreationArgs : Args<CreationArgs>
        {
            [DataMember(Name = "explicit_appname", IsRequired = true)]
            public string ExplicitApplicationName
            { get; set; }

            [DataMember(Name = "filename", IsRequired = true)]
            public bool? Filename
            { get; set; }

            [DataMember(Name = "name", IsRequired = true)]
            public string Name
            { get; set; }

            [DataMember(Name = "template", EmitDefaultValue = true)]
            public string Template
            { get; set; }

            [DataMember(Name = "update", EmitDefaultValue = false)]
            public bool? Update
            { get; set; }
        }

        class UpdateArgs : Args<UpdateArgs>
        {
            /// <summary>
            /// Gets a value that indicates whether Splunk should check Splunkbase
            /// for updates to an <see cref="Application"/>.
            /// </summary>
            [DataMember(Name = "check_for_updates", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool CheckForUpdates
            { get; set; }
        }

        /// <summary>
        /// Provides selection criteria for retrieving a slice of an <see cref=
        /// "ApplicationCollection"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/pqZJco">REST API: GET apps/local</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        public sealed class Filter : Args<Filter>
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value specifying the maximum number of <see cref=
            /// "Application"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is set to zero, then all <see cref=
            /// "Application"/> entries are returned. The default value is 30.
            /// </remarks>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive) from 
            /// which to begin returning entries.
            /// </summary>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. 
            /// The default value is zero.
            /// </remarks>
            /// <remarks>
            /// This value is zero-based and cannot be negative. The default value
            /// is zero.
            /// </remarks>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to scan for new <see cref=
            /// "Application"/> instances and reload any objects those new <see 
            /// cref="Application"/> instances contain.
            /// </summary>
            /// <remarks>
            /// The default is <c>false</c>.
            /// </remarks>
            [DataMember(Name = "refresh", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool Refresh // TODO: Verify default value (it's not in the docs)
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref="Application"/> 
            /// entries. 
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on <see
            /// cref="Application"/> properties.
            /// </remarks>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search // TODO: Good search example for App
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to sort returned <see cref=
            /// "Application"/>entries in ascending or descending order.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Ascending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the <see cref="SortMode"/> for <see
            /// cref="Application"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortMode"/>.Automatic.
            /// </remarks>
            [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
            [DefaultValue(SortMode.Automatic)]
            public SortMode SortMode
            { get; set; }

            #endregion
        }

        #endregion
    }
}
