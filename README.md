# IsAwaitable

[![License MIT](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/IsAwaitable)](https://www.nuget.org/packages/IsAwaitable)
[![CI](https://img.shields.io/github/workflow/status/tommasobertoni/IsAwaitable/CI)](https://github.com/tommasobertoni/IsAwaitable/actions?query=workflow%3ACI)
[![Coverage Status](https://coveralls.io/repos/github/tommasobertoni/IsAwaitable/badge.svg?branch=main)](https://coveralls.io/github/tommasobertoni/IsAwaitable?branch=main)

Given an infinite amount of time, everything that can happen will eventually happen... including _**needing to know at runtime if an object or type can be dynamically awaited.**_

<br/>

This library can help you with that.<br/>
The algorithm follows the [c# language specification for awaitable expressions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#awaitable-expressions):

> The task of an await expression is required to be ***awaitable***. An expression `t` is awaitable if one of the following holds:
> *  `t` is of compile time type `dynamic`
> *  `t` has an accessible instance or extension method called `GetAwaiter` with no parameters and no type parameters, and a return type `A` for which all of the following hold:
>    * `A` implements the interface `System.Runtime.CompilerServices.INotifyCompletion` (hereafter known as `INotifyCompletion` for brevity)
>    * `A` has an accessible, readable instance property `IsCompleted` of type `bool`
>    * `A` has an accessible instance method `GetResult` with no parameters and no type parameters

<br/>

# How to use

```csharp
// On instances
var promise = GetSomethingAsync();
_ = promise.IsAwaitable(); // true

// On types
_ = typeof(Task).IsAwaitable(); // true

// On value tasks
_ = typeof(ValueTask).IsAwaitable(); // true
_ = typeof(ValueTask<>).IsAwaitable(); // true

// On custom awaitables!
class CustomDelay
{
    private readonly TimeSpan _delay;

    public CustomDelay(TimeSpan delay) =>
        _delay = delay;

    public TaskAwaiter GetAwaiter() =>
        Task.Delay(_delay).GetAwaiter();
}

var delay = new CustomDelay(TimeSpan.FromSeconds(2));
_ = delay.IsAwaitable(); // true
await delay;
```

## Use with dynamic await
```csharp
async Task<object> AwaitResultOrReturn(object instance)
{
    return instance.IsAwaitable()
        ? await (dynamic)instance // magic
        : instance;
}

var foo = GetFoo();
var fooTask = Task.FromResult(foo);

var result1 = await AwaitResultOrReturn(foo);
var result2 = await AwaitResultOrReturn(fooTask);

// foo == result1 == result2
```
