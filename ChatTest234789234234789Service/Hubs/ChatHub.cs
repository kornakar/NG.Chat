using System;
using System.Collections;
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

            //_chatStorage.AddOrUpdateUser(userName);

            //AddUserToStorage(userName, clientId);

            //var users = GetAllUsersFromStorage();
            //foreach (ChatUser user in users)
            //{
            //    Clients.Caller.userJoined(user);
            //}

            return base.OnConnected();
        }

        /// <summary>
        /// Provides the handler for SignalR OnDisconnected event
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            string userName = connectionId;

            //_chatStorage.DeleteUser(userName);

            RemoveUserFromStorage(userName);

            return base.OnDisconnected(stopCalled);
        }

        public void Send(ChatMessage message)
        {
            message.SendTime = DateTime.Now;

            SaveMessageToStorage(message);

            Clients.All.broadcastMessage(message);
        }

        public void UserJoined(string username)
        {
            string connectionId = Context.ConnectionId;

            // TODO: how to get client id?
            var users = GetAllUsersFromStorage();
            foreach (ChatUser user in users)
            {
                Clients.Caller.userJoined(user);
            }

            ChatUser newUser = AddUserToStorage(username, connectionId);

            Clients.All.userJoined(newUser);
        }

        public void UserLeft(string username)
        {
            ChatUser user = RemoveUserFromStorage(username);

            Clients.All.userLeft(user);
        }

        public void GetActiveUsers()
        {
            IList<ChatUser> users = GetAllUsersFromStorage();
            Clients.Caller.activeUsers(users);
        }

        public void GetLatestMessages()
        {
            IList<ChatMessage> msgs = GetAllMessagesFromStorage();
            Clients.Caller.messages(msgs);
        }

        // TODO: move to service?
        #region Db handling

        private ChatUser AddUserToStorage(string userName, string clientId)
        {
            ChatUser user = null;

            using (var db = new ChatContext())
            {
                user = db.ChatUsers.SingleOrDefault(u => u.Name == userName);

                if (user == null)
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

            return user;
        }

        private ChatUser RemoveUserFromStorage(string userName)
        {
            ChatUser currentUser = null;

            using (var db = new ChatContext())
            {
                currentUser = db.ChatUsers.SingleOrDefault(u => u.Name == userName);
                
                if (currentUser != null)
                {
                    currentUser.LeaveTime = DateTime.Now;
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

            return currentUser;
        }

        private void SaveMessageToStorage(ChatMessage message)
        {
            using (var db = new ChatContext())
            {
                db.Entry(message).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        private IList<ChatUser> GetAllUsersFromStorage()
        {
            using (var db = new ChatContext())
            {
                var users = db.ChatUsers;
                return users.ToList();
            }
        }

        private IList<ChatMessage> GetAllMessagesFromStorage()
        {
            using (var db = new ChatContext())
            {
                var msgs = db.ChatMessages;
                return msgs.ToList();
            }
        }

        #endregion  Db handling
    }
}