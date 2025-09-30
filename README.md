# Hacker News Project

Coding challenge featuring an Angular 20 frontend, a full ASP.NET Core 8 Web API for local development, and an Azure Functions backend that powers the Static Web Apps deployment.

## Project Layout

- `Frontend/` – Angular standalone app with Material UI.
- `Backend/` – ASP.NET Core Web API (`HackerNewsApi`) for rich local debugging and integration tests.
- `api/` – Azure Functions (.NET 8 isolated) surface that mirrors the Web API for Static Web Apps hosting.
- `scripts/` – Convenience scripts for starting each service.

## Prerequisites

- Node.js 20 (or 18 LTS) and npm 9+ for the frontend.
- .NET 8 SDK (`dotnet --list-sdks` should show an 8.x entry) for both backend codebases.
- Optional: Angular CLI (`npm install -g @angular/cli`) for extra tooling.
- Optional: Azure Functions Core Tools (`npm i -g azure-functions-core-tools@4`) if you want to run the Functions app locally.

## Quick Start

- `./scripts/start-backend.sh` – restores dependencies and runs the ASP.NET API on `http://localhost:5163` / `https://localhost:7291`.
- `./scripts/start-frontend.sh` – installs dependencies on first run and launches the dev server on `http://localhost:4200`.
- Azure Functions: `cd api && func start` (requires Azure Functions Core Tools) to emulate the Static Web Apps backend locally on `http://localhost:7071/api`.

> Run the scripts/commands from the repository root in separate terminals so the dev servers stay alive. Press `Ctrl+C` in each terminal to stop.

### Manual steps

- ASP.NET backend: `cd Backend/HackerNewsApi`, `dotnet restore`, then `dotnet watch run` (or `dotnet run`).
- Azure Functions backend: `cd api`, `dotnet restore`, then `func start`.
- Frontend: `cd Frontend`, `npm install`, then `npm start` and open `http://localhost:4200`.

### Useful commands

- `npm run build` – production build of the Angular app.
- `npm test` – frontend unit tests (Karma/Jasmine).
- `dotnet test Backend/HackerNewsApi.Tests` – backend unit tests.
- `dotnet watch run --project Backend/HackerNewsApi` – hot-reload API during development.
- `dotnet publish api/HackerNewsFunctions.csproj -c Release` – build the Azure Functions app the same way the CI workflow does.

## API Surface (Shared)

Both backends expose the same endpoints:

- `GET /api/hackernews/newest?page=1&pageSize=20` – newest stories with pagination.
- `GET /api/hackernews/search?query=Angular&page=1&pageSize=20` – basic title search over the cached newest stories.

The ASP.NET Web API additionally exposes Swagger UI at `/swagger` in development mode.

## Why Two Backends?

- **ASP.NET Core Web API (`Backend/`)** – Stays in the repo for rich local debugging, existing integration tests, and scenarios that benefit from the full ASP.NET hosting model (Swagger, middleware, etc.).
- **Azure Functions (`api/`)** – Mirrors the same business logic but ships as a lightweight serverless app that Azure Static Web Apps can build and host alongside the Angular frontend. This keeps the cloud deployment simple while avoiding changes to the original API used throughout the coding exercise.

Both implementations mirror the same Hacker News service logic, so behavior stays consistent whether you run locally or in the managed Azure environment.

## Technologies

- Angular 20 standalone components with the modern signals API for local state.
- Angular Material 20 (cards, buttons, spinner).
- RxJS `firstValueFrom` for bridging Angular HTTP observables into async/await flows.
- ASP.NET Core 8, HttpClient factory, and IMemoryCache for resilient Hacker News API access.
- Azure Functions .NET 8 isolated worker for the Static Web Apps backend.
- xUnit test project scaffold (`Backend/HackerNewsApi.Tests`) for backend verification.

## Design Decisions & Trade-offs

- **Angular Material + custom theme** – accelerates UI delivery with accessible components while allowing branding via CSS. Drawback: increases bundle size.
- **Standalone Angular architecture with signals** – removes NgModule boilerplate and keeps state co-located inside services.
- **Story card extracted into its own component** – improves readability and reusability of the feed. Adds a small amount of component overhead but makes future variations (e.g., compact cards) easier.
- **Backend caching of Hacker News responses** – reduces external API calls and speeds up repeat requests with a 5-minute cache window. Can serve slightly stale content and increases memory footprint proportional to cached story count.
- **Search via filtered newest stories** – keeps implementation simple and offline-friendly by filtering in-memory results. Limits search coverage to the newest 500 stories and may miss older/archived items.

## Configuration Notes

- The frontend expects the API at `http://localhost:5163/api` during ASP.NET local development and `/api` when hosted in Azure Static Web Apps (see `Frontend/src/app/shared/config/api.config.ts`).
- CORS for the ASP.NET API is configured to allow `http://localhost:4200`; adjust in `Backend/HackerNewsApi/Program.cs` when hosting elsewhere.