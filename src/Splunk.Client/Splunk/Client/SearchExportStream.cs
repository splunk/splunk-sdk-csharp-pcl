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

//// References:
//// 1. [Async, await, and yield return](http://goo.gl/RLVDK5)

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchExportStream"/> class represents a streaming XML
    /// reader for Splunk <see cref="SearchResultStream"/>.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Observable{Splunk.Client.SearchResultStream}"/>
    /// <seealso cref="T:System.IDisposable"/>
    /// <seealso cref="T:System.Collections.Generic.IEnumerable{Splunk.Client.SearchResultStream}"/>
    public sealed class SearchExportStream : Observable<SearchResultStream>, IDisposable, IEnumerable<SearchResultStream>
    {
        #region Constructors

        SearchExportStream(Response response)
        {
            this.response = response;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Returns the raw HTTP response message for the job.
        /// </summary>
        /// <value>
        /// The HTTP response message.
        /// </value>
        public HttpResponseMessage Response
        {
            get { return response.Message; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new <see cref="SearchExportStream"/>
        /// using the specified <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="message">
        /// An object from which search results are read.
        /// </param>
        /// <returns>
        /// A <see cref="SearchExportStream"/> object.
        /// </returns>
        public static async Task<SearchExportStream> CreateAsync(HttpResponseMessage message)
        {
            return await CreateAsync(await Splunk.Client.Response.CreateAsync(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="SearchExportStream"/>
        /// using the specified <see cref="Response"/>.
        /// </summary>
        /// <exception cref="InvalidDataException">
        /// Thrown when an Invalid Data error condition occurs.
        /// </exception>
        /// <param name="response">
        /// An object from which search results are read.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        public static async Task<SearchExportStream> CreateAsync(Response response)
        {
            response.XmlReader.MoveToElement(); // ensures we're at an element, not an attribute

            if (!response.XmlReader.IsStartElement("results"))
            {
                if (!await response.XmlReader.ReadToFollowingAsync("results").ConfigureAwait(false))
                {
                    throw new InvalidDataException();  // TODO: diagnostics
                }
            }

            return new SearchExportStream(response);
        }

        /// <summary>
        /// Releases all disposable resources used by the current <see cref=
        /// "SearchExportStream"/>.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            if (this.disposed)
                return;

            this.response.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the current <see cref=
        /// "SearchResultStream"/> synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResultStream"/> enumerator structure for the 
        /// current <see cref="SearchExportStream"/>.
        /// </returns>
        public IEnumerator<SearchResultStream> GetEnumerator()
        {
            do
            {
                var results = SearchResultStream.CreateAsync(this.response).Result;
                yield return results;
            }
            while (this.response.XmlReader.ReadToFollowingAsync("results").Result);
        }

        IEnumerator IEnumerable.GetEnumerator()
        { 
            return this.GetEnumerator(); 
        }

        /// <summary>
        /// Pushes <see cref="SearchResultStream"/> to subscribers and then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        override protected async Task PushObservations()
        {
            do
            {
                var searchResults = await SearchResultStream.CreateAsync(this.response).ConfigureAwait(false);
                this.OnNext(searchResults);
            }
            while (await this.response.XmlReader.ReadToFollowingAsync("results").ConfigureAwait(false));

            this.OnCompleted();
        }

        #endregion

        #region Privates

        readonly Response response;
        bool disposed;

        #endregion
    }
}