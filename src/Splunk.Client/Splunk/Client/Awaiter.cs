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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    abstract class Awaiter<TStream, TEvent> : INotifyCompletion
    {
        #region Constructors

        protected Awaiter(TStream stream, CancellationToken token)
        {
            this.task = new Task(this.ReadEventsAsync, token);

            this.resetEvent = new ManualResetEvent(true);
            this.cancellationToken = token;
            this.lastError = null;
            this.stream = stream;
            this.readCount = 0;
            this.readState = 0;

            this.cancellationToken.Register(() =>
                {
                    this.resetEvent.Reset();
                    WaitHandle.WaitAll(new WaitHandle[] { this.cancellationToken.WaitHandle, this.resetEvent });
                    return;
                });

            this.task.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Exception LastError
        {
            get { return this.lastError; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long ReadCount
        {
            get { return Interlocked.Read(ref this.readCount); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected TStream Stream
        {
            get { return this.stream; }
        }
    
        #endregion

        #region Protected methods

        protected void Enqueue(TEvent result)
        {
            Interlocked.Increment(ref this.readCount);
            this.results.Enqueue(result);
            this.Continue();
        }

        protected void EnsureNotCancelled()
        {
            if (this.cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
        }

        protected abstract Task ReadToEndAsync();

        #endregion

        #region Methods supporting IEnumerable<TEvent>

        /// <summary>
        ///
        /// </summary>
        /// <param name="result">
        ///
        /// </param>
        /// <returns>
        ///
        /// </returns>
        public bool TryTake(out TEvent result)
        {
            result = this.AwaitEventAsync().Result;
            return result != null;
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
            get
            {
                bool result = this.readState == 2 || this.results.Count > 0;
                return result;
            }
        }

        /// <summary>
        /// Returns the current awaiter to the async state machine.
        /// </summary>
        /// <returns>
        /// A reference to the current awaiter.
        /// </returns>
        public Awaiter<TStream, TEvent> GetAwaiter()
        { return this; }

        /// <summary>
        /// Returns the next event from the current event stream.
        /// </summary>
        /// <returns>
        /// The current event or <c>null</c>.
        /// </returns>
        public TEvent GetResult()
        {
            TEvent result = default(TEvent);
            results.TryDequeue(out result);
            return result;
        }

        /// <summary>
        /// Tells the current awaiter what method to invoke on completion.
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

        ConcurrentQueue<TEvent> results = new ConcurrentQueue<TEvent>();
        CancellationToken cancellationToken;
        ManualResetEvent resetEvent;
        Action continuation;
        Exception lastError;
        TStream stream;
        int readState;
        long readCount;
        Task task;

        bool IsReading
        {
            get { return this.readState < 2; }
        }

        async Task<TEvent> AwaitEventAsync()
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
        }

        async void ReadEventsAsync()
        {
            if (Interlocked.CompareExchange(ref this.readState, 1, 0) != 0)
            {
                throw new InvalidOperationException("Stream has been enumerated; The enumeration operation may not execute again.");
            }

            try
            {
                await this.ReadToEndAsync();
            }
            catch (TaskCanceledException)
            {
                this.resetEvent.Set();
            }
            catch (Exception error)
            {
                this.lastError = error;
            }

            this.Continue();
            readState = 2;
        }

        #endregion
    }
}
