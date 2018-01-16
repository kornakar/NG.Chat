using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace NG.Chat.SignalRChatClient
{
    /// <summary>
    /// Factory class for creating IHubProxy
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/24784531/moq-can-mock-hubconnection-but-rhinomocks-cant
    /// </remarks>
    [Export(typeof(IHubProxyFactory))]
    public class HubProxyFactory : IHubProxyFactory
    {
        public async Task<IHubProxy> CreateAsync(string hubUrl, string hubName, Action<Exception> faulted)
        {
            return await CreateAsync(hubUrl, hubName, faulted, null, null, null, null);
        }

        public async Task<IHubProxy> CreateAsync(string hubUrl, string hubName, Action<Exception> faulted, Action<IHubConnection> configureConnection, Action<IHubProxy> onStarted, Action reconnected, Action connected)
        {
            HubConnection connection = new HubConnection(hubUrl);
            configureConnection?.Invoke(connection);

            IHubProxy proxy = connection.CreateHubProxy(hubName);

            if (reconnected != null)
            {
                connection.Reconnected += reconnected;
            }

            if (faulted != null)
            {
                connection.Error += faulted;
            }

            bool isConnected = false;

            try
            {
                await connection.Start();

                if (isConnected)
                {
                    reconnected();
                }
                else
                {
                    isConnected = true;

                    onStarted?.Invoke(proxy);
                    connected?.Invoke();
                }
            }
            catch (Exception ex)
            {
                faulted?.Invoke(ex);
            }

            return proxy;
        }
    }
}
