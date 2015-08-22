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
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    #if __MonoCs__

    using Contract = Splunk.Client.Contract;

    #endif

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
        ///     <item><description>   
        ///       <a href="http://goo.gl/ppbIlm">How to Avoid Creating Real
        ///       Tasks When Unit Testing Async</a>.
        ///     </description></item>
        ///     <item><description>
        ///       <a href="http://goo.gl/YUFhAO">ObjectContent Class</a>.
        ///     </description></item>
        ///   </list>
        /// </remarks>
        /// 
        public MockContext(Scheme protocol, string host, int port, TimeSpan timeout = default(TimeSpan))
            : base(protocol, host, port, timeout, CreateMessageHandler())
        { }

        #endregion

        #region Properties

        public static string CallerId
        {
            get { return session == null ? null : session.Name; }
        }

        public static MockContextMode Mode
        {
            get { return mode; }
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
            
            RecordingFilename = Path.Combine(recordingDirectoryName, string.Join(".", callerId, "json", "gz"));

            switch (Mode)
            {
                case MockContextMode.Run:
                    session = new Session(callerId);
                    getOrElse = Noop;
                    break;

                case MockContextMode.Record:
                    session = new Session(callerId);
                    getOrElse = Enqueue;
                    break;

                case MockContextMode.Playback:

                    var serializer = new DataContractJsonSerializer(
                        typeof(Session), null, int.MaxValue, false, new JsonDataContractSurrogate(), false);

                    using (var stream = new FileStream(RecordingFilename, FileMode.Open))
                    {
                        using (var compressedStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            session = (Session)serializer.ReadObject(compressedStream);
                        }
                    }

                    getOrElse = Dequeue;
                    break;

                default: throw new InvalidOperationException();
            }
        }

        public static void End()
        {
            switch (Mode)
            {
                case MockContextMode.Run:
                    break;

                case MockContextMode.Record:

                    var serializer = new DataContractJsonSerializer(typeof(Session));

                    Directory.CreateDirectory(RecordingDirectoryName);
                    session.Data.TrimExcess();
                    session.Recordings.TrimExcess();

                    using (var stream = new FileStream(RecordingFilename, FileMode.Create))
                    {
                        using (var compressedStream = new GZipStream(stream, CompressionLevel.Optimal))
                        {
                            serializer.WriteObject(compressedStream, session);
                        }
                    }

                    break;
                
                case MockContextMode.Playback:

                    Debug.Assert(session.Data.Count == 0);
                    Debug.Assert(session.Recordings.Count == 0);
                    break;

                default: throw new InvalidOperationException();
            }
            
            session = null;
        }

        public static T GetOrElse<T>(T value)
        {
            object o = getOrElse(value);
            return (T)o;
        }

        #endregion

        #region Privates/internals

        static readonly string recordingDirectoryName;
        static readonly MockContextMode mode;
        static Func<object, object> getOrElse;
        static Session session;

        static MockContext()
        {
            string setting;

            setting = ConfigurationManager.AppSettings["MockContext.RecordingDirectoryName"];
            recordingDirectoryName = setting ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Recordings");

            setting = ConfigurationManager.AppSettings["MockContext.Mode"];
            mode = setting == null ? MockContextMode.Run : (MockContextMode)Enum.Parse(typeof(MockContextMode), setting);
        }

        static HttpMessageHandler CreateMessageHandler()
        {
            switch (Mode)
            {
                case MockContextMode.Run:
                    return null;
                case MockContextMode.Record:
                    return new Recorder();
                case MockContextMode.Playback:
                    return new Player();
                default: throw new InvalidOperationException();
            }
        }

        static object Dequeue(object o)
        {
            return session.Data.Dequeue();
        }

        static object Enqueue(object o)
        {
            //// DateTime values are serialized with millisecond precision, not
            //// at the precision of a tick, hence we must compensate.

            var dt = o as DateTime?;

            if (dt.HasValue)
            {
                o = dt.Value.AddTicks(-(dt.Value.Ticks % TimeSpan.TicksPerSecond));
            }

            session.Data.Enqueue(o);
            return o;
        }

        static object Noop(object o)
        {
            return o;
        }

        #endregion

        #region Types

        class JsonDataContractSurrogate : IDataContractSurrogate
        {
            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            {
                return null; // unused
            }

            public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
            {
                return null; // unused
            }

            public Type GetDataContractType(Type type)
            {
                return null; // unused
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                var data = obj as Queue<object>;

                if (data == null)
                {
                    return obj;
                }
                
                if (data.Count == 0)
                {
                    return obj;
                }

                var replacement = new Queue<object>(data.Count);

                foreach (var item in data)
                {
                    var enqueuedItem = item;
                    var s = item as string;

                    if (s != null)
                    {
                        var match = DateTimeFormat.Match(s);

                        if (match.Success)
                        {
                            var timeZoneDesignator = short.Parse(match.Groups[2].Value);
                            var milliseconds = long.Parse(match.Groups[1].Value);
                            var dateTime = Epoch.Add(TimeSpan.FromMilliseconds(milliseconds));

                            dateTime = dateTime.AddHours(timeZoneDesignator / 100);
                            dateTime = dateTime.AddMinutes(timeZoneDesignator % 100);

                            enqueuedItem = dateTime;
                        }
                    }

                    replacement.Enqueue(enqueuedItem);
                }

                return replacement;
            }

            public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
            {
                // unused
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                return obj; // unused
            }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            {
                return null; // unused
            }

            public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
            {
                return typeDeclaration; // unused
            }

            static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            static readonly Regex DateTimeFormat = new Regex(@"/Date\((\d+)([+-]\d+)\)/");
        }

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
                var checksum = Convert.ToBase64String(hash);

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
            readonly HttpClient client = new HttpClient(new HttpClientHandler { UseCookies = false } );

            #endregion
        }

        class Player : MessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var recording = session.Recordings.Dequeue();

                using (var stream = new MemoryStream())
                {
                    var result = await ComputeChecksum(request, stream);
                    string checksum = result.Item1;

                    if (checksum != recording.Checksum)
                    {
                        string text = string.Format(
                            "The recording in {0} is out of sync with {1}.", 
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
                        session.Recordings.Enqueue(recording);

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
                                var buffer = Convert.FromBase64String(this.content);

                                var response = new HttpResponseMessage(this.statusCode)
                                {
                                    Content = new StreamContent(new MemoryStream(buffer)),
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
            readonly string content;

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
                this.content = Convert.ToBase64String(content);
                this.contentHeaders = response.Content.Headers.ToList();
                this.headers = response.Headers.ToList();
                this.reasonPhrase = response.ReasonPhrase;
                this.statusCode = response.StatusCode;
                this.version = response.Version;
            }

            #endregion
        }

        [DataContract]
        class Session
        {
            public Session(string name)
            {
                this.Name = name;
                this.Data = new Queue<object>();
                this.Recordings = new Queue<Recording>();
            }

            [DataMember(Order = 1)]
            public string Name
            { get; private set; }

            [DataMember(Order = 2)]
            public Queue<object> Data;

            [DataMember(Order = 3)]
            public Queue<Recording> Recordings;
        }

        #endregion
    }
}
