using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NG.Chat.Model.Interface;

namespace NG.Chat.Model
{
    public class ChatMessage : IChatMessage
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string MessageText { get; set; }
        public DateTime SendTime { get; set; }
    }
}
