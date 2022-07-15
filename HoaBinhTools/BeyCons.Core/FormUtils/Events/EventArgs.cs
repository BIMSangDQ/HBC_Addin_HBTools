#region Using
using System;
#endregion

namespace BeyCons.Core.FormUtils.Events
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }
        public T Value { get; private set; }
    }
}