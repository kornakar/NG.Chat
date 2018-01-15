using System;
using System.Collections.Generic;
using System.Text;
using NG.Chat.Model.Interface;

namespace NG.Chat.Interface
{
    public interface IChatMessageProvider<out T> : IObservable<T> where T : IChatMessage
    {

    }
}
