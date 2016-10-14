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

namespace Splunk.Client.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class TestAtomFeed
    {
        public static readonly string Directory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Data", "Client"));
        
        [Trait("unit-test", "Splunk.Client.AtomEntry")]
        [Fact]
        public async Task CanReadAtomEntry()
        {
            var entry = await ReadEntry(Path.Combine(Directory, "Job.GetAsync.xml"));
            Assert.Equal(
                "AtomEntry(Title=search search index=_internal | head 1000, Author=admin, Id=https://localhost:8089/services/search/jobs/scheduler__admin__search__RMD50aa4c13eb03d1730_at_1401390000_866, Published=5/29/2014 12:00:01 PM, Updated=5/29/2014 1:01:08 PM)", 
                entry.ToString());
        }

        [Trait("unit-test", "Splunk.Client.AtomFeed")]
        [Fact]
        public async Task CanReadAtomFeed()
        {
            var feed = await ReadFeed(Path.Combine(Directory, "JobCollection.GetAsync.xml"));
            Assert.Equal(
                "AtomFeed(Title=jobs, Author=Splunk, Id=https://localhost:8089/services/search/jobs, Updated=5/29/2014 12:13:01 PM)",
                feed.ToString());
        }

        public static async Task<AtomEntry> ReadEntry(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var entry = new AtomEntry();
                await entry.ReadXmlAsync(reader);

                return entry;
            }
        }

        public static async Task<AtomFeed> ReadFeed(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(reader);

                return feed;
            }
        }

        [Trait("unit-test", "Splunk.Client.AtomFeed")]
        [Fact]
        public static async Task DefaultCollections()
        {
            var path = Path.Combine(Directory, "AtomFeed.DefaultCollection.xml");
            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(reader);
               
                Assert.Equal(new ReadOnlyCollection<AtomEntry>(new List<AtomEntry>()), feed.Entries);
                Assert.Equal(new ReadOnlyDictionary<string, Uri>(new Dictionary<string, Uri>()), feed.Links);
                Assert.Equal(new ReadOnlyCollection<Message>(new List<Message>()), feed.Messages);
            }   
        }

        [Trait("unit-test", "Splunk.Client.AtomFeed")]
        [Fact]
        public static async Task ParseDictionaryAsync()
        {
            // Test to verify GitHub PR #57 - handling empty key name to "Empty" for <s:key name="">
            var path = Path.Combine(Directory, "AtomFeed.EmptyDictionaryKeyName.xml");
            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(reader);
                
                Assert.Equal(1, feed.Entries.Count);
                AtomEntry entry = feed.Entries[0];
                dynamic content = entry.Content;
                Assert.NotNull(content);
                Assert.NotNull(content.Key);
                Assert.NotNull(content.Key.Empty);
                Assert.Equal(1, ((IDictionary<string, object>)content.Key.Empty).Count);
                Assert.Equal("num", content.Key.Empty.Type);
                Assert.Equal(new ReadOnlyDictionary<string, Uri>(new Dictionary<string, Uri>()), feed.Links);
                Assert.Equal(new ReadOnlyCollection<Message>(new List<Message>()), feed.Messages);
            }
        }

        #region Privates/internals

        static readonly XmlReaderSettings XmlReaderSettings = new XmlReaderSettings()
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true
        };

        #endregion
    }
}
