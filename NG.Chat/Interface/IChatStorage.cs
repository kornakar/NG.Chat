using NG.Chat.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NG.Chat.Interface
{
    public interface IChatStorage
    {
        /// <summary>
        /// Saves the message to the persistent storage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SaveMessage(IChatMessage message);

        /// <summary>
        /// Gets all the messages saved
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IChatMessage>> GetMessages();

        /// <summary>
        /// Gets all the messages based on the given date limit
        /// </summary>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        Task<IEnumerable<IChatMessage>> GetLatestMessages(DateTime timeLimit);

        IChatUser AddOrUpdateUser(string username);

        void DeleteUser(string username);
    }
}
