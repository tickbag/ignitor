# Ignitor
Ignite your Blazor project. Opinionated Blazor State management framework developed for the Tickbag project.

This project is in active development flux and will be changing a regular basis over the coming weeks.
It's very much a work in progress right now. Use at your own risk!

# Current situation
Initial untested implementations around:
- State management interface using `IState<TId, TEntity>` to inject state access where-ever it's needed.
- Immutable data type wrapper class around all data retrieved from the state.
- You can use `.Unwrap()` to retrieve the data object
  - All unwrapped data is isolated and does not effect the state, even reference types.
- Includes a highly efficient and specialised deep cloner to achieve data isolation
  - This cloner uses compiled expression trees without recursion to achieve maximum performance (no refkection)
  - In a comparisons with an off-the-shelf cloner library, the Ignitor cloner was around 43x faster.
  - Ancedotal times: 1 complex type, containing 100,000 complex reference types and 100,000 value types, was cloned in 7ms.


# Still to come
- Process flow using a type of mediator pattern
  - Mediatr library is unsuitable for Blazor Webassembly apps, so a custom one will need to be created
- Persistant stores to Local Storage in the browser
  - This will be made available via an `IPersistantState<TId, TEntity>` injectable.
- General improvements to the library based on actually using it in a project
  - Already a lot of changes have occurred around the `IImmutable` interface
- XML comments to stop the build warnings
- Tests really need writing to impart confidence in the library
- Documentation
- Pubishing to NuGet

# How to use
Clone the repo, reference the project from your Blazor project.

Add :
```c#
services.AddIgnitor()
```
in `Program.cs` in your project.

Inject the state into each component that needs it, e.g.

```c#
@inject IState<Guid, Todo> TodoState
```

Now you can read the state like so:
```c#
var todos = await TodoState.GetState((ct) => GetDefaultState(ct), cancellationSource.Token);
```

You updtae the state like this:
```c#
await ToDoState.GetUpdater(todoId).Update(new Todo());
```
