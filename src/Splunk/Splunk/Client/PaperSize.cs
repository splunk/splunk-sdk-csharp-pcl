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

namespace Splunk.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the paper size of a report.
    /// </summary>
    public enum PaperSize
    {
        /// <summary>
        /// 8.5 × 11 inches.
        /// </summary>
        Letter,

        /// <summary>
        /// 420 x 594 millimeters.
        /// </summary>
        A2,

        /// <summary>
        /// 297 x 420 millimeters.
        /// </summary>
        A3,

        /// <summary>
        /// 210 x 297 millimeters.
        /// </summary>
        A4,

        /// <summary>
        /// 148 x 210 millimeters.
        /// </summary>
        A5,

        /// <summary>
        /// 17 × 11 inches.
        /// </summary>
        Ledger,

        /// <summary>
        /// 8.5 × 14 inches.
        /// </summary>
        Legal
    }
}
