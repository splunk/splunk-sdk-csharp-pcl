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
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchPreviewStream"/> class represents a streaming XML 
    /// reader for Splunk <see cref="SearchResultStream"/>.
    /// </summary>
    public sealed class SearchPreviewStream : Observable<SearchPreview>, IDisposable, IEnumerable<Task<SearchPreview>>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPreviewStream"/>
        /// class.
        /// </summary>
        /// <param name="response">
        /// The underlying <see cref="Response"/> object.
        /// </param>
        internal SearchPreviewStream(Response response)
        {
            this.response = response;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all disposable resources used by the current <see cref=
        /// "SearchPreviewStream"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
                return;

            this.response.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through <see cref=
        /// "SearchPreview"/> objects on the current <see cref=
        /// "SearchPreviewStream"/> asynchronously.
        /// </summary>
        /// <returns>
        /// An enumerator structure for the <see cref="SearchPreviewStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="SearchPreviewStream"/> has already been
        /// enumerated.
        /// </exception>
        /// <remarks>
        /// You can use the <see cref="GetEnumerator"/> method to
        /// <list type="bullet">
        /// <item><description>
        ///     Perform LINQ to Objects queries to obtain a filtered set of 
        ///     search previews.</description></item>
        /// <item><description>
        ///     Append search previews to an existing collection of search
        ///     previews.</description></item>
        /// </list>
        /// </remarks>
        public IEnumerator<Task<SearchPreview>> GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.enumerated, 1, 0) != 0)
            {
                throw new InvalidOperationException("Stream has been enumerated; The enumeration operation may not execute again.");
            }

            /// TODO: Correct bad check for end of sequence; ReadSearchPreviewAsync never returns null
            /// Thought: check xml reader state for end of file

            XmlReader reader = this.response.XmlReader;

            while (reader.ReadState == ReadState.Interactive)
            {
                yield return ReadSearchPreviewAsync(reader);
            }

            this.enumerated = 2;
        }

        /// <inheritdoc cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
 
        /// <summary>
        /// Pushes <see cref="SearchPreview"/> instances to subscribers 
        /// and then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected override async Task PushObservations()
        {
            if (Interlocked.CompareExchange(ref this.enumerated, 1, 0) != 0)
            {
                throw new InvalidOperationException("Stream has been enumerated; The push operation may not execute again.");
            }

            XmlReader reader = this.response.XmlReader;

            while (reader.ReadState == ReadState.Interactive)
            {
                this.OnNext(await ReadSearchPreviewAsync(reader));
            }

            this.OnCompleted();
            this.enumerated = 2;
        }

        #endregion

        #region Privates

        readonly Response response;
        bool disposed;
        int enumerated;

        static async Task<SearchPreview> ReadSearchPreviewAsync(XmlReader reader)
        {
            if (await reader.ReadToFollowingAsync("results"))
            {
                var preview = new SearchPreview();

                await preview.ReadXmlAsync(reader);
                return preview;
            }

            Debug.Assert(reader.ReadState == ReadState.EndOfFile);
            return null;
        }

        #endregion
    }
}