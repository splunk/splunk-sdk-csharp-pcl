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
//// [ ] Contracts
//// [ ] Documentation
//// [ ] Ensure that Response.EnsureStatusCodeAsync is used instead of
////     throwing RequestException or its derivatives directly.

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides a class that represents a Splunk service response.
    /// </summary>
    public class Response : IDisposable
    {
        #region Constructors

        Response(HttpResponseMessage message)
        { this.message = message; }

        #endregion

        #region Properties

        public HttpResponseMessage Message
        {
            get { return this.message; }
        }

        public Stream Stream
        { get; private set; }

        public XmlReader XmlReader
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<Response> CreateAsync(HttpResponseMessage message)
        {
            Contract.Requires(message != null);

            var response = new Response(message);
            response.Stream = await message.Content.ReadAsStreamAsync();
            response.XmlReader = XmlReader.Create(response.Stream, XmlReaderSettings);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        { 
            this.Dispose(true); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expected">
        /// </param>
        /// <returns>
        /// </returns>
        public async Task EnsureStatusCodeAsync(HttpStatusCode expected)
        {
            var statusCode = this.Message.StatusCode;

            if (statusCode != expected)
            {
                var details = await Splunk.Client.Message.ReadMessagesAsync(this.XmlReader);

                switch (statusCode)
                {
                    case HttpStatusCode.Forbidden:
                        throw new UnauthorizedAccessException(this.Message, details);
                    case HttpStatusCode.NotFound:
                        throw new ResourceNotFoundException(this.Message, details);
                    case HttpStatusCode.Unauthorized:
                        throw new AuthenticationFailureException(this.Message, details);
                    default:
                        throw new RequestException(this.Message, details);
                }
            }
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

        HttpResponseMessage message;
        bool disposed;

        void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.Message.Dispose();
                this.XmlReader.Dispose();
                this.disposed = true;
                
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
