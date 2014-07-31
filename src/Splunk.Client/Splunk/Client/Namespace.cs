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
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies the user/app context for a resource.
    /// </summary>
    /// <seealso cref="T:System.IComparable"/>
    /// <seealso cref="T:System.IComparable{Splunk.Client.Namespace}"/>
    /// <seealso cref="T:System.IEquatable{Splunk.Client.Namespace}"/>
    public class Namespace : IComparable, IComparable<Namespace>, IEquatable<Namespace>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Namespace"/> class with a
        /// user and app name.
        /// </summary>
        /// <param name="user">
        /// The name of a user or <see cref="Namespace"/><c>.AllUsers</c>.
        /// </param>
        /// <param name="app">
        /// The name of an application or <see cref="Namespace"/><c>.AllApps</c>.
        /// </param>
        public Namespace(string user, string app)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user), "user");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(app), "app");

            this.User = user;
            this.App = app;
        }

        private Namespace()
        {
            this.User = this.App = "";
        }

        #endregion

        #region Fields

        /// <summary>
        /// The default.
        /// </summary>
        public static readonly Namespace Default = new Namespace();

        /// <summary>
        /// All users wildcard.
        /// </summary>
        public const string AllUsers = "-";

        /// <summary>
        /// All apps wildcard.
        /// </summary>
        public const string AllApps = "-";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public string App
        { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object is specific.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is specific, <c>false</c> if not.
        /// </value>
        public bool IsSpecific
        {
            get { return !this.IsWildcard; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is wildcard.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is wildcard, <c>false</c> if not.
        /// </value>
        public bool IsWildcard
        {
            get { return this.User == AllUsers || this.App == AllApps; }
        }

        /// <summary>
        /// Gets the user of the current <see cref="Namespace"/>.
        /// </summary>
        /// <value>
        /// The user of the current <see cref="Namespace"/>.
        /// </value>
        public string User
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="Namespace"/> with another object and
        /// returns an integer that indicates whether the current
        /// <see cref="Namespace"/> precedes, follows, or appears in the same
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="Namespace"/>.
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
        ///     <paramref name= "other"/> is not a <see cref="Namespace"/>, or
        ///     <paramref name= "other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <seealso cref="M:System.IComparable.CompareTo(object)"/>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Namespace);
        }

        /// <summary>
        /// Compares the current <see cref="Namespace"/> with another one and returns
        /// an integer that indicates whether the current <see cref= "Namespace"/>
        /// precedes, follows, or appears in the same position in the sort order as
        /// the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Namespace"/>.
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
        ///      <paramref name="other"/>.
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
        public int CompareTo(Namespace other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            int difference = string.Compare(this.User, other.User, StringComparison.Ordinal);
            return difference != 0 ? difference : string.Compare(this.App, other.App, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the current <see cref="Namespace"/> and another object
        /// are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Namespace"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c>
        /// <see cref="Namespace"/> and is the same as the current
        /// <see cref= "Namespace"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="M:System.Object.Equals(object)"/>
        public override bool Equals(object other)
        {
            return this.Equals(other as Namespace);
        }

        /// <summary>
        /// Determines whether the current <see cref="Namespace"/> and another one
        /// are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Namespace"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is the
        /// same as the current <see cref="Namespace"/>; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool Equals(Namespace other)
        {
            if ((object)other == null)
            {
                return false;
            }
            return object.ReferenceEquals(this, other) || (this.User == other.User && this.App == other.App);
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="Namespace"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="Namespace"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.User.GetHashCode();
            hash = (hash * 23) + this.App.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Greater-than comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >(Namespace a, Namespace b)
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
        /// The first <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >=(Namespace a, Namespace b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Determines whether two <see cref="Namespace"/> instances have the same
        /// value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value
        /// of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Namespace a, Namespace b)
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
        /// Determines whether two <see cref="Namespace"/> instances have different
        /// values.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different than the
        /// value of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Namespace a, Namespace b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Less-than comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <(Namespace a, Namespace b)
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
        /// The first <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Namespace"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <=(Namespace a, Namespace b)
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
            return this.ToString(part => part);
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
            return this.ToString(Uri.EscapeDataString);
        }

        #endregion

        #region Privates

        string ToString(Func<string, string> encode)
        {
            if (this == Default)
            {
                return "services";
            }

            return string.Join("/", "servicesNS", encode(this.User), encode(this.App));
        }

        #endregion
    }
}
