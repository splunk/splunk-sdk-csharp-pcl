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

namespace Splunk.ModularInputs
{
    /// <summary>
    /// Represents a stream of events to standard output written using the XML 
    /// streaming mode.
    /// </summary>
    public sealed class EventStream
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream"/>
        /// class.
        /// </summary>
        public EventStream()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an <see cref="EventElement"/> to the current <see cref=
        /// "EventStream"/>.
        /// </summary>
        /// <param name="element">
        /// An object representing an event element.
        /// </param>
        /// <remarks>
        /// <example>Sample event stream</example>
        /// <code>
        /// <stream>
        ///   <event>
        ///     <index>sdk-tests2</index>
        ///     <sourcetype>test sourcetype</sourcetype>
        ///     <source>test source</source>
        ///     <host>test host</host>
        ///     <data>Event with all default fields set</data>
        ///   </event>
        ///   <event stanza="modular_input:UnitTest2" unbroken="1">
        ///     <data>Part 1 of channel 2 without a newline </data>
        ///   </event>
        /// </stream>
        /// </code>
        /// </remarks>
        public void Write(EventElement element)
        {
            EventWriter.Add(element);
        }

        #endregion
    }
}
