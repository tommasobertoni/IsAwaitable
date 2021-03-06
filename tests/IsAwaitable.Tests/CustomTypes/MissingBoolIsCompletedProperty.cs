﻿using System;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    class MissingBoolIsCompletedProperty : INotifyCompletion
    {
        public string IsCompleted { get; set; }

        public void GetResult() { }

        public void OnCompleted(Action continuation) { }

        public MissingBoolIsCompletedProperty GetAwaiter() => this;
    }
}
