using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;
using WpfChatApp.ViewModels;

namespace NG.Chat.Tests
{
    [TestClass]
    public class ViewModelTests : ReactiveTest
    {
        TestScheduler _testScheduler = new TestScheduler();

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TextNgChatViewModel_Constructor()
        {
            Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);
            clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(Mock.Of<IDisposable>());
            clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatUser>>())).Returns(Mock.Of<IDisposable>());

            NgChatViewModel vm = new NgChatViewModel(clientMock.Object);
        }

        [TestMethod]
        public void TextNgChatViewModel_SendMessage()
        {
            // Arrange
            Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);
            clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(Mock.Of<IDisposable>());
            clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatUser>>())).Returns(Mock.Of<IDisposable>());
            clientMock.Setup(c => c.SendMessage(It.IsAny<IChatMessage>())).Returns(Task.CompletedTask);

            string messageText = "TESTMSG";
            NgChatViewModel vm = new NgChatViewModel(clientMock.Object);

            // Act
            vm.CurrentMessage = messageText;
            vm.SendMessage();

            // Assert
            clientMock.Verify(c => c.SendMessage(It.Is<ChatMessage>(m => m.MessageText == messageText)), Times.Once);
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
        //    Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);
        //    clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(Mock.Of<IDisposable>());
        //    clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatUser>>())).Returns(userStream);
        //    NgChatViewModel vm = new NgChatViewModel(clientMock.Object);
        //    ChatUser user = new ChatUser {Name = "TEST USER", ClientId = "client id", JoinTime = DateTime.Now};

        //    // Act
        //    var input = _testScheduler.CreateHotObservable(OnNext(100, user), OnCompleted<ChatUser>(200));
        //    userStream.OnNext(user);

        //    // Assert
        //    Assert.AreEqual(1, vm.ChatUsers.Count);
        //}

        [TestMethod]
        public void TextNgChatViewModel_ClosedAndDisposed()
        {
            // Arrange
            Mock<IDisposable> messageObservableMock = new Mock<IDisposable>();
            Mock<IDisposable> userObservableMock = new Mock<IDisposable>();
            Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);
            clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatMessage>>())).Returns(messageObservableMock.Object);
            clientMock.Setup(c => c.Subscribe(It.IsAny<IObserver<IChatUser>>())).Returns(userObservableMock.Object);
            NgChatViewModel vm = new NgChatViewModel(clientMock.Object);

            // Act
            vm.TryClose();

            // Assert
            messageObservableMock.Verify(m => m.Dispose(), Times.Once);
            userObservableMock.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
