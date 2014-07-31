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
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the direction in which to sort a list of items.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// No sort direction is set.
        /// </summary>
        None,

        /// <summary>
        /// Sort from smallest to largest. For example, from A to Z.
        /// </summary>
        [EnumMember(Value = "asc")]
        Ascending,

        /// <summary>
        /// Sort from largest to smallest. For example, from Z to A.
        /// </summary>
        [EnumMember(Value = "desc")]
        Descending
    }
}
