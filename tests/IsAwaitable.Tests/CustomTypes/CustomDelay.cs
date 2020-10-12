using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IsAwaitable
{
    public class CustomDelay
    {
        private readonly TimeSpan _delay;

        public CustomDelay(TimeSpan delay) =>
            _delay = delay;

        public TaskAwaiter GetAwaiter() =>
            Task.Delay(_delay).GetAwaiter();
    }
}
