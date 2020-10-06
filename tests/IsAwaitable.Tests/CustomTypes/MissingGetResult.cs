using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class MissingGetResult : INotifyCompletion
    {
        public bool IsCompleted { get; }

        public void OnCompleted(Action continuation) { }

        public MissingGetResult GetAwaiter() => this;
    }
}
