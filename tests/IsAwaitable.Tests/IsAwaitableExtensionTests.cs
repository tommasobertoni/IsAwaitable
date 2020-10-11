using System;
using System.Threading.Tasks;
using Xunit;

namespace IsAwaitable.Tests
{
    public class IsAwaitableExtensionTests : IDisposable
    {
        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(IsAwaitableExtensionTests))]
        public void Control_types_are_not_awaitable(Type type)
        {
            Assert.False(type.IsAwaitable());
        }

        [Fact]
        public void Task_type_is_awaitable()
        {
            Assert.True(typeof(Task).IsAwaitable());
        }

        [Fact]
        public async Task Task_instance_is_awaitable()
        {
            var instance = Task.CompletedTask;
            Assert.True(instance.IsAwaitable());
            await instance;
        }

        [Fact]
        public void Task_type_with_result_is_awaitable()
        {
            Assert.True(typeof(Task<>).IsAwaitable());
        }

        [Fact]
        public async Task Task_instance_with_result_is_awaitable()
        {
            var instance = Task.FromResult(true);
            Assert.True(instance.IsAwaitable());
            await instance;
        }

        [Fact]
        public void ValueTask_type_is_awaitable()
        {
            Assert.True(typeof(ValueTask).IsAwaitable());
        }

        [Fact]
        public async Task ValueTask_instance_is_awaitable()
        {
            var instance = new ValueTask();
            Assert.True(instance.IsAwaitable());
            await instance;
        }

        [Fact]
        public void ValueTask_type_with_result_is_awaitable()
        {
            Assert.True(typeof(ValueTask<>).IsAwaitable());
        }

        [Fact]
        public async Task ValueTask_instance_with_result_is_awaitable()
        {
            var instance = new ValueTask<bool>(true);
            Assert.True(instance.IsAwaitable());
            await instance;
        }

        [Fact]
        public void CustomAwaitable_type_is_awaitable()
        {
            Assert.True(typeof(CustomAwaitable).IsAwaitable());
        }

        [Fact]
        public async Task CustomAwaitable_instance_is_awaitable()
        {
            var instance = new CustomAwaitable();
            Assert.True(instance.IsAwaitable());
            await instance;
        }

        [Fact]
        public void Error_if_null_type()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsAwaitable());
        }

        [Fact]
        public void Does_not_throw_on_null_instance()
        {
            Assert.False((null as object).IsAwaitable());
        }

        [Fact]
        public void CustomAwaitableViaExtension_type_is_awaitable()
        {
            Assert.True(typeof(CustomAwaitableViaExtension).IsAwaitable());
        }

        [Fact]
        public async Task CustomAwaitableViaExtension_instance_is_awaitable()
        {
            var instance = new CustomAwaitableViaExtension();
            Assert.True(instance.IsAwaitable());
            await instance;
        }

        [Fact]
        public void Awaitable_must_have_a_GetAwaiter_method()
        {
            Assert.False(typeof(MissingGetAwaiter).IsAwaitable());

            // await new MissingGetAwaiter();
            // Error CS1061 'MissingGetAwaiter' does not contain a definition for
            // 'GetAwaiter' and no accessible extension method 'GetAwaiter'
            // accepting a first argument of type 'MissingGetAwaiter' could be found
            // (are you missing a using directive or an assembly reference ?)
        }

        [Fact]
        public void Awaiter_must_implement_INotifyCompletion()
        {
            Assert.False(typeof(MissingINotifyCompletion).IsAwaitable());

            // await new MissingINotifyCompletion();
            // Error CS4027 'MissingINotifyCompletion' does not implement 'INotifyCompletion'
        }

        [Fact]
        public void Awaiter_must_have_IsCompleted_property()
        {
            Assert.False(typeof(MissingIsCompletedProperty).IsAwaitable());

            // await new MissingIsCompletedProperty();
            // Error CS0117 'MissingIsCompletedProperty' does not contain
            // a definition for 'IsCompleted'
        }

        [Fact]
        public void Awaiter_must_have_public_IsCompleted_property()
        {
            Assert.False(typeof(MissingPublicIsCompletedProperty).IsAwaitable());

            // await new MissingPublicIsCompletedProperty();
            // Error CS0117 'MissingIsCompletedProperty' does not contain
            // a definition for 'IsCompleted'
        }

        [Fact]
        public void Awaiter_must_have_bool_IsCompleted_property()
        {
            Assert.False(typeof(MissingBoolIsCompletedProperty).IsAwaitable());

            // await new MissingBoolIsCompletedProperty();
            // Error CS0117 'MissingIsCompletedProperty' does not contain
            // a definition for 'IsCompleted'
        }

        [Fact]
        public void Awaiter_must_have_GetResult_method()
        {
            Assert.False(typeof(MissingGetResult).IsAwaitable());

            // await new MissingGetResult();
            // Error CS0117  'MissingGetResult' does not contain a definition for 'GetResult'
        }

        public void Dispose()
        {
            EvaluationCache.Clear();
        }
    }
}
