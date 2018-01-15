using System;
using System.Collections.Generic;
using System.Text;

namespace NG.Chat.Model.Interface
{
    public interface IChatUser
    {
        string Name { get; set; }
        string ClientId { get; set; }
        DateTime JoinTime { get; set; }
        DateTime? LeaveTime { get; set; }
    }
}
