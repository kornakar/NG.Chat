using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;

namespace NG.Chat
{
    public abstract class ChatClientBase : IChatClient
    {
        protected IChatStorage<IChatMessage> ChatStorage;
        protected List<IObserver<IChatMessage>> MessageObservers;
        protected List<IObserver<IChatUser>> UserObservers;

        protected ChatClientBase()
        {
            MessageObservers = new List<IObserver<IChatMessage>>();
            UserObservers = new List<IObserver<IChatUser>>();
        }

        public abstract Task SendMessage(IChatMessage message);
        public abstract IList<ChatUser> GetActiveUsers();
        public abstract IList<IChatMessage> GetLatestMessages();
        
        public virtual void Disconnect()
        {
            throw new NotImplementedException();
        }

        #region IObservable

        public IDisposable SubscribeMessages(IObserver<IChatMessage> observer)
        {
            if (!MessageObservers.Contains(observer))
                MessageObservers.Add(observer);
            return new Unsubscriber<IChatMessage>(MessageObservers, observer);
        }

        public void MessagesCompleted()
        {
            foreach (var observer in MessageObservers.ToArray())
                if (MessageObservers.Contains(observer))
                    observer.OnCompleted();

            MessageObservers.Clear();
        }

        public IDisposable SubscribeUsers(IObserver<IChatUser> observer)
        {
            if (!UserObservers.Contains(observer))
                UserObservers.Add(observer);
            return new Unsubscriber<IChatUser>(UserObservers, observer);
        }

        public void UsersCompleted()
        {
            foreach (var observer in MessageObservers.ToArray())
                if (MessageObservers.Contains(observer))
                    observer.OnCompleted();

            MessageObservers.Clear();
        }

        private class Unsubscriber<TObserve> : IDisposable
        {
            private List<IObserver<TObserve>> _observers;
            private IObserver<TObserve> _observer;

            public Unsubscriber(List<IObserver<TObserve>> observers, IObserver<TObserve> observer)
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
        
        #endregion IObservable

        protected void OnNextMessage(IChatMessage item)
        {
            foreach (var observer in MessageObservers)
            {
                observer.OnNext(item);
            }
        }

        protected void OnNextUser(IChatUser item)
        {
            foreach (var observer in UserObservers)
            {
                observer.OnNext(item);
            }
        }

        public IDisposable Subscribe(IObserver<IChatMessage> observer)
        {
            return SubscribeMessages(observer);
        }

        public IDisposable Subscribe(IObserver<IChatUser> observer)
        {
            return SubscribeUsers(observer);
        }
    }
}
