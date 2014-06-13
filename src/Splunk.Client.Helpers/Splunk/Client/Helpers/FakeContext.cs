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

namespace Splunk.Client.Helpers
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
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
        public FakeContext(Scheme protocol, string host, int port, string callerId)
            : base(protocol, host, port, CreateMessageHandler(callerId))
        {
            Contract.Requires<ArgumentNullException>(callerId != null);
        }


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

        #region Privates/internals

        static readonly Dictionary<string, Queue<Recording>> Recordings = new Dictionary<string, Queue<Recording>>();

        static readonly string filePath = ConfigurationManager.AppSettings["MockContext.FilePath"];
        static readonly bool isEnabled = bool.Parse(ConfigurationManager.AppSettings["MockContext.IsEnabled"]);
        static readonly bool isRecording = bool.Parse(ConfigurationManager.AppSettings["MockContext.IsRecording"]);

        static HttpMessageHandler CreateMessageHandler(string callerId)
        {
            var handler = FakeContext.isRecording
                ? (HttpMessageHandler)new Recorder(callerId)
                : (HttpMessageHandler)new Player(callerId);

            return handler;
        }

        #endregion

        #region Types

        abstract class MessageHandler : HttpMessageHandler
        {
            public MessageHandler(string callerId)
            {
                this.callerId = callerId;
            }

            public string CallerId
            {
                get { return this.callerId; }
            }

            public HttpClient Client
            {
                get { return this.client; }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.client.Dispose();
                    base.Dispose(true);
                }
            }

            protected static async Task<Tuple<string, HttpRequestMessage>> DuplicateRequestAndComputeChecksum(
                HttpRequestMessage request)
            {
                Contract.Requires<ArgumentNullException>(request != null);
                var stream = new MemoryStream();

                try
                {
                    string text;
                    byte[] bytes;

                    text = string.Format("{0} {1} HTTP/{2}", request.Method, request.RequestUri, request.Version);
                    bytes = Encoding.UTF8.GetBytes(text);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Write(crlf, 0, crlf.Length);

                    foreach (var header in request.Headers)
                    {
                        text = string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value));
                        bytes = Encoding.UTF8.GetBytes(text);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Write(crlf, 0, crlf.Length);
                    }

                    if (request.Content != null)
                    {
                        foreach (var header in request.Content.Headers)
                        {
                            text = string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value));
                            bytes = Encoding.UTF8.GetBytes(text);
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Write(crlf, 0, crlf.Length);
                        }
                    }

                    stream.Write(crlf, 0, crlf.Length);
                    long offset = stream.Position;

                    if (request.Content != null)
                    {
                        await request.Content.CopyToAsync(stream);
                    }

                    var hashAlgorithm = SHA512.Create();
                    stream.Seek(0, SeekOrigin.Begin);

                    byte[] hash = hashAlgorithm.ComputeHash(stream);

                    var builder = hash.Aggregate(new StringBuilder(2 * hash.Length), (b, x) =>
                        {
                            return b.Append(x.ToString("x2"));
                        });

                    var checksum = builder.ToString();
                    StreamContent content = null;

                    if (request.Content == null)
                    {
                        stream.Close();
                    }
                    else
                    {
                        content = new StreamContent(stream);

                        foreach (var header in request.Content.Headers)
                        {
                            content.Headers.Add(header.Key, header.Value);
                        }

                        content.Headers.ContentLength = request.Content.Headers.ContentLength;
                        stream.Seek(offset, SeekOrigin.Begin);
                    }

                    var duplicateRequest = new HttpRequestMessage(request.Method, request.RequestUri)
                        {
                            Content = content,
                            Version = request.Version
                        };

                    foreach (var header in request.Headers)
                    {
                        duplicateRequest.Headers.Add(header.Key, header.Value);
                    }

                    return new Tuple<string, HttpRequestMessage>(checksum, duplicateRequest);
                }
                catch
                {
                    stream.Dispose();
                    throw;
                }
            }

            #region Privates/internals

            static readonly byte[] crlf = new byte[] { 0x0D, 0x0A };
            readonly HttpClient client = new HttpClient();
            readonly string callerId;

            #endregion
        }

        class Player : MessageHandler
        {
            public Player(string callerId)
                : base(callerId)
            {
                this.recordings = Recordings[this.CallerId];
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await this.Client.SendAsync(request, cancellationToken);
                return response;
            }

            #region Privates/internals

            readonly Queue<Recording> recordings;

            #endregion
        }

        class Recorder : MessageHandler
        {
            public Recorder(string callerId)
                : base(callerId)
            {
                if (Recordings.TryGetValue(this.CallerId, out this.recordings))
                {
                    this.recordings = new Queue<Recording>();
                    Recordings.Add(callerId, this.recordings);
                }
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var tuple = await DuplicateRequestAndComputeChecksum(request);
                var checksum = tuple.Item1;
                var forward = tuple.Item2;

                var response = await this.Client.SendAsync(forward, cancellationToken);
                var headers = new Dictionary<string, List<string>>();

                foreach (var header in response.Headers)
                {
                    headers[header.Key] = header.Value.ToList();
                }

                var content = await response.Content.ReadAsStringAsync();

                var contentHeaders = new Dictionary<string, List<string>>();

                foreach (var header in response.Content.Headers)
                {
                    contentHeaders[header.Key] = header.Value.ToList();
                }

                return response;
            }

            #region Privates/internals

            readonly Queue<Recording> recordings;

            #endregion
        }

        struct Recording
        {
            public Recording(string checksum, string response)
            {
                this.Checksum = checksum;
                this.Response = response;
            }

            public readonly string Checksum;
            public readonly string Response;
        }

        #endregion
    }
}
