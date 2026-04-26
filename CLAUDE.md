# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

No local .NET SDK — everything runs in Docker.

```bash
# Build
docker compose build

# Run unit tests
docker compose build test
docker compose run --rm test

# Run E2E tests (Playwright + Firebase Emulator)
docker compose -f docker-compose.yml -f docker-compose.e2e.yml build
docker compose -f docker-compose.yml -f docker-compose.e2e.yml run --rm e2e

# Run the app (serves Blazor WASM on http://localhost:5000, uses real Firebase)
docker compose build web
docker compose up web
```

## Visual / Playwright checks (Playwright MCP)

Authenticated pages can't be screenshot using the production stack (real Firebase, Google OAuth). Use the playwright stack instead, which wires the emulator to localhost so the browser can reach it.

**Start the stack**
```bash
docker compose -f docker-compose.yml -f docker-compose.e2e.yml -f docker-compose.playwright.yml up -d emulator web
```
`docker-compose.playwright.yml` exposes emulator ports 8080/9099 to the host and mounts `appsettings.playwright.json` (identical to the e2e settings but with `localhost` instead of `emulator` as the host, so the browser can reach the emulator).

**Seed the test user** (idempotent — safe to run even if the user already exists)
```bash
curl -s -X POST "http://localhost:9099/identitytoolkit.googleapis.com/v1/accounts:signUp?key=demo-api-key" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@dunit.app","password":"test1234","returnSecureToken":false}'
```

**Sign in via Playwright MCP** (in the same turn to keep the browser session alive)

1. Clear the PWA service worker cache — it caches the real-Firebase `appsettings.json` and must be evicted before the emulator config takes effect:
```js
// browser_evaluate
async () => { const regs = await navigator.serviceWorker.getRegistrations(); for (const r of regs) await r.unregister(); }
```
2. Navigate to `http://localhost:5000`, wait for the login page, then sign in:
```js
// browser_evaluate
async () => { await new Promise(r => setTimeout(r, 2000)); return window.firebase_interop.testSignIn('test@dunit.app', 'test1234'); }
```
3. Navigate to the target page and screenshot.

**Credentials**: `test@dunit.app` / `test1234` (defined in `FirebaseAuthEmulator.cs`).

**Tear down** when done:
```bash
docker compose down
docker compose up -d web   # restart the normal production stack
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

Data access is behind repository interfaces (`IChoreRepository`, `IChildRepository`). `Firebase*` implementations back both the web app and E2E tests. `InMemory*` implementations remain for unit tests only.

### Test conventions

- Unit tests use AutoFixture for randomised test data creation.
- Assertions use Shouldly (`ShouldBe`, `ShouldNotBeNull`, etc.).
- Tests follow Arrange / Act / Assert with explicit comments.
- Test method names follow `ShouldXXX_WhenYYY` convention (e.g., `ShouldMarkComplete_WhenChoreExists`).
- Test class names match the class under test with a `Tests` suffix (e.g., `ChoreServiceTests`).
- Do not use `[TestFixture]` attribute — it is redundant in NUnit 3+.
