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
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for faking HTTP requests and responses from a Splunk server.
    /// </summary>
    public class FakeContext : Context
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeContext"/> class with a 
        /// protocol, host, port number.
        /// </summary>
        /// <param name="protocol">
        /// The <see cref="Protocol"/> used to communiate with <see cref="Host"/>.
        /// </param>
        /// <param name="host">
        /// The DNS name of a Splunk server instance.
        /// </param>
        /// <param name="port">
        /// The port number used to communicate with <see cref="Host"/>.
        /// </param>
        /// <remarks>
        ///   <para><b>References</b></para>
        ///   <list type="number">
        ///     <item>
        ///       <description>   
        ///         <a href="http://goo.gl/ppbIlm">How to Avoid Creating Real
        ///         Tasks When Unit Testing Async</a>.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <description>
        ///         <a href="http://goo.gl/YUFhAO">ObjectContent Class</a>.
        ///       </description>
        ///     </item>
        ///   </list>
        /// </remarks>
        /// 
        public FakeContext(Scheme protocol, string host, int port)
            : base(protocol, host, port, new MessageHandler())
        { }

        #endregion

        #region Methods

        public void LoadResponseMessages(string path)
        {
            // Consider using the <a href="http://goo.gl/3BFVqg">JavaScriptSerializer Class</a>.
            // Consider matching the full text of an HTTP request to the elements of a response.
            // Sample JSON:
            //      [ 
            //          { Request: "full-text", Response: { 
            //              StatusCode: "status-code", ReasonPhrase: "reason-phrase", Content: {
            //                  ...
            //              }
            //          },
            //          ...
            //      ]
            throw new NotImplementedException();
        }

        #endregion

        #region Types

        class MessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    ReasonPhrase = "foo bar",
                    Content = new StringContent("foo bar")
                };
                return Task.FromResult(response);
            }
        }

        #endregion
    }
}
