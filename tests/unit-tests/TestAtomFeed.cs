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

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Xunit;

    public class TestAtomFeed
    {
        [Trait("class", "AtomFeed")]
        [Fact]
        public void CanConstruct()
        {
            var feed = new AtomFeed(document.Root);
        }

        [Trait("class", "AtomFeed")]
        [Fact]
        public void CanAccessEntries()
        {
            var expected = new List<string>() { "AtomEntry(Title=search *, Author=admin, Id=https://localhost:8089/services/search/jobs/1392687998.313, Published=2/17/2014 5:46:39 PM, Updated=2/17/2014 5:46:39 PM)" };
            var feed = new AtomFeed(document.Root);
            List<string> actual;
            
            actual = new List<string>();

            foreach (var entry in feed.Entries)
            {
                actual.Add(entry.ToString());
            }

            Assert.Equal(expected, actual);

            actual = new List<string>();

            for (int i = 0; i < feed.Entries.Count; i++)
            {
                actual.Add(feed.Entries[i].ToString());
            }

            Assert.Equal(expected, actual);
        }

        static readonly XDocument document = XDocument.Load(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "data", "AtomFeed.xml")));
    }
}
