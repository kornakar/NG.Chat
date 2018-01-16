using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;
using NG.Chat.SignalRChatClient;

namespace NG.Chat.Client.Tests
{
    [TestClass]
    public class ChatClientTests
    {
        //private TestScheduler _testScheduler = new TestScheduler();

        private Mock<IHubProxyFactory> _proxyFactoryMock = new Mock<IHubProxyFactory>(MockBehavior.Strict);
        //private Mock<IHubProxyEventManager> _hubProxyEventManagerMock = new Mock<IHubProxyEventManager>(MockBehavior.Strict);
        private Mock<IHubProxy> _hubProxyMock = new Mock<IHubProxy>();

        private IChatClient _client;
        private IHubProxyEventManager _fakeHubProxyEventManager = new FakeHubProxyEventManager();

        [TestInitialize]
        public void Initialize()
        {
            _proxyFactoryMock.Setup(p => p.CreateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_hubProxyMock.Object);

            //_hubProxyEventManagerMock.Setup(p => p.RegisterToEvent(_hubProxyMock.Object, nameof(IChat.broadcastMessage), It.IsAny<Action<ChatMessage>>())).Callback((IHubProxy p, string m, Action<ChatMessage> a) => _broadcastMessageCallback = a);
            //_hubProxyEventManagerMock.Setup(p => p.RegisterToEvent(_hubProxyMock.Object, nameof(IChat.userJoined), It.IsAny<Action<ChatUser>>()));
            //_hubProxyEventManagerMock.Setup(p => p.RegisterToEvent(_hubProxyMock.Object, nameof(IChat.userLeft), It.IsAny<Action<ChatUser>>()));
            //_hubProxyEventManagerMock.Setup(p => p.RegisterToEvent(_hubProxyMock.Object, nameof(IChat.messages), It.IsAny<Action<IList<ChatMessage>>>()));

            _client = new SignalRChatClient.SignalRChatClient(_proxyFactoryMock.Object, _fakeHubProxyEventManager);
        }

        [TestMethod]
        public async Task TestSignalRClient_SendMessage()
        {
            // Arrange
            ChatMessage msg = new ChatMessage { MessageText = "TESTMSG", Username = "TEST USER" };

            // Act
            await _client.SendMessage(msg).ConfigureAwait(false);

            // Assert
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChat.broadcastMessage), msg));
        }

        [TestMethod]
        public async Task TestSignalRClient_Join()
        {
            // Arrange
            string userName = "TEST USER";

            // Act
            await _client.Join(userName).ConfigureAwait(false);

            // Assert
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChat.userJoined), userName));
        }

        [TestMethod]
        public async Task TestSignalRClient_Leave()
        {
            // Arrange
            string userName = "TEST USER";

            // Act
            await _client.Leave(userName).ConfigureAwait(false);

            // Assert
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChat.userLeft), userName));
        }

        [TestMethod]
        public async Task TestSignalRClient_GetMessages()
        {
            await _client.GetLatestMessages().ConfigureAwait(false);
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChat.messages)));
        }

        [TestMethod]
        public async Task TestSignalRClient_GetActiveUsers()
        {
            await _client.GetActiveUsers().ConfigureAwait(false);
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChat.activeUsers)));
        }

        private class FakeHubProxyEventManager : IHubProxyEventManager
        {
            // TODO: check that the event was invoked
            public void RegisterToEvent<T>(IHubProxy hubProxy, string eventName, Action<T> onEventName)
            {
                
            }
        }
    }
}
