using System;
using System.Threading.Tasks;
using Xunit;

namespace IsAwaitable
{
    public class InternalsTests
    {
        [Theory]
        [InlineData(typeof(Task))]
        [InlineData(typeof(CustomTask))]
        [InlineData(typeof(Task<>))]
        [InlineData(typeof(CustomTask<>))]
        [InlineData(typeof(Task<object>))]
        [InlineData(typeof(CustomTask<object>))]
        [InlineData(typeof(ValueTask))]
        [InlineData(typeof(ValueTask<>))]
        [InlineData(typeof(ValueTask<object>))]
        public void Known_awaitable_types_are_awaitable(Type type)
        {
            Assert.True(IsAwaitableExtension.IsKnownAwaitable(type));
        }
    }
}
