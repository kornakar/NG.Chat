using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatTest234789234234789Service.Models;
using Microsoft.AspNet.SignalR;
using NG.Chat.Interface;
using NG.Chat.Model;
using NG.Chat.Model.Interface;

namespace ChatTest234789234234789Service.Hubs
{
    /// <summary>
    /// The Chat Hub
    /// </summary>
    /// <remarks>
    /// https://code.msdn.microsoft.com/Broadcast-Real-time-SQL-69dd9fcc
    /// </remarks>
    //[Authorize]
    public class ChatHub : Hub<IChat>, IChatHub
    {
        private static readonly ConcurrentDictionary<string, ChatUser> Users = new ConcurrentDictionary<string, ChatUser>();
        private readonly object _syncLock = new object();

        public ChatHub()
        {
        }

        /// <summary>
        /// Provides the handler for SignalR OnConnected event
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
            string clientId = connectionId;
            string userName = connectionId;

            // TODO: how to get user's name <-> client id?
            var user = Users.GetOrAdd(clientId, new ChatUser
                                                     {
                                                         Name = userName,
                                                         JoinTime = DateTime.Now,
                                                         ClientId = clientId,
                                                         ConnectionIds = new HashSet<string>()
                                                     });
            lock (_syncLock)
            {
                user.ConnectionIds.Add(connectionId);
            }

            using (var db = new ChatContext())
            {
                var currentUser = db.ChatUsers.SingleOrDefault(u => u.Name == userName);

                if (currentUser == null)
                {
                    user = new ChatUser
                           {
                               Name = userName,
                               JoinTime = DateTime.Now,
                               ClientId = clientId,
                    };
                    db.ChatUsers.Add(user);
                }

                //user.Connections.Add(new Connection
                //                     {
                //                         ConnectionID = Context.ConnectionId,
                //                         UserAgent = Context.Request.Headers["User-Agent"],
                //                         Connected = true
                //                     });
                db.SaveChanges();
            }

            return base.OnConnected();
        }
        
        /// <summary>
        /// Provides the handler for SignalR OnDisconnected event
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            string clientId = connectionId;
            string userName = connectionId;

            ChatUser user;
            Users.TryGetValue(clientId, out user);

            if (user != null)
            {
                lock (_syncLock)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        Users.TryRemove(clientId, out user);
                    }
                }

                using (var db = new ChatContext())
                {
                    var currentUser = db.ChatUsers.SingleOrDefault(u => u.Name == userName);

                    if (currentUser != null)
                    {
                        db.Entry(currentUser).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    }

                    //user.Connections.Add(new Connection
                    //                     {
                    //                         ConnectionID = Context.ConnectionId,
                    //                         UserAgent = Context.Request.Headers["User-Agent"],
                    //                         Connected = true
                    //                     });
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Provide the ability to get currently connected user
        /// </summary>
        public IEnumerable<string> GetConnectedUser()
        {
            return Users.Where(x =>
                               {
                                   lock (_syncLock)
                                   {
                                       return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
                                   }
                               }).Select(x => x.Key);
        }

        public void Send(ChatMessage message)
        {
            message.SendTime = DateTime.Now;

            using (var db = new ChatContext())
            {
                db.Entry(message).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }

            Clients.All.broadcastMessage(message);
        }

        public void GetActiveUsers()
        {
            using (var db = new ChatContext())
            {
                var users = db.ChatUsers;
                Clients.Caller.activeUsers(users.ToList());
            }
        }

        public void GetLatestMessages()
        {
            using (var db = new ChatContext())
            {
                var msgs = db.ChatMessages;
                Clients.Caller.messages(msgs.ToList());
            }
        }
    }
}