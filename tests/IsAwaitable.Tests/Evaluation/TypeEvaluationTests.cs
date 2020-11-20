using System;
using System.Threading.Tasks;
using IsAwaitable.Analysis;
using Xunit;

namespace IsAwaitable
{
    public class TypeEvaluationTests
    {
        private readonly TypeEvaluation _awaitableEvaluation;
        private readonly TypeEvaluation _awaitableWithResultEvaluation;

        public TypeEvaluationTests()
        {
            _awaitableEvaluation = TypeEvaluation.From(Awaitable.Describe<Task>());
            _awaitableWithResultEvaluation = TypeEvaluation.From(Awaitable.Describe<Task<int>>());
        }

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
            Assert.True(_awaitableEvaluation.IsAwaitable);
            Assert.False(_awaitableEvaluation.IsAwaitableWithResult);
        }

        [Fact]
        public void Known_awaitable_matches_description()
        {
            Assert.True(_awaitableEvaluation.IsKnownAwaitable);
        }

        [Fact]
        public void Awaitable_with_result_matches_description()
        {
            Assert.True(_awaitableWithResultEvaluation.IsAwaitableWithResult);
        }

        [Fact]
        public void Known_awaitable_with_result_matches_description()
        {
            Assert.False(_awaitableEvaluation.IsKnownAwaitableWithResult);
            Assert.True(_awaitableWithResultEvaluation.IsKnownAwaitableWithResult);
        }

        [Fact]
        public void Awaitable_with_result_has_a_result()
        {
            Assert.True(_awaitableWithResultEvaluation.IsAwaitableWithResult);
            Assert.NotNull(_awaitableWithResultEvaluation.ResultType);
            Assert.Equal(typeof(int), _awaitableWithResultEvaluation.ResultType);
        }

        [Fact]
        public void Awaitable_without_result_does_not_have_a_result()
        {
            Assert.Throws<InvalidOperationException>(() =>
                TypeEvaluation.NotAwaitable.ResultType);

            Assert.Throws<InvalidOperationException>(() =>
                _awaitableEvaluation.ResultType);
        }
    }
}
