using NG.Chat.Interface;
using NG.Chat.Model.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using NG.Chat.Model;

namespace NG.Chat.SignalRChatClient
{
    // TODO: observable pattern https://msdn.microsoft.com/en-us/library/dd990377(v=vs.110).aspx
    [Export(typeof(IChatClient))]
    public class SignalRChatClient : ChatClientBase
    {
        private string _hubUrl = @"http://wpfchatbackend342535344.azurewebsites.net/signalr/hubs";
        private Guid _userGuid = Guid.NewGuid();
        private IHubProxyFactory _hubProxyFactory;
        private IHubProxy _proxy;

        public SignalRChatClient() : base()
        {
        }

        [ImportingConstructor]
        public SignalRChatClient(IHubProxyFactory hubProxyFactory)
        {
            _hubProxyFactory = hubProxyFactory;
        }

        private async Task BroadcastAsync(IChatMessage msg)
        {
            await DoConnect().ConfigureAwait(true);
            await _proxy.Invoke("Send", msg).ConfigureAwait(true);
        }

        private async Task DoConnect()
        {
            _proxy = await _hubProxyFactory.CreateAsync(_hubUrl, "ChatHub", (conn) => { }, (proxy) => { }, () => { }, (ex) => { }, () => { });

            _proxy.On<ChatMessage>(nameof(IChat.broadcastMessage), OnMessage);
            //_proxy.On<ChatMessage>(nameof(IChat.activeUsers), OnMessage);
        }

        private async void OnMessage(ChatMessage msg)
        {
            msg.SendTime = DateTime.Now;
            this.OnNextMessage(msg);
        }

        public override async Task SendMessage(IChatMessage message)
        {
            await BroadcastAsync(message);
        }

        public override IList<ChatUser> GetActiveUsers()
        {
            throw new NotImplementedException();
        }

        public override IList<IChatMessage> GetLatestMessages()
        {
            throw new NotImplementedException();
        }
    }
}
