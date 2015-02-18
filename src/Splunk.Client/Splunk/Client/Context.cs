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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for sending HTTP requests and receiving HTTP responses
    /// from a Splunk server.
    /// </summary>
    /// <seealso cref="T:System.IDisposable"/>
    public class Context : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class with a
        /// protocol, host, and port number.
        /// </summary>
        /// <param name="scheme">
        /// The <see cref="Scheme"/> used to communicate with <see cref="Host"/>
        /// </param>
        /// <param name="host">
        /// The DNS name of a Splunk server instance.
        /// </param>
        /// <param name="port">
        /// The port number used to communicate with <see cref="Host"/>.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>
        public Context(Scheme scheme, string host, int port, TimeSpan timeout = default(TimeSpan))
            : this(scheme, host, port, timeout, null)
        {
            // NOTE: This constructor obviates the need for callers to include a 
            // using for System.Net.Http.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class with a
        /// protocol, host, port number, and optional message handler.
        /// </summary>
        /// <param name="scheme">
        /// The <see cref="Scheme"/> used to communicate with <see cref="Host"/>.
        /// </param>
        /// <param name="host">
        /// The DNS name of a Splunk server instance.
        /// </param>
        /// <param name="port">
        /// The port number used to communicate with <see cref="Host"/>.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="handler">
        /// The <see cref="HttpMessageHandler"/> responsible for processing the HTTP
        /// response messages.
        /// </param>
        /// <param name="disposeHandler">
        /// <c>true</c> if the inner handler should be disposed of by Dispose,
        /// <c>false</c> if you intend to reuse the inner handler.
        /// </param>
        /// <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>
        public Context(Scheme scheme, string host, int port, TimeSpan timeout, HttpMessageHandler handler, bool disposeHandler = true)
        {
            Contract.Requires<ArgumentException>(scheme == Scheme.Http || scheme == Scheme.Https);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(host));
            Contract.Requires<ArgumentException>(0 <= port && port <= 65535);

            this.Scheme = scheme;
            this.Host = host;
            this.Port = port;
            this.httpClient = handler == null ? new HttpClient() : new HttpClient(handler, disposeHandler);
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "splunk-sdk-csharp/2.0");

            if (timeout != default(TimeSpan))
            {
                this.httpClient.Timeout = timeout;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Splunk host name associated with the current
        /// <see cref= "Context"/>.
        /// </summary>
        /// <value>
        /// A Splunk host name.
        /// </value>
        public string Host
        { get; private set; }

        /// <summary>
        /// Gets the management port number used to communicate with
        /// <see cref= "Host"/>
        /// </summary>
        /// <value>
        /// A Splunk management port number.
        /// </value>
        public int Port
        { get; private set; }

        /// <summary>
        /// Gets the scheme used to communicate with <see cref="Host"/> on
        /// <see cref="Port"/>.
        /// </summary>
        /// <value>
        /// The scheme used to communicate with <see cref="Host"/> on
        /// <see cref="Port"/>.
        /// </value>
        public Scheme Scheme
        { get; private set; }

        /// <summary>
        /// Gets or sets the session key used by the current <see cref= "Context"/>.
        /// </summary>
        /// <remarks>
        /// This value is <c>null</c> until it is set.
        /// </remarks>
        /// <value>
        /// The session key used by the current <see cref="Context"/> or <c>
        /// null</c>.
        /// </value>
        public string SessionKey
        { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all disposable resources used by the current
        /// <see cref= "Context"/>.
        /// </summary>
        /// <remarks>
        /// Do not override this method. Override <see cref="Dispose(bool)"/>
        /// instead.
        /// </remarks>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this); // Enables derivatives that introduce finalizers from needing to reimplement
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Context"/>.
        /// </summary>
        /// <remarks>
        /// Subclasses should implement the disposable pattern as follows:
        /// <list type="bullet">
        /// <item><description>
        ///   Override this method and call it from the override.
        /// </description></item>
        /// <item><description>
        ///   Provide a finalizer, if needed, and call this method from it.
        /// </description></item>
        /// <item><description>
        ///   To help ensure that resources are always cleaned up appropriately,
        ///   ensure that the override is callable multiple times without throwing an
        ///   exception.
        /// </description></item>
        /// </list>
        /// There is no performance benefit in overriding this method on types that
        /// use only managed resources (such as arrays) because they are
        /// automatically reclaimed by the garbage collector. See
        /// <a href="http://goo.gl/VPIovn">Implementing a Dispose Method</a>.
        /// </remarks>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.httpClient != null)
            {
                this.httpClient.Dispose();
                this.httpClient = null;
            }
        }

        /// <summary>
        /// Sends a DELETE request as an asynchronous operation.
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// 
        /// </param>
        /// <param name="argumentSets">
        /// 
        /// </param>
        /// <returns>
        /// The response to the DELETE request.
        /// </returns>
        public virtual async Task<Response> DeleteAsync(Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Delete, ns, resource, null, token, argumentSets).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Sends a GET request as an asynchronous operation.
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// 
        /// </param>
        /// <param name="argumentSets">
        /// 
        /// </param>
        /// <returns>
        /// The response to the GET request.
        /// </returns>
        public virtual async Task<Response> GetAsync(Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Get, ns, resource, null, token, argumentSets).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Sends a GET request as an asynchronous operation.
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resourceName">
        /// 
        /// </param>
        /// <param name="token">
        /// 
        /// </param>
        /// <param name="argumentSets">
        /// 
        /// </param>
        /// <returns>
        /// The response to the GET request.
        /// </returns>
        public virtual async Task<Response> GetAsync(Namespace ns, ResourceName resourceName, CancellationToken token,
            params IEnumerable<Argument>[] argumentSets)
        {
            var response = await this.SendAsync(HttpMethod.Get, ns, resourceName, null, token, argumentSets).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Sends a POST request as an asynchronous operation.
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// 
        /// </param>
        /// <param name="argumentSets">
        /// 
        /// </param>
        /// <returns>
        /// The response to the GET request.
        /// </returns>
        public virtual async Task<Response> PostAsync(Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            var content = CreateStringContent(argumentSets);
            return await this.PostAsync(ns, resource, content, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a POST request as an asynchronous operation.
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// 
        /// </param>
        /// <param name="content">
        /// 
        /// </param>
        /// <param name="argumentSets">
        /// 
        /// </param>
        /// <returns>
        /// The response to the GET request.
        /// </returns>
        public virtual async Task<Response> PostAsync(Namespace ns, ResourceName resource,
            HttpContent content, params IEnumerable<Argument>[] argumentSets)
        {
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Post, ns, resource, content, token, argumentSets).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Sends a GET, POST, or DELETE request as an asynchronous operation.
        /// </summary>
        /// <param name="method">
        /// 
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// 
        /// </param>
        /// <param name="argumentSets">
        /// 
        /// </param>
        /// <returns>
        /// The response to the GET, POST, or DELETE request.
        /// </returns>
        public virtual async Task<Response> SendAsync(HttpMethod method, Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            Contract.Requires<ArgumentNullException>(method != null);
            Contract.Requires<ArgumentException>(method == HttpMethod.Delete || method == HttpMethod.Get || method == HttpMethod.Post);

            var token = CancellationToken.None;
            HttpContent content = null;

            if (method == HttpMethod.Post)
            {
                content = CreateStringContent(argumentSets);
                argumentSets = null;
            }

            var response = await this.SendAsync(method, ns, resource, content, token, argumentSets).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Converts the current <see cref="Context"/> to its string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Context"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            var text = string.Concat(CultureInfo.InvariantCulture, SchemeStrings[(int)this.Scheme], "://", this.Host, 
                ":", this.Port.ToString(CultureInfo.InvariantCulture.NumberFormat));
            return text;
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

        Uri CreateServiceUri(Namespace ns, ResourceName name, params IEnumerable<Argument>[] argumentSets)
        {
            var builder = new StringBuilder(this.ToString());

            builder.Append("/");
            builder.Append(ns.ToUriString());
            builder.Append("/");
            builder.Append(name.ToUriString());

            if (argumentSets != null)
            {
                var query = string.Join("&",
                    from args in argumentSets
                    where args != null
                    from arg in args
                    select string.Join("=", Uri.EscapeDataString(arg.Name), Uri.EscapeDataString(arg.Value.ToString())));

                if (query.Length > 0)
                {
                    builder.Append('?');
                    builder.Append(query);
                }
            }

            var uri = UriConverter.Instance.Convert(builder.ToString());
            return uri;
        }

        static StringContent CreateStringContent(params IEnumerable<Argument>[] argumentSets)
        {
            if (argumentSets == null)
            {
                return new StringContent(string.Empty);
            }

            var body = string.Join("&",
                from args in argumentSets
                where args != null
                from arg in args
                select string.Join("=", Uri.EscapeDataString(arg.Name), Uri.EscapeDataString(arg.Value.ToString())));

            var stringContent = new StringContent(body);
            return stringContent;
        }

        async Task<Response> SendAsync(HttpMethod method, Namespace ns, ResourceName resource, HttpContent
            content, CancellationToken cancellationToken, IEnumerable<Argument>[] argumentSets)
        {
            Contract.Requires<ArgumentNullException>(ns != null);
            Contract.Requires<ArgumentNullException>(resource != null);

            var serviceUri = this.CreateServiceUri(ns, resource, argumentSets);

            using (var request = new HttpRequestMessage(method, serviceUri) { Content = content })
            {
                if (this.SessionKey != null)
                {
                    request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
                }

                var message = await this.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var response = await Response.CreateAsync(message).ConfigureAwait(false);

                return response;
            }
        }

        #endregion
    }
}
