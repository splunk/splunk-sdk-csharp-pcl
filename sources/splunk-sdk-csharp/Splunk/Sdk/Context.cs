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

// TODO:
// [ ] Synchronization strategy
// [X] Settable SessionKey
// [X] Dead code removal
// [X] Contracts
// [O] Documentation
// [O] All unsealed classes that implement IDisposable must also implement 
//     this method: protected virtual void Dispose(bool);
//     See [Implementing a Dispose Method](http://goo.gl/VPIovn)
//     All sealed classes that implement IDisposable must also implemnt this
//     method: void Dispose(bool);
//     See [GC.SuppressFinalize Method](http://goo.gl/XiI3HZ) and note the
//     private void Dispose(bool disposing) implementation.

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Provides a class for sending HTTP requests and receiving HTTP responses 
    /// from a Splunk server.
    /// </summary>
    public class Context : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class with
        /// a protocol, host, and port number.
        /// </summary>
        /// <param name="protocol">
        /// The <see cref="Scheme"/> used to communicate with <see cref="Host"/>
        /// </param>
        /// <param name="host">
        /// The DNS name of a Splunk server instance.
        /// </param>
        /// <param name="port">
        /// The port number used to communicate with <see cref="Host"/>.
        /// </param>
        public Context(Scheme protocol, string host, int port)
            : this(protocol, host, port, null)
        {
            // NOTE: This constructor obviates the need for callers to include a 
            // using for System.Net.Http.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class with
        /// a protocol, host, port number, and optional message handler.
        /// </summary>
        /// <param name="scheme">
        /// The <see cref="Scheme"/> used to communicate with <see cref="Host"/>
        /// </param>
        /// <param name="host">
        /// The DNS name of a Splunk server instance
        /// </param>
        /// <param name="port">
        /// The port number used to communicate with <see cref="Host"/>
        /// </param>
        /// <param name="handler">
        /// The <see cref="HttpMessageHandler"/> responsible for processing the HTTP 
        /// response messages.
        /// </param>
        /// <param name="disposeHandler">
        /// <c>true</c> if the inner handler should be disposed of by Dispose, 
        /// <c>false</c> if you intend to reuse the inner handler.
        /// </param>
        public Context(Scheme scheme, string host, int port, HttpMessageHandler handler, bool disposeHandler = true)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!(scheme == Scheme.Http || scheme == Scheme.Https));
            Contract.Requires(!string.IsNullOrEmpty(host));
            Contract.Requires(0 <= port && port <= 65535);

            this.Scheme = scheme;
            this.Host = host;
            this.Port = port;
            this.httpClient = handler == null ? new HttpClient() : new HttpClient(handler, disposeHandler);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Splunk host associated with this instance.
        /// </summary>
        public string Host
        { get; private set; }

        /// <summary>
        /// Gets the management port number used to communicate with 
        /// <see cref="Host"/>
        /// </summary>
        public int Port
        { get; private set; }

        /// <summary>
        /// Gets the scheme used to communicate with <see cref="Host"/> 
        /// </summary>
        public Scheme Scheme
        { get; private set; }

        /// <summary>
        /// Gets or sets the session key used by the <see cref="Context"/>.
        /// </summary>
        /// <remarks>
        /// The value returned is null until it is set.
        /// </remarks>
        public string SessionKey
        { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources used by the <see cref="Context"/>.
        /// </summary>
        /// <remarks>
        /// Do not override this method. Override 
        /// <see cref="Context.Dispose(bool disposing)"/> instead.
        /// </remarks>

        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Context"/>.
        /// </summary>
        /// <remarks>
        /// Subclasses should implement the disposable pattern as follows:
        /// <list type="bullet">
        /// <item><description>
        ///     Override this method and call it from the override.
        ///     </description></item>
        /// <item><description>
        ///     Provide a finalizer, if needed, and call this method from it.
        ///     </description></item>
        /// <item><description>
        ///     To help ensure that resources are always cleaned up 
        ///     appropriately, ensure that the override is callable multiple
        ///     times without throwing an exception.
        ///     </description></item>
        /// </list>
        /// There is no performance benefit in overriding this method on types
        /// that use only managed resources (such as arrays) because they are 
        /// automatically reclaimed by the garbage collector. See 
        /// <a href="http://goo.gl/VPIovn">Implementing a Dispose Method</a>.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.httpClient != null)
            {
                this.httpClient.Dispose();
                this.httpClient = null;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="argumentSets"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(Namespace @namespace, ResourceName resource, params IEnumerable<KeyValuePair<string, object>>[] argumentSets)
        {
            Contract.Requires<ArgumentNullException>(@namespace != null);
            Contract.Requires<ArgumentNullException>(resource != null);

            using (var request = new HttpRequestMessage(HttpMethod.Get, this.CreateServiceUri(@namespace, resource, argumentSets)))
            {
                if (this.SessionKey != null)
                {
                    request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
                }
                HttpResponseMessage response = await this.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="argumentSets"></param>
        /// <returns></returns>
        public async Task<XDocument> GetDocumentAsync(Namespace @namespace, ResourceName resource, params IEnumerable<KeyValuePair<string, object>>[] argumentSets)
        {
            using (var response = await this.GetAsync(@namespace, resource, argumentSets))
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new RequestException(response.StatusCode, response.ReasonPhrase, new Message(MessageType.Warning, string.Format("Resource '{0}/{1}' is not ready.", @namespace, resource)));
                }

                var document = XDocument.Parse(await response.Content.ReadAsStringAsync());

                if (!response.IsSuccessStatusCode)
                {
                    throw new RequestException(response.StatusCode, response.ReasonPhrase, document);
                }

                return document;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="resource"></param>
        /// <param name="argumentSets"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(Namespace @namespace, ResourceName resource, params IEnumerable<KeyValuePair<string, object>>[] argumentSets)
        {
            Contract.Requires<ArgumentNullException>(@namespace != null);
            Contract.Requires<ArgumentNullException>(resource != null);

            using (var request = new HttpRequestMessage(HttpMethod.Post, this.CreateServiceUri(@namespace, resource, null)) { Content = this.CreateStringContent(argumentSets) })
            {
                if (this.SessionKey != null)
                {
                    request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
                }
                HttpResponseMessage response = await this.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                return response;
            }
        }

        public override string ToString()
        {
            return string.Concat(SchemeStrings[(int)this.Scheme], "://", this.Host, ":", this.Port.ToString());
        }

        #endregion

        #region Privates

        static readonly string[] SchemeStrings = { "http", "https" };
        HttpClient httpClient;

        HttpClient HttpClient
        {
            get
            {
                if (httpClient == null)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                return this.httpClient;
            }
        }

        Uri CreateServiceUri(Namespace @namespace, ResourceName resource, params IEnumerable<KeyValuePair<string, object>>[] argumentSets)
        {
            var builder = new StringBuilder(this.ToString());

            builder.Append("/");
            builder.Append(@namespace.ToString());
            builder.Append("/");
            builder.Append(resource.ToString());

            if (argumentSets == null)
                return new Uri(builder.ToString());

            builder.Append('?');

            foreach (var args in argumentSets)
            {
                if (args == null)
                {
                    continue;
                }
                foreach (var arg in args)
                {
                    builder.Append(Uri.EscapeUriString(arg.Key));
                    builder.Append('=');
                    builder.Append(Uri.EscapeUriString(arg.Value.ToString()));
                    builder.Append('&');
                }
            }

            builder.Length = builder.Length - 1; // Removes trailing '&'
            return new Uri(builder.ToString());
        }

        StringContent CreateStringContent(params IEnumerable<KeyValuePair<string, object>>[] argumentSets)
        {
            if (argumentSets == null)
            {
                return new StringContent(string.Empty);
            }

            var body = string.Join("&",
                from args in argumentSets
                where args != null
                from arg in args
                select string.Join("=", Uri.EscapeDataString(arg.Key), Uri.EscapeDataString(arg.Value.ToString())));

            var stringContent = new StringContent(body);
            return stringContent;
        }

        #endregion
    }
}
