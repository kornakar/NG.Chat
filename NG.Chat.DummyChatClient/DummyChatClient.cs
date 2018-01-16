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
        public override Task Join(string username)
        {
            OnNextUser(new ChatUser { Name = username, ClientId = Guid.NewGuid().ToString() });
            return Task.CompletedTask;
        }

        public override Task Leave(string username)
        {
            OnNextUser(new ChatUser {Name = username, ClientId = Guid.NewGuid().ToString(), LeaveTime = DateTime.Now});
            return Task.CompletedTask;
        }

        public override Task SendMessage(IChatMessage message)
        {
            OnNextMessage(message);
            return Task.CompletedTask;
        }

        public override Task GetActiveUsers()
        {
            OnNextUser(new ChatUser {Name = "Dummy User", ClientId = Guid.NewGuid().ToString()});
            return Task.CompletedTask;
        }

        public override Task GetLatestMessages()
        {
            OnNextMessage(new ChatMessage {MessageText = "Some message", Username = "Dummy User", SendTime = DateTime.Now});
            return Task.CompletedTask;
        }
    }
}
