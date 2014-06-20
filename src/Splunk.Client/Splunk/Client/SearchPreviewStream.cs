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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchPreviewStream"/> class represents a streaming XML 
    /// reader for Splunk <see cref="SearchResultStream"/>.
    /// </summary>
    public sealed class SearchPreviewStream : Observable<SearchPreview>, IDisposable, IEnumerable<SearchPreview>
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
            this.cancellationTokenSource = new CancellationTokenSource();
            this.response = response;

            this.awaiter = new Awaiter(this, cancellationTokenSource.Token);
        }

        #endregion

        #region Properties

        public Exception LastError
        {
            get { return this.awaiter.LastError; }
        }

        /// <summary>
        /// Gets the <see cref="SearchPreview"/> read count for the current
        /// <see cref="SearchResultStream"/>.
        /// </summary>
        public long ReadCount
        {
            get { return this.awaiter.ReadCount; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all disposable resources used by the current <see cref=
        /// "SearchPreviewStream"/>.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
            {
                return;
            }

            //// TODO: BUG FIX: The Cancel method returns immediately, before this.awaiter cancels. Consequently,
            //// when we call this.response.Dispose the underlying XmlReader may throw an InvalidOperationException
            //// because "An asynchronous operation is already in progress."

            this.cancellationTokenSource.Cancel();
            this.response.Dispose();
            this.cancellationTokenSource.Dispose();
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
        ///   Perform LINQ to Objects queries to obtain a filtered set of 
        ///   search previews.</description></item>
        /// <item><description>
        ///   Append search previews to an existing collection of search
        ///   previews.</description></item>
        /// </list>
        /// </remarks>
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
        /// Pushes <see cref="SearchPreview"/> instances to subscribers 
        /// and then completes.
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

        readonly CancellationTokenSource cancellationTokenSource;
        readonly Awaiter awaiter;
        readonly Response response;
        int disposed;

        ReadState ReadState
        {
            get { return this.response.XmlReader.ReadState; }
        }

        void EnsureAwaiterSucceeded()
        {
            if (this.awaiter.LastError != null)
            {
                var text = string.Format("Enumeration ended prematurely : {0}.", this.awaiter.LastError);
                throw new TaskCanceledException(text, this.awaiter.LastError);
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

            reader.Requires(await reader.MoveToDocumentElementAsync("results"));
            var preview = new SearchPreview();
            await preview.ReadXmlAsync(reader);
            await reader.ReadAsync();

            return preview;
        }

        #endregion

        #region Types

        sealed class Awaiter : Awaiter<SearchPreviewStream, SearchPreview>
        {
            public Awaiter(SearchPreviewStream stream, CancellationToken token)
                : base(stream, token)
            { }

            protected override async Task ReadToEndAsync()
            {
                while (this.Stream.ReadState <= ReadState.Interactive)
                {
                    this.EnsureNotCancelled();

                    var preview = await this.Stream.ReadPreviewAsync();
                    this.Enqueue(preview);
                }
            }
        }

        #endregion
    }
}