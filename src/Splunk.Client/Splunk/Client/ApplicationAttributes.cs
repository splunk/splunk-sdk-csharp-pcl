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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for creating and updating the attributes
    /// of <see cref="Application"/> resources.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/dKraaR">REST API Reference: POST 
    ///   apps/local</a>
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/dKraaR">REST API Reference: POST 
    ///   apps/local/{name}</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class ApplicationAttributes : Args<ApplicationAttributes>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAttributes"/> 
        /// class.
        /// </summary>
        public ApplicationAttributes()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the username of the splunk.com account for publishing
        /// an application to Splunkbase.
        /// </summary>
        [DataMember(Name = "author", EmitDefaultValue = false)]
        public string ApplicationAuthor
        { get; set; }

        /// <summary>
        /// Gets a value that indicates whether Splunk should check Splunkbase
        /// for updates to an <see cref="Application"/>.
        /// </summary>
        [DataMember(Name = "check_for_updates", EmitDefaultValue = false)]
        public bool? CheckForUpdates
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether custom setup has been 
        /// performed on an <see cref="Application"/>.
        /// </summary>
        [DataMember(Name = "configured", EmitDefaultValue = false)]
        public bool? Configured
        { get; set; }

        /// <summary>
        /// Gets or sets the short explanatory string displayed underneath the 
        /// title of an <see cref="Application"/> in Launcher.
        /// </summary>
        /// <remarks>
        /// Short descriptions of about 200 characters are most effective.
        /// </remarks>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        { get; set; }

        /// <summary>
        /// Gets or sets the name of an <see cref="Application"/> for display 
        /// in the Splunk GUI and Launcher.
        /// </summary>
        /// <remarks>
        /// Splunk recommends lengths between five and eighty characters. Values
        /// must not include "Splunk For" as a prefix.
        /// <example>Examples of good labels:</example>
        /// <code>
        /// "IMAP Monitor"
        /// "SQL Server Integration Services"
        /// "FISMA Compliance"
        /// </code>
        /// </remarks>
        public string Label
        { get; set; }

        /// <summary>
        /// Gets or sets the version string for an <see cref="Applcation"/>.
        /// </summary>
        /// <remarks>
        /// Version strings are a number followed by a sequence of numbers or 
        /// dots. Pre-release versions can append a space and a single-word 
        /// suffix like "beta2". Each release of an application must change the
        /// version number. 
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
        { get; set; }

        /// <summary>
        /// Gets or sets  a value that indicates if an <see cref="Application"/> 
        /// is visible and navigable from Splunk Web.
        /// </summary>
        /// <remarks>
        /// Visible apps require at least one view that is available from 
        /// Splunk Web.
        /// </remarks>
        public bool Visible
        { get; set; }

        #endregion
    }
}
