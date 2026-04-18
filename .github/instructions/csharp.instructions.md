---
applyTo: "**/*.cs"
---

# C# Coding Conventions

## File structure

Every `.cs` file must open with the `namespace` declaration, followed by a blank line, then `using` directives:

```csharp
namespace DunIt.Core.Services;

using System.Collections.Generic;
using DunIt.Core.Models;
```

Never put `using` directives above the `namespace` declaration.

## Naming

- Do **not** suffix async method names with `Async`. The `Task` / `ValueTask` return type is sufficient.
  - Good: `Task<Child> GetChild(ChildId id)`
  - Avoid: `Task<Child> GetChildAsync(ChildId id)`

## Constructor design

When a class has two distinct modes, use two explicit constructors and a private `bool` field. Do not use nullable optional parameters to distinguish modes.

```csharp
// Good
public FirebaseConfig(string apiKey, string projectId)
{
    _isUsingEmulator = false;
}

public FirebaseConfig(string apiKey, string projectId, string emulatorHost) : this(apiKey, projectId)
{
    _isUsingEmulator = true;
    EmulatorHost = emulatorHost;
}

private readonly bool _isUsingEmulator;
public bool IsUsingEmulator => _isUsingEmulator;

// Avoid
public FirebaseConfig(string apiKey, string projectId, string? emulatorHost = null) { }
```

## Null Object pattern

Do not use nullable types for "nothing selected / not set" state in view models or domain objects.  
Instead, add a static `Empty` sentinel:

```csharp
public record Child(ChildId Id, string Name)
{
    public static readonly Child Empty = new(new ChildId(string.Empty), string.Empty);
}
```

Initialise properties to `Type.Empty` — never to `null`. This eliminates null-checks and `!` operators throughout callers.
