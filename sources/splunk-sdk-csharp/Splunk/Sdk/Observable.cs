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
// [X] Remove Observers property and replace it with a gated NotifySubscribers
//     operation
//
// [X] Write a gated Complete method
//
// [O] Contracts
//
// [O] Documentation
//
// [ ] Trace messages (e.g., when there are no observers)

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a provider for push-based notification.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object that provides notification information.
    /// </typeparam>
    public abstract class Observable<T> : IObservable<T>
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        protected void Complete()
        {
            if (this.observers == null)
            {
                return;
            }

            lock (this.gate)
            {
                foreach (var observer in this.observers)
                {
                    observer.OnCompleted();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observation"></param>
        protected void NotifySubscribers(T observation)
        {
            if (this.observers == null)
            {
                return;
            }

            lock (this.gate)
            {
                try
                {
                    foreach (var observer in this.observers)
                    {
                        observer.OnNext(observation);
                    }
                }
                catch (Exception e)
                {
                    foreach (var observer in this.observers)
                    {
                        observer.OnError(e);
                    }
                    throw;
                }
            }
        }

        protected internal abstract Task PushObservations();

        /// <summary>
        /// Notifies the current <see cref="SearchResultsReader"/> that an 
        /// observer is to receive notifications.
        /// </summary>
        /// <param name="observer">The object that is to receive notifications.
        /// </param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving
        /// notifications before the current <see cref="SearchResultsReader"/>
        /// has finished sending them.
        /// </returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            IDisposable unsubscriber;

            lock (this.gate)
            {
                if (this.observers == null)
                {
                    this.observers = new LinkedList<IObserver<T>>();
                }
                unsubscriber = new Unsubscriber(this, this.observers.AddLast(observer));
            }
            this.PushObservations();
            return unsubscriber;
        }

        #endregion

        #region Privates

        LinkedList<IObserver<T>> observers;
        object gate = new object();

        #endregion

        #region Types

        struct Unsubscriber : IDisposable
        {
            public Unsubscriber(Observable<T> observable, LinkedListNode<IObserver<T>> node)
            {
                Contract.Requires<ArgumentNullException>(node != null, "node");
                this.node = node;
                this.observable = observable;
            }

            public void Dispose()
            { lock (observable.gate) node.List.Remove(node); }

            readonly LinkedListNode<IObserver<T>> node;
            readonly Observable<T> observable;
        }

        #endregion
    }
}
