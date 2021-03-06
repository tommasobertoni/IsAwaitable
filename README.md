# <img src="assets/logo/logo-128.png" height="32px" /> IsAwaitable

[![License MIT](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/IsAwaitable)](https://www.nuget.org/packages/IsAwaitable)
[![netstandard2.0](https://img.shields.io/badge/netstandard-2.0-blue)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)

| branch | build | coverage | quality |
|:------:|:-----:|:--------:|:-------:|
| main   | [![CI](https://img.shields.io/github/workflow/status/tommasobertoni/IsAwaitable/CI/main)](https://github.com/tommasobertoni/IsAwaitable/actions?query=workflow%3ACI+branch%3Amain) | [![Coverage](https://img.shields.io/coveralls/github/tommasobertoni/IsAwaitable/main)](https://coveralls.io/github/tommasobertoni/IsAwaitable?branch=main) | [![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/tommasobertoni/IsAwaitable/main)](https://www.codefactor.io/repository/github/tommasobertoni/isawaitable/overview/main) |
| dev    | [![CI](https://img.shields.io/github/workflow/status/tommasobertoni/IsAwaitable/CI/dev)](https://github.com/tommasobertoni/IsAwaitable/actions?query=workflow%3ACI+branch%3Adev) | [![Coverage](https://img.shields.io/coveralls/github/tommasobertoni/IsAwaitable/dev)](https://coveralls.io/github/tommasobertoni/IsAwaitable?branch=dev) | [![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/tommasobertoni/IsAwaitable/dev)](https://www.codefactor.io/repository/github/tommasobertoni/isawaitable/overview/dev) |

Given an infinite amount of time, everything that can happen will eventually happen... including _**needing to know at runtime if an object or type can be awaited**_.

[TL;DR](#Usage)

The library provides the following extension methods:

```csharp
using System.Threading.Tasks;

// Checks if it's awaitable
bool IsAwaitable(this object? instance);
bool IsAwaitable(this Type type);
// await x;

// Check if it's awaitable and returns a result
bool IsAwaitableWithResult(this object? instance);
bool IsAwaitableWithResult(this object? instance, out Type? resultType);
bool IsAwaitableWithResult(this Type type);
bool IsAwaitableWithResult(this Type type, out Type? resultType);
// var foo = await x;
```
...and some bonus ones:
```csharp
using IsAwaitable;

// Known awaitables: Task, Task<T>, ValueTask, ValueTask<T>
bool IsKnownAwaitable(this object? instance);
bool IsKnownAwaitable(this Type type);

// Is Task<T> or ValueTask<T>
bool IsKnownAwaitableWithResult(this object? instance);
bool IsKnownAwaitableWithResult(this object? instance, out Type? resultType);
bool IsKnownAwaitableWithResult(this Type type);
bool IsKnownAwaitableWithResult(this Type type, out Type? resultType);
```
If you want to see _how_ a type, or instance, is compliant with an awaitable expression, you can use the `Awaitable` type:
```csharp
using IsAwaitable.Analysis;

_ = Awaitable.Describe("hello");
// null

var description = Awaitable.Describe(typeof(MyCustomAwaitableType));
if (description is not null)
{
    var resultType = description.ResultType;
}

var taskDescription = Awaitable.Describe<Task>();
var isKnownAwaitable = taskDescripti.IsKnownAwaitable;
```
The `Describe` function inspects the type to check if it matches the [c# language specification for awaitable expressions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#awaitable-expressions):

> An expression `t` is awaitable if one of the following holds:
> * `t` is of compile time type `dynamic`
> *  `t` has an accessible instance or extension method called `GetAwaiter` with no parameters and no type parameters, and a return type `A` for which all of the following hold:
>    * `A` implements the interface `INotifyCompletion`
>    * `A` has an accessible, readable instance property `IsCompleted` of type `bool`
>    * `A` has an accessible instance method `GetResult` with no parameters and no type parameters

## Usage

```csharp
// On instances
Task doAsync = DoSomethingAsync();
_ = doAsync.IsAwaitable(); // true

// Returing a result
Task<int> promise = GetSomethingAsync();
_ = promise.IsAwaitable(); // true
_ = promise.IsAwaitableWithResult(); // true

// On types
_ = typeof(Task).IsAwaitable(); // true

// On value tasks
_ = typeof(ValueTask).IsAwaitable(); // true
_ = typeof(ValueTask<>).IsAwaitableWithResult(); // true

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
_ = delay.IsAwaitableWithResult(); // false
```

## Dynamically await anything
```csharp
async Task<object> AwaitResultOrReturn(object instance)
{
    return instance.IsAwaitableWithResult()
        ? await (dynamic)instance
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
[![xUnit](https://img.shields.io/badge/using-xUnit-512bd4)](https://xunit.net/)
[![coverlet](https://img.shields.io/badge/using-coverlet-512bd4)](https://github.com/coverlet-coverage/coverlet)
[![coveralls.io](https://img.shields.io/badge/using-coveralls.io-c05547)](https://coveralls.io/)
[![codefactor.io](https://img.shields.io/badge/using-codefactor.io-35bd97)](https://www.codefactor.io/)

## Icon

Created by [The Icon Z](https://thenounproject.com/theiconz/) from [The Noun Project](https://thenounproject.com/).
