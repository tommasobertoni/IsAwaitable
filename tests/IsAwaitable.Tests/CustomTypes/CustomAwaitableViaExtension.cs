using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class CustomAwaitableViaExtension
    {
    }

    static class AwaitableExtensions
    {
        public static ExtensionAwaiter GetAwaiter(this CustomAwaitableViaExtension _) =>
            new ExtensionAwaiter();
    }

    class ExtensionAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; private set; }

        public void OnCompleted(Action continuation)
        {
            IsCompleted = true;
            continuation();
        }

        public void GetResult() { }
    }
}
