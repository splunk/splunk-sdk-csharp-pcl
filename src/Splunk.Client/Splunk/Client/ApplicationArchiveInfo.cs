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

// TODO:
// [ ] Contracts
// [ ] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public sealed class ApplicationArchiveInfo : Entity<ApplicationArchiveInfo>
    {
        #region Constructors

        internal ApplicationArchiveInfo(Context context, Namespace @namespace, string name)
            : base(context, @namespace, new ResourceName(ApplicationCollection.ClassResourceName, name, "package"))
        { }

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
