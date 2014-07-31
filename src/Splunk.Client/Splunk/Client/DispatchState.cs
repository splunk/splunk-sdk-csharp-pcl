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
    /// Specifies the state of a search <see cref="Job"/>.
    /// </summary>
    public enum DispatchState
    {
        /// <summary>
        /// Specifies that a <see cref="Job"/> has not yet been created.
        /// </summary>
        None,
        
        /// <summary>
        /// Specifies that a <see cref="Job"/> has been queued by the dispatcher.
        /// </summary>
        Queued, 
        
        /// <summary>
        /// Specifies that a <see cref="Job"/> is being parsed.
        /// </summary>
        Parsing, 
        
        /// <summary>
        /// Specifies that a <see cref="Job"/> is running.
        /// </summary>
        Running,

        /// <summary>
        /// Specifies that a <see cref="Job"/> is finalizing.
        /// </summary>
        Finalizing,

        /// <summary>
        /// Specifies that a <see cref="Job"/> completed.
        /// </summary>
        Done,

        /// <summary>
        /// Specifies that a <see cref="Job"/> terminated due to a fatal error.
        /// </summary>
        Failed,

        /// <summary>
        /// Specifies that a <see cref="Job"/> is paused.
        /// </summary>
        Paused
    }
}
