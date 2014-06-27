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
    /// Interface for application setup information.
    /// </summary>
    /// <seealso cref="T:IBaseResource"/>
    public interface IApplicationSetupInfo : IBaseResource
    {
        /// <summary>
        /// Gets the extensible administration interface properties.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai { get; }

        /// <summary>
        /// Gets a value indicating whether the refresh.
        /// </summary>
        /// <value>
        /// <c>true</c> if refresh, <c>false</c> if not.
        /// </value>
        bool Refresh { get; }

        /// <summary>
        /// Gets the setup.
        /// </summary>
        /// <value>
        /// The setup.
        /// </value>
        dynamic Setup { get; }
    }
}
