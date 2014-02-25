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
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    
    using Xunit;

    public class TestContext
    {
        static TestContext()
        {
            // TODO: Use WebRequestHandler.ServerCertificateValidationCallback instead
            // 1. Instantiate a WebRequestHandler
            // 2. Set its ServerCertificateValidationCallback
            // 3. Instantiate a Splunk.Sdk.Context with the WebRequestHandler

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        [Trait("class", "Context")]
        [Fact]
        public void Construct()
        {
            client = new Context(Scheme.Https, "localhost", 8089);

            Assert.Equal(client.Protocol, Scheme.Https);
            Assert.Equal(client.Host, "localhost");
            Assert.Equal(client.Port, 8089);
            Assert.Null(client.SessionKey);

            Assert.Equal(client.ToString(), "https://localhost:8089");
        }

        static Context client;
    }
}
