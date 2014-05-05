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
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the value of a parameter to a Splunk REST API endpoint.
    /// </summary>
    public class Argument : IComparable, IComparable<Argument>, IEquatable<Argument>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, string value)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the current <see cref="Argument"/>.
        /// </summary>
        public string Name
        { get; private set; }

        /// <summary>
        /// Gets the value of the current <see cref="Argument"/>.
        /// </summary>
        public string Value
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="Argument"/> with another object
        /// and returns an integer that indicates whether the current <see 
        /// cref="Argument"/> precedes, follows, or appears in the same 
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="Argument"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <see cref="other"/>.
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
        ///     This instance precedes <see cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as <see
        ///     cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <see cref="other"/>, <see cref="other"/>
        ///     is not an <see cref="Argument"/>, or <see cref="other"/> is
        ///     <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Argument);
        }

        /// <summary>
        /// Compares the current <see cref="Argument"/> with another one and
        /// returns an integer that indicates whether the current <see cref=
        /// "Argument"/> precedes, follows, or appears in the same position in
        /// the sort order as the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Argument"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <see cref="other"/>.
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
        ///     This instance precedes <see cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as <see
        ///     cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <see cref="other"/> or <see cref="other"/>
        ///     is <c>null</c>.
        ///   </description>
        /// </item>
        /// </returns>
        public int CompareTo(Argument other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            int result = this.Name.CompareTo(other.Name);
            return result != 0 ? result : this.Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Determines whether the current <see cref="Argument"/> and another
        /// object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Argument"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="other"/> is a non <c>null</c> <see cref=
        /// "Argument"/> and is the same as the current <see cref="Argument"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Argument);
        }

        /// <summary>
        /// Determines whether the current <see cref="Argument"/> and another
        /// one are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Argument"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="other"/> is non <c>null</c> and is the 
        /// same as the current <see cref="Argument"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Argument other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Name == other.Name && this.Value.Equals(other.Value);
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="Argument"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="Argument"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.Name.GetHashCode();
            hash = (hash * 23) + this.Value.GetHashCode();

            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("=", this.Name, this.Value);
        }

        #endregion
    }
}
