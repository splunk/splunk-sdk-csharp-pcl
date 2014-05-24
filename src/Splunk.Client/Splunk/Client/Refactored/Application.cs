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
    public class Application : Entity
    {
        #region Constructors

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
        internal Application(Context context, Namespace ns, string name)
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

        /// <summary>
        /// Gets the username of the splunk.com account for publishing the
        /// current <see cref="Application"/> to Splunkbase.
        /// </summary>
        public string ApplicationAuthor
        {
            get { return this.GetValue("Author", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether Splunk should check Splunkbase
        /// for updates to the current <see cref="Application"/>.
        /// </summary>
        public bool CheckForUpdates
        {
            get { return this.GetValue("CheckForUpdates", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether custom setup has been performed
        /// on the current <see cref="Application"/>.
        /// </summary>
        public bool Configured
        {
            get { return this.GetValue("Configured", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the short explanatory string displayed underneath the title of
        /// the current <see cref="Application"/> in Launcher.
        /// </summary>
        public string Description
        {
            get { return this.GetValue("Description", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="Application"/>
        /// is disabled.
        /// </summary>
        public bool Disabled
        {
            get { return this.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the current <see cref=
        /// "Application"/>.
        /// </summary>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets the name of the current <see cref="Application"/> for display 
        /// in the Splunk GUI and Launcher.
        /// </summary>
        public string Label
        {
            get { return this.GetValue("Label", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether objects contained in the 
        /// current <see cref="Application"/> should be reloaded.
        /// </summary>
        public bool Refresh
        {
            get { return this.GetValue("Refresh", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether changing the state of the 
        /// current <see cref="Application"/> always requires restarting 
        /// Splunk.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that a state change always 
        /// requires a restart. A value of <c>false</c> indicates that modifying 
        /// state may or may not require a restart depending on what state
        /// has been changed. A value of <c>false</c> does not indicate that a 
        /// restart will never be required to effect a state change. State 
        /// changes include enabling or disabling an <see cref="Application"/>.
        /// </remarks>
        public bool StateChangeRequiresRestart
        {
            get { return this.GetValue("StateChangeRequiresRestart", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the version string for the current <see cref="Application"/>.
        /// </summary>
        /// <remarks>
        /// Version strings are a number followed by a sequence of numbers or 
        /// dots. Pre-release versions may append a space and a single-word 
        /// suffix like "beta2".
        /// <example>Examples</example>
        /// <code>
        /// "1.2"
        /// "11.0.34"
        /// "2.0 beta"
        /// "1.3 beta2"
        /// "1.0 b2"
        /// "12.4 alpha"
        /// "11.0.34.234.254"
        /// </code>
        /// </remarks>
        public string Version
        {
            get { return this.GetValue("Version", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="Application"/> 
        /// is visible and navigable from Splunk Web.
        /// </summary>
        public bool Visible
        {
            get { return this.GetValue("Visible", BooleanConverter.Instance); }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates the current <see cref="Application"/>.
        /// instance.
        /// </summary>
        /// <param name="template">
        /// 
        /// </param>
        /// <param name="attributes">
        /// 
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to create the current <see cref=
        /// "Application"/>.
        /// </remarks>
        public async Task CreateAsync(string template, ApplicationAttributes attributes = null)
        {
            var resourceName = ApplicationCollection.ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = this.Name,
                Filename = false,
                Name = this.Name,
                Template = template
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously disables the current <see cref="Application"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the POST apps/local/{name}/disable endpoint to 
        /// disable the current <see cref="Application"/>.
        /// </remarks>
        public async Task DisableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "disable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously enables the current <see cref="Application"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the POST apps/local/{name}/enable endpoint to 
        /// enable the current <see cref="Application"/>.
        /// </remarks>
        public async Task EnableAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "enable");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously retrieves setup information for the current <see 
        /// cref="Application"/>.
        /// </summary>
        /// <returns>
        /// An object containing setup information for the current <see cref=
        /// "Application"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/mUT9gU">GET 
        /// apps/local/{name}/setup</a> endpoint to construct the <see cref=
        /// "ApplicationSetupInfo"/> instance it returns.
        /// </remarks>
        public async Task<ApplicationSetupInfo> GetSetupInfoAsync()
        {
            var resource = new ApplicationSetupInfo(this.Context, this.Namespace, this.Name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously gets update information for the current <see cref=
        /// "Application"/>.
        /// </summary>
        /// <returns>
        /// An object containing update information for the current <see cref=
        /// "Application"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/mrbtRj">GET 
        /// apps/local/{name}/update</a> endpoint to construct the <see cref=
        /// "ApplicationUpdateInfo"/> instance it returns.
        /// </remarks>
        public async Task<ApplicationUpdateInfo> GetUpdateInfoAsync()
        {
            var resource = new ApplicationUpdateInfo(this.Context, this.Namespace, this.Name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously installs an application from a Splunk application
        /// archive file.
        /// </summary>
        /// <param name="path">
        /// Specifies the location of a Splunk application archive file.
        /// </param>
        /// <param name="update">
        /// <c>true</c> if Splunk should allow the installation to update an
        /// existing application. The default value is <c>false</c>.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to install the application from the archive
        /// file on <paramref name="path"/>.
        /// </remarks>
        public async Task InstallAsync(string path, bool update = false)
        {
            var resourceName = ApplicationCollection.ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = this.Name,
                Filename = true,
                Name = path,
                Update = update
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously archives the current <see cref="Application"/>.
        /// </summary>
        /// <returns>
        /// An object containing information about the newly created archive.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/DJkT7S">GET 
        /// apps/local/{name}/package</a> endpoint to create an archive of the 
        /// current <see cref="Application"/>.
        /// </remarks>
        public async Task<ApplicationArchiveInfo> PackageAsync()
        {
            var resource = new ApplicationArchiveInfo(this.Context, this.Namespace, this.Name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously updates the attributes of the application represented 
        /// by the current instance.
        /// </summary>
        /// <param name="attributes">
        /// New attributes for the current <see cref="Application"/> instance.
        /// </param>
        /// <param name="checkForUpdates">
        /// A value of <c>true</c>, if Splunk should check Splunkbase for 
        /// updates to the current <see cref="Application"/> instance.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dKraaR">POST 
        /// apps/local/{name}</a> endpoint to update the attributes of the 
        /// current <see cref="Application"/> and optionally check for
        /// updates on Splunkbase.
        /// </remarks>
        public async Task UpdateAsync(ApplicationAttributes attributes, bool checkForUpdates = false)
        {
            var arguments = new Argument[] { new Argument("check_for_updates", checkForUpdates) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

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

        #endregion
    }
}
