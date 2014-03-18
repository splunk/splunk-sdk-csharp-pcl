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

// [ ] TODO: AtomFeed: Add properties, not just entries.

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk Atom feed.
    /// </summary>
    class AtomFeed
    {
        #region Constructors

        public AtomFeed()
        { }

        #endregion

        #region Properties

        public IReadOnlyList<AtomEntry> Entries
        { get; private set; }

        #endregion

        #region Methods

        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            if (reader.ReadState == ReadState.Initial)
            {
                await reader.ReadAsync();

                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    await reader.ReadAsync();
                }
            }
            else
            {
                reader.MoveToElement();
            }

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "feed"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadToFollowingAsync("entry");
            var entries = new List<AtomEntry>();

            while (reader.NodeType == XmlNodeType.Element && reader.Name == "entry")
            {
                var entry = new AtomEntry();
                
                await entry.ReadXmlAsync(reader);
                entries.Add(entry);
            }
            
            if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "feed"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadAsync();
        }

        #endregion
    }
}
