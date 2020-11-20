using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace IsAwaitable.Analysis
{
    public class AwaitableTests
    {
        [Fact]
        public void Error_if_instance_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Awaitable.Describe(null as object));
        }

        [Fact]
        public void Error_if_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Awaitable.Describe(null as Type));
        }

        [Fact]
        public void Awaitable_is_described_correctly()
        {
            var description = Awaitable.Describe<Task<int>>();

            Assert.NotNull(description);
            Assert.Equal(typeof(Task<int>), description.Type);
            Assert.Equal(typeof(int), description.ResultType);
            Assert.NotNull(description.GetAwaiterMethod);
            Assert.NotNull(description.AwaiterDescriptor);
            Assert.Equal(typeof(TaskAwaiter<int>), description.AwaiterDescriptor.Type);
            Assert.NotNull(description.AwaiterDescriptor.IsCompletedProperty);
            Assert.NotNull(description.AwaiterDescriptor.GetResultMethod);
            Assert.Equal(description.ResultType, description.AwaiterDescriptor.GetResultMethod.ReturnType);
        }

        [Fact]
        public void Non_awaitable_is_not_described()
        {
            Assert.Null(Awaitable.Describe<object>());
            Assert.Null(Awaitable.Describe<bool>());
            Assert.Null(Awaitable.Describe<int>());
            Assert.Null(Awaitable.Describe<string>());
            Assert.Null(Awaitable.Describe<Guid>());
        }

        [Fact]
        public void Known_awaitables_are_identified()
        {
            Assert.False(Awaitable.Describe<CustomAwaitable>().IsKnownAwaitable);

            Assert.True(Awaitable.Describe<Task>().IsKnownAwaitable);
            Assert.True(Awaitable.Describe<Task<int>>().IsKnownAwaitable);
            Assert.True(Awaitable.Describe<ValueTask>().IsKnownAwaitable);
            Assert.True(Awaitable.Describe<ValueTask<int>>().IsKnownAwaitable);
        }

        [Fact]
        public void Void_returning_awaitables_are_described()
        {
            var taskDescription = Awaitable.Describe<Task>();
            Assert.Equal(typeof(void), taskDescription.ResultType);

            var valueTaskDescription = Awaitable.Describe<ValueTask>();
            Assert.Equal(typeof(void), valueTaskDescription.ResultType);
        }

        [Fact]
        public void All_overloads_work()
        {
            var d1 = Awaitable.Describe<Task<int>>();
            var d2 = Awaitable.Describe(typeof(Task<int>));
            var d3 = Awaitable.Describe(Task.FromResult(42));

            Test(d1);
            Test(d2);
            Test(d3);

            // Local functions.

            static void Test(AwaitableDescription description)
            {
                Assert.NotNull(description);
                Assert.Equal(typeof(Task<int>), description.Type);
                Assert.Equal(typeof(int), description.ResultType);
                Assert.NotNull(description.GetAwaiterMethod);
                Assert.NotNull(description.AwaiterDescriptor);
                Assert.Equal(typeof(TaskAwaiter<int>), description.AwaiterDescriptor.Type);
                Assert.NotNull(description.AwaiterDescriptor.IsCompletedProperty);
                Assert.NotNull(description.AwaiterDescriptor.GetResultMethod);
                Assert.Equal(description.ResultType, description.AwaiterDescriptor.GetResultMethod.ReturnType);
            }
        }
    }
}
