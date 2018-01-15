using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NG.Chat.Interface;
using NG.Chat.Model.Interface;

namespace NG.Chat
{
    public abstract class ChatStorageBase : IChatStorage<IChatMessage>
    {
        protected ChatStorageBase()
        {
        }

        public Task SaveMessage(IChatMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IChatMessage>> GetMessages()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IChatMessage>> GetLatestMessages(DateTime timeLimit)
        {
            throw new NotImplementedException();
        }
    }
}
