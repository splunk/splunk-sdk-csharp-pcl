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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents a Splunk resource name.
    /// </summary>
    /// <seealso cref="T:System.Collections.ObjectModel.ReadOnlyCollection{T}"/>
    /// <seealso cref="T:System.IComparable"/>
    /// <seealso cref="T:System.IComparable{T}"/>
    /// <seealso cref="T:System.IEquatable{T}"/>
    public sealed class ResourceName : ReadOnlyCollection<string>, IComparable, IComparable<ResourceName>, 
        IEquatable<ResourceName>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceName"/> class.
        /// </summary>
        /// <param name="resourceName">
        /// Another resource name.
        /// </param>
        /// <param name="parts">
        /// Names to be appended <paramref name="resourceName"/>.
        /// </param>
        public ResourceName(ResourceName resourceName, params string[] parts)
            : this(resourceName.Concat(parts))
        {
            Contract.Requires<ArgumentNullException>(resourceName != null);
            Contract.Requires<ArgumentNullException>(parts != null);
        }

        /// <summary>
        /// Intializes a new instance of the <see cref="ResourceName"/> class.
        /// </summary>
        /// <param name="parts">
        /// 
        /// </param>
        public ResourceName(params string[] parts)
            : base(parts)
        {
            Contract.Requires<ArgumentNullException>(parts != null);

            foreach (var part in this)
            {
                if (string.IsNullOrEmpty(part))
                {
                    throw new ArgumentException(string.Concat("parts: ", this.ToString()));
                }
            }
        }

        /// <summary>
        /// Intializes a new instance of the <see cref="ResourceName"/> class.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        /// <param name="parts">
        /// 
        /// </param>
        public ResourceName(IEnumerable<string> parts)
            : this(parts.ToArray())
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        public string Collection
        {
            get { return this.Items.Count > 1 ? this.Items[this.Items.Count - 2] : null; }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return this.Items[this.Items.Count - 1]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="ResourceName"/> with another object and
        /// returns an integer that indicates whether the current
        /// <see cref="ResourceName"/> precedes, follows, or appears in the same
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes,
        /// follows, or appears in the same position in the sort order as
        /// <paramref name="other"/>.
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
        ///     This instance follows <paramref name="other"/>,
        ///     <paramref name= "other"/> is not a <see cref="ResourceName"/>, or
        ///     <paramref name="other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <seealso cref="M:System.IComparable.CompareTo(object)"/>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as ResourceName);
        }

        /// <summary>
        /// Compares the current <see cref="ResourceName"/> with another one and
        /// returns an integer that indicates whether the current
        /// <see cref= "ResourceName"/> precedes, follows, or appears in the same
        /// position in the sort order as the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes,
        /// follows, or appears in the same position in the sort order as
        /// <paramref name="other"/>.
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
        ///     This instance follows <paramref name="other"/> or
        ///     <paramref name="other"/> is <c>null</c>.
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

            int diff = this.Items.Count - other.Items.Count;

            if (diff != 0)
            {
                return diff;
            }

            var pair = this.Items
                .Zip(other.Items, (p1, p2) => new { ThisPart = p1, OtherPart = p2 })
                .FirstOrDefault(p => p.ThisPart != p.OtherPart);

            if (pair == null)
            {
                return 0;
            }

            return string.Compare(pair.ThisPart, pair.OtherPart, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the current <see cref="ResourceName"/> and another
        /// object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c>
        /// <see cref="ResourceName"/> and is the same as the current
        /// <see cref= "ResourceName"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="M:System.Object.Equals(object)"/>
        public override bool Equals(object other)
        {
            return this.Equals(other as ResourceName);
        }

        /// <summary>
        /// Determines whether the current <see cref="ResourceName"/> and another one
        /// are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="ResourceName"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is the
        /// same as the current <see cref="ResourceName"/>; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool Equals(ResourceName other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.Items.Count != other.Items.Count)
            {
                return false;
            }

            return this.Items.SequenceEqual(other.Items);
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="ResourceName"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="ResourceName"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            return this.Items.Aggregate(seed: 17, func: (value, part) => (value * 23) + part.GetHashCode());
        }

        /// <summary>
        /// Greater-than comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >(ResourceName a, ResourceName b)
        {
            if ((object)a == null)
            {
                return false;
            }

            return a.CompareTo(b) > 0;
        }

        /// <summary>
        /// Greater-than-or-equal comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >=(ResourceName a, ResourceName b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Determines whether two <see cref="ResourceName"/> instances have the same
        /// value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value
        /// of <paramref name="b"/>; otherwise, <c>false</c>.
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
        /// <c>true</c> if the value of <paramref name="a"/> is different than the
        /// value of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ResourceName a, ResourceName b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Less-than comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <(ResourceName a, ResourceName b)
        {
            if ((object)a == null)
            {
                return (object)b != null;
            }

            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Less-than-or-equal comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="ResourceName"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <=(ResourceName a, ResourceName b)
        {
            if ((object)a == null)
            {
                return true;
            }

            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Converts the value of the current <see cref="Namespace"/> to its
        /// equivalent string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Namespace"/>
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            return string.Join("/", from segment in this select segment);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Namespace"/> object to its
        /// equivalent URI encoded string representation.
        /// </summary>
        /// <remarks>
        /// The value is converted using <see cref="Uri.EscapeUriString"/>.
        /// </remarks>
        /// <returns>
        /// A string representation of the current <see cref="Namespace"/>
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification =
            "This is by design")
        ]
        public string ToUriString()
        {
            return string.Join("/", from segment in this select Uri.EscapeDataString(segment));
        }

        #endregion
    }
}
