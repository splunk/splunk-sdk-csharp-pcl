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
//// [O] Properties & Methods

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.ComponentModel;
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
    public class Application : Entity, IApplication
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        protected internal Application(Service service, string name)
            : this(service.Context, service.Namespace, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
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
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal Application(Context context, Namespace ns, string name)
            : base(context, ns, ApplicationCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Application"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain an <see cref="Application"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Service.CreateApplicationAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new Splunk application from a template.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetApplicationAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing Splunk application.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.InstallApplicationAsync"/></term>
        ///   <description>
        ///   Asynchronously installs a new Splunk application from an archive 
        ///   file.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public Application()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string ApplicationAuthor
        {
            get { return this.GetValue("Author", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool CheckForUpdates
        {
            get { return this.GetValue("CheckForUpdates", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool Configured
        {
            get { return this.GetValue("Configured", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public string Description
        {
            get { return this.GetValue("Description", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool Disabled
        {
            get { return this.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public string Label
        {
            get { return this.GetValue("Label", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool Refresh
        {
            get { return this.GetValue("Refresh", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool StateChangeRequiresRestart
        {
            get { return this.GetValue("StateChangeRequiresRestart", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public string Version
        {
            get { return this.GetValue("Version", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public bool Visible
        {
            get { return this.GetValue("Visible", BooleanConverter.Instance); }
        }
        
        #endregion

        #region Methods

        /// <inheritdoc/>
        public async Task DisableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "disable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <inheritdoc/>
        public async Task EnableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "enable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <inheritdoc/>
        public async Task<ApplicationSetupInfo> GetSetupInfoAsync()
        {
            var resource = new ApplicationSetupInfo(this.Context, this.Namespace, this.Title);
            await resource.GetAsync();
            return resource;
        }

        /// <inheritdoc/>
        public async Task<ApplicationUpdateInfo> GetUpdateInfoAsync()
        {
            var resource = new ApplicationUpdateInfo(this.Context, this.Namespace, this.Title);
            await resource.GetAsync();
            return resource;
        }

        /// <inheritdoc/>
        public async Task<ApplicationArchiveInfo> PackageAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "package");

            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var resource = await Resource.CreateAsync<ApplicationArchiveInfo>(response);
                return resource;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(ApplicationAttributes attributes, bool checkForUpdates = false)
        {
            var arguments = new Argument[] { new Argument("check_for_updates", checkForUpdates) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion
    }
}
