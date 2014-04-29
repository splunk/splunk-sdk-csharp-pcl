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
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class that represents a Splunk ACL.
    /// </summary>
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

        public bool CanList
        {
            get { return this.GetValue("CanList", BooleanConverter.Instance); }
        }

        public bool CanWrite
        {
            get { return this.GetValue("CanWrite", BooleanConverter.Instance); }
        }

        public bool Modifiable
        {
            get { return this.GetValue("Modifiable", BooleanConverter.Instance); }
        }

        public string Owner
        {
            get { return this.GetValue("Owner", StringConverter.Instance); }
        }

        public Permissions Permissions
        {
            get { return this.GetValue("Perms", Permissions.Converter.Instance); }
        }

        public bool Removable
        {
            get { return this.GetValue("Removable", BooleanConverter.Instance); }
        }

		public SharingMode Sharing
        {
            get { return this.GetValue("Sharing", EnumConverter<SharingMode>.Instance); }
        }

        #endregion
    }
}
