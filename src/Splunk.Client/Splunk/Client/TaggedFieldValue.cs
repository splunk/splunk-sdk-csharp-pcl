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

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TaggedFieldValue
    {
        internal TaggedFieldValue(string value, IEnumerable<string> tags)
        {
            this.tags = tags.ToImmutableHashSet();
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value
        { get { return this.value; } }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableHashSet<string> Tags
        { get { return this.tags; } }

        #region Privates

        readonly string value;
        readonly ImmutableHashSet<string> tags;

        #endregion
    }
}
