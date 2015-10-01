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
    using Splunk.Client;

    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Xunit;

    public class TestSearchResultStream
    {
        [Trait("unit-test", "Splunk.Client.SearchResultStream")]
        [Fact]
        async Task CanEnumerate()
        {
            var expected = new String[]
            {
                "SearchResult(name: \"thruput\" tagged \"Performance\", host: \"Dnoble-WIN10\" tagged \"owned-by-dnoble\", \"splunk-server\", " +
                "splunk_server: \"Dnoble-WIN10\" tagged \"owned-by-dnoble\", tag: [Performance, owned-by-dnoble, splunk-server], " +
                "tag::host: [owned-by-dnoble, splunk-server], tag::name: [Performance], tag::splunk_server: [owned-by-dnoble], " +
                "_bkt: _internal~129~C78009F7-61C8-4AB9-8C95-259FBF9D0C0C, _cd: 129:723662, _indextime: 1443653087, _kv: 1, " +
                "_raw: 09-30-2015 15:44:47.076 -0700 INFO  Metrics - group=thruput, name=thruput, instantaneous_kbps=0.973068, " +
                "instantaneous_eps=3.645169, average_kbps=0.387645, total_k_processed=39932.000000, kb=30.165039, ev=113.000000, " +
                "_si: [Dnoble-WIN10, _internal], _sourcetype: splunkd, _subsecond: .076, _time: 2015-09-30T15:44:47.076-07:00)",


            };

            var message = new HttpResponseMessage(HttpStatusCode.OK);
            var path = Path.Combine(TestAtomFeed.Directory, "TaggedSearchResults.xml");

            message.Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read));

            using (var stream = await SearchResultStream.CreateAsync(message))
            {
                int count = 0;

                foreach (var result in stream)
                {
                    Assert.Equal(expected[count], result.ToString());
                    ++count;
                }

                Assert.Equal(count, stream.ReadCount);
            }
        }
    }
}
