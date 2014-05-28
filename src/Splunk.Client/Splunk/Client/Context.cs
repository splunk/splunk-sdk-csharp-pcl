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

//// TODO:
//// [X] Synchronization strategy
//// [X] Settable SessionKey
//// [X] Dead code removal
//// [X] Contracts
//// [O] Documentation
//// [X] All unsealed classes that implement IDisposable must also implement 
////     this method: protected virtual void Dispose(bool);
////     See [Implementing a Dispose Method](http://goo.gl/VPIovn)
////     All sealed classes that implement IDisposable must also implemnt this
////     method: void Dispose(bool);
////     See [GC.SuppressFinalize Method](http://goo.gl/XiI3HZ) and note the
////     private void Dispose(bool disposing) implementation.

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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
        /// <param name="scheme">
        /// The <see cref="Scheme"/> used to communicate with <see cref="Host"/>
        /// </param>
        /// <param name="host">
        /// The DNS name of a Splunk server instance.
        /// </param>
        /// <param name="port">
        /// The port number used to communicate with <see cref="Host"/>.
        /// </param>
        /// <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>

        public Context(Scheme scheme, string host, int port)
            : this(scheme, host, port, null)
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
        /// <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>
        public Context(Scheme scheme, string host, int port, HttpMessageHandler handler, bool disposeHandler = true)
        {
            Contract.Requires<ArgumentException>(scheme == Scheme.Http || scheme == Scheme.Https);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(host));
            Contract.Requires<ArgumentException>(0 <= port && port <= 65535);

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
        /// Releases all disposable resources used by the current <see cref=
        /// "Context"/>.
        /// </summary>
        /// <remarks>
        /// Do not override this method. Override <see cref="Dispose(bool)"/>
        /// instead.
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
        ///   Override this method and call it from the override.
        /// </description></item>
        /// <item><description>
        ///   Provide a finalizer, if needed, and call this method from it.
        /// </description></item>
        /// <item><description>
        ///   To help ensure that resources are always cleaned up 
        ///   appropriately, ensure that the override is callable multiple
        ///   times without throwing an exception.
        /// </description></item>
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
            }
        }

        // TODO: Remove these comments before release

        // FJR: Having separate GET, POST, and DELETE methods turns out to be less sensible that you would think.
        // First, you end up repeating a lot of logic. Second, Splunk's REST API doesn't actually respect the different
        // semantics you expect from them. For example, the HTTP inputs expect to receive arguments as though they were
        // GET requests and a POST body of data as well. This lack of clarity makes it simpler just to have one method
        // that takes the method as an argument, and accepts both URI arguments and a POST body (which can be either a
        // dictionary or a string -- it's always a dictionary, except for those darned HTTP inputs again). I also ended
        // up adding a request by URL method that the request method called because I had some situation where I had to
        // use a link from the links element of the Atom body, and rather than parse it and reencode it, I just wanted to
        // use it directly.			

        // DSN: HttpClient class enforces these semantics on HTTP methods:
        //
        // + HttpMethod.Delete    May include a message body
        //
        // + HttpMethod.Get       May not include a message body
        //
        // + HttpMethod.Post      May include a message body
        //
        // The HttpClient class imposes no restrictions on query parameter. Any HTTP method may include a query.
        // Hence, I've done the following.
        //
        // + Consolidated the code into one private method: Context.SendAsync accepts both HttpContent and parameters.
        //
        // + Provided these public entry points:
        //
        //   o Context.DeleteAsync  Accepts parameters, but not HttpContent. 
        //
        //   o Context.GetAsync     Accepts parameters, but not HttpContent.
        //
        //   o Context.PostAsync    Two variants. The first accepts parameters and it puts them into the message body. The 
        //                          other accepts HttpContent and parameters. Parameters are provided on the Service URL.
        //
        // TODO: Is there any need for delete to accept HttpContent?

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// </param>
        /// <param name="argumentSets">
        /// </param>
        /// <returns></returns>
        public async Task<Response> DeleteAsync(Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Delete, ns, resource, null, token, argumentSets);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// </param>
        /// <param name="argumentSets">
        /// </param>
        /// <returns></returns>
        public async Task<Response> GetAsync(Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Get, ns, resource, null, token, argumentSets);
            return response;
        }

        /// <summary>
        /// 
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
        /// 
        /// </returns>
        public async Task<Response> GetAsync(Namespace ns, ResourceName resourceName, CancellationToken token, 
            params IEnumerable<Argument>[] argumentSets)
        {
            var response = await this.SendAsync(HttpMethod.Get, ns, resourceName, null, token, argumentSets);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource"></param>
        /// <param name="argumentSets"></param>
        /// <returns></returns>
        public async Task<Response> PostAsync(Namespace ns, ResourceName resource,
            params IEnumerable<Argument>[] argumentSets)
        {
            var content = this.CreateStringContent(argumentSets);
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Post, ns, resource, content, token, null);
            return response;
        }

        /// <summary>
        /// 
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
        /// 
        /// </returns>
        public async Task<Response> PostAsync(Namespace ns, ResourceName resource,
            HttpContent content, params IEnumerable<Argument>[] argumentSets)
        {
            var token = CancellationToken.None;
            var response = await this.SendAsync(HttpMethod.Post, ns, resource, content, token, argumentSets);
            return response;
        }

        /// <summary>
        /// Converts the current <see cref="Context"/> to its string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Context"/>.
        /// </returns>
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

            return new Uri(builder.ToString());
        }

        StringContent CreateStringContent(params IEnumerable<Argument>[] argumentSets)
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

                var completionOption = HttpCompletionOption.ResponseHeadersRead;
                var response = await this.HttpClient.SendAsync(request, completionOption, cancellationToken);

                return await Response.CreateAsync(response);
            }
        }

        #endregion
    }
}
