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
//// [ ] Represent ApplicationSetupInfo.Setup as a static type

namespace Splunk.Client
{
    /// <summary>
    /// Represents the setup information for an <see cref="Application"/>.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Resource"/>
    /// <seealso cref="T:Splunk.Client.IApplicationSetupInfo"/>
    public class ApplicationSetupInfo : Resource, IApplicationSetupInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSetupInfo"/>
        /// class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        internal ApplicationSetupInfo(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ApplicationSetupInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public ApplicationSetupInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets The extensible administration interface properties for the current
        /// <see cref="ApplicationSetupInfo"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
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

        /// <summary>
        /// Gets a listing of the setup script for the application.
        /// </summary>
        /// <value>
        /// The setup.
        /// </value>
        public dynamic Setup
        {
            get { return this.Eai.GetValue("Setup"); }
        }

        #endregion
    }
}
