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

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reactive.Threading.Tasks;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchResultsReader"/> class represents a streaming XML 
    /// reader for Splunk <see cref="SearchResults"/>.
    /// </summary>
    public sealed class SearchResultsReader : IDisposable, IEnumerable<SearchResults>
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

        readonly Response response;
        bool disposed;

        /// <summary>
        /// Releases all disposable resources used by the current <see cref=
        /// "SearchResultsReader"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
                return;

            this.response.Dispose();
            this.disposed = true;
        }

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
                var results = SearchResults.CreateAsync(this.response, leaveOpen: true).Result;
                yield return results;
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
        { 
            return this.GetEnumerator(); 
        }


        IObservable<SearchResults> observableResult = null;
        public IObservable<SearchResults> AsObservable()
        {
            // NB: Because this result is inherently *live data*, we
            // can't return a Cold Observable. Stop us from ending up
            // with skewed data (i.e. one subscriber having some data
            // and another subscriber having a different set of data)
            if (observableResult != null) return observableResult;

            var readOneItem = Observable.Defer(() => Observable.StartAsync(async () =>
            {
                var ret = await SearchResults.CreateAsync(this.response, leaveOpen: true);
                await this.response.XmlReader.ReadToFollowingAsync("results");

                return ret;
            }));

            observableResult = readOneItem
                .Repeat()
                .TakeWhile(x => x != null)
                .Finally(() => this.Dispose ())
                .Publish()
                .RefCount();

            return observableResult;
        }

        #endregion
    }
}
