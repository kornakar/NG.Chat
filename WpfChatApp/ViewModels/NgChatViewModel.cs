using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;
using WpfChatApp.Interface;

namespace WpfChatApp.ViewModels
{
    [Export(typeof(IChatScreen))]
    public class NgChatViewModel : Screen, IChatScreen
    {
        private Guid _userGuid = Guid.NewGuid();

        private IDisposable messageSubscription;
        private IDisposable userSubscription;

        #region Imports

        [Import]
        private IChatClient _chatClient;

        #endregion Imports

        public int TabOrder => 1;

        public string CurrentMessage { get; set; }

        public ObservableCollection<IChatMessage> ChatMessages { get; set; } = new ObservableCollection<IChatMessage>();
        public ObservableCollection<IChatUser> ChatUsers { get; set; } = new ObservableCollection<IChatUser>();

        public NgChatViewModel()
        {
            Initialize();
        }

        [ImportingConstructor]
        public NgChatViewModel(IChatClient chatClient) : this()
        {
            _chatClient = chatClient;

            messageSubscription = _chatClient.Subscribe<IChatMessage>(m => ChatMessages.Add(m));
            userSubscription = _chatClient.Subscribe<IChatUser>(u => ChatUsers.Add(u));
        }

        public async void SendMessage()
        {
            await SendCurrentMessage();
        }

        public override void TryClose(bool? dialogResult = default(bool?))
        {
            messageSubscription.Dispose();
            userSubscription.Dispose();

            base.TryClose(dialogResult);
        }

        private void Initialize()
        {
            DisplayName = "Chat";
        }

        private async Task SendCurrentMessage()
        {
            await _chatClient.SendMessage(new ChatMessage { Username = _userGuid.ToString(), MessageText = CurrentMessage }).ConfigureAwait(true);

            CurrentMessage = null;
            NotifyOfPropertyChange(nameof(CurrentMessage));
        }
    }
}
