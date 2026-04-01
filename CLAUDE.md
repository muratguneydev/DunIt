# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

No local .NET SDK — everything runs in Docker.

```bash
# Build
docker compose build

# Run all tests
docker compose build test
docker compose run --rm test

# Run the app (serves Blazor WASM on http://localhost:5000)
docker compose build web
docker compose up web
```

## Architecture

DunIt is a kids' chore tracker — a Blazor WebAssembly PWA backed by Firebase (Firestore, Auth).

- `src/DunIt.Core` — domain models and services (plain class library, no Blazor dependency)
- `src/DunIt.Web` — Blazor WebAssembly PWA (references DunIt.Core)
- `tests/DunIt.UnitTests` — unit tests (NUnit + AutoFixture + Shouldly)
- `tests/DunIt.Testing` — shared test helpers and fixtures

### Tech stack

| Layer | Technology |
|---|---|
| Frontend | Blazor WebAssembly (PWA) |
| Data store | Firebase Firestore |
| Auth | Firebase Authentication |
| Hosting | Firebase Hosting (static files) |

### Domain model

- **Chore** — a recurring task assigned to a child (title, frequency, assigned days)
- **ChoreCompletion** — a record that a child completed a chore on a given date
- **Child** — a user profile (name, avatar)
- **Parent** — admin user who manages chores and children

### Repository pattern

Data access is behind repository interfaces (`IChoreRepository`, `IChildRepository`). `InMemory*` implementations are used in development and tests. `Firebase*` implementations will be added in Phase 6.

### Test conventions

- Unit tests use AutoFixture for randomised test data creation.
- Assertions use Shouldly (`ShouldBe`, `ShouldNotBeNull`, etc.).
- Tests follow Arrange / Act / Assert with explicit comments.
- Test method names follow `ShouldXXX_WhenYYY` convention (e.g., `ShouldMarkComplete_WhenChoreExists`).
- Test class names match the class under test with a `Tests` suffix (e.g., `ChoreServiceTests`).
- Do not use `[TestFixture]` attribute — it is redundant in NUnit 3+.
