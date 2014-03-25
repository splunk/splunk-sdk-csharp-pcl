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
// [O] Contracts
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct Pagination
    {
        public static readonly Pagination Empty;

        public Pagination(int itemsPerPage, int startIndex, int totalResults)
        {
            Contract.Requires<ArgumentOutOfRangeException>(itemsPerPage >= 0, "itemsPerPage < 0");
            Contract.Requires<ArgumentOutOfRangeException>(startIndex >= 0, "startIndex < 0");
            Contract.Requires<ArgumentOutOfRangeException>(totalResults >= 0, "totalResults < 0");

            this.itemsPerPage = itemsPerPage;
            this.startIndex = startIndex;
            this.totalResults = totalResults;
        }
        public int ItemsPerPage
        {
            get { return this.itemsPerPage;  }
        }

        public int StartIndex
        {
            get { return this.startIndex; }
        }

        public int TotalResults
        {
            get { return this.totalResults; }
        }

        readonly int itemsPerPage;
        readonly int startIndex;
        readonly int totalResults;
    }
}
