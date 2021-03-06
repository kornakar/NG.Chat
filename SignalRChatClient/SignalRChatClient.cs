﻿using NG.Chat.Interface;
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
    [Export(typeof(IChatClient))]
    public class SignalRChatClient : ChatClientBase
    {
        //private string _hubUrl = @"http://wpfchatbackend342535344.azurewebsites.net/";
        private string _hubUrl = @"http://localhost:50010/";

        private Guid _userGuid = Guid.NewGuid();
        private IHubProxyFactory _hubProxyFactory;
        private IHubProxyEventManager _hubProxyeventManager;
        private IHubProxy _proxy;

        public SignalRChatClient() : base()
        {
        }

        [ImportingConstructor]
        public SignalRChatClient(IHubProxyFactory hubProxyFactory, IHubProxyEventManager hubProxyEventManager) : this()
        {
            _hubProxyFactory = hubProxyFactory;
            _hubProxyeventManager = hubProxyEventManager;
        }

        public override async Task Join(string username)
        {
            await DoConnect().ConfigureAwait(false);
            await _proxy.Invoke(nameof(IChatHub.UserJoined), username).ConfigureAwait(false);
        }

        public override async Task Leave(string username)
        {
            await DoConnect().ConfigureAwait(false);
            await _proxy.Invoke(nameof(IChatHub.UserLeft), username).ConfigureAwait(false);
        }

        public override async Task SendMessage(ChatMessage message)
        {
            await DoConnect().ConfigureAwait(false);
            await _proxy.Invoke(nameof(IChatHub.Send), message).ConfigureAwait(false);
        }

        public override async Task GetActiveUsers()
        {
            await DoConnect().ConfigureAwait(false);
            await _proxy.Invoke(nameof(IChatHub.GetActiveUsers)).ConfigureAwait(false);
        }

        public override async Task GetLatestMessages()
        {
            await DoConnect().ConfigureAwait(false);
            await _proxy.Invoke(nameof(IChatHub.GetLatestMessages)).ConfigureAwait(false);
        }

        private async Task DoConnect()
        {
            if (_proxy == null)
            {
                _proxy = await _hubProxyFactory.CreateAsync(_hubUrl, "ChatHub", OnError).ConfigureAwait(false);

                _hubProxyeventManager.RegisterToEvent<ChatMessage>(_proxy, nameof(IChat.broadcastMessage), OnMessage);
                _hubProxyeventManager.RegisterToEvent<ChatUser>(_proxy, nameof(IChat.userJoined), OnUserJoined);
                _hubProxyeventManager.RegisterToEvent<ChatUser>(_proxy, nameof(IChat.userLeft), OnUserLeft);
                _hubProxyeventManager.RegisterToEvent<IList<ChatMessage>>(_proxy, nameof(IChat.messages), OnMessages);
            }
        }

        private void OnMessages(IList<ChatMessage> messages)
        {
            foreach (var msg in messages)
            {
                this.OnNextMessage(msg);
            }
        }

        private void OnUserJoined(ChatUser user)
        {
            this.OnNextUser(user);
        }

        private void OnUserLeft(ChatUser user)
        {
            // This is how we identify left users in the observable stream
            if (!user.LeaveTime.HasValue)
            {
                user.LeaveTime = DateTime.Now;
            }

            this.OnNextUser(user);
        }

        private void OnMessage(ChatMessage msg)
        {
            msg.SendTime = DateTime.Now;
            this.OnNextMessage(msg);
        }

        private void OnError(Exception ex)
        {
            this.OnNextMessage(new ChatMessage {MessageText = ex.Message, Username = "ERROR"});
        }
    }
}
