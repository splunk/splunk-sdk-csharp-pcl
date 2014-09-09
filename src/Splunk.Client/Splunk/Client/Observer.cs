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

namespace Splunk.Client
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for faking git requests and responses from a Splunk server.
    /// </summary>
    public class Observer<T> : IObserver<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Observer&lt;T&gt;"/> 
        /// class.
        /// </summary>
        public Observer(Action<T> onNext, Action onCompleted = null, Action<Exception> onError = null)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted ?? onCompletedNoop;
            this.onError = onError ?? onErrorNoop;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the completed action.
        /// </summary>
        public void OnCompleted()
        {
            this.onCompleted();
        }

        /// <summary>
        /// Executes the error action.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.onError(error);
        }

        /// <summary>
        /// Executes the next action.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(T value)
        {
            this.onNext(value);
        }

        #endregion

        #region Privates/internals

        static readonly Action onCompletedNoop = new Action(() => { return; });
        static readonly Action<Exception> onErrorNoop = new Action<Exception>((e) => { return; }); 

        Action onCompleted;
        Action<T> onNext;
        Action<Exception> onError;

        #endregion
    }
}
