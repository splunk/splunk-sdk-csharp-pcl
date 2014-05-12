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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a Splunk resource name.
    /// </summary>
    public sealed class ResourceName : IComparable, IComparable<ResourceName>, IEquatable<ResourceName>, IReadOnlyList<string>
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="parts"></param>
        public ResourceName(ResourceName resourceName, params string[] parts)
            : this(resourceName.Concat(parts))
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            Contract.Requires<ArgumentNullException>(parts != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts"></param>
        public ResourceName(params string[] parts)
            : this(parts.AsEnumerable<string>())
        {
            Contract.Requires<ArgumentNullException>(parts != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts"></param>
        public ResourceName(IEnumerable<string> parts)
        {
            this.parts = parts.Select((part, i) =>
            {
                if (string.IsNullOrEmpty(part))
                {
                    throw new ArgumentException(string.Format("parts[{0}]", i));  // TODO: Diagnostics
                }
                return part;
            }).ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
	        get { return this.parts[index]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this.parts.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Collection
        {
            get { return this.parts.Count > 1 ? this.parts[this.parts.Count - 2] : null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return this.parts[this.parts.Count - 1]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="ResourceName"/> with another object
        /// and returns an integer that indicates whether the current <see 
        /// cref="ResourceName"/> precedes, follows, or appears in the same 
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <paramref name="other"/>.
        /// <list type="table">
        /// <listheader>
        ///   <term>
        ///     Value
        ///   </term>
        ///   <description>
        ///     Condition
        ///   </description>
        /// </listheader>
        /// <item>
        ///   <term>
        ///     Less than zero
        ///   </term>
        ///   <description>
        ///     This instance precedes <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as 
        ///     <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <paramref name="other"/>, <paramref name=
        ///     "other"/> is not a <see cref="ResourceName"/>, or <paramref 
        ///     name="other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as ResourceName);
        }

        /// <summary>
        /// Compares the current <see cref="ResourceName"/> with another one and
        /// returns an integer that indicates whether the current <see cref=
        /// "ResourceName"/> precedes, follows, or appears in the same position in
        /// the sort order as the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <paramref name="other"/>.
        /// <list type="table">
        /// <listheader>
        ///   <term>
        ///     Value
        ///   </term>
        ///   <description>
        ///     Condition
        ///   </description>
        /// </listheader>
        /// <item>
        ///   <term>
        ///     Less than zero
        ///   </term>
        ///   <description>
        ///     This instance precedes <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as 
        ///     <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <paramref name="other"/> or <paramref
        ///     name="other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(ResourceName other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            int diff = this.parts.Count - other.parts.Count;

            if (diff != 0)
            {
                return diff;
            }

            var pair = this.parts
                .Zip(other.parts, (p1, p2) => new { ThisPart = p1, OtherPart = p2 })
                .FirstOrDefault(p => p.ThisPart != p.OtherPart);

            if (pair == null)
            {
                return 0;
            }

            return pair.ThisPart.CompareTo(pair.OtherPart);
        }

        /// <summary>
        /// Determines whether the current <see cref="ResourceName"/> and 
        /// another object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c> <see 
        /// cref="ResourceName"/> and is the same as the current <see cref=
        /// "ResourceName"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as ResourceName);
        }

        /// <summary>
        /// Determines whether the current <see cref="ResourceName"/> and another
        /// one are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is 
        /// the same as the current <see cref="ResourceName"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(ResourceName other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.parts.Count != other.parts.Count)
            {
                return false;
            }

            return this.parts.SequenceEqual(other.parts);
        }

        /// <summary>
        /// Gets an enumerator for iterating over the parts of this <see cref=
        /// "ResourceName"/>
        /// </summary>
        /// <returns>
        /// An enumerator for iterating over the parts of this <see cref=
        /// "ResourceName"/>
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for iterating over the parts of this <see cref=
        /// "ResourceName"/>
        /// </summary>
        /// <returns>
        /// An enumerator for iterating over the parts of this <see cref=
        /// "ResourceName"/>
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="ResourceName"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="ResourceName"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            return this.parts.Aggregate(seed: 17, func: (value, part) => (value * 23) + part.GetHashCode());
        }

        /// <summary>
        /// Gets the parent of this ResourceName.
        /// </summary>
        /// <returns>
        /// A <see cref="ResourceName"/> representing the parent of the current
        /// instance.
        /// </returns>
        public ResourceName GetParent()
        {
            return new ResourceName(parts.Take(parts.Count - 1));
        }

        /// <summary>
        /// Determines whether two <see cref="ResourceName"/> instances have 
        /// the same value. 
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the 
        /// value of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ResourceName a, ResourceName b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two <see cref="ResourceName"/> instances have 
        /// different values. 
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different than 
        /// the value of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ResourceName a, ResourceName b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Namespace"/> to its
        /// equivalent string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Namespace"/>
        /// </returns>
        public override string ToString()
        {
            return string.Join("/", from segment in this select segment);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Namespace"/> object to
        /// its equivalent URI encoded string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Namespace"/>
        /// </returns>
        /// <remarks>
        /// The value is converted using <see cref="Uri.EscapeUriString"/>.
        /// </remarks>
        public string ToUriString()
        {
            return string.Join("/", from segment in this select Uri.EscapeDataString(segment));
        }

        #endregion

        #region Privates

        readonly IReadOnlyList<string> parts;

        #endregion
    }
}
