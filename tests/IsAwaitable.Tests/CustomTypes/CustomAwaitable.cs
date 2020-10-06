using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IsAwaitable
{
    class CustomAwaitable
    {
        public CustomAwaiter GetAwaiter() =>
            new CustomAwaiter();
    }

    class CustomAwaiter : INotifyCompletion
    {
        private readonly DateTime _start;
        private TimeSpan _result;

        public CustomAwaiter()
        {
            _start = DateTime.UtcNow;
            IsCompleted = false;
        }

        public bool IsCompleted { get; private set; }

        public void OnCompleted(Action continuation)
        {
            ThreadPool.QueueUserWorkItem(_ => Compute());

            // Local functions.

            void Compute()
            {
                // Bad CPU-bound work.
                var x = 42d;
                for (int i = 0; i < 10_000; i++)
                    x *= Math.PI % x;

                _result = DateTime.UtcNow - _start;
                IsCompleted = true;
                continuation();
            }
        }

        public TimeSpan GetResult() => _result;
    }
}
