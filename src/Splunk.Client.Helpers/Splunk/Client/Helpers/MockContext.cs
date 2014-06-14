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
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for faking HTTP requests and responses from a Splunk server.
    /// </summary>
    public class MockContext : Context
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MockContext"/> class with a 
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
        public MockContext(Scheme protocol, string host, int port)
            : base(protocol, host, port, CreateMessageHandler())
        { }

        #endregion

        #region Properties

        public static string CallerId
        { get; private set; }

        public static bool IsEnabled
        {
            get { return isEnabled; }
        }

        public static bool IsRecording
        {
            get { return isRecording; }
        }

        public static string RecordingDirectoryName
        {
            get { return recordingDirectoryName; }
        }

        public static string RecordingFilename
        { get; private set; }

        #endregion

        #region Methods

        public static void Begin(string callerId)
        {
            Contract.Requires<ArgumentNullException>(callerId != null);
            CallerId = callerId;

            if (IsEnabled)
            {
                RecordingFilename = Path.Combine(recordingDirectoryName, string.Join(".", CallerId, "xml"));

                if (IsRecording)
                {
                    Recordings = new Queue<Recording>();
                }
                else
                {
                    var serializer = new DataContractSerializer(typeof(Queue<Recording>));

                    using (var stream = new FileStream(RecordingFilename, FileMode.Open))
                    {
                        Recordings = (Queue<Recording>)serializer.ReadObject(stream);
                    }
                }
            }
        }

        public static void End()
        {
            if (IsEnabled)
            {
                if (IsRecording)
                {
                    var serializer = new DataContractSerializer(typeof(Queue<Recording>));
                    Directory.CreateDirectory(RecordingDirectoryName);

                    using (var stream = new FileStream(RecordingFilename, FileMode.Create))
                    {
                        serializer.WriteObject(stream, Recordings/*.ToList()*/);
                    }
                }
                else
                {
                    Debug.Assert(Recordings.Count == 0);
                }

                Recordings = null;
            }
        }

        #endregion

        #region Privates/internals

        static readonly string recordingDirectoryName;
        static readonly bool isEnabled = bool.Parse(ConfigurationManager.AppSettings["MockContext.IsEnabled"]);
        static readonly bool isRecording = bool.Parse(ConfigurationManager.AppSettings["MockContext.IsRecording"]);

        static Queue<Recording> Recordings;

        static MockContext()
        {
            recordingDirectoryName = ConfigurationManager.AppSettings["MockContext.RecordingDirectoryName"] ?? 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recordings");
        }

        static HttpMessageHandler CreateMessageHandler()
        {
            if (IsEnabled)
            {
                var handler = IsRecording ? (HttpMessageHandler)new Recorder() : (HttpMessageHandler)new Player();
                return handler;
            }

            return null;
        }

        #endregion

        #region Types

        abstract class MessageHandler : HttpMessageHandler
        {
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

            protected static async Task<Tuple<string, long>> ComputeChecksum(HttpRequestMessage request, MemoryStream stream)
            {
                Contract.Requires<ArgumentNullException>(request != null);
                Contract.Requires<ArgumentNullException>(stream != null);

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

                return new Tuple<string, long>(checksum, offset);
            }

            protected static async Task<Tuple<string, HttpRequestMessage>> DuplicateAndComputeChecksum(
                HttpRequestMessage request)
            {
                Contract.Requires<ArgumentNullException>(request != null);
                var stream = new MemoryStream();

                try
                {
                    var result = await ComputeChecksum(request, stream);
                    string checksum = result.Item1;
                    long offset = result.Item2;

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

            #endregion
        }

        class Player : MessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var recording = Recordings.Dequeue();

                using (var stream = new MemoryStream())
                {
                    var result = await ComputeChecksum(request, stream);
                    string checksum = result.Item1;

                    if (checksum != recording.Checksum)
                    {
                        string text = string.Format("The recording in {0} is out of sync with {1}.", 
                            MockContext.recordingDirectoryName,
                            MockContext.CallerId);
                        throw new InvalidOperationException(text);
                    }
                }

                var response = recording.Response;
                response.RequestMessage = request;

                return response;
            }
        }

        class Recorder : MessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var tuple = await DuplicateAndComputeChecksum(request);
                var checksum = tuple.Item1;

                using (var forward = tuple.Item2)
                {
                    using (var response = await this.Client.SendAsync(forward, cancellationToken))
                    {
                        var recording = await Recording.CreateRecording(checksum, response);

                        recording.Response.RequestMessage = request;
                        MockContext.Recordings.Enqueue(recording);

                        return recording.Response;
                    }
                }
            }
        }

        [DataContract]
        class Recording
        {
            public string Checksum
            {
                get { return this.checksum; }
            }

            public HttpResponseMessage Response
            {
                get
                {
                    if (this.response == null)
                    {
                        lock (this.gate)
                        {
                            if (this.response == null)
                            {
                                var response = new HttpResponseMessage(this.statusCode)
                                {
                                    Content = new StreamContent(new MemoryStream(this.content)),
                                    ReasonPhrase = this.reasonPhrase,
                                    Version = this.version
                                };

                                foreach (var header in this.headers)
                                {
                                    response.Headers.Add(header.Key, header.Value);
                                }

                                foreach (var header in this.contentHeaders)
                                {
                                    response.Content.Headers.Add(header.Key, header.Value);
                                }

                                this.response = response;
                            }
                        }
                    }

                    return this.response;
                }
            }

            public static async Task<Recording> CreateRecording(string checksum, HttpResponseMessage response)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var recording = new Recording(response, content, checksum);

                return recording;
            }

            #region Privates/internals

            [DataMember(Name = "Checksum", IsRequired = true)]
            readonly string checksum;

            [DataMember(Name = "Response.Content", IsRequired = true)]
            readonly byte[] content;

            [DataMember(Name = "Response.Content.Headers", IsRequired = true)]
            readonly List<KeyValuePair<string, IEnumerable<string>>> contentHeaders;

            [DataMember(Name = "Response.Headers", IsRequired = true)]
            readonly List<KeyValuePair<string, IEnumerable<string>>> headers;

            [DataMember(Name = "Response.ReasonPhrase", IsRequired = true)]
            readonly string reasonPhrase;

            [DataMember(Name = "Response.StatusCode", IsRequired = true)]
            readonly HttpStatusCode statusCode;

            [DataMember(Name = "Response.Version", IsRequired = true)]
            readonly Version version;

            [DataMember]
            object gate = new object();

            HttpResponseMessage response;
            
            Recording()
            { }

            Recording(HttpResponseMessage response, byte[] content, string checksum)
            {
                Contract.Requires<ArgumentNullException>(response != null);
                Contract.Requires<ArgumentNullException>(content != null);
                Contract.Requires<ArgumentNullException>(checksum != null);

                this.checksum = checksum;
                this.content = content;
                this.contentHeaders = response.Content.Headers.ToList();
                this.headers = response.Headers.ToList();
                this.reasonPhrase = response.ReasonPhrase;
                this.statusCode = response.StatusCode;
                this.version = response.Version;
            }

            #endregion
        }

        #endregion
    }
}
