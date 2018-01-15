using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;
using WpfChatApp.ViewModels;

namespace NG.Chat.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        [TestMethod]
        public void TextNgChatViewModel_Constructor()
        {
            Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);

            NgChatViewModel vm = new NgChatViewModel(clientMock.Object);
        }

        [TestMethod]
        public void TextNgChatViewModel_SendMessage()
        {
            Mock<IChatClient> clientMock = new Mock<IChatClient>(MockBehavior.Strict);
            string messageText = "TESTMSG";
            NgChatViewModel vm = new NgChatViewModel(clientMock.Object);

            vm.CurrentMessage = messageText;
            vm.SendMessage();

            clientMock.Verify(c => c.SendMessage(It.Is<ChatMessage>(m => m.MessageText == messageText)), Times.Once);
        }
    }
}
