using System;
using System.Collections;
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
        private TestScheduler _testScheduler = new TestScheduler();

        private Mock<IHubProxyFactory> _proxyFactoryMock = new Mock<IHubProxyFactory>(MockBehavior.Strict);
        private Mock<IHubProxy> _hubProxyMock = new Mock<IHubProxy>();

        private IChatClient _client;
        private FakeHubProxyEventManager _fakeHubProxyEventManager = new FakeHubProxyEventManager();

        [TestInitialize]
        public void Initialize()
        {
            _proxyFactoryMock.Setup(p => p.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<Exception>>())).ReturnsAsync(_hubProxyMock.Object);
            _client = new SignalRChatClient.SignalRChatClient(_proxyFactoryMock.Object, _fakeHubProxyEventManager);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _fakeHubProxyEventManager.RegisteredEvents.Clear();
        }

        [TestMethod]
        public async Task TestSignalRClient_EventsRegistered()
        {
            // Arrange
            ChatMessage msg = new ChatMessage { MessageText = "TESTMSG", Username = "TEST USER" };

            // Act
            await _client.SendMessage(msg).ConfigureAwait(false);

            var broadcastAction = _fakeHubProxyEventManager.RegisteredEvents[nameof(IChat.broadcastMessage)];
            var joinAction = _fakeHubProxyEventManager.RegisteredEvents[nameof(IChat.userJoined)];
            var leaveAction = _fakeHubProxyEventManager.RegisteredEvents[nameof(IChat.userLeft)];
            var messagesAction = _fakeHubProxyEventManager.RegisteredEvents[nameof(IChat.messages)];

            // Assert
            Assert.IsNotNull(broadcastAction, $"{nameof(IChat.broadcastMessage)} was not registered!");
            Assert.IsNotNull(joinAction, $"{nameof(IChat.userJoined)} was not registered!");
            Assert.IsNotNull(leaveAction, $"{nameof(IChat.userLeft)} was not registered!");
            Assert.IsNotNull(messagesAction, $"{nameof(IChat.messages)} was not registered!");
        }

        [TestMethod]
        public async Task TestSignalRClient_SendMessage()
        {
            // Arrange
            ChatMessage msg = new ChatMessage { MessageText = "TESTMSG", Username = "TEST USER" };

            // Act
            await _client.SendMessage(msg).ConfigureAwait(false);

            // Assert
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChatHub.Send), msg));
        }

        [TestMethod]
        public async Task TestSignalRClient_Join()
        {
            // Arrange
            string userName = "TEST USER";

            // Act
            await _client.Join(userName).ConfigureAwait(false);

            // Assert
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChatHub.UserJoined), userName));
        }

        [TestMethod]
        public async Task TestSignalRClient_Leave()
        {
            // Arrange
            string userName = "TEST USER";

            // Act
            await _client.Leave(userName).ConfigureAwait(false);

            // Assert
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChatHub.UserLeft), userName));
        }

        [TestMethod]
        public async Task TestSignalRClient_GetMessages()
        {
            await _client.GetLatestMessages().ConfigureAwait(false);
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChatHub.GetLatestMessages)));
        }

        [TestMethod]
        public async Task TestSignalRClient_GetActiveUsers()
        {
            await _client.GetActiveUsers().ConfigureAwait(false);
            _hubProxyMock.Verify(p => p.Invoke(nameof(IChatHub.GetActiveUsers)));
        }

        private class FakeHubProxyEventManager : IHubProxyEventManager
        {
            public IDictionary<string, object> RegisteredEvents = new Dictionary<string, object>();

            // TODO: check that the event was invoked
            public void RegisterToEvent<T>(IHubProxy hubProxy, string eventName, Action<T> onEventName)
            {
                RegisteredEvents.Add(eventName, onEventName);
            }
        }
    }
}
