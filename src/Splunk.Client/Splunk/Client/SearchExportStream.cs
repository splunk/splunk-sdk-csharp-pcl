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
////
//// [O] Work out semantics for skipping preview SearchResults
////     Use Reactive Extensions Operators
////
//// [O] Contracts
////
//// [O] Documentation
////
//// [X] Refactor SearchExportStream/SearchResultStream
////
////     1. SearchResultStream should read one and only one result set
////     2. SearchExportStream should accept a stream and instantiate a new 
////        SearchResults instance for each result set.
////     3. Care should be taken to ensure that SearchResults instances do
////        not dispose their stream prematurely; perhaps by creating each
////        SearchResults instance with an XmlReader instance. When 
////        constructed in this fashion, we would simply short-circuit 
////        Dispose and leave it to our creator to close the stream.
////
//// [X] Thread safety
////     Addresed by modifications to Observable<T> class. See 
////     Observable<T>.NotifySubscribers and Observable<T>.Complete.
////
//// References:
//// 1. [Async, await, and yield return](http://goo.gl/RLVDK5)

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchExportStream"/> class represents a streaming XML 
    /// reader for Splunk <see cref="SearchResultStream"/>.
    /// </summary>
    public sealed class SearchExportStream : Observable<SearchResultStream>, IDisposable, IEnumerable<SearchResultStream>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchExportStream"/>
        /// class.
        /// </summary>
        /// <param name="response">
        /// The underlying <see cref="Response"/> object.
        /// </param>
        SearchExportStream(Response response)
        {
            this.response = response;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal static async Task<SearchExportStream> CreateAsync(Response response)
        {
            response.XmlReader.MoveToElement(); // ensures we're at an element, not an attribute

            if (!response.XmlReader.IsStartElement("results"))
            {
                if (!await response.XmlReader.ReadToFollowingAsync("results"))
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
        public void Dispose()
        {
            if (this.disposed)
                return;

            this.response.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through <see cref="SearchResultStream"/>
        /// synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResultStream"/> enumerator structure for the current <see 
        /// cref="SearchExportStream"/>.
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

        /// <summary>
        /// Returns an enumerator that iterates through <see cref="SearchResultStream"/>
        /// synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResultStream"/> enumerator structure for the current <see 
        /// cref="SearchExportStream"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        { 
            return this.GetEnumerator(); 
        }

        /// <summary>
        /// Pushes <see cref="SearchResultStream"/> to subscribers and then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/>
        /// </returns>
        override protected async Task PushObservations()
        {
            do
            {
                var searchResults = await SearchResultStream.CreateAsync(this.response);
                this.OnNext(searchResults);
            }
            while (await this.response.XmlReader.ReadToFollowingAsync("results"));

            this.OnCompleted();
        }

        #endregion

        #region Privates

        readonly Response response;
        bool disposed;

        #endregion
    }
}