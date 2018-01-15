using System;
using System.Collections.Generic;
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
    public class HubProxyFactory : IHubProxyFactory
    {
        public async Task<IHubProxy> CreateAsync(string hubUrl, string hubName, Action<IHubConnection> configureConnection, Action<IHubProxy> onStarted, Action reconnected, Action<Exception> faulted, Action connected)
        {
            HubConnection connection = new HubConnection(hubUrl);
            configureConnection?.Invoke(connection);

            IHubProxy proxy = connection.CreateHubProxy(hubName);
            connection.Reconnected += reconnected;
            connection.Error += faulted;

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
                    onStarted(proxy);
                    connected();
                }
            }
            catch (Exception ex)
            {
                faulted(ex);
            }

            return proxy;
        }
    }
}
