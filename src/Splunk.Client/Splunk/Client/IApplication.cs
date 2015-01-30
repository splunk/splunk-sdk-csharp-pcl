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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to Splunk application entities.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    [ContractClass(typeof(IApplicationContract))]
    public interface IApplication : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets the application author.
        /// </summary>
        /// <value>
        /// The application author.
        /// </value>
        string ApplicationAuthor
        { get; }

        /// <summary>
        /// Gets the username of the splunk.com account for publishing the current
        /// <see cref="Application"/> to Splunkbase.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        string Author
        { get; }

        /// <summary>
        /// Gets a value that indicates whether Splunk should check Splunkbase for
        /// updates to the current <see cref="Application"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if check for updates, <c>false</c> if not.
        /// </value>
        bool CheckForUpdates
        { get; }

        /// <summary>
        /// Gets a value that indicates whether custom setup has been performed on
        /// the current <see cref="Application"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if configured, <c>false</c> if not.
        /// </value>
        bool Configured
        { get; }

        /// <summary>
        /// Gets the short explanatory string displayed underneath the title of the
        /// current <see cref="Application"/> in Launcher.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description
        { get; }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="Application"/>
        /// is disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if disabled, <c>false</c> if not.
        /// </value>
        bool Disabled
        { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the current
        /// <see cref= "Application"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the name of the current <see cref="Application"/> for display in the
        /// Splunk GUI and Launcher.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        string Label
        { get; }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        IReadOnlyDictionary<string, Uri> Links
        { get; }

        /// <summary>
        /// Gets a value that indicates whether objects contained in the current
        /// <see cref="Application"/> should be reloaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if refresh, <c>false</c> if not.
        /// </value>
        bool Refresh
        { get; }

        /// <summary>
        /// Gets a value that indicates whether changing the state of the current
        /// <see cref="Application"/> always requires restarting Splunk.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that a state change always requires a
        /// restart. A value of <c>false</c> indicates that modifying state may or
        /// may not require a restart depending on what state has been changed. A
        /// value of <c>false</c> does not indicate that a restart will never be
        /// required to effect a state change. State changes include enabling or
        /// disabling an <see cref="Application"/>.
        /// </remarks>
        /// <value>
        /// <c>true</c> if a state change requires a restart, <c>false</c> if not.
        /// </value>
        bool StateChangeRequiresRestart
        { get; }

        /// <summary>
        /// Gets the version string for the current <see cref="Application"/>.
        /// </summary>
        /// <remarks>
        /// Version strings are a number followed by a sequence of numbers or dots.
        /// Pre-release versions may append a space and a single-word suffix like
        /// "beta2".
        /// <example>Examples:</example>
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
        /// <value>
        /// The version.
        /// </value>
        string Version
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="Application"/>
        /// is visible and navigable from Splunk Web.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible, <c>false</c> if not.
        /// </value>
        bool Visible
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously disables the current <see cref="Application"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the POST apps/local/{name}/disable endpoint to disable
        /// the current <see cref="Application"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task DisableAsync();

        /// <summary>
        /// Asynchronously enables the current <see cref="Application"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the POST apps/local/{name}/enable endpoint to enable the
        /// current <see cref="Application"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task EnableAsync();

        /// <summary>
        /// Asynchronously retrieves setup information for the current
        /// <see cref="Application"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/mUT9gU">GET
        /// apps/local/{name}/setup</a> endpoint to construct the
        /// <see cref= "ApplicationSetupInfo"/> instance it returns.
        /// </remarks>
        /// <returns>
        /// An object containing setup information for the current
        /// <see cref= "Application"/>.
        /// </returns>
        Task<ApplicationSetupInfo> GetSetupInfoAsync();

        /// <summary>
        /// Asynchronously gets update information for the current
        /// <see cref= "Application"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/mrbtRj">GET
        /// apps/local/{name}/update</a> endpoint to construct the
        /// <see cref= "ApplicationUpdateInfo"/> instance it returns.
        /// </remarks>
        /// <returns>
        /// An object containing update information for the current
        /// <see cref= "Application"/>.
        /// </returns>
        Task<ApplicationUpdateInfo> GetUpdateInfoAsync();

        /// <summary>
        /// Asynchronously archives the current <see cref="Application"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/DJkT7S">GET
        /// apps/local/{name}/package</a> endpoint to create an archive of the
        /// current <see cref="Application"/>.
        /// </remarks>
        /// <returns>
        /// An object containing information about the newly created archive.
        /// </returns>
        Task<ApplicationArchiveInfo> PackageAsync();

        /// <summary>
        /// Asynchronously updates the attributes of the application represented by
        /// the current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dKraaR">POST
        /// apps/local/{name}</a> endpoint to update the attributes of the current
        /// <see cref="Application"/> and optionally check for updates on Splunkbase.
        /// </remarks>
        /// <param name="attributes">
        /// New attributes for the current <see cref="Application"/> instance.
        /// </param>
        /// <param name="checkForUpdates">
        /// A value of <c>true</c>, if Splunk should check Splunkbase for updates to
        /// the current <see cref="Application"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        Task<bool> UpdateAsync(ApplicationAttributes attributes, bool checkForUpdates = false);

        #endregion
    }

    /// <summary>
    /// An application contract.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.IApplication"/>
    [ContractClassFor(typeof(IApplication))]
    abstract class IApplicationContract : IApplication
    {
        #region Properties

        public abstract string ApplicationAuthor { get; }
        public abstract string Author { get; }
        public abstract bool CheckForUpdates { get; }
        public abstract bool Configured { get; }
        public abstract string Description { get; }
        public abstract bool Disabled { get; }
        public abstract Eai Eai { get; }
        public abstract string Label { get; }
        public abstract IReadOnlyDictionary<string, Uri> Links { get; }
        public abstract bool Refresh { get; }
        public abstract bool StateChangeRequiresRestart { get; }
        public abstract string Version { get; }
        public abstract bool Visible { get; }

        #endregion

        #region Methods

        public abstract Task DisableAsync();

        public abstract Task EnableAsync();

        public abstract Task GetAsync();

        public abstract Task<ApplicationSetupInfo> GetSetupInfoAsync();

        public abstract Task<ApplicationUpdateInfo> GetUpdateInfoAsync();

        public abstract Task<ApplicationArchiveInfo> PackageAsync();

        public abstract Task RemoveAsync();

        public abstract Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(IEnumerable<Argument> arguments);

        public Task<bool> UpdateAsync(ApplicationAttributes attributes, bool checkForUpdates)
        {
            Contract.Requires<ArgumentNullException>(attributes != null);
            return default(Task<bool>);
        }

        #endregion
    }
}
