using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    public class DecoyGetAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; set; }

        public void GetResult() { }

        public void OnCompleted(Action continuation) { }

        public DecoyGetAwaiter GetAwaiter(bool foo) => this;
    }
}
