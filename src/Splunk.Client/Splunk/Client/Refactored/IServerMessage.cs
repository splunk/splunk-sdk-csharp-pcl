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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Provides an object representation of a Splunk server message entity.
    /// </summary>
    public interface IServerMessage : IEntity
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        Eai Eai
        { get; }

        /// <summary>
        /// 
        /// </summary>
        ServerMessageSeverity Severity
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string Text
        { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime TimeCreated
        { get; }

        #endregion
    }
}
