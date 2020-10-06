using System;

namespace IsAwaitable
{
    class MissingINotifyCompletion
    {
        public MissingINotifyCompletion GetAwaiter() => this;

        public bool IsCompleted { get; }

        public void OnCompleted(Action continuation) { }

        public void GetResult() { }
    }
}
