using System;
using Xunit;

namespace IsAwaitable
{
    public class TypeEvaluationTests
    {
        [Fact]
        public void Non_awaitable_matches_description()
        {
            Assert.NotNull(TypeEvaluation.NotAwaitable);
            Assert.False(TypeEvaluation.NotAwaitable.IsAwaitable);
            Assert.False(TypeEvaluation.NotAwaitable.IsAwaitableWithResult);
        }

        [Fact]
        public void Awaitable_matches_description()
        {
            Assert.NotNull(TypeEvaluation.Awaitable);
            Assert.True(TypeEvaluation.Awaitable.IsAwaitable);
            Assert.False(TypeEvaluation.Awaitable.IsAwaitableWithResult);
        }

        [Fact]
        public void Awaitable_with_result_matches_description()
        {
            var eval = TypeEvaluation.AwaitableWithResult(typeof(int));
            Assert.NotNull(eval);
            Assert.True(eval.IsAwaitable);
            Assert.True(eval.IsAwaitableWithResult);
        }

        [Fact]
        public void Awaitable_with_result_has_a_result()
        {
            var eval = TypeEvaluation.AwaitableWithResult(typeof(int));
            Assert.True(eval.IsAwaitableWithResult);
            Assert.NotNull(eval.ResultType);
            Assert.Equal(typeof(int), eval.ResultType);
        }

        [Fact]
        public void Error_if_result_type_is_void()
        {
            Assert.Throws<ArgumentException>(() =>
                TypeEvaluation.AwaitableWithResult(typeof(void)));
        }

        [Fact]
        public void Awaitable_without_result_does_not_have_a_result()
        {
            Assert.Throws<InvalidOperationException>(() =>
                TypeEvaluation.NotAwaitable.ResultType);

            Assert.Throws<InvalidOperationException>(() =>
                TypeEvaluation.Awaitable.ResultType);
        }
    }
}
