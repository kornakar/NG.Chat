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
        private IDisposable messageSubscription;
        private IDisposable userSubscription;

        #region Imports

        [Import]
        private IChatClient _chatClient;

        #endregion Imports

        public int TabOrder => 1;

        public string CurrentMessage { get; set; }
        public string UserName { get; set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyOfPropertyChange(nameof(IsConnected));
                NotifyOfPropertyChange(nameof(IsNotConnected));
            }
        }
        public bool IsNotConnected => !IsConnected;

        /// <summary>
        /// Contains all the chat messages
        /// </summary>
        public ObservableCollection<IChatMessage> ChatMessages { get; set; } = new ObservableCollection<IChatMessage>();

        /// <summary>
        /// Contains all the chat users
        /// </summary>
        public ObservableCollection<IChatUser> ChatUsers { get; set; } = new ObservableCollection<IChatUser>();

        public NgChatViewModel()
        {
            Initialize();
        }

        [ImportingConstructor]
        public NgChatViewModel(IChatClient chatClient) : this()
        {
            _chatClient = chatClient;

            messageSubscription = _chatClient.Subscribe<IChatMessage>(OnChatMessage);
            userSubscription = _chatClient.Subscribe<IChatUser>(OnChatUser);
        }

        #region Methods

        public async void SendMessage()
        {
            await SendCurrentMessageAsync();
        }

        public async void JoinChat()
        {
            await JoinChatAsync();
        }

        public async void LeaveChat()
        {
            await LeaveChatAsync();
        }

        public override async void TryClose(bool? dialogResult = default(bool?))
        {
            base.TryClose(dialogResult);

            await LeaveChatAndCloseAsync();
        }

        private async Task LeaveChatAndCloseAsync()
        {
            if (IsConnected)
            {
                await LeaveChatAsync().ConfigureAwait(false);
            }

            messageSubscription.Dispose();
            userSubscription.Dispose();
        }

        private void Initialize()
        {
            DisplayName = "Chat";
            IsConnected = false;
        }

        private void OnChatMessage(IChatMessage msg)
        {
            Execute.OnUIThread(() => { ChatMessages.Add(msg); });
        }

        private void OnChatUser(IChatUser user)
        {
            if (user.LeaveTime.HasValue)
            {
                IChatUser leftUser = ChatUsers.FirstOrDefault(u => u.Name == user.Name);
                if (leftUser != null)
                {
                    Execute.OnUIThread(() => { ChatUsers.Remove(leftUser); });
                }
            }
            else
            {
                Execute.OnUIThread(() => { ChatUsers.Add(user); });
            }
        }

        private async Task JoinChatAsync()
        {
            await _chatClient.Join(UserName).ConfigureAwait(true);
            await _chatClient.GetLatestMessages().ConfigureAwait(true);

            IsConnected = true;
        }

        private async Task LeaveChatAsync()
        {
            await _chatClient.Leave(UserName).ConfigureAwait(true);

            ChatUsers = new ObservableCollection<IChatUser>();
            ChatMessages = new ObservableCollection<IChatMessage>();

            NotifyOfPropertyChange(nameof(ChatUsers));
            NotifyOfPropertyChange(nameof(ChatMessages));

            IsConnected = false;
        }

        private async Task SendCurrentMessageAsync()
        {
            await _chatClient.SendMessage(new ChatMessage { Username = UserName, MessageText = CurrentMessage }).ConfigureAwait(true);

            CurrentMessage = null;
            NotifyOfPropertyChange(nameof(CurrentMessage));
        }

        #endregion Methods
    }
}
