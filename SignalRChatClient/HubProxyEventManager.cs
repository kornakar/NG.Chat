using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace NG.Chat.SignalRChatClient
{
    [Export(typeof(IHubProxyEventManager))]
    public class HubProxyEventManager : IHubProxyEventManager
    {
        public void RegisterToEvent<T>(IHubProxy hubProxy, string eventName, Action<T> onEventName)
        {
            hubProxy.On<T>(eventName, onEventName);
        }
    }
}
