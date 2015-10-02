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
    using System.Text;
    using System.Threading.Tasks;

    using Xunit;

    public class TestSearchResultStream
    {
        [Trait("unit-test", "Splunk.Client.SearchResultStream")]
        [Fact]
        async Task CanEnumerate()
        {
            var baseFileName = Path.Combine(TestAtomFeed.Directory, "TaggedSearchResults");

            using (var expectedResults = new StreamReader(baseFileName + ".expected.text", encoding: Encoding.UTF8))
            {
                var message = new HttpResponseMessage(HttpStatusCode.OK);

                message.Content = new StreamContent(new FileStream(baseFileName + ".xml", FileMode.Open, FileAccess.Read));

                using (var stream = await SearchResultStream.CreateAsync(message))
                {
                    int count = 0;

                    foreach (var observedResult in stream)
                    {
                        string expectedResult = null;
                        ++count;

                        try
                        {
                            expectedResult = await expectedResults.ReadLineAsync();
                            continue;
                        }
                        catch (Exception e)
                        {
                            Assert.False(true, string.Format("Error while reading expected results: {0}", e.Message));
                        }

                        Assert.NotNull(expectedResult);
                        Assert.Equal(expectedResult, observedResult.ToString());
                    }

                    Assert.Null(expectedResults.ReadLine());
                    Assert.Equal(count, stream.ReadCount);
                }
            }
        }
    }
}
