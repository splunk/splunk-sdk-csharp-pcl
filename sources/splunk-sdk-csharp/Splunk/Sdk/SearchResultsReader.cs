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
//
// [X] Refactor SearchResultsReader/SearchResults
//
//     1. SearchResults should read one and only one result set
//     2. SearchResultsReader should accept a stream and instantiate a new 
//        SearchResults instance for each result set.
//     3. Care should be taken to ensure that SearchResults instances do
//        not dispose their stream prematurely; perhaps by creating each
//        SearchResults instance with an XmlReader instance. When 
//        constructed in this fashion, we would simply short-circuit 
//        Dispose and leave it to our creator to close the stream.
//
// [O] Work out semantics for skipping preview SearchResults
//     Use Reactive Extensions Operators
//
// [O] Contracts
//
// [O] Documentation
//
// [X] Thread safety
//     Addresed by modifications to Observable<T> class. See 
//     Observable<T>.NotifySubscribers and Observable<T>.Complete.
//
// References:
// 1. [Async, await, and yield return](http://goo.gl/RLVDK5)

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchResultsReader"/> class represents a streaming XML 
    /// reader for Splunk <see cref="SearchResults"/>.
    /// </summary>
    public sealed class SearchResultsReader : Observable<SearchResults>, IDisposable, IEnumerable<SearchResults>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultsReader"/> class.
        /// This is a base contructor that should not be used directly.
        /// </summary>
        /// <param name="stream">
        /// The underlying <see cref="Stream"/>.
        /// </param>
        SearchResultsReader(Response response)
        {
            this.response = response;
        }

        #endregion

        #region Methods

        internal static async Task<SearchResultsReader> CreateAsync(Response response)
        {
            response.XmlReader.MoveToElement(); // ensures we're at an element, not an attribute

            if (!response.XmlReader.IsStartElement("results"))
            {
                if (!await response.XmlReader.ReadToFollowingAsync("results"))
                {
                    throw new InvalidDataException();  // TODO: diagnostics
                }
            }

            return new SearchResultsReader(response);
        }

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        public void Dispose()
        { this.Dispose(true); }

        /// <summary>
        /// Returns an enumerator that iterates through <see cref="SearchResults"/>
        /// synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResults"/> enumerator structure for the current <see 
        /// cref="SearchResultsReader"/>.
        /// </returns>
        public IEnumerator<SearchResults> GetEnumerator()
        {
            do
            {
                yield return SearchResults.CreateAsync(this.response, leaveOpen: true).Result;
            }
            while (this.response.XmlReader.ReadToFollowingAsync("results").Result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through <see cref="SearchResults"/>
        /// synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResults"/> enumerator structure for the current <see 
        /// cref="SearchResultsReader"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        /// <summary>
        /// Pushes <see cref="SearchResults"/> to subscribers and then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/>
        /// </returns>
        override protected async Task PushObservations()
        {
            do
            {
                var searchResults = await SearchResults.CreateAsync(this.response, leaveOpen: true);
                this.OnNext(searchResults);
            }
            while (await this.response.XmlReader.ReadToFollowingAsync("results"));

            this.OnCompleted();
        }

        #endregion

        #region Privates

        readonly Response response;
        bool disposed;

        void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.response.Dispose();
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}