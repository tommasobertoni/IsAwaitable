using System;
using System.Threading.Tasks;

namespace IsAwaitable
{
    public class CustomTask : Task
    {
        public CustomTask(Action action)
            : base(action)
        {
            Start();
        }
    }

    public class CustomTask<TResult> : Task<TResult>
    {
        public CustomTask(Func<TResult> function)
            : base(function)
        {
            Start();
        }
    }

    public class CustomGenericTaskWithoutResult<T> : Task
    {
        public CustomGenericTaskWithoutResult(Action action)
            : base(action)
        {
            Start();
        }
    }
}
