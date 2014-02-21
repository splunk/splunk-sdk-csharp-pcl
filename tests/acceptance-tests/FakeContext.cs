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

namespace Splunk.Sdk.UnitTesting
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Splunk.Sdk;
    
    /// <summary>
    /// Provides a class for faking HTTP requests and responses from a Splunk server.
    /// </summary>
    public class FakeContext : Splunk.Sdk.Context
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeContext"/> class 
        /// with a protocol, host, and port number.
        /// </summary>
        /// <param name="protocol">The <see cref="Protocol"/> used to communiate 
        /// with <see cref="Host"/></param>
        /// <param name="host">The DNS name of a Splunk server instance</param>
        /// <param name="port">The port number used to communicate with 
        /// <see cref="Host"/>.</param>
        public FakeContext(Scheme protocol, string host, int port)
            : base(protocol, host, port, new MessageHandler())
        { }

        #endregion

        #region Types

        class MessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion
    }
}
