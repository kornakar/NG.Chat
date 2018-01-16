using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;
using WpfChatApp.ViewModels;
using System.Reactive;

namespace NG.Chat.Tests
{
    [TestClass]
    public class ViewModelTests : ReactiveTest
    {
        private TestScheduler _testScheduler = new TestScheduler();
        private Mock<IChatClient> _clientMock;

        [TestInitialize]
        public void Initialize()
        {
            _clientMock = new Mock<IChatClient>(MockBehavior.Strict);
            _clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(Mock.Of<IDisposable>());
            _clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatUser>>())).Returns(Mock.Of<IDisposable>());
        }

        [TestMethod]
        public void TextNgChatViewModel_Constructor()
        {


            NgChatViewModel vm = new NgChatViewModel(_clientMock.Object);
        }

        [TestMethod]
        public void TextNgChatViewModel_SendMessage()
        {
            // Arrange
            _clientMock.Setup(c => c.SendMessage(It.IsAny<ChatMessage>())).Returns(Task.CompletedTask);

            string messageText = "TESTMSG";
            NgChatViewModel vm = new NgChatViewModel(_clientMock.Object);

            // Act
            vm.CurrentMessage = messageText;
            vm.SendMessage();

            // Assert
            _clientMock.Verify(c => c.SendMessage(It.Is<ChatMessage>(m => m.MessageText == messageText)), Times.Once);
        }

        [TestMethod]
        public void TextNgChatViewModel_JoinChat()
        {
            // Arrange
            _clientMock.Setup(c => c.Join(It.IsAny<string>())).Returns(Task.CompletedTask);
            _clientMock.Setup(c => c.GetLatestMessages()).Returns(Task.CompletedTask);

            string userName = "TEST USER";
            NgChatViewModel vm = new NgChatViewModel(_clientMock.Object);

            // Act
            Assert.IsTrue(vm.IsNotConnected);
            vm.UserName = userName;
            vm.JoinChat();

            // Assert
            _clientMock.Verify(c => c.Join(It.Is<string>(s => s == userName)), Times.Once);
            Assert.IsFalse(vm.IsNotConnected);
        }

        [TestMethod]
        public void TextNgChatViewModel_JoinAndLeaveChat()
        {
            // Arrange
            _clientMock.Setup(c => c.Join(It.IsAny<string>())).Returns(Task.CompletedTask);
            _clientMock.Setup(c => c.Leave(It.IsAny<string>())).Returns(Task.CompletedTask);
            _clientMock.Setup(c => c.GetLatestMessages()).Returns(Task.CompletedTask);

            string userName = "TEST USER";
            NgChatViewModel vm = new NgChatViewModel(_clientMock.Object);

            // Act
            Assert.IsTrue(vm.IsNotConnected);
            vm.UserName = userName;
            vm.JoinChat();
            vm.LeaveChat();

            // Assert
            _clientMock.Verify(c => c.Leave(It.Is<string>(s => s == userName)), Times.Once);
            Assert.IsFalse(vm.IsConnected);
            Assert.AreEqual(0, vm.ChatMessages.Count);
            Assert.AreEqual(0, vm.ChatUsers.Count);
        }

        [TestMethod]
        public void TextNgChatViewModel_ReceiveMessageFromClient()
        {
            // Arrange
            IChatClient fakeClient = new FakeChatClient();
            ChatMessage msg = new ChatMessage { MessageText = "TESTMSG" };
            NgChatViewModel vm = new NgChatViewModel(fakeClient);

            // Act
            fakeClient.SendMessage(msg);

            // Assert
            Assert.AreEqual(1, vm.ChatMessages.Count);
            Assert.AreEqual(msg, vm.ChatMessages.First());
        }

        //[TestMethod]
        //public void TextNgChatViewModel_UserConnected()
        //{
        //    // Arrange
        //    Subject<IChatUser> userStream = new Subject<IChatUser>();
        //    IObservable<ChatUser> userObservable;
        //    Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);
        //    clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(Mock.Of<IDisposable>());
        //    NgChatViewModel vm = new NgChatViewModel(clientMock.Object);
        //    ChatUser user = new ChatUser { Name = "TEST USER", ClientId = "client id", JoinTime = DateTime.Now };

        //    // Act
        //    ITestableObservable<ChatUser> source = _testScheduler.CreateColdObservable(
        //        new Recorded<Notification<ChatUser>>(100, Notification.CreateOnNext<ChatUser>(user)),
        //        new Recorded<Notification<ChatUser>>(200, Notification.CreateOnCompleted<ChatUser>()));

        //    // Assert
        //    Assert.AreEqual(1, vm.ChatUsers.Count);
        //}

        [TestMethod]
        public void TextNgChatViewModel_ClosedAndDisposed()
        {
            // Arrange
            Mock<IDisposable> messageObservableMock = new Mock<IDisposable>();
            Mock<IDisposable> userObservableMock = new Mock<IDisposable>();
            _clientMock = new Mock<IChatClient>(MockBehavior.Strict);
            _clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(messageObservableMock.Object);
            _clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatUser>>())).Returns(userObservableMock.Object);
            NgChatViewModel vm = new NgChatViewModel(_clientMock.Object);

            // Act
            vm.TryClose();

            // Assert
            messageObservableMock.Verify(m => m.Dispose(), Times.Once);
            userObservableMock.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
