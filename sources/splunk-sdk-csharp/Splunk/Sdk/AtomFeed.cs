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
    using System.Linq;
    using System.Xml.Linq;
    
    /// <summary>
    /// Provides an object representation of a Splunk Atom feed.
    /// </summary>
    class AtomFeed
    {
        #region Constructors

        public AtomFeed(XElement feed)
        {
            Contract.Requires<ArgumentNullException>(feed != null, "element");

            if (feed.Name != ElementName.Feed)
            {
                throw new InvalidDataException();
            }

            var entries = feed.Elements(ElementName.Entry);

            if (entries == null)
            {
                throw new InvalidDataException();
            }

            this.Entries = new List<AtomEntry>(from entry in entries select new AtomEntry(entry));
        }

        #endregion

        #region Properties

        public IReadOnlyList<AtomEntry> Entries
        { get; private set; }

        #endregion

        internal static class ElementName
        {
            public static readonly XName Author = XName.Get("author", "http://www.w3.org/2005/Atom");
            public static readonly XName Content = XName.Get("content", "http://www.w3.org/2005/Atom");
            public static readonly XName Entry = XName.Get("entry", "http://www.w3.org/2005/Atom");
            public static readonly XName Id = XName.Get("id", "http://www.w3.org/2005/Atom");
            public static readonly XName Feed = XName.Get("feed", "http://www.w3.org/2005/Atom");
            public static readonly XName Link = XName.Get("link", "http://www.w3.org/2005/Atom");
            public static readonly XName Published = XName.Get("published", "http://www.w3.org/2005/Atom");
            public static readonly XName Title = XName.Get("title", "http://www.w3.org/2005/Atom");
            public static readonly XName Updated = XName.Get("updated", "http://www.w3.org/2005/Atom");

            public static readonly XName Dict = XName.Get("dict", "http://dev.splunk.com/ns/rest");
            public static readonly XName Item = XName.Get("item", "http://dev.splunk.com/ns/rest");
            public static readonly XName Key = XName.Get("key", "http://dev.splunk.com/ns/rest");
            public static readonly XName List = XName.Get("list", "http://dev.splunk.com/ns/rest");
        };
    }
}
