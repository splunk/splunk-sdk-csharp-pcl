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
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TaggedFieldValue
    {
        internal TaggedFieldValue(string value, IEnumerable<string> tags)
        {
            this.tags = tags.ToImmutableSortedSet();
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
        public ImmutableSortedSet<string> Tags
        { get { return this.tags; } }

        public override string ToString()
        {
            if (this.tags == null || this.tags.Count == 0)
            {
                return this.value ?? "\"\"";
            }

            var builder = new StringBuilder();

            builder.Append('"');
            builder.Append(this.value ?? string.Empty);
            builder.Append('"');
            builder.Append(" tagged ");

            foreach (var tag in this.tags)
            {
                builder.Append('"');
                builder.Append(tag);
                builder.Append('"');
                builder.Append(", ");
            }

            builder.Length = builder.Length - 2;

            return builder.ToString();
        }

        #region Privates

        readonly string value;
        readonly ImmutableSortedSet<string> tags;

        #endregion
    }
}
