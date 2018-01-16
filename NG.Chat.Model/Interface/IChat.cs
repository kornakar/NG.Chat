using System;
using System.Collections.Generic;
using System.Text;

namespace NG.Chat.Model.Interface
{
    /// <summary>
    /// Interface definition for the Chat Hub client side
    /// </summary>
    public interface IChat
    {
        void broadcastMessage(ChatMessage message);

        void messages(IList<ChatMessage> messages);
        void activeUsers(IList<ChatUser> users);

        void userJoined(ChatUser user);
        void userLeft(ChatUser user);
    }
}
