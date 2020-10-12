using System;
using System.Threading.Tasks;
using Xunit;

namespace IsAwaitable
{
    public class IsKnownAwaitableExtensionTests
    {
        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(IsKnownAwaitableExtensionTests))]
        public void Control_types_are_not_known_awaitables(Type type)
        {
            Assert.False(type.IsKnownAwaitable());
            Assert.False(type.IsKnownAwaitableWithResult());
        }

        [Theory]
        [InlineData(typeof(CustomAwaitable))]
        [InlineData(typeof(CustomDelay))]
        [InlineData(typeof(CustomAwaitableViaExtension))]
        public void Not_all_awaitable_types_are_known_awaitables(Type type)
        {
            Assert.False(type.IsKnownAwaitable());
            Assert.False(type.IsKnownAwaitableWithResult());
        }

        [Theory]
        [InlineData(typeof(Task), false)]
        [InlineData(typeof(CustomTask), false)]
        [InlineData(typeof(Task<>), true)]
        [InlineData(typeof(CustomTask<>), true)]
        [InlineData(typeof(Task<object>), true)]
        [InlineData(typeof(CustomTask<object>), true)]
        [InlineData(typeof(ValueTask), false)]
        [InlineData(typeof(ValueTask<>), true)]
        [InlineData(typeof(ValueTask<object>), true)]
        public void Known_awaitable_types_are_awaitable(Type type, bool shouldBeWithResult)
        {
            Assert.True(type.IsKnownAwaitable());

            if (shouldBeWithResult)
                Assert.True(type.IsKnownAwaitableWithResult());
        }

        [Theory]
        [InlineData(typeof(CustomGenericTaskWithoutResult<>))]
        [InlineData(typeof(CustomGenericTaskWithoutResult<object>))]
        public void Non_task_related_generics_are_not_confused(Type type)
        {
            Assert.True(type.IsKnownAwaitable());
            Assert.False(type.IsKnownAwaitableWithResult());
        }
    }
}
