using System;
using System.Collections.Generic;
using System.Text;

namespace NG.Chat.Model.Interface
{
    public interface IChatMessage
    {
        string Username { get; set; }
        string MessageText { get; set; }
        DateTime SendTime { get; set; }
    }
}
