using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class MissingIsCompletedProperty : INotifyCompletion
    {
        public void GetResult() { }

        public void OnCompleted(Action continuation) { }

        public MissingIsCompletedProperty GetAwaiter() => this;
    }
}
