using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    public class DoubleGetAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; set; }

        public void GetResult() { }

        public void OnCompleted(Action continuation) { }

        public DoubleGetAwaiter GetAwaiter() => this;

        public DoubleGetAwaiter GetAwaiter(bool foo) => this;
    }
}
