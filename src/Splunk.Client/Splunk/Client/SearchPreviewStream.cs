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

            this.previewAwaiter = new SearchPreviewAwaiter(this, cancellationTokenSource.Token);
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
            {
                return;
            }

            if (this.previewAwaiter.ReadStatus < TaskStatus.RanToCompletion)
            {
                this.cancellationTokenSource.Cancel();
                this.previewAwaiter.Wait();
            }

            this.cancellationTokenSource.Dispose();
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
        public IEnumerator<SearchPreview> GetEnumerator()
        {
            for (SearchPreview preview; this.previewAwaiter.TryTake(out preview); )
            {
                yield return preview;
            }
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
            for (var preview = await this.previewAwaiter; preview != null; preview = await this.previewAwaiter)
            {
                this.OnNext(preview);
            }

            this.OnCompleted();
        }

        #endregion

        #region Privates/internals

        readonly CancellationTokenSource cancellationTokenSource;
        readonly SearchPreviewAwaiter previewAwaiter;
        readonly Response response;
        bool disposed;

        ReadState ReadState
        {
            get { return this.response.XmlReader.ReadState; }
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

        sealed class SearchPreviewAwaiter : INotifyCompletion
        {
            #region Constructors

            public SearchPreviewAwaiter(SearchPreviewStream stream, CancellationToken token)
            {
                this.task = new Task(this.ReadPreviewsAsync, token);
                this.cancellationToken = token;
                this.stream = stream;
                this.task.Start();
            }

            #endregion

            #region Properties

            public TaskStatus ReadStatus
            {
                get { return this.task.Status; }
            }

            #endregion

            #region Methods supporting IDisposable and IEnumerable<SearchPreview>

            /// <summary>
            /// 
            /// </summary>
            /// <param name="preview"></param>
            /// <returns></returns>
            public bool TryTake(out SearchPreview preview)
            {
                preview = this.AwaitResultAsync().Result;
                return preview != null;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Wait()
            {
                this.task.Wait();
            }

            #endregion

            #region Members called by the async await state machine

            //// The async await state machine requires that you implement 
            //// INotifyCompletion and provide three additional members: 
            //// 
            ////     IsCompleted property
            ////     GetAwaiter method
            ////     GetResult method
            ////
            //// INotifyCompletion itself defines just one member:
            ////
            ////     OnCompletion method
            ////
            //// See Jeffrey Richter's excellent discussion of the topic of 
            //// awaiters in CLR via C# (4th Edition).

            /// <summary>
            /// Tells the state machine if any results are available.
            /// </summary>
            public bool IsCompleted
            {
                get { return previews.Count > 0; }
            }

            /// <summary>
            /// Returns the current <see cref="SearchPreviewAwaiter"/> to the
            /// state machine.
            /// </summary>
            /// <returns>
            /// A reference to the current <see cref="SearchPreviewAwaiter"/>.
            /// </returns>
            public SearchPreviewAwaiter GetAwaiter()
            { return this; }

            /// <summary>
            /// Returns the current <see cref="SearchResult"/> from the current
            /// <see cref="SearchResultStream"/>.
            /// </summary>
            /// <returns>
            /// The current <see cref="SearchResult"/> or <c>null</c>.
            /// </returns>
            public SearchPreview GetResult()
            {
                SearchPreview preview = null;

                previews.TryDequeue(out preview);
                return preview;
            }

            /// <summary>
            /// Tells the current <see cref="SearchPreviewAwaiter"/> what method
            /// to invoke on completion.
            /// </summary>
            /// <param name="continuation">
            /// The method to call on completion.
            /// </param>
            public void OnCompleted(Action continuation)
            {
                Volatile.Write(ref this.continuation, continuation);
            }

            #endregion

            #region Privates/internals

            ConcurrentQueue<SearchPreview> previews = new ConcurrentQueue<SearchPreview>();
            CancellationToken cancellationToken;
            SearchPreviewStream stream;
            Action continuation;
            int enumerated;
            Task task;

            async Task<SearchPreview> AwaitResultAsync()
            {
                return await this;
            }

            void Continue()
            {
                Action continuation = Interlocked.Exchange(ref this.continuation, null);

                if (continuation != null)
                {
                    continuation();
                }

                this.cancellationToken.ThrowIfCancellationRequested();
            }

            async void ReadPreviewsAsync()
            {
                if (Interlocked.CompareExchange(ref this.enumerated, 1, 0) != 0)
                {
                    throw new InvalidOperationException("Stream has been enumerated; The enumeration operation may not execute again.");
                }

                cancellationToken.ThrowIfCancellationRequested();

                while (stream.ReadState <= ReadState.Interactive)
                {
                    var preview = await stream.ReadPreviewAsync();
                    this.previews.Enqueue(preview);
                    this.Continue();
                }

                this.Continue();
                enumerated = 2;
            }

            #endregion
        }

        #endregion
    }
}