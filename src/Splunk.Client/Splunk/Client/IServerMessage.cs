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

    /// <summary>
    /// Provides an object representation of a Splunk server message entity.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    public interface IServerMessage : IEntity
    {
        /// <summary>
        /// Gets the extensible administration interface properties.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        ServerMessageSeverity Severity
        { get; }

        /// <summary>
        /// Gets the text of the server message.
        /// </summary>
        /// <value>
        /// Text of the server messages.
        /// </value>
        string Text
        { get; }

        /// <summary>
        /// Gets the Date/Time of the time created.
        /// </summary>
        /// <value>
        /// The time created.
        /// </value>
        DateTime TimeCreated
        { get; }
    }
}
