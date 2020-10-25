using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class NonReadableIsCompletedProperty : INotifyCompletion
    {
        public string IsCompleted { set { } }

        public void GetResult() { }

        public void OnCompleted(Action continuation) { }

        public NonReadableIsCompletedProperty GetAwaiter() => this;
    }
}
