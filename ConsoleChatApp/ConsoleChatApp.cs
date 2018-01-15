using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Loader;
using NG.Chat.Interface;
using NG.Chat.Model.Interface;

namespace ConsoleChatApp
{
    class ConsoleChatApp
    {
        private CompositionContainer _container;

        [Import]
        private IChatClient _chatClient { get; set; }

        static void Main(string[] args)
        {
            ConsoleChatApp app = new ConsoleChatApp();
            app.Run();
        }

        public void Run()
        {
            Compose();

            IDisposable subscription = _chatClient.Subscribe<IChatMessage>(
                m => Console.WriteLine("[{0}] {1}: {2}", m.SendTime, m.Username, m.MessageText),
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnCompleted"));

            string line;
            do
            {
                line = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    _chatClient.SendMessage(new NG.Chat.Model.ChatMessage { MessageText = line, Username = "consoletest" });
                }

            } while (!string.IsNullOrWhiteSpace(line));

            subscription.Dispose();
        }

        private void Compose()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ConsoleChatApp).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(@"./"));

            //Create the CompositionContainer with the parts in the catalog  
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object  
            try
            {
                _container.SatisfyImportsOnce(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}
