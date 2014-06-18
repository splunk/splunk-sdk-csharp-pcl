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
    using System.IO;

    /// <summary>
    /// Provides information about an application archive produced by Splunk.
    /// </summary>
    /// <remarks>
    /// You can produce an application archive using the <see cref=
    /// "Application.PackageAsync"/> method.
    /// </remarks>
    public class ApplicationArchiveInfo : Resource, IApplicationArchiveInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationArchiveInfo"/> 
        /// class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        internal ApplicationArchiveInfo(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ApplicationArchiveInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. The <see cref=
        /// "ApplicationArchiveInfo"/> class is an information object
        /// returned by these methods.
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Application.PackageAsync"/></term>
        ///   <description>
        ///   Asychronously packages the current Splunk application into an 
        ///   archive file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="ApplicationCollection.PackageApplicationAsync"/></term>
        ///   <description>
        ///   Asychronously packages the named Splunk application into an 
        ///   archive file.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ApplicationArchiveInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the access control lists for the current <see cref=
        /// "ApplicationArchiveInfo"/>.
        /// </summary>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets the name of the application contained by the archive represented 
        /// by the current <see cref="ApplicationArchiveInfo"/> instance.
        /// </summary>
        /// <remarks>
        /// This value is the default the name of the folder on disk that 
        /// contains the application.
        /// </remarks>
        public string ApplicationName
        {
            get { return this.Content.GetValue("Name", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the local path to the archive represented by the current <see 
        /// cref="ApplicationArchiveInfo"/> instance.
        /// </summary>
        public string Path
        {
            get { return this.Content.GetValue("Path", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether to to reload the objects contained 
        /// in the locally installed application.
        /// </summary>
        public bool Refresh
        {
            get { return this.Content.GetValue("Refresh", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the download URI for the archive represented by the current
        /// <see cref="ApplicationArchiveInfo"/> instance.
        /// </summary>
        public Uri Uri
        {
            get { return this.Content.GetValue("Url", UriConverter.Instance); }
        }

        #endregion
    }
}
