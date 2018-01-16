using NG.Chat.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NG.Chat.Model.Interface;

namespace NG.Chat.EntityDbStorage
{
    public class EntityDbChatStorage : DbContext, IChatStorage
    {
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

        public IChatUser AddOrUpdateUser(string username)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
