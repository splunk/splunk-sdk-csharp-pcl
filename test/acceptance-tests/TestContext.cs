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

namespace Splunk.Client.AcceptanceTests
{
    using Splunk.Client.Helpers;
    using Xunit;

    public class TestContext
    {
        [Trait("acceptance-test", "Splunk.Client.Context")]
        [MockContext]
        [Fact]
        public void CanConstructContext()
        {
            client = new Context(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port);

            Assert.Equal(client.Scheme, Scheme.Https);
            Assert.Equal(client.Host.ToLower(), SdkHelper.Splunk.Host);
            Assert.Equal(client.Port, SdkHelper.Splunk.Port);
            Assert.Null(client.SessionKey);

            Assert.Equal(client.ToString().ToLower(), string.Format("https://{0}:{1}", SdkHelper.Splunk.Host.ToLower(), SdkHelper.Splunk.Port));
        }

        static Context client;
       
    }
}
