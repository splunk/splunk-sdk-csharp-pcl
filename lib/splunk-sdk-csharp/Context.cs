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
    /// Provides a class for sending HTTP requests and receiving HTTP responses 
    /// from a Splunk server.
    /// </summary>
    public sealed class Context
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class 
        /// with a protocol, host, and port number.
        /// </summary>
        /// <param name="protocol">The <see cref="Protocol"/> used to communiate 
        /// with <see cref="Host"/></param>
        /// <param name="host">The DNS name of a Splunk server instance</param>
        /// <param name="port">The port number used to communicate with 
        /// <see cref="Host"/></param>
        public Context(Scheme protocol, string host, int port)
        {
            this.Initialize(protocol, host, port, null, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class 
        /// with a protocol, host, and port number.
        /// </summary>
        /// <param name="protocol">The <see cref="Protocol"/> used to communiate 
        /// with <see cref="Host"/></param>
        /// <param name="host">The DNS name of a Splunk server instance</param>
        /// <param name="port">The port number used to communicate with
        /// <see cref="Host"/></param>
        /// <param name="handler"></param>
        /// <param name="disposeHandler"></param>
        public Context(Scheme protocol, string host, int port, HttpMessageHandler handler, bool disposeHandler = true)
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
        /// Gets the management port number used to communicate with the 
        /// <see cref="Host"/> associated with this instance.
        /// </summary>
        public int Port
        { get; private set; }

        /// <summary>
        /// Gets the protocol used to communicate with the <see cref="Host"/> 
        /// associated with this instance.
        /// </summary>
        public Scheme Protocol
        { get; private set; }

        /// <summary>
        /// Gets the session key associated with this instance.
        /// </summary>
        /// <remarks>
        /// The value returned is null until <see cref="Login"/> is successfully
        /// completed.
        /// </remarks>
        public string SessionKey
        { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<XDocument> GetDocumentAsync(Namespace @namespace, ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            Contract.Requires(@namespace != null);
            Contract.Requires(resource != null);

            var request = new HttpRequestMessage(HttpMethod.Get, this.CreateServicesUri(@namespace, resource, parameters));

            if (this.SessionKey != null)
            {
                request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
            }

            HttpResponseMessage response = await this.client.SendAsync(request);
            return await this.ReadDocument(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<XDocument> GetDocumentAsync(ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            return await this.GetDocumentAsync(Namespace.Default, resource, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Stream> GetDocumentStreamAsync(Namespace @namespace, ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            Contract.Requires(@namespace != null);
            Contract.Requires(resource != null);

            var request = new HttpRequestMessage(HttpMethod.Get, this.CreateServicesUri(@namespace, resource, parameters));

            if (this.SessionKey != null)
            {
                request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
            }

            HttpResponseMessage response = await this.client.SendAsync(request);
            return await this.ReadDocumentStream(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Stream> GetDocumentStreamAsync(ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return await this.GetDocumentStreamAsync(Namespace.Default, resource, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<XDocument> Post(Namespace @namespace, ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, this.CreateServicesUri(@namespace, resource, null))
            {
                Content = this.CreateContent(parameters) 
            };

            if (this.SessionKey != null)
            {
                request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
            }

            HttpResponseMessage response = await this.client.SendAsync(request);
            return await this.ReadDocument(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<XDocument> PostAsync(ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return await Post(Namespace.Default, resource, parameters);
        }

        public override string ToString()
        {
            return string.Concat(Scheme[(int)this.Protocol], "://", this.Host, ":", this.Port.ToString());
        }

        #endregion

        #region Privates

        static readonly string[] Scheme = { "http", "https" };
        HttpClient client;

        StringContent CreateContent(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
            {
                return new StringContent(string.Empty);
            }

            var body = string.Join("&", 
                from parameter in parameters select string.Join("=", 
                    Uri.EscapeDataString(parameter.Key), Uri.EscapeDataString(parameter.Value.ToString())));
            
            var stringContent = new StringContent(body);
            return stringContent;
        }

        IEnumerable<Message> CreateMessages(XDocument document)
        {
            var messages = from element in document.Element("response").Element("messages").Elements() select new Message(element);
            return messages;
        }

        Uri CreateServicesUri(Namespace @namespace, ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var builder = new StringBuilder(this.ToString());

            builder.Append("/");
            builder.Append(@namespace.ToString());
            builder.Append("/");
            builder.Append(resource.ToString());
            
            if (parameters == null)
                return new Uri(builder.ToString());

            builder.Append('?');

            foreach (var parameter in parameters)
            {
                builder.Append(Uri.EscapeUriString(parameter.Key));
                builder.Append('=');
                builder.Append(Uri.EscapeUriString(parameter.Value.ToString()));
                builder.Append('&');
            }

            builder.Length = builder.Length - 1; // Removes trailing '&'

            return new Uri(builder.ToString());
        }

        Uri CreateServicesUri(ResourceName resource, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return CreateServicesUri(Namespace.Default, resource, parameters);
        }

        void Initialize(Scheme protocol, string host, int port, HttpMessageHandler handler, bool disposeHandler)
        {
            Contract.Requires(!string.IsNullOrEmpty(host));
            Contract.Requires(0 <= port && port <= 65535);
            
            this.Protocol = protocol;
            this.Host = host;
            this.Port = port;
            this.client = this.client == null ? new HttpClient() : new HttpClient(handler, disposeHandler);
        }

        async Task<XDocument> ReadDocument(HttpResponseMessage response)
        {
            var document = XDocument.Parse(await response.Content.ReadAsStringAsync());

            if (!response.IsSuccessStatusCode)
            {
                throw new RequestException(response.StatusCode, response.ReasonPhrase, details: this.CreateMessages(document));
            }
            return document;
        }

        async Task<Stream> ReadDocumentStream(HttpResponseMessage response)
        {
            Stream documentStream = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                var document = XDocument.Load(documentStream);
                throw new RequestException(response.StatusCode, response.ReasonPhrase, details: this.CreateMessages(document));
            }

            return documentStream;
        }

        #endregion
    }
}
