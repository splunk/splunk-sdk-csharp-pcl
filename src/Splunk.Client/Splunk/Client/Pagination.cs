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
    /// A pagination.
    /// </summary>
    /// <seealso cref="T:IEquatable{Pagination}"/>
    public struct Pagination : IEquatable<Pagination>
    {
        /// <summary>
        /// Initializes a new instance of the Pagination class.
        /// </summary>
        /// <param name="itemsPerPage">
        /// 
        /// </param>
        /// <param name="startIndex">
        /// 
        /// </param>
        /// <param name="totalResults">
        /// 
        /// </param>
        public Pagination(int itemsPerPage, int startIndex, int totalResults)
        {
            Contract.Requires<ArgumentOutOfRangeException>(itemsPerPage >= 0, "itemsPerPage < 0");
            Contract.Requires<ArgumentOutOfRangeException>(startIndex >= 0, "startIndex < 0");
            Contract.Requires<ArgumentOutOfRangeException>(totalResults >= 0, "totalResults < 0");

            this.itemsPerPage = itemsPerPage;
            this.startIndex = startIndex;
            this.totalResults = totalResults;
        }

        #region Fields

        /// <summary>
        /// A readonly instance of the <see cref="Pagination"/> structure that is all
        /// zeros.
        /// </summary>
        public static readonly Pagination None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of entries returned by an operation.
        /// </summary>
        /// <remarks>
        /// The maximum number of entries returned by a GET operation on an entity
        /// collection is specified by the <c>Count</c> property. The default value
        /// of the <c>Count</c> parameter is <c>30</c>. This property gets the actual
        /// number of entries received.
        /// </remarks>
        /// <value>
        /// The number of entries returned.
        /// </value>
        public int ItemsPerPage
        {
            get { return this.itemsPerPage;  }
        }

        /// <summary>
        /// Gets the offset of the first entry returned.
        /// </summary>
        /// <remarks>
        /// Use the offset parameter in a GET operation on an entity collection to
        /// override the default value of <c>0</c> which specifies that the first
        /// <see cref="ItemsPerPage"/> items in the entity collection should be
        /// returned.
        /// </remarks>
        /// <value>
        /// Offset of the first entry returned.
        /// </value>
        public int StartIndex
        {
            get { return this.startIndex; }
        }

        /// <summary>
        /// Gets the total number of entries that can be returned for an operation.
        /// </summary>
        /// <value>
        /// The total number of entries that can be returned.
        /// </value>
        public int TotalResults
        {
            get { return this.totalResults; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the current <see cref="Pagination"/> and another one
        /// are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Pagination"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is the
        /// same as the current <see cref="Pagination"/>; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool Equals(Pagination other)
        {
            return this.itemsPerPage == other.itemsPerPage && this.startIndex == other.startIndex && this.totalResults == other.totalResults;
        }

        /// <summary>
        /// Determines whether the current <see cref="Pagination"/> and another
        /// object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Pagination"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c>
        /// <see cref="Pagination"/> and is the same as the current
        /// <see cref= "Pagination"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="M:System.ValueType.Equals(object)"/>
        public override bool Equals(object other)
        {
            var otherPagination = other as Pagination?;
            return otherPagination.HasValue ? this.Equals(otherPagination.Value) : false;
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="Pagination"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="Pagination"/>.
        /// </returns>
        /// <seealso cref="M:System.ValueType.GetHashCode()"/>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.ItemsPerPage;
            hash = (hash * 23) + this.StartIndex;
            hash = (hash * 23) + this.TotalResults;

            return hash;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Pagination"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Pagination"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator ==(Pagination a, Pagination b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="Pagination"/> to compare or <c>null</c>.
        /// </param>
        /// <param name="b">
        /// The second <see cref="Pagination"/> to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator !=(Pagination a, Pagination b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Gets a string representation for the current <see cref="Pagination"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Pagination"/>.
        /// </returns>
        /// <seealso cref="M:System.ValueType.ToString()"/>
        public override string ToString()
        {
            var text = string.Format(CultureInfo.CurrentCulture, "ItemsPerPage = {0}, StartIndex = {1}, TotalResults = {2}", 
                this.ItemsPerPage, this.StartIndex, this.TotalResults);
            return text;
        }

        #endregion

        #region Privates

        readonly int itemsPerPage;
        readonly int startIndex;
        readonly int totalResults;

        #endregion
    }
}
