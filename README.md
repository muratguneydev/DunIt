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

## Rebuild after code changes

The web image bakes the published app into the container at build time, so after any source change you need to rebuild:

```bash
docker compose build web && docker compose up web -d
```
