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

namespace Splunk.Sdk
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies the user/app context for a resource
    /// </summary>
    public class Namespace
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Namespace class with a user and app name.
        /// </summary>
        /// <param name="user">The name of a user or Namespace.AllUsers</param>
        /// <param name="app">The name of an application or Namespace.AllApps</param>
        public Namespace(string user, string app)
        {
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(app != null);

            this.User = user;
            this.App = app;
        }

        #endregion

        #region Fields

        public const string AllUsers = "-", AllApps = "-";

        #endregion

        #region Properties

        public bool IsSpecific
        {
            get { return this.User != AllUsers && this.App != AllApps; }
        }

        public string App 
        { get; private set; }
        
        public string User
        { get; private set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Join("/", "servicesNS", this.User, this.App);
        }

        #endregion
    }
}
