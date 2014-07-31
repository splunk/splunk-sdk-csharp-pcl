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
//// [X] Documentation

namespace Splunk.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides arguments for setting the attributes of an
    /// <see cref= "Application"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/dKraaR">REST API Reference: POST apps/local</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/dKraaR">REST API Reference: POST
    ///   apps/local/{name}</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.ApplicationAttributes}"/>
    public sealed class ApplicationAttributes : Args<ApplicationAttributes>
    {
        /// <summary>
        /// Gets or sets the username of the splunk.com account for publishing an
        /// application to Splunkbase.
        /// </summary>
        /// <value>
        /// Username of the splunk.com account for publishing an application to
        /// Splunkbase.
        /// </value>
        [DataMember(Name = "author", EmitDefaultValue = false)]
        public string ApplicationAuthor
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether custom setup has been
        /// performed on an application.
        /// </summary>
        /// <value>
        /// <c>true</c>, if custom setup has been performed on the application;
        /// otherwise <c>false</c>.
        /// </value>
        [DataMember(Name = "configured", EmitDefaultValue = false)]
        public bool? Configured
        { get; set; }

        /// <summary>
        /// Gets or sets the short explanatory string displayed underneath the title
        /// of an application in Launcher.
        /// </summary>
        /// <remarks>
        /// Short descriptions of about 200 characters are most effective.
        /// </remarks>
        /// <value>
        /// A short explanatory string that is displayed underneath the title of the
        /// application in Launcher.
        /// </value>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        { get; set; }

        /// <summary>
        /// Gets or sets the name of an application for display in the Splunk GUI and
        /// Launcher.
        /// </summary>
        /// <value>
        /// The name of the application for display in the Splunk GUI and Launcher.
        /// </value>
        /// <remarks>
        /// Splunk recommends lengths between five and eighty characters. Values must
        /// not include "Splunk For" as a prefix.
        /// <example>Examples of good labels:</example>
        /// <code>
        /// "IMAP Monitor"
        /// "FISMA Compliance"
        /// "SQL Server Integration Services"
        /// </code>
        /// </remarks>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        public string Label
        { get; set; }

        /// <summary>
        /// Gets or sets the version string for an application.
        /// </summary>
        /// <remarks>
        /// Version strings are a number followed by a sequence of numbers or dots.
        /// Pre-release versions can append a space and a single-word suffix like
        /// "beta2". Each release of an application must change the version number.
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
        /// An application version string.
        /// </value>
        [DataMember(Name = "version", EmitDefaultValue = false)]
        public string Version
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if an <see cref="Application"/>
        /// is visible and navigable from Splunk Web.
        /// </summary>
        /// <remarks>
        /// Visible apps require at least one view that is available from Splunk Web.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the application is visible and navigable from Splunk Web;
        /// otherwise <c>false</c>.
        /// </value>
        [DataMember(Name = "visible", EmitDefaultValue = false)]
        public bool? Visible
        { get; set; }
    }
}
