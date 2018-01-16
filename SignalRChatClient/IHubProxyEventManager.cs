using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace NG.Chat.SignalRChatClient
{
    public interface IHubProxyEventManager
    {
        void RegisterToEvent<T>(IHubProxy hubProxy, string eventName, Action<T> onEventName);
    }
}
