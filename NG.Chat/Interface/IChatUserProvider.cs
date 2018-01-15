using System;
using System.Collections.Generic;
using System.Text;
using NG.Chat.Model.Interface;

namespace NG.Chat.Interface
{
    public interface IChatUserProvider<out T> where T : IChatUser
    {
        IObservable<T> ActiveUsers { get; }
    }
}
