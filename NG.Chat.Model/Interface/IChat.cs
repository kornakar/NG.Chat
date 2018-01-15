using System;
using System.Collections.Generic;
using System.Text;

namespace NG.Chat.Model.Interface
{
    /// <summary>
    /// Interface definition for the Chat Hub
    /// </summary>
    public interface IChat
    {
        void broadcastMessage(ChatMessage message);
        void messages(IList<ChatMessage> messages);
        void activeUsers(IList<ChatUser> users);
        void userConnected(string username);
        void userDisconnected(string username);
    }
}
