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
// [ ] Name table (?)
// [ ] Contracts
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class Permissions
    {
        internal Permissions(dynamic permissions)
        {
            List<object> list;
                
            list = permissions.Read as List<object>;
            this.Read = (ISet<string>)new SortedSet<string>(from userName in list select userName.ToString());

            list = permissions.Write as List<object>;
            this.Write = (ISet<string>)new SortedSet<string>(from userName in list select userName.ToString());
        }

        public ISet<string> Read
        { get; private set; }

        public ISet<string> Write
        { get; private set; }
    }
}
