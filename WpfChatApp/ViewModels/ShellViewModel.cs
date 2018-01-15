using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using WpfChatApp.Interface;

namespace WpfChatApp.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IChatScreen>.Collection.OneActive, IShell
    {
        [ImportMany(typeof(IChatScreen))]
        private IEnumerable<IChatScreen> _chatScreens;

        public IEnumerable<IChatScreen> ChatScreens
        {
            get { return _chatScreens.OrderBy(x => x.TabOrder); }
        }

        public ShellViewModel()
        {
            _chatScreens = new BindableCollection<IChatScreen>();
            DisplayName = "NextGames Chat";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }
    }
}
