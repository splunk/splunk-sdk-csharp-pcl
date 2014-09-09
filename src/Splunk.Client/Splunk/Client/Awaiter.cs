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
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An awaiter.
    /// </summary>
    /// <typeparam name="TStream">
    /// Type of the stream.
    /// </typeparam>
    /// <typeparam name="TEvent">
    /// Type of the event.
    /// </typeparam>
    /// <seealso cref="T:System.Runtime.CompilerServices.INotifyCompletion"/>
    abstract class Awaiter<TStream, TEvent> : INotifyCompletion
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Splunk.Client.Awaiter&lt;TStream,
        /// TEvent&gt; class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification =
            @"Unless performance or scalability testing reveals that you must dispose of tasks based on your usage
            patterns in order to meet your goals don't bother disposing them. See <a href='http://goo.gl/SFkBYs'>
            Parallel Programming with .NET, Do I need to dispose of Tasks</a>.")
        ]
        protected Awaiter(TStream stream)
        {
            var task = new Task(this.ReadEventsAsync);

            this.lastError = null;
            this.stream = stream;
            this.readCount = 0;
            this.readState = 0;

            task.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <value>
        /// The last error.
        /// </value>
        public Exception LastError
        {
            get { return this.lastError; }
        }

        /// <summary>
        /// Gets the number of reads.
        /// </summary>
        /// <value>
        /// The number of reads.
        /// </value>
        public long ReadCount
        {
            get { return Interlocked.Read(ref this.readCount); }
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        protected TStream Stream
        {
            get { return this.stream; }
        }
    
        #endregion

        #region Protected methods

        /// <summary>
        /// Adds an object onto the end of this queue.
        /// </summary>
        /// <param name="result">
        /// 
        /// </param>
        protected void Enqueue(TEvent result)
        {
            Interlocked.Increment(ref this.readCount);
            this.results.Enqueue(result);
            this.Continue();
        }

        /// <summary>
        /// Asynchronously reads to the end of <see cref="Stream"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected abstract Task ReadToEndAsync();

        #endregion

        #region Methods supporting IEnumerable<TEvent>

        /// <summary>
        /// Attempts to take from the given data.
        /// </summary>
        /// <param name="result">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if it succeeds, <c>false</c> if it fails.
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
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
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
                await this.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (Exception error)
            {
                this.lastError = error;
            }

            Interlocked.Increment(ref this.readState);
            await Task.Delay(1).ConfigureAwait(false);
            this.Continue();
        }

        #endregion
    }
}
