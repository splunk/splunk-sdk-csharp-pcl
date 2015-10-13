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
    using System.Collections.Immutable;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Xunit;

    public class TestSearchPreviewStream
    {
        [Trait("unit-test", "Splunk.Client.SearchPreviewStream")]
        [Fact]
        async Task CanHandleInFlightErrorsReportedBySplunk()
        {
            var path = Path.Combine(TestAtomFeed.Directory, "Service.ExportSearchPreviews-failure.xml");
            var message = new HttpResponseMessage(HttpStatusCode.OK);

            message.Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read));
            SearchPreviewStream stream = null;

            try
            {
                stream = await SearchPreviewStream.CreateAsync(message);
                int count = 0;

                foreach (var preview in stream)
                {
                    ++count;
                }

                Assert.False(true, "Expected RequestException");
            }
            catch (RequestException e)
            {
                Assert.Equal(e.Message, "Fatal: JournalSliceDirectory: Cannot seek to 0");
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }
    }
}
