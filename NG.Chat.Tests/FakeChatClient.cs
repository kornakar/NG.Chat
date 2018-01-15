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
        public override Task SendMessage(IChatMessage message)
        {
            OnNextMessage(message);
            return Task.CompletedTask;
        }

        public override IList<IChatMessage> GetLatestMessages()
        {
            throw new NotImplementedException();
        }

        public override IList<ChatUser> GetActiveUsers()
        {
            throw new NotImplementedException();
        }
    }
}
