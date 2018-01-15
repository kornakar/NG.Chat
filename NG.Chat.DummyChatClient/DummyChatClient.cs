using NG.Chat.Interface;
using NG.Chat.Model.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using NG.Chat.Model;

namespace NG.Chat.DummyChatClient
{
    [Export(typeof(IChatClient))]
    public class DummyChatClient : ChatClientBase
    {
        public override Task SendMessage(IChatMessage message)
        {
            OnNextMessage(message);
            return Task.CompletedTask;
        }

        public override IList<ChatUser> GetActiveUsers()
        {
            return new List<ChatUser> { new ChatUser { Name = "Dummy User", ClientId=Guid.NewGuid().ToString() } };
        }

        public override IList<IChatMessage> GetLatestMessages()
        {
            return new List<IChatMessage> { new ChatMessage { MessageText = "Some message", Username = "Dummy User", SendTime = DateTime.Now } };
        }
    }
}
