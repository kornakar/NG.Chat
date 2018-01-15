using System;
using System.Collections.Generic;
using System.Text;
using NG.Chat.Interface;
using NG.Chat.Model.Interface;

namespace NG.Chat
{
    public abstract class ChatMessageProviderBase<T> : IChatMessageProvider<T> where T : IChatMessage
    {
        protected List<IObserver<T>> Observers;

        protected ChatMessageProviderBase()
        {
            Observers = new List<IObserver<T>>();
        }

        #region IObservable

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!Observers.Contains(observer))
                Observers.Add(observer);
            return new Unsubscriber(Observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in Observers.ToArray())
                if (Observers.Contains(observer))
                    observer.OnCompleted();

            Observers.Clear();
        }

        #endregion IObservable

        protected void OnNext(T item)
        {
            foreach (var observer in Observers)
            {
                observer.OnNext(item);
            }
        }
    }
}
