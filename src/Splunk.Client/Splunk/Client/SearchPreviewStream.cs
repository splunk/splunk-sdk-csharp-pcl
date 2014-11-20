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
//// 2. [CLR via C# (4th Edition)](http://goo.gl/SmpI3W)

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchPreviewStream"/> class represents a streaming XML
    /// reader for Splunk <see cref="SearchResultStream"/>.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Observable{Splunk.Client.SearchPreview}"/>
    /// <seealso cref="T:System.IDisposable"/>
    /// <seealso cref="T:System.Collections.Generic.IEnumerable{Splunk.Client.SearchPreview}"/>
    public sealed class SearchPreviewStream : Observable<SearchPreview>, IDisposable, IEnumerable<SearchPreview>
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="SearchPreview"/> read count for the current
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
        /// Asynchronously creates a new <see cref="SearchExportStream"/>
        /// using the specified <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="message">
        /// An object from which search results are read.
        /// </param>
        /// <returns>
        /// A <see cref="SearchExportStream"/> object.
        /// </returns>
        public static async Task<SearchPreviewStream> CreateAsync(HttpResponseMessage message)
        {
            return await CreateAsync(await Splunk.Client.Response.CreateAsync(message).ConfigureAwait(false));
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="SearchExportStream"/>
        /// using the specified <see cref="Response"/>.
        /// </summary>
        /// <param name="response">
        /// An object from which search results are read.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        /// <exception cref="System.IO.InvalidDataException">
        /// Thrown when an invalid Data error condition occurs.
        /// </exception>
        public static Task<SearchPreviewStream> CreateAsync(Response response)
        {
            return Task.FromResult(new SearchPreviewStream(response));
        }

        /// <summary>
        /// Releases all disposable resources used by the current
        /// <see cref= "SearchPreviewStream"/>.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
            {
                return;
            }

            //// TODO: BUG FIX: The Cancel method returns immediately, before this.awaiter cancels. Consequently,
            //// when we call this.response.Dispose the underlying XmlReader may throw an InvalidOperationException
            //// because "An asynchronous operation is already in progress."

            this.response.Dispose();
        }

        /// <summary>
        /// Gets an enumerator that iterates through <see cref="SearchPreview"/>
        /// objects on the current <see cref="SearchPreviewStream"/> asynchronously.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="GetEnumerator"/> method to
        /// <list type="bullet">
        /// <item><description>
        ///   Perform LINQ to Objects queries to obtain a filtered set of search
        ///   previews.</description></item>
        /// <item><description>
        ///   Append search previews to an existing collection of search
        ///   previews.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// An enumerator structure for the <see cref="SearchPreviewStream"/>.
        /// </returns>
        ///
        /// ### <exception cref="InvalidOperationException">
        /// The current <see cref="SearchPreviewStream"/> has already been enumerated.
        /// </exception>
        public IEnumerator<SearchPreview> GetEnumerator()
        {
            this.EnsureNotDisposed();

            for (SearchPreview preview; this.awaiter.TryTake(out preview); )
            {
                yield return preview;
            }

            this.EnsureAwaiterSucceeded();
        }

        /// <inheritdoc cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Pushes <see cref="SearchPreview"/> instances to subscribers and then
        /// completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected override async Task PushObservations()
        {
            this.EnsureNotDisposed();

            for (var preview = await this.awaiter; preview != null; preview = await this.awaiter)
            {
                this.OnNext(preview);
            }

            this.EnsureAwaiterSucceeded();
            this.OnCompleted();
        }

        #endregion

        #region Privates/internals

        SearchResultMetadata metadata;
        readonly Response response;
        readonly Awaiter awaiter;
        int disposed;

        SearchPreviewStream(Response response)
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

        async Task<SearchPreview> ReadPreviewAsync()
        {
            XmlReader reader = this.response.XmlReader;

            reader.Requires(await reader.MoveToDocumentElementAsync("results").ConfigureAwait(false));
            var preview = new SearchPreview();
            await preview.ReadXmlAsync(reader).ConfigureAwait(false);
            await reader.ReadAsync().ConfigureAwait(false);

            return preview;
        }

        #endregion

        #region Types

        /// <summary>
        /// An awaiter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.Awaiter{Splunk.Client.SearchPreviewStream,Splunk.Client.SearchPreview}"/>
        sealed class Awaiter : Awaiter<SearchPreviewStream, SearchPreview>
        {
            /// <summary>
            /// Initializes a new instance of the
            /// Splunk.Client.SearchPreviewStream.Awaiter class.
            /// </summary>
            /// <param name="stream">
            /// The stream.
            /// </param>
            public Awaiter(SearchPreviewStream stream)
                : base(stream)
            { }

            /// <summary>
            /// Reads to the end of the current <see cref="SearchPreviewStream"/>
            /// asynchronously.
            /// </summary>
            /// <returns>
            /// A <see cref="Task"/> representing the operation.
            /// </returns>
            protected override async Task ReadToEndAsync()
            {
                while (this.Stream.ReadState <= ReadState.Interactive)
                {
                    var preview = await this.Stream.ReadPreviewAsync().ConfigureAwait(false);
                    this.Enqueue(preview);
                }
            }
        }

        #endregion
    }
}