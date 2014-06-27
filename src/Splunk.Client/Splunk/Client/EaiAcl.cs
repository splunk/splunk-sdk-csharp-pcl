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
    /// <summary>
    /// Provides a class that represents a Splunk ACL.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.EaiAcl}"/>
    public sealed class EaiAcl : ExpandoAdapter<EaiAcl>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EaiAcl"/> class.
        /// </summary>
        public EaiAcl()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public string App
        {
            get { return this.GetValue("App", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether we can list.
        /// </summary>
        /// <value>
        /// <c>true</c> if we can list, <c>false</c> if not.
        /// </value>
        public bool CanList
        {
            get { return this.GetValue("CanList", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether we can write.
        /// </summary>
        /// <value>
        /// <c>true</c> if we can write, <c>false</c> if not.
        /// </value>
        public bool CanWrite
        {
            get { return this.GetValue("CanWrite", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is modifiable.
        /// </summary>
        /// <value>
        /// <c>true</c> if modifiable, <c>false</c> if not.
        /// </value>
        public bool Modifiable
        {
            get { return this.GetValue("Modifiable", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public string Owner
        {
            get { return this.GetValue("Owner", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the permissions.
        /// </summary>
        /// <value>
        /// The permissions.
        /// </value>
        public Permissions Permissions
        {
            get { return this.GetValue("Perms", Permissions.Converter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is removable.
        /// </summary>
        /// <value>
        /// <c>true</c> if removable, <c>false</c> if not.
        /// </value>
        public bool Removable
        {
            get { return this.GetValue("Removable", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the sharing.
        /// </summary>
        /// <value>
        /// The sharing.
        /// </value>
		public SharingMode Sharing
        {
            get { return this.GetValue("Sharing", EnumConverter<SharingMode>.Instance); }
        }

        #endregion
    }
}
