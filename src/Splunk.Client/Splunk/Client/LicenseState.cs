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
    /// <summary>
    /// Specifies the state of the license installed on a Splunk server instance.
    /// </summary>
    public enum LicenseState
    {
        /// <summary>
        /// The license is expired.
        /// </summary>
        Expired,

        /// <summary>
        /// The license is in force.
        /// </summary>
        Ok
    }
}
