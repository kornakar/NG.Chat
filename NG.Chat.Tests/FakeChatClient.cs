using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NG.Chat.Model;
using NG.Chat.Model.Interface;

namespace NG.Chat.Tests
{
    public class FakeChatClient : ChatClientBase
    {
        public override Task Join(string username)
        {
            throw new NotImplementedException();
        }

        public override Task Leave(string username)
        {
            throw new NotImplementedException();
        }

        public override Task SendMessage(ChatMessage message)
        {
            OnNextMessage(message);
            return Task.CompletedTask;
        }

        public override Task GetLatestMessages()
        {
            throw new NotImplementedException();
        }

        public override Task GetActiveUsers()
        {
            throw new NotImplementedException();
        }
    }
}
