﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace NG.Chat.SignalRChatClient
{
    public interface IHubProxyFactory
    {
        Task<IHubProxy> CreateAsync(string hubUrl, string hubName, Action<Exception> faulted);
        Task<IHubProxy> CreateAsync(string hubUrl, string hubName, Action<Exception> faulted, Action<IHubConnection> configureConnection, Action<IHubProxy> onStarted, Action reconnected, Action connected);
    }
}
