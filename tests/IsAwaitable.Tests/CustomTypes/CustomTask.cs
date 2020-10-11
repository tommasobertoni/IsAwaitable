using System;
using System.Threading.Tasks;

namespace IsAwaitable
{
    public class CustomTask : Task
    {
        public CustomTask(Action action)
            : base(action)
        {
        }
    }

    public class CustomTask<TResult> : Task<TResult>
    {
        public CustomTask(Func<TResult> function)
            : base(function)
        {
            new ValueTask<int>();
        }
    }

    public class CustomGenericTaskWithoutResult<T> : Task
    {
        public CustomGenericTaskWithoutResult(Action action)
            : base(action)
        {
        }
    }
}
