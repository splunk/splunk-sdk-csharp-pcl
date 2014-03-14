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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Acl
    {
        internal Acl(dynamic acl)
        {
            this.CanList = acl.CanList != "0";
            this.CanWrite = acl.CanWrite != "0";
            this.Modifiable = acl.Modifiable != "0";
            this.Owner = acl.Owner;
            this.Permissions = new Permissions(acl.Perms);
            this.Removable = acl.Removable != "0";
            this.Sharing = acl.Sharing;
        }

        public bool CanList
        { get; private set; }
		
        public bool CanWrite
        { get; private set; }

        public bool Modifiable
        { get; private set; }

		public string Owner
        { get; private set; }

        public dynamic Permissions
        { get; private set; }
		
        public bool Removable
        { get; private set; }

		public string Sharing
        { get; private set; }
    }
}
