using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NG.Chat.Model;

namespace NG.Chat.Interface
{
    public interface IChatHub
    {
        void Send(ChatMessage message);
        void GetActiveUsers();
        void GetLatestMessages();
        void UserJoined(string username);
        void UserLeft(string username);
    }
}
