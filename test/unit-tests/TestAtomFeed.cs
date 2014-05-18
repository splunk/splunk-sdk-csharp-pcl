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
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class TestAtomFeed
    {
        [Trait("unit-test", "AtomFeed: operation: read")]
        [Fact]
        public async Task CanReadAtomFeed()
        {
            using (var stream = new FileStream(AtomFeedPath, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, XmlReaderSettings);
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(reader);
            }
        }

        [Trait("unit-test", "AtomEntry: operation: read")]
        [Fact]
        public async Task CanReadAtomEntry()
        {
            var expected = "AtomEntry(Title=search *, Author=admin, Id=https://localhost:8089/services/search/jobs/1392687998.313, Published=2/17/2014 5:46:39 PM, Updated=2/17/2014 5:46:39 PM)";
            var stream = new FileStream(AtomFeedPath, FileMode.Open, FileAccess.Read);
            var reader = XmlReader.Create(stream, XmlReaderSettings);

            bool result = await reader.ReadToFollowingAsync("entry");
            Assert.True(result);

            var entry = new AtomEntry();
            await entry.ReadXmlAsync(reader);
            Assert.Equal(expected, entry.ToString());
        }

        static readonly string AtomFeedPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "data", "Client", "AtomFeed.xml"));

        static readonly XmlReaderSettings XmlReaderSettings = new XmlReaderSettings()
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true
        };
    }
}
