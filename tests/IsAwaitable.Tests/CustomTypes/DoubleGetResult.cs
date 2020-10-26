using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    public class DoubleGetResult : INotifyCompletion
    {
        public bool IsCompleted { get; set; }

        public void GetResult() { }

        public void GetResult(bool foo) { }

        public void OnCompleted(Action continuation) { }

        public DoubleGetResult GetAwaiter() => this;
    }
}
