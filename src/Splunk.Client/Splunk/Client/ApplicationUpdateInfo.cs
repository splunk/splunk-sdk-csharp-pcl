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
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class ApplicationUpdateInfo : Resource
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUpdateInfo"/> 
        /// class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        internal ApplicationUpdateInfo(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ApplicationUpdateInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. The <see cref=
        /// "ApplicationUpdateInfo"/> class is an information object
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
        public ApplicationUpdateInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the access control lists for the current instance.
        /// </summary>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets the properties of the update.
        /// </summary>
        /// <remarks>
        /// A value of <c>null</c> indicates that no update is available.
        /// </remarks>
        public Update_t Update
        {
            get { return this.Content.GetValue("Update", Update_t.Converter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether to to reload the objects contained 
        /// in the locally installed application.
        /// </summary>
        public bool Refresh
        {
            get { return this.Content.GetValue("Refresh", BooleanConverter.Instance); }
        }

        #endregion

        #region Types

        public class Update_t : ExpandoAdapter<Update_t>
        {
            /// <summary>
            /// Get the name of the application.
            /// </summary>
            public string ApplicationName
            {
                get { return this.GetValue("Name", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the download URI for the application update.
            /// </summary>
            public Uri ApplicationUri
            {
                get { return this.GetValue("Appurl", UriConverter.Instance); }
            }

            /// <summary>
            /// Gets the checksum for the application update.
            /// </summary>
            public string Checksum
            {
                get { return this.GetValue("Checksum", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the name of the checksum type used to compute the application
            /// update <see cref="Checksum"/>.
            /// </summary>
            public string ChecksumType
            {
                get { return this.GetValue("ChecksumType", StringConverter.Instance); }
            }

            /// <summary>
            /// Get the URI to the Splunkbase page for the application.
            /// </summary>
            public string HomePage
            {
                get { return this.GetValue("Homepage", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets a value that indicates if the application has an explicit 
            /// ID in app.conf.
            /// </summary>
            /// <remarks>
            /// Splunk uses application IDs to help identify them during updates.
            /// </remarks>
            public bool ImplicitIdRequired
            {
                get { return this.GetValue("ImplicitIdRequired", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Get the size of the application update.
            /// </summary>
            public long Size
            {
                get { return this.GetValue("Size", Int64Converter.Instance); }
            }

            /// <summary>
            /// Get the version of the application update.
            /// </summary>
            public string Version
            {
                get { return this.GetValue("Version", StringConverter.Instance); }
            }
        }

        #endregion
    }
}
