using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class MissingGetAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; }

        public void GetResult() { }

        public void OnCompleted(Action continuation) { }
    }
}
