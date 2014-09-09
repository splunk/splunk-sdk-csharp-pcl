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
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for managing Splunk applications.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/OsgrYx">Apps and add-ons: an introduction</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/1txQUG">Package your app or add-on</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/a7HqRp">REST API Reference: Applications</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Entity{Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IApplication"/>
    public class Application : Entity<Resource>, IApplication
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within
        /// <paramref name= "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal Application(Service service, string name)
            : this(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        protected internal Application(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// The name of the <see cref="Application"/>.
        /// </param>
        protected internal Application(Context context, Namespace ns, string name)
            : base(context, ns, ApplicationCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "Application"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public Application()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual string ApplicationAuthor
        {
            get { return this.Content.GetValue("Author", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Author
        {
            get { return this.GetValue("Author", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool CheckForUpdates
        {
            get { return this.Content.GetValue("CheckForUpdates", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool Configured
        {
            get { return this.Content.GetValue("Configured", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Description
        {
            get { return this.Content.GetValue("Description", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool Disabled
        {
            get { return this.Content.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Label
        {
            get { return this.Content.GetValue("Label", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IApplication.Links"/>
        public virtual IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.Snapshot.GetValue("Links"); }
        }

        /// <inheritdoc/>
        public virtual bool Refresh
        {
            get { return this.Content.GetValue("Refresh", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool StateChangeRequiresRestart
        {
            get { return this.Content.GetValue("StateChangeRequiresRestart", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Version
        {
            get { return this.Content.GetValue("Version", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool Visible
        {
            get { return this.Content.GetValue("Visible", BooleanConverter.Instance); }
        }
        
        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task DisableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "disable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task EnableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "enable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ApplicationSetupInfo> GetSetupInfoAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "setup");

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var resource = await BaseResource.CreateAsync<ApplicationSetupInfo>(response).ConfigureAwait(false);
                return resource;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ApplicationUpdateInfo> GetUpdateInfoAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "update");

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var resource = await BaseResource.CreateAsync<ApplicationUpdateInfo>(response).ConfigureAwait(false);
                return resource;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ApplicationArchiveInfo> PackageAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "package");

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var resource = await BaseResource.CreateAsync<ApplicationArchiveInfo>(response).ConfigureAwait(false);
                return resource;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(ApplicationAttributes attributes, bool checkForUpdates = false)
        {
            var updateArgs = new UpdateArgs { CheckForUpdates = checkForUpdates };
            return await this.UpdateAsync(updateArgs.AsEnumerable().Concat(attributes)).ConfigureAwait(false);
        }

        #endregion

        #region Types

        /// <summary>
        /// Arguments for update.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.Application.UpdateArgs}"/>
        class UpdateArgs : Args<UpdateArgs>
        {
            /// <summary>
            /// Gets or sets a value that indicates whether Splunk should check
            /// Splunkbase for updates to an <see cref="Application"/>.
            /// </summary>
            /// <value>
            /// <c>true</c> if check for updates, <c>false</c> if not.
            /// </value>
            [DataMember(Name = "check_for_updates", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool CheckForUpdates
            { get; set; }
        }

        #endregion
    }
}
