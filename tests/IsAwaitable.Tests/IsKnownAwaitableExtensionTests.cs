using System;
using System.Collections.Generic;
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

        [Fact]
        public void Error_if_null_type()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsKnownAwaitable());
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsKnownAwaitableWithResult());
        }

        [Fact]
        public void Does_not_throw_on_null_instance()
        {
            Assert.False((null as object).IsKnownAwaitable());
            Assert.False((null as object).IsKnownAwaitableWithResult());
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
        [MemberData(nameof(KnownAwaitableTypes))]
        public async Task Known_awaitable_instances_are_awaitable(object instance, bool shouldBeWithResult)
        {
            Assert.True(instance.IsKnownAwaitable());

            if (shouldBeWithResult)
            {
                Assert.True(instance.IsKnownAwaitableWithResult());
                _ = await (dynamic)instance;
            }
            else
            {
                await (dynamic)instance;
            }
        }

        [Theory]
        [InlineData(typeof(CustomGenericTaskWithoutResult<>))]
        [InlineData(typeof(CustomGenericTaskWithoutResult<object>))]
        public void Non_task_related_generics_are_not_confused(Type type)
        {
            Assert.True(type.IsKnownAwaitable());
            Assert.False(type.IsKnownAwaitableWithResult());
        }

        public static IEnumerable<object[]> KnownAwaitableTypes() =>
            new object[][]
            {
                new object[] { Task.CompletedTask, false },
                new object[] { new CustomTask(() => { }), false },
                new object[] { Task.FromResult(42), true },
                new object[] { new CustomTask<int>(() => 42), true },
                new object[] { new ValueTask(), false },
                new object[] { new ValueTask<int>(42), true },
            };
    }
}
