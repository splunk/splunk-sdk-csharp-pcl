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
// [ ] Contracts
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    public class Response : IDisposable
    {
        #region Constructors

        Response(HttpResponseMessage message)
        { this.Message = message; }

        #endregion

        #region Properties

        public HttpResponseMessage Message
        { get; private set; }

        public XmlReader XmlReader
        { get; private set; }

        #endregion

        #region Methods

        public static async Task<Response> CreateAsync(HttpResponseMessage message)
        {
            Contract.Requires(message != null);

            var response = new Response(message);
            var stream = await message.Content.ReadAsStreamAsync();

            response.XmlReader = XmlReader.Create(stream, XmlReaderSettings);

            return response;
        }

        public void Dispose()
        { this.Dispose(true); }

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
