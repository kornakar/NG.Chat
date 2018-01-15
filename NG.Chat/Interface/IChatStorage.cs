using NG.Chat.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NG.Chat.Interface
{
    public interface IChatStorage<T>
    {
        /// <summary>
        /// Saves the message to the persistent storage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SaveMessage(T message);

        /// <summary>
        /// Gets all the messages saved
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetMessages();

        /// <summary>
        /// Gets all the messages based on the given date limit
        /// </summary>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetLatestMessages(DateTime timeLimit);
    }
}
