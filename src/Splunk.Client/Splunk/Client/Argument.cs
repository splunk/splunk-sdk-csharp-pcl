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
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Represents the value of a parameter to a Splunk REST API endpoint.
    /// </summary>
    /// <seealso cref="T:System.IComparable"/>
    /// <seealso cref="T:System.IComparable{Splunk.Client.Argument}"/>
    /// <seealso cref="T:System.IEquatable{Splunk.Client.Argument}"/>
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
        public Argument(string name, byte value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, SByte value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, char value)
            : this(name, value.ToString())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, decimal value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, int value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, uint value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, double value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, float value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, long value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, ulong value)
            : this(name, value.ToString(CultureInfo.InvariantCulture.NumberFormat))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="value">
        /// Argument value.
        /// </param>
        public Argument(string name, object value)
            : this(name, value == null ? null : value.ToString())
        { }

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
        /// <value>
        /// Name of the current <see cref="Argument"/>.
        /// </value>
        public string Name
        { get; private set; }

        /// <summary>
        /// Gets the value of the current <see cref="Argument"/>.
        /// </summary>
        /// <value>
        /// Value of the current <see cref="Argument"/>.
        /// </value>
        public string Value
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="Argument"/> with another object and
        /// returns an integer that indicates whether the current
        /// <see cref="Argument"/> precedes, follows, or appears in the same position
        /// in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="Argument"/>.
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
        ///     <paramref name= "other"/> is not an <see cref="Argument"/>, or
        ///     <paramref name= "other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <seealso cref="M:System.IComparable.CompareTo(object)"/>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Argument);
        }

        /// <summary>
        /// Compares the current <see cref="Argument"/> with another one and returns
        /// an integer that indicates whether the current <see cref= "Argument"/>
        /// precedes, follows, or appears in the same position in the sort order as
        /// the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Argument"/>.
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

            int result = string.Compare(this.Name, other.Name, StringComparison.Ordinal);
            return result != 0 ? result : string.Compare(this.Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the current <see cref="Argument"/> and another object
        /// are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Argument"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c>
        /// <see cref="Argument"/> and is the same as the current
        /// <see cref="Argument"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="M:System.Object.Equals(object)"/>
        public override bool Equals(object other)
        {
            return this.Equals(other as Argument);
        }

        /// <summary>
        /// Determines whether the current <see cref="Argument"/> and another one are
        /// equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Argument"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is the
        /// same as the current <see cref="Argument"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Argument other)
        {
            if ((object)other == null)
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
        /// <seealso cref="M:System.Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.Name.GetHashCode();
            hash = (hash * 23) + this.Value.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Greater-than comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >(Argument a, Argument b)
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
        /// The first <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >=(Argument a, Argument b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Determines whether two <see cref="Argument"/> instances have the same
        /// value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value
        /// of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Argument a, Argument b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two <see cref="Argument"/> instances have different
        /// values.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different than the
        /// value of <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Argument a, Argument b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Less-than comparison operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <(Argument a, Argument b)
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
        /// The first <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Argument"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <=(Argument a, Argument b)
        {
            if ((object)a == null)
            {
                return true;
            }

            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Gets a string representation for the current <see cref="Argument"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Argument"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            return string.Join("=", this.Name, this.Value);
        }

        #endregion
    }
}
