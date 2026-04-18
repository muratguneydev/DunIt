# DunIt — Copilot Instructions

DunIt is a kids' chore tracker: a Blazor WebAssembly PWA backed by Firebase (Firestore + Auth).  
No backend server. No local .NET SDK — everything builds and runs in Docker.

## Project layout

```
src/DunIt.Core          # domain models and services — no Blazor dependency
src/DunIt.Web           # Blazor WASM PWA
tests/DunIt.UnitTests   # NUnit unit tests
tests/DunIt.Testing     # shared test helpers and fixtures
tests/DunIt.IntegrationTests  # Playwright E2E tests
```

## Domain model

- **Chore** — recurring task (title, frequency, assigned days)
- **ChoreCompletion** — record that a child completed a chore on a given date
- **Child** — user profile (name, avatar)
- **Parent** — admin who manages chores and children

Data access is behind `IChoreRepository` / `IChildRepository`. `Firebase*` implementations are used in the app and E2E tests. `InMemory*` implementations are for unit tests only.

## C# conventions

- `namespace` declaration is **always first** in every `.cs` file, before `using` directives.
- Do **not** append `Async` to method names. `Task` return type signals async.
- Prefer two explicit constructors + a private `bool` field over one constructor with a nullable optional parameter.
- Use `Type.Empty` static sentinel instead of `Type?` nullable for "nothing selected" state in view models.

## Docker commands

```bash
docker compose build
docker compose run --rm test          # unit tests
docker compose -f docker-compose.yml -f docker-compose.e2e.yml run --rm e2e  # E2E tests
docker compose up web                 # run the app on http://localhost:5000
```

## Workflow

Follow TDD: write failing tests first, then implement.

1. Write unit tests (red)
2. Write E2E tests (red)
3. Implement to make tests pass (green)
4. Review and refactor
5. `git add <files>`, `git commit`, `git push`

Every frontend change requires E2E tests in the same iteration.  
End every iteration with a commit **and** push.
