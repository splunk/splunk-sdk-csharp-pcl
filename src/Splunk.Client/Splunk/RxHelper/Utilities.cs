using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splunk.RxHelper
{
    public static class Utilities
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, 
            Action<T> onNext = null, Action<Exception> onError = null, 
            Action onCompleted = null) {
            IObserver<T> observer = new SimpleObserver<T> {
                onNextDelegate = onNext != null ? onNext : (v) => {},
                onErrorDelegate = onError != null ? onError : (e) => {},
                onCompletedDelegate = onCompleted != null ? onCompleted : () => {}
            };

            return observable.Subscribe(observer);
        }
        
        class SimpleObserver<T> : IObserver<T>
        {
            public Action<T> onNextDelegate { set; get; }
            public Action<Exception> onErrorDelegate { set; get; }
            public Action onCompletedDelegate { set; get; }

            public void OnNext(T value) { this.onNextDelegate(value); }
            public void OnError(Exception e) { this.onErrorDelegate(e); }
            public void OnCompleted() { this.onCompletedDelegate(); }
        }
    }
}
