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
//// [O] Documentation
//// [ ] Close with Fred on this:
////     FJR: As I mentioned in the kitchen, I finally remembered why the Ruby SDK
////     has all those namespace types. When you are creating an entity instance
////     from an Atom entry, you have to produce a namespace path to access the entity
////     with. That namespace is created from the ACL in the Atom data, and there are
////     a bunch of weird cases that require special handling.

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Specifies the user/app context for a resource.
    /// </summary>
    public class Namespace : IComparable, IComparable<Namespace>, IEquatable<Namespace>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Namespace"/> class with a user and app name.
        /// </summary>
        /// <param name="user">The name of a user or Namespace.AllUsers</param>
        /// <param name="app">The name of an application or Namespace.AllApps</param>
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
        /// 
        /// </summary>
        public static readonly Namespace Default = new Namespace();
        
        /// <summary>
        /// 
        /// </summary>
        public const string AllUsers = "-";

        /// <summary>
        /// 
        /// </summary>
        public const string AllApps = "-";

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string App
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSpecific
        {
            get { return !this.IsWildcard; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWildcard
        {
            get { return this.User == AllUsers || this.App == AllApps; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string User
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="Namespace"/> with another object
        /// and returns an integer that indicates whether the current <see 
        /// cref="Namespace"/> precedes, follows, or appears in the same 
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="Namespace"/>.
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
        ///     "other"/> is not a <see cref="Namespace"/>, or <paramref name=
        ///     "other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Namespace);
        }

        /// <summary>
        /// Compares the current <see cref="Namespace"/> with another one and
        /// returns an integer that indicates whether the current <see cref=
        /// "Namespace"/> precedes, follows, or appears in the same position in
        /// the sort order as the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Namespace"/>.
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
        ///      <paramref name="other"/>.
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

            int difference = this.User.CompareTo(other.User);
            return difference != 0 ? difference : this.App.CompareTo(other.App);
        }

        /// <summary>
        /// Determines whether the current <see cref="Namespace"/> and another
        /// object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Namespace"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c> <see 
        /// cref="Namespace"/> and is the same as the current <see cref=
        /// "Namespace"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Namespace);
        }

        /// <summary>
        /// Determines whether the current <see cref="Namespace"/> and another
        /// one are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Namespace"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is 
        /// the same as the current <see cref="Namespace"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(Namespace other)
        {
            if (other == null)
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
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.User.GetHashCode();
            hash = (hash * 23) + this.App.GetHashCode();

            return hash;
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
            return this.ToString(part => part);
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
