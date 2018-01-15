using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NG.Chat.Model.Interface;

namespace NG.Chat.Model
{
    public class ChatUser : IChatUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public DateTime JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }

        /// <summary> 
        /// Propoerty to get/set multiple ConnectionId 
        /// </summary> 
        public HashSet<string> ConnectionIds
        {
            get;
            set;
        }
    }
}
