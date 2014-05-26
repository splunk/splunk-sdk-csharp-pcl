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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Represents a Splunk service response.
    /// </summary>
    public sealed class Response : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Intializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="message">
        /// An object representing an HTTP response message including the status
        /// code and data.
        /// </param>
        Response(HttpResponseMessage message)
        { this.message = message; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the HTTP response message associated with the current <see 
        /// cref="Response"/>.
        /// </summary>
        public HttpResponseMessage Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> associated with the current <see 
        /// cref="Response.Message"/>.
        /// </summary>
        /// <remarks>
        /// This object is the one returned by <see cref="HttpContent.ReadAsStreamAsync()"/>.
        /// </remarks>
        public Stream Stream
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="XmlReader"/> for reading HTTP body data from
        /// the current <see cref="Response.Stream"/>.
        /// </summary>
        public XmlReader XmlReader
        { 
            get 
            { 
                if (this.XmlReader == null)
                {
                    this.reader = XmlReader.Create(this.Stream, XmlReaderSettings);
                }

                return this.reader;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a <see cref="Response"/> object from an <see
        /// cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="message">
        /// The <see cref="HttpResponseMessage"/> from which to create a <see 
        /// cref="Response"/> object.
        /// </param>
        /// <returns>
        /// The <see cref="Response"/> object created.
        /// </returns>
        public static async Task<Response> CreateAsync(HttpResponseMessage message)
        {
            Contract.Requires(message != null);

            var response = new Response(message);
            response.Stream = await message.Content.ReadAsStreamAsync();

            return response;
        }

        /// <summary>
        /// Releases all disposable resources used by the current <see cref=
        /// "Response"/>.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.Message.Dispose();

                if (this.XmlReader != null) // because it's possible to be disposed before this.XmlReader is created
                {
                    this.XmlReader.Dispose();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Throws a <see cref="RequestException"/> if the current <see cref=
        /// "Response"/>.Message.StatusCode is different than <paramref name=
        /// "expected"/>.
        /// </summary>
        /// <param name="expected">
        /// The expected <see cref="HttpStatusCode"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task EnsureStatusCodeAsync(HttpStatusCode expected)
        {
            var statusCode = this.Message.StatusCode;

            if (statusCode == expected)
                return;

            var details = await Splunk.Client.Message.ReadMessagesAsync(this.XmlReader);
            RequestException requestException;

            switch (statusCode)
            {
                case HttpStatusCode.Forbidden:
                    requestException = new UnauthorizedAccessException(this.Message, details);
                    break;
                case HttpStatusCode.NotFound:
                    requestException = new ResourceNotFoundException(this.Message, details);
                    break;
                case HttpStatusCode.Unauthorized:
                    requestException = new AuthenticationFailureException(this.Message, details);
                    break;
                default:
                    requestException = new RequestException(this.Message, details);
                    break;
            }

            throw requestException;
        }

        #endregion

        #region Privates

        static readonly XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
        {
            ConformanceLevel = ConformanceLevel.Fragment,
            CloseInput = false,
            Async = true,
            IgnoreProcessingInstructions = true,
            IgnoreComments = true,
            IgnoreWhitespace = true
        };

        readonly HttpResponseMessage message;
        XmlReader reader;
        bool disposed;

        #endregion
    }
}
