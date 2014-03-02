namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines a provider for push-based notification.
    /// </summary>
    /// <typeparam name="T">
    /// The object that provides notification information.</typeparam>
    public abstract class Observable<T> : IObservable<T>
    {
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

            if (this.observers == null)
            {
                this.observers = new LinkedList<IObserver<T>>();
            }
            var unsubscriber = new Unsubscriber(this.observers.AddLast(observer));
            return unsubscriber;
        }

        protected IEnumerable<IObserver<T>> Observers
        { get { return this.observers; } }

        LinkedList<IObserver<T>> observers;

        struct Unsubscriber : IDisposable
        {
            public Unsubscriber(LinkedListNode<IObserver<T>> node)
            {
                Contract.Requires<ArgumentNullException>(node != null, "node");
                this.node = node;
            }

            public void Dispose()
            { node.List.Remove(node); }

            readonly LinkedListNode<IObserver<T>> node;
        }
    }
}
