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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Provides a class for sending HTTP requests and receiving HTTP responses from a Splunk server.
    /// </summary>
    public sealed class SplunkClient
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SplunkClient"/> class with a protocol, host, and port number.
        /// </summary>
        /// <param name="protocol">The <see cref="Protocol"/> used to communiate with <see cref="Host"/></param>
        /// <param name="host">The DNS name of a Splunk server instance</param>
        /// <param name="port">The port number used to communicate with <see cref="Host"/></param>
        public SplunkClient(Protocol protocol, string host, int port)
        {
            this.Initialize(protocol, host, port, null, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplunkClient"/> class with a protocol, host, and port number.
        /// </summary>
        /// <param name="protocol">The <see cref="Protocol"/> used to communiate with <see cref="Host"/></param>
        /// <param name="host">The DNS name of a Splunk server instance</param>
        /// <param name="port">The port number used to communicate with <see cref="Host"/></param>
        /// <param name="handler"></param>
        /// <param name="disposeHandler"></param>
        public SplunkClient(Protocol protocol, string host, int port, HttpMessageHandler handler, bool disposeHandler = true)
        {
            Contract.Requires<ArgumentNullException>(handler != null);
            Initialize(protocol, host, port, handler, disposeHandler);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the host associated with this instance.
        /// </summary>
        public string Host
        { get; private set; }

        /// <summary>
        /// Gets the management port number used to communicate with the <see cref="Host"/> associated with this instance.
        /// </summary>
        public int Port
        { get; private set; }

        /// <summary>
        /// Gets the protocol used to communicate with the <see cref="Host"/> associated with this instance.
        /// </summary>
        public Protocol Protocol
        { get; private set; }

        /// <summary>
        /// Gets the session key associated with this instance.
        /// </summary>
        /// <remarks>
        /// The value returned is null until <see cref="Login"/> is successfully completed.
        /// </remarks>
        public string SessionKey
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Provides user authentication.
        /// </summary>
        /// <param name="username">The Splunk account username.</param>
        /// <param name="password">The password for the user specified with username.</param>
        /// <remarks>This method uses the Splunk <a href="http://goo.gl/yXJX75">auth/login</a> 
        /// endpoint. The session key that it returns is used for subsequent requests. It is
        /// accessible via the <see cref="SessionKey"/> property.
        /// </remarks>
        public async Task Login(string username, string password)
        {
            Contract.Requires(username != null);
            Contract.Requires(password != null);

            using (var content = new StringContent(string.Format("username={0}&password={1}", username, password)))
            {
                HttpResponseMessage response = await this.client.PostAsync(this.CreateUri(new string[] { "auth", "login" }), content);
                XDocument document = this.ReadDocument(response).Result;
                this.SessionKey = document.Element("response").Element("sessionKey").Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<XDocument> GetDocument(Namespace @namespace, string[] resource, IDictionary<string, object> parameters = null)
        {
            Contract.Requires(@namespace != null);
            Contract.Requires(resource != null);

            HttpResponseMessage response = await this.client.GetAsync(this.CreateUri(@namespace, resource, parameters));
            return this.ReadDocument(response).Result;
        }

        public async Task<XDocument> GetDocument(string[] resource, IDictionary<string, object> parameters = null)
        {
            Contract.Requires(resource != null);

            HttpResponseMessage response = await this.client.GetAsync(this.CreateUri(resource, parameters));
            return this.ReadDocument(response).Result;
        }

        public async Task<Stream> GetDocumentStream(Namespace @namespace, string[] resource, IDictionary<string, object> parameters)
        {
            Contract.Requires(@namespace != null);
            Contract.Requires(resource != null);

            HttpResponseMessage response = await this.client.GetAsync(this.CreateUri(@namespace, resource, parameters));
            return this.ReadDocumentStream(response).Result;
        }

        public async Task<Stream> GetDocumentStream(string[] resource, IDictionary<string, object> parameters)
        {
            Contract.Requires(resource != null);

            HttpResponseMessage response = await this.client.GetAsync(this.CreateUri(resource, parameters));
            return this.ReadDocumentStream(response).Result;
        }

        public async Task<XDocument> Post(Namespace @namespace, string[] resource, IDictionary<string, object> parameters)
        {
            HttpResponseMessage response = await this.client.PostAsync(this.CreateUri(@namespace, resource), this.CreateContent(parameters));
            return this.ReadDocument(response).Result;
        }

        public async Task<XDocument> Post(string[] resource, IDictionary<string, object> parameters)
        {
            HttpResponseMessage response = await this.client.PostAsync(this.CreateUri(resource), this.CreateContent(parameters));
            return ReadDocument(response).Result;
        }

        public override string ToString()
        {
            return string.Concat(Scheme[(int)this.Protocol], "://", this.Host, ":", this.Port.ToString());
        }

        #endregion

        #region Privates

        private static readonly string[] Scheme = { "http", "https" };
        private HttpClient client;

        private StringContent CreateContent(IDictionary<string, object> parameters)
        {
            var body = string.Join("&",
                from parameter in parameters
                select string.Join("=",
                    Uri.EscapeDataString(parameter.Key),
                    Uri.EscapeDataString(parameter.Value.ToString())));
            return new StringContent(body);
        }

        private Uri CreateUri(Namespace @namespace, string[] resource, IDictionary<string, object> parameters = null)
        {
            return this.CreateUri(new string[] { string.Empty, "servicesNS", @namespace.User, @namespace.App }.Concat(resource), parameters);
        }

        private Uri CreateUri(string[] resource, IDictionary<string, object> parameters = null)
        {
            return this.CreateUri(new string[] { string.Empty, "services" }.Concat(resource), parameters);
        }

        private Uri CreateUri(IEnumerable<string> segments, IDictionary<string, object> parameters)
        {
            var builder = new UriBuilder(Scheme[(int)this.Protocol], this.Host, this.Port);

            builder.Path = string.Join("/", from segment in segments select Uri.EscapeUriString(segment));

            if (parameters == null)
                return builder.Uri;

            builder.Query = string.Join("&", 
                from parameter in parameters 
                select string.Join("=", 
                    Uri.EscapeUriString(parameter.Key), 
                    Uri.EscapeUriString(parameter.Value.ToString())));

            return builder.Uri;
        }

        private void Initialize(Protocol protocol, string host, int port, HttpMessageHandler handler, bool disposeHandler)
        {
            this.Protocol = protocol;
            this.Host = host;
            this.Port = port;
            this.client = this.client == null ? new HttpClient() : new HttpClient(handler, disposeHandler);
        }

        private async Task<XDocument> ReadDocument(HttpResponseMessage response)
        {
            var document = XDocument.Parse(await response.Content.ReadAsStringAsync());

            if (!response.IsSuccessStatusCode)
            {
                // TODO: Parse message body into a list of messages. The message body looks like this:
                // <?xml version="1.0' encoding="UTF-8"?>\n
                // <response>
                //    <messages>    
                //        <msg type="WARN">Login failed</msg>
                //    </messages>
                // </response>
                throw new SplunkRequestException(response.StatusCode, response.ReasonPhrase, details: document);
            }
            return document;
        }

        private async Task<Stream> ReadDocumentStream(HttpResponseMessage response)
        {
            Stream stream = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                // TODO: Parse message body into a list of messages. The message body looks like this:
                // <?xml version="1.0' encoding="UTF-8"?>\n
                // <response>
                //    <messages>    
                //        <msg type="WARN">Login failed</msg>
                //    </messages>
                // </response>
                throw new SplunkRequestException(response.StatusCode, response.ReasonPhrase, details: XDocument.Load(stream));
            }

            return stream;
        }

        #endregion
    }
}
