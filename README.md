# IsAwaitable

[![License MIT](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/IsAwaitable)](https://www.nuget.org/packages/IsAwaitable)
[![netstandard2.1](https://img.shields.io/badge/netstandard-2.1-blue)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)

| branch | build | coverage |
|:------:|:-----:|:--------:|
| main   | [![CI](https://img.shields.io/github/workflow/status/tommasobertoni/IsAwaitable/CI/main)](https://github.com/tommasobertoni/IsAwaitable/actions?query=workflow%3ACI+branch%3Amain) | [![Coverage](https://img.shields.io/coveralls/github/tommasobertoni/IsAwaitable/main)](https://coveralls.io/github/tommasobertoni/IsAwaitable?branch=main) |
| dev    | [![CI](https://img.shields.io/github/workflow/status/tommasobertoni/IsAwaitable/CI/dev)](https://github.com/tommasobertoni/IsAwaitable/actions?query=workflow%3ACI+branch%3Adev) | [![Coverage](https://img.shields.io/coveralls/github/tommasobertoni/IsAwaitable/dev)](https://coveralls.io/github/tommasobertoni/IsAwaitable?branch=dev) |

Given an infinite amount of time, everything that can happen will eventually happen... including _**needing to know at runtime if an object or type can be dynamically awaited**_.

The evaluation follows the [c# language specification for awaitable expressions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#awaitable-expressions):

> The task of an await expression is required to be ***awaitable***. An expression `t` is awaitable if one of the following holds:
> *  `t` is of compile time type `dynamic`
> *  `t` has an accessible instance or extension method called `GetAwaiter` with no parameters and no type parameters, and a return type `A` for which all of the following hold:
>    * `A` implements the interface `System.Runtime.CompilerServices.INotifyCompletion` (hereafter known as `INotifyCompletion` for brevity)
>    * `A` has an accessible, readable instance property `IsCompleted` of type `bool`
>    * `A` has an accessible instance method `GetResult` with no parameters and no type parameters

# How to use

```csharp
// The extension is defined within this namespace
// in order to be readily available
using System.Threading.Tasks;

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

## Continuous Integration

[![github-actions](https://img.shields.io/badge/using-GitHub%20Actions-2088FF)](https://github.com/features/actions)
[![xUnit](https://img.shields.io/badge/using-xUnit-indigo)](https://xunit.net/)
[![minicover](https://img.shields.io/badge/using-minicover-indigo)](https://github.com/lucaslorentz/minicover)
[![coveralls](https://img.shields.io/badge/using-coveralls-c05547)](https://coveralls.io/)
