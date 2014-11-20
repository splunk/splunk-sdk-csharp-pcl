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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Represents an enumerable, observable stream of <see cref="SearchResult"/>
    /// records.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Observable{Splunk.Client.SearchResult}"/>
    /// <seealso cref="T:System.IDisposable"/>
    /// <seealso cref="T:System.Collections.Generic.IEnumerable{Splunk.Client.SearchResult}"/>
    public sealed class SearchResultStream : Observable<SearchResult>, IDisposable, IEnumerable<SearchResult>
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the current
        /// <see cref="SearchResultStream"/>
        /// is yielding the final results from a search job.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is final, <c>false</c> if not.
        /// </value>
        public bool IsFinal
        {
            get { return this.metadata.IsFinal; }
        }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a
        /// <see cref="SearchResult"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result may contain a subset of these fields.
        /// </remarks>
        /// <value>
        /// A list of names of the fields.
        /// </value>
        public ReadOnlyCollection<string> FieldNames
        {
            get { return this.metadata.FieldNames; }
        }

        /// <summary>
        /// Gets the <see cref="SearchResult"/> read count for the current
        /// <see cref="SearchResultStream"/>.
        /// </summary>
        /// <value>
        /// The number of reads.
        /// </value>
        public long ReadCount
        {
            get { return this.awaiter.ReadCount; }
        }


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
        /// Asynchronously creates a new <see cref="SearchResultStream"/>
        /// using the specified <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="message">
        /// An object from which search results are read.
        /// </param>
        /// <returns>
        /// A <see cref="SearchResultStream"/> object.
        /// </returns>
        public static async Task<SearchResultStream> CreateAsync(HttpResponseMessage message)
        {
            return await CreateAsync(await Splunk.Client.Response.CreateAsync(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="SearchResultStream"/>
        /// using the specified <see cref="Response"/>.
        /// </summary>
        /// <param name="response">
        /// An object from which search results are read.
        /// </param>
        /// <returns>
        /// A <see cref="SearchResultStream"/> object.
        /// </returns>
        public static async Task<SearchResultStream> CreateAsync(Response response)
        {
            XmlReader reader = response.XmlReader;

            await reader.MoveToDocumentElementAsync("results", "response").ConfigureAwait(false);

            if (reader.Name == "response")
            {
                await response.ThrowRequestExceptionAsync().ConfigureAwait(false);
            }

            var stream = new SearchResultStream(response);
            return stream;
        }

        /// <summary>
        /// Releases all disposable resources used by the current
        /// <see cref="SearchResultStream"/>.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
            {
                return;
            }

            //// TODO: BUG FIX: Cancellation completes immediately after Cancel is called, in spite of the fact that 
            //// this.awaiter.task has not completed.

            this.response.Dispose();
        }

        /// <summary>
        /// Returns an enumerator that iterates through search results.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="GetEnumerator"/> method to
        /// <list type="bullet">
        /// <item><description>
        ///   Perform LINQ to Objects queries to obtain a filtered set of search
        ///   result records.</description></item>
        /// <item><description>
        ///   Append search results to an existing search result collection.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// A <see cref="SearchResult"/> enumerator structure for the
        /// <see cref="SearchResultStream"/>.
        /// </returns>
        public IEnumerator<SearchResult> GetEnumerator()
        {
            this.EnsureNotDisposed();

            for (SearchResult result; this.awaiter.TryTake(out result); )
            {
                this.metadata = result.Metadata;
                yield return result;
            }

            this.EnsureAwaiterSucceeded();
        }

        /// <inheritdoc cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Asynchronously pushes <see cref="SearchResult"/> objects to observers and
        /// then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected override async Task PushObservations()
        {
            this.EnsureNotDisposed();

            for (var result = await this.awaiter; result != null; result = await this.awaiter)
            {
                this.metadata = result.Metadata;
                this.OnNext(result);
            }

            this.EnsureAwaiterSucceeded();
            this.OnCompleted();
        }

        #endregion

        #region Privates/internals

        readonly Response response;
        readonly Awaiter awaiter;
        int disposed;

        volatile SearchResultMetadata metadata;

        SearchResultStream(Response response)
        {
            this.metadata = SearchResultMetadata.Missing;
            this.response = response;
            this.awaiter = new Awaiter(this);
        }

        ReadState ReadState
        {
            get { return this.response.XmlReader.ReadState; }
        }

        void EnsureAwaiterSucceeded()
        {
            if (this.awaiter.LastError != null && this.disposed == 0)
            {
                throw this.awaiter.LastError;
            }
        }

        void EnsureNotDisposed()
        {
            if (this.disposed != 0)
            {
                throw new ObjectDisposedException("Search result stream");
            }
        }

        async Task<SearchResultMetadata> ReadMetadataAsync()
        {
            var metadata = new SearchResultMetadata();

            await metadata.ReadXmlAsync(this.response.XmlReader).ConfigureAwait(false);

            return metadata;
        }

        async Task<SearchResult> ReadResultAsync(SearchResultMetadata metadata)
        {
            var reader = this.response.XmlReader;

            if (reader.NodeType == XmlNodeType.Element && reader.Name == "results")
            {
                return null;
            }

            Debug.Assert(reader.ReadState <= ReadState.Interactive, string.Concat("ReadState: ", reader.ReadState));
            reader.MoveToElement();

            reader.EnsureMarkup(XmlNodeType.Element, "result");

            var result = new SearchResult(metadata);
            await result.ReadXmlAsync(reader).ConfigureAwait(false);
            await reader.ReadAsync().ConfigureAwait(false);

            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "results")
            {
                await reader.ReadAsync().ConfigureAwait(false);
            }
            else
            {
                reader.EnsureMarkup(XmlNodeType.Element, "result");
            }

            return result;
        }

        #endregion

        #region Types

        /// <summary>
        /// An awaiter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.Awaiter{Splunk.Client.SearchResultStream,Splunk.Client.SearchResult}"/>
        sealed class Awaiter : Awaiter<SearchResultStream, SearchResult>
        {
            /// <summary>
            /// Initializes a new instance of the
            /// Splunk.Client.SearchResultStream.Awaiter class.
            /// </summary>
            /// <param name="stream">
            /// The stream.
            /// </param>
            public Awaiter(SearchResultStream stream)
                : base(stream)
            { }

            /// <summary>
            /// Reads to the end of the current <see cref="SearchResultStream"/>
            /// asynchronously.
            /// </summary>
            /// <returns>
            /// A <see cref="Task"/> representing the operation.
            /// </returns>
            protected override async Task ReadToEndAsync()
            {
                var metadata = await this.Stream.ReadMetadataAsync().ConfigureAwait(false);

                while (this.Stream.ReadState <= ReadState.Interactive)
                {
                    while (this.Stream.ReadState <= ReadState.Interactive)
                    {
                        SearchResult result = await this.Stream.ReadResultAsync(metadata).ConfigureAwait(false);

                        if (result == null)
                        {
                            break;
                        }

                        this.Enqueue(result);
                    }

                    metadata = await this.Stream.ReadMetadataAsync().ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}
