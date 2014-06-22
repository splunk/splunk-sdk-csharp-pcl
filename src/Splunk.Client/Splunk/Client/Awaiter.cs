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
    /// <typeparam name="TStream">
    /// 
    /// </typeparam>
    /// <typeparam name="TEvent">
    /// 
    /// </typeparam>
    abstract class Awaiter<TStream, TEvent> : INotifyCompletion
    {
        #region Constructors

        protected Awaiter(TStream stream)
        {
            this.task = new Task(this.ReadEventsAsync);

            this.lastError = null;
            this.stream = stream;
            this.readCount = 0;
            this.readState = 0;

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
            if (this.results.TryDequeue(out result))
            {
                return true;
            }

            result = default(TEvent);

            if (this.IsReading)
            {
                result = this.AwaitEventAsync().Result;
            }

            return result != null;
        }

        #endregion

        #region Members called by the async await state machine

        //// The async await state machine requires that you implement 
        //// INotifyCompletion and provide three additional members: 
        //// 
        ////     IsCompleted property
        ////     Tells the async state machine whether results are avaiable.
        ////
        ////     GetAwaiter method
        ////     Returns the current awaiter to the async state machine.
        ////
        ////     GetResult method
        ////     Returns the next event to the async state machine.
        ////
        //// INotifyCompletion itself defines just one member:
        ////
        ////     OnCompletion method
        ////
        //// See Jeffrey Richter's excellent discussion of the topic of 
        //// awaiters in CLR via C# (4th Edition).

        /// <summary>
        /// Tells the state machine whether results are available.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                bool result = this.readState == 2 || this.results.Count > 0;
                return result;
            }
        }

        ReaderWriterLockSlim gate = new ReaderWriterLockSlim();

        /// <summary>
        /// Returns the current awaiter to the async state machine.
        /// </summary>
        /// <returns>
        /// An object reprsenting the current awaiter.
        /// </returns>
        public Awaiter<TStream, TEvent> GetAwaiter()
        { return this; }

        /// <summary>
        /// Returns the next event to the async state machine.
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
        Action continuation;
        Exception lastError;
        TStream stream;
        long readCount;
        int readState;
        Task task;

        bool IsReading
        {
            get { return this.readState < 2; }
        }

        async Task<TEvent> AwaitEventAsync()
        {
            return await this;
        }

        bool Continue()
        {
            Action continuation = Interlocked.Exchange(ref this.continuation, null);

            if (continuation != null)
            {
                continuation();
                return true;
            }

            return false;
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
            catch (Exception error)
            {
                this.lastError = error;
            }

            Interlocked.Increment(ref this.readState);
            await Task.Delay(1);
            this.Continue();
        }

        #endregion
    }
}
