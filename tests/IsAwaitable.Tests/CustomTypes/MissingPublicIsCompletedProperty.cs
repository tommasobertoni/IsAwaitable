using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class MissingPublicIsCompletedProperty : INotifyCompletion
    {
        private bool IsCompleted { get; set; }

        public void GetResult() { }

        public void OnCompleted(Action continuation) { }

        public MissingPublicIsCompletedProperty GetAwaiter() => this;
    }
}
