using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    public class DecoyGetResult : INotifyCompletion
    {
        public bool IsCompleted { get; set; }

        public void GetResult(bool foo) { }

        public void OnCompleted(Action continuation) { }

        public DecoyGetResult GetAwaiter() => this;
    }
}
