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
// [O] Contracts
// [O] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq;

    /// <summary>
    /// Provides access to read/write permissions.
    /// </summary>
    public class Permissions : ExpandoAdapter
    {
        internal Permissions(ExpandoObject expandoObject)
            :base(expandoObject)
        { }

        public ISet<string> Read
        {
            get { return this.GetValue("Read", CollectionConverter<string, SortedSet<string>, StringConverter>.Instance); }
        }

        public ISet<string> Write
        {
            get { return this.GetValue("Write", CollectionConverter<string, SortedSet<string>, StringConverter>.Instance); }
        }
    }
}
