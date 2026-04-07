# DunIt

A family chore tracker for kids. Parents assign daily chores to each child. Children open the app on their phone, see their chores for the day, and tap to mark each one done.

Built with Blazor WebAssembly (PWA) + Firebase. Runs on iPhone and Android with no App Store required.

## Requirements

- [Docker](https://www.docker.com/products/docker-desktop)

No local .NET SDK needed — everything runs in Docker.

## Run the tests

```bash
docker compose build test   # first time, or when .csproj files change
docker compose run --rm test
```

Source files are mounted from the host at runtime, so **no rebuild is needed after code changes** — just run the tests again.

## Run the app

```bash
docker compose build web
docker compose up web
```

Then open **http://localhost:5000** in your browser.

To stop the app:

```bash
docker compose down
```

## Run end-to-end tests (Playwright)

The E2E tests run against the live web container and a local Firebase emulator in a real Chromium browser inside Docker.

```bash
# First time, or when .csproj files change
docker compose -f docker-compose.yml -f docker-compose.e2e.yml build web e2e

# Run E2E tests (starts emulator + web automatically, then runs tests)
docker compose -f docker-compose.yml -f docker-compose.e2e.yml run --rm e2e
```

E2E test source is mounted from the host, so **no rebuild is needed after test changes** — only rebuild `e2e` when `.csproj` files change. Rebuild `web` whenever app source changes.

### Watching the tests live

Before the tests start, the container launches a noVNC server and prints:

```
  Open http://localhost:7900/vnc.html in your browser to watch the tests.
  Tests start in 5 seconds...
```

Open that URL in your browser during those 5 seconds and you will see Playwright
clicking through the app in real time. The default pause is 5 seconds; override it
with the `PAUSE_BEFORE_TESTS` environment variable:

```bash
# Wait 10 seconds instead
PAUSE_BEFORE_TESTS=10 docker compose -f docker-compose.yml -f docker-compose.e2e.yml run --rm e2e

# Skip the pause entirely (useful in CI)
PAUSE_BEFORE_TESTS=0 docker compose -f docker-compose.yml -f docker-compose.e2e.yml run --rm e2e
```

### HTML report

After every run an HTML report is written to `test-results/TestReport.html` on your
host machine (Docker mounts the `test-results/` directory automatically). Open it
when the tests finish:

```bash
# Windows
start test-results/TestReport.html

# Mac / Linux
open test-results/TestReport.html
```

The report shows pass/fail status and duration. For a richer view, use the traces.

### Traces

Every test writes a Playwright trace to `test-results/traces/<TestName>.zip`.
Traces contain a full timeline of every action, DOM snapshot, and screenshot.

To view a trace, drag the `.zip` file onto **https://trace.playwright.dev** — no
install needed. You'll see exactly what Playwright did, step by step.

On a test failure a screenshot is also saved to `test-results/screenshots/`.

### How it works

`run-e2e.sh` is the container entry point. It:

1. Starts **Xvfb** — a virtual X display so Chromium has a screen to render on.
2. Starts **x11vnc** — a VNC server that streams that virtual display.
3. Starts **noVNC** — a web-based VNC client served on port 7900, so you can
   watch the browser from any desktop browser without installing a VNC client.
4. Waits `PAUSE_BEFORE_TESTS` seconds (default 5) to give you time to connect.
5. Runs `dotnet test` with `HEADED=1` and `PLAYWRIGHT_SLOW_MO=500`, so Chromium
   is visible and each action is slowed down by 500 ms.

## How the app works in production

The live app is at **https://dunit-f0149.web.app**.

### User accounts

The app uses Firebase Authentication with email and password. There is no
self-registration — accounts are created manually in the Firebase Console:

1. Go to [Firebase Console](https://console.firebase.google.com) → **dunit-f0149** → **Authentication** → **Users**
2. Click **"Add user"** and enter an email and password for each family member
3. Up to 4 accounts: 1 parent + up to 3 children

### Signing in

Each family member opens the app, enters their email and password, and lands on
their daily chore view. The session persists across page refreshes — no need to
sign in again unless they explicitly sign out.

### Parent vs child

- **Children** see their own chore list for today and can tap to mark chores done or undo them.
- **The parent** uses the Admin page (linked from the home page) to add/remove chores and manage children.

### Real-time sync

All devices update live via Firestore `onSnapshot` listeners — when a child marks
a chore done, the parent's view updates instantly with no page refresh needed.

## Deploy to Firebase Hosting

The app is hosted at **https://dunit-f0149.web.app**.

Deployment requires a Firebase service account key. Set the path once:

```bash
export SERVICE_ACCOUNT_KEY="C:/Code/.firebase/dunit-f0149-firebase-adminsdk-fbsvc-9a6175d2a8.json"
```

Then build and deploy:

```bash
docker compose --profile deploy build deploy
SERVICE_ACCOUNT_KEY="$SERVICE_ACCOUNT_KEY" docker compose --profile deploy run --rm deploy
```

The deploy image builds the Blazor WASM app and runs `firebase deploy --only hosting` in one step.

## Rebuild after code changes

The web image bakes the published app into the container at build time, so after any source change you need to rebuild:

```bash
docker compose build web && docker compose up web -d
```
