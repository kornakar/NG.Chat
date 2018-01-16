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
        public string UserName { get; set; }
        public bool IsNotConnected { get; set; }

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
            await SendCurrentMessageAsync();
        }

        public async void JoinChat()
        {
            await JoinChatAsync();
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
            IsNotConnected = true;
        }

        private async Task JoinChatAsync()
        {
            await _chatClient.Join(UserName).ConfigureAwait(true);

            // NOTE: double negative for reverse visibility
            IsNotConnected = false;
            NotifyOfPropertyChange(nameof(IsNotConnected));
        }

        private async Task SendCurrentMessageAsync()
        {
            await _chatClient.SendMessage(new ChatMessage { Username = UserName, MessageText = CurrentMessage }).ConfigureAwait(true);

            CurrentMessage = null;
            NotifyOfPropertyChange(nameof(CurrentMessage));
        }
    }
}
