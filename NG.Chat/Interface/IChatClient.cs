using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NG.Chat.Model;
using NG.Chat.Model.Interface;

namespace NG.Chat.Interface
{
    public interface IChatClient : IObservable<IChatMessage>,
                                   IObservable<IChatUser>
    {
        Task SendMessage(IChatMessage message);
        IList<IChatMessage> GetLatestMessages();
        //IObservable<T> MessagesObservable();

        IList<ChatUser> GetActiveUsers();
        //IObservable<ChatUser> UsersObservable();
    }
}
