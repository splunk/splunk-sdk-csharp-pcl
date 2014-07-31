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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides an object representation for the update information available
    /// for an application.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Resource"/>
    /// <seealso cref="T:Splunk.Client.IApplicationUpdateInfo"/>
    public class ApplicationUpdateInfo : Resource, Splunk.Client.IApplicationUpdateInfo
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
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ApplicationUpdateInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public ApplicationUpdateInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the extensible administration interface properties for the current
        /// <see cref="ApplicationUpdateInfo"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
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
        /// <value>
        /// The update.
        /// </value>
        public UpdateAdapter Update
        {
            get { return this.Content.GetValue("Update", UpdateAdapter.Converter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether to to reload the objects contained in the
        /// locally installed application.
        /// </summary>
        /// <value>
        /// <c>true</c> if refresh, <c>false</c> if not.
        /// </value>
        public bool Refresh
        {
            get { return this.Content.GetValue("Refresh", BooleanConverter.Instance); }
        }

        #endregion

        #region Types

        /// <summary>
        /// An update adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.ApplicationUpdateInfo.UpdateAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = 
            "This is by design.")
        ]
        public class UpdateAdapter : ExpandoAdapter<UpdateAdapter>
        {
            /// <summary>
            /// Get the name of the application.
            /// </summary>
            /// <value>
            /// The name of the application.
            /// </value>
            public string ApplicationName
            {
                get { return this.GetValue("Name", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the download URI for the application update.
            /// </summary>
            /// <value>
            /// The application URI.
            /// </value>
            public Uri ApplicationUri
            {
                get { return this.GetValue("Appurl", UriConverter.Instance); }
            }

            /// <summary>
            /// Gets the checksum for the application update.
            /// </summary>
            /// <value>
            /// The checksum.
            /// </value>
            public string Checksum
            {
                get { return this.GetValue("Checksum", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the name of the checksum type used to compute the application update
            /// <see cref="Checksum"/>.
            /// </summary>
            /// <value>
            /// The type of the checksum.
            /// </value>
            public string ChecksumType
            {
                get { return this.GetValue("ChecksumType", StringConverter.Instance); }
            }

            /// <summary>
            /// Get the URI to the Splunkbase page for the application.
            /// </summary>
            /// <value>
            /// The homepage.
            /// </value>
            public string Homepage
            {
                get { return this.GetValue("Homepage", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets a value that indicates if the application has an explicit ID in
            /// app.conf.
            /// </summary>
            /// <remarks>
            /// Splunk uses application IDs to help identify them during updates.
            /// </remarks>
            /// <value>
            /// <c>true</c> if implicit identifier required, <c>false</c> if not.
            /// </value>
            public bool ImplicitIdRequired
            {
                get { return this.GetValue("ImplicitIdRequired", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Gets the size of the application update.
            /// </summary>
            /// <value>
            /// Size of the application update.
            /// </value>
            public long Size
            {
                get { return this.GetValue("Size", Int64Converter.Instance); }
            }

            /// <summary>
            /// Get the version of the application update.
            /// </summary>
            /// <value>
            /// Version of the applicaiton update.
            /// </value>
            public string Version
            {
                get { return this.GetValue("Version", StringConverter.Instance); }
            }
        }

        #endregion
    }
}
