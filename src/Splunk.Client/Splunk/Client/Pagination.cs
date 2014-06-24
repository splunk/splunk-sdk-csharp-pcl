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
//// [ ] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public struct Pagination
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsPerPage"></param>
        /// <param name="startIndex"></param>
        /// <param name="totalResults"></param>
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
        /// A readonly instance of the <see cref="Pagination"/> structure that
        /// is all zeros.
        /// </summary>
        public static readonly Pagination None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of entries returned by an operation.
        /// </summary>
        /// <value>
        /// The number of entries returned.
        /// </value>
        /// <remarks>
        /// The maximum number of entries returned by a GET operation on an
        /// entity collection is specified by the <c>Count</c> property. The 
        /// default value of the <c>Count</c> parameter is <c>30</c>. This 
        /// property gets the actual number of entries received.
        /// </remarks>
        public int ItemsPerPage
        {
            get { return this.itemsPerPage;  }
        }

        /// <summary>
        /// Gets the offset of the first entry returned.
        /// </summary>
        /// <value>
        /// Offset of the first entry returned.
        /// </value>
        /// <remarks>
        /// Use the offset parameter in a GET operation on an entity collection
        /// to override the default value of <c>0</c> which specifies that the
        /// first <see cref="ItemsPerPage"/> items in the entity collection
        /// should be returned.
        /// </remarks>
        public int StartIndex
        {
            get { return this.startIndex; }
        }

        /// <summary>
        /// Gets the total number of entries that can be returned for an 
        /// operation.
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
        /// Gets a string representation for the current <see cref="Pagination"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Pagination"/>.
        /// </returns>
        public override string ToString()
        {
            var text = string.Format("ItemsPerPage = {0}, StartIndex = {1}, TotalResults = {2}", 
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
