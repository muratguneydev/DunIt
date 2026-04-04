# DunIt

A family chore tracker for kids. Parents assign daily chores to each child. Children open the app on their phone, see their chores for the day, and tap to mark each one done.

Built with Blazor WebAssembly (PWA) + Firebase. Runs on iPhone and Android with no App Store required.

## Requirements

- [Docker](https://www.docker.com/products/docker-desktop)

No local .NET SDK needed — everything runs in Docker.

## Run the tests

```bash
docker compose build test
docker compose run --rm test
```

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

The E2E tests run against the live web container in a real Chromium browser inside Docker.

```bash
docker compose build web e2e
docker compose up web -d        # start the app
docker compose run --rm e2e     # run E2E tests
```

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
PAUSE_BEFORE_TESTS=10 docker compose run --rm e2e

# Skip the pause entirely (useful in CI)
PAUSE_BEFORE_TESTS=0 docker compose run --rm e2e
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

The report shows pass/fail status, duration, and error details for every test — no
live connection required.

### How it works

`run-e2e.sh` is the container entry point. It:

1. Starts **Xvfb** — a virtual X display so Chromium has a screen to render on.
2. Starts **x11vnc** — a VNC server that streams that virtual display.
3. Starts **noVNC** — a web-based VNC client served on port 7900, so you can
   watch the browser from any desktop browser without installing a VNC client.
4. Waits `PAUSE_BEFORE_TESTS` seconds (default 5) to give you time to connect.
5. Runs `dotnet test` with `HEADED=1` and `PLAYWRIGHT_SLOW_MO=500`, so Chromium
   is visible and each action is slowed down by 500 ms.

## Rebuild after code changes

The web image bakes the published app into the container at build time, so after any source change you need to rebuild:

```bash
docker compose build web && docker compose up web -d
```
