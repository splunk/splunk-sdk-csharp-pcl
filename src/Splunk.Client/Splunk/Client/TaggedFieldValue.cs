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
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides an object representation of a tagged field value.
    /// </summary>
    public sealed class TaggedFieldValue : IComparable, IComparable<TaggedFieldValue>, IEquatable<TaggedFieldValue>
    {
        #region Constructors

        internal TaggedFieldValue(string value, ImmutableSortedSet<string> tags)
        {
            Debug.Assert(value != null);
            Debug.Assert(tags != null);

            this.value = value;
            this.tags = tags;
            this.hashCode = this.tags.Aggregate(31 * 17 + value.GetHashCode(), (h, t) => 31 * h + t.GetHashCode());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value assigned to the current instance.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the set of tags assigned to the current instance.
        /// </summary>
        public ImmutableSortedSet<string> Tags
        {
            get { return this.tags; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TaggedFieldValue other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            var result = this.value.CompareTo(other.value);

            if (result != 0)
            {
                return result;
            }

            foreach (var operand in this.tags.Zip(other.tags, (x, y) => new { x = x, y = y }))
            {
                result = operand.x.CompareTo(operand.y);

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            if (!(other is TaggedFieldValue))
            {
                throw new ArgumentException("other is not a TaggedFieldValue");
            }
            return this.CompareTo(other as TaggedFieldValue);
        }

        /// <summary>
        /// Determines whether the specified object refers to the same tagged field value as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="TaggedFieldValue"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same <see cref="TaggedFieldValue"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        /// <seealso cref="M:System.Object.Equals(object)"/>
        public override bool Equals(object other)
        {
            return this.Equals(other as TaggedFieldValue);
        }

        /// <summary>
        /// Determines whether the specified instance refers to the same tagged field value as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="TaggedFieldValue"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same tagged field value; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(TaggedFieldValue other)
        {
            if ((object)other == null)  // We cast so we that don't call TaggedFieldValue.operator==
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            var result = this.hashCode == other.hashCode && this.value == other.value && this.tags.SetEquals(other.tags);
            return result;
        }

        /// <summary>
        /// Returns the hash code for the current instance.
        /// </summary>
        /// <returns>
        /// Hash code for the current instance.
        /// </returns>
        /// <seealso cref="M:System.Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            return this.hashCode;
        }

        /// <summary>
        /// Gets a string representation of the current instance.
        /// </summary>
        /// <returns>
        /// A string representation of the current instance.
        /// </returns>
        /// <seealso cref="M:System.Object.GetHashCode()"/>
        public override string ToString()
        {
            if (this.formattedValue != null)
            {
                return this.formattedValue;
            }

            lock (this.locker)
            {
                if (this.formattedValue != null)
                    return this.formattedValue;

                if (this.tags.Count == 0)
                {
                    return this.formattedValue = value;
                }

                var builder = new StringBuilder();

                builder.Append("TaggedFieldValue(Value: ");
                builder.Append(this.value);
                builder.Append(", Tags: [");

                foreach (var tag in this.tags)
                {
                    builder.Append(tag);
                    builder.Append(", ");
                }

                builder.Length = builder.Length - 2;
                builder.Append("])");

                return this.formattedValue = builder.ToString();
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(TaggedFieldValue a, TaggedFieldValue b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            return a == null ? false : a.Equals(b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(TaggedFieldValue a, TaggedFieldValue b)
        {
            if (object.ReferenceEquals(a, b))
                return false;
            return a == null ? true : a.Equals(b);
        }

        #endregion

        #region Privates

        readonly object locker = new object();

        readonly int hashCode;
        readonly string value;
        readonly ImmutableSortedSet<string> tags;

        string formattedValue = null;

        #endregion
    }
}
