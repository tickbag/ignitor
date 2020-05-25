# Ignitor
Ignite your Blazor project. Opinionated Blazor State management framework developed for the Tickbag project.

This project is in active development flux and will be changing a regular basis over the coming weeks.
It's very much a work in progress right now. Use at your own risk!

# Current situation
Initial untested implementations around:
- State management interface using `IState` to inject state access where-ever it's needed.
  - State is split into Contexts and Scopes to form a state tree.
- State Monitoring and notifications to Components.
- Immutable data type wrapper class around all data retrieved from the state.
- You can use `.Emit()` to retrieve the data object
  - All emitted data is isolated and does not effect the state, even reference types.
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

Inject the Global Application State (GAS) into each component that needs it, e.g.

```c#
@inject IState Gas
```

Now you can set up and read the state like so:
```c#
var todos = await Gas.Ignite<Guid, Todo>().Fuel((_, ct) => GetDefaultState(ct)).GetAsync(cancellationSource.Token);
```
This creates (or uses) a state object with `Guid` ids and `Todo` models. Default data for the Todo state is loaded via the `Fuel()` method.
Finally, `GetAsync` provides the state data.

In the form above, the state data will be a read-only dictionary of immutable Todo objects.

The state will hang around even if the Component unloads and the user navigates elsewhere in the application.

State is only cleared in 3 scenarios:
1. The user refreshs the browser page or leaves the site.
2. You call `Dispose()` on the GAS, which will remove all application state.
3. You call `Dispose()` on any of the scoped states you've cretaed, whcih will remove the scoped state and any child states.

You can update the state like this:
```c#
awauit Gas.Ignite<Guid, Todo>().Updater(todoId).Update(new Todo());
```

## Immutable objects
Any data you retrieve from the GAS will be immutable.
This is enforced by the `IImmutable` wrapper that sits over the object.

The immutable allows you to access properties in the data model via `Value()` and `Ref()` methods. You can also check a value in the immutable via the `Check` method.
A generic, delegate function called `Get()` allows more nuanced access to the internal data for consumption.

If you want read/write access to the data in the immutable you'll need to call the `Emit()` method. This will generate an isolated version of the imnmutable data.
Any changes you make to this emitted object will not affect the original immutable. It's simple a copy of the data.
