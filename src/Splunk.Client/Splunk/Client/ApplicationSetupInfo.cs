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
//// [ ] Contracts
//// [ ] Documentation

namespace Splunk.Client
{
    using Splunk;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the setup information for an <see cref="Application"/>.
    /// </summary>
    public class ApplicationSetupInfo : Resource
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
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ApplicationSetupInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. The <see cref=
        /// "ApplicationSetupInfo"/> class is an information object
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
        public ApplicationSetupInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected ExpandoAdapter Content
        {
            get { return this.content; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationAuthor
        {
            get { return this.Content.GetValue("Author", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckForUpdates
        {
            get { return this.Content.GetValue("CheckForUpdates", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Configured
        {
            get { return this.Content.GetValue("Configured", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return this.Content.GetValue("Description", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Disabled
        {
            get { return this.Content.GetValue("Disabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Label
        {
            get { return this.Content.GetValue("Label", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool StateChangeRequiresRestart
        {
            get { return this.Content.GetValue("StateChangeRequiresRestart", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Version
        {
            get { return this.Content.GetValue("Version", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Visible
        {
            get { return this.Content.GetValue("Visible", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the 
        /// </summary>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether to to reload the objects contained 
        /// in the locally installed application.
        /// </summary>
        public bool Refresh
        {
            get { return this.GetValue("Refresh", BooleanConverter.Instance); }
        }

        #endregion

        #region Methods

        protected internal override void Initialize(AtomFeed feed)
        {
            if (feed.Entries.Count != 1)
            {
                throw new InvalidDataException(string.Format("feed.Entries.Count = {0}", feed.Entries.Count));
            }

            base.Initialize(feed.Entries[0], feed.GeneratorVersion);
            this.content = this.GetValue("Content", ExpandoAdapter.Converter.Instance) ?? ExpandoAdapter.Empty;
        }

        #endregion

        #region Privates/internals

        ExpandoAdapter content;

        #endregion
    }
}
