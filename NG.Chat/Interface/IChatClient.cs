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
        Task Join(string username);
        Task Leave(string username);
        Task SendMessage(ChatMessage message);
        Task GetLatestMessages();
        Task GetActiveUsers();
    }
}
