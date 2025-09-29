# nextech-interview-assessment

Nextech coding challenge featuring an Angular 20 frontend and a .NET 8 Web API that displays curated Hacker News stories.

## Project Layout

- `Frontend/` – Angular standalone app with Material UI.
- `Backend/` – ASP.NET Core Web API (`HackerNewsApi`).
- `scripts/` – Convenience scripts for starting each service.

## Prerequisites

- Node.js 20 (or 18 LTS) and npm 9+ for the frontend.
- .NET 8 SDK (`dotnet --list-sdks` should show an 8.x entry) for the backend.
- Optional: Angular CLI (`npm install -g @angular/cli`) for extra tooling.

## Quick Start

- `./scripts/start-backend.sh` – restores dependencies and runs the API on `http://localhost:5163` / `https://localhost:7291`.
- `./scripts/start-frontend.sh` – installs dependencies on first run and launches the dev server on `http://localhost:4200`.

> Run the scripts from the repository root in separate terminals so the dev servers stay alive. Press `Ctrl+C` in each terminal to stop.

### Manual steps

- Backend: `cd Backend/HackerNewsApi`, `dotnet restore`, then `dotnet watch run` (or `dotnet run`).
- Frontend: `cd Frontend`, `npm install`, then `npm start` and open `http://localhost:4200`.

### Useful commands

- `npm run build` – production build of the Angular app.
- `npm test` – frontend unit tests (Karma/Jasmine).
- `dotnet test Backend/HackerNewsApi.Tests` – backend unit tests.
- `dotnet watch run --project Backend/HackerNewsApi` – hot-reload API during development.

## API Surface (Backend)

- `GET /api/hackernews/newest?page=1&pageSize=20` – newest stories with pagination.
- `GET /api/hackernews/search?query=Angular&page=1&pageSize=20` – basic title search over the cached newest stories.

Swagger UI is available at `/swagger` in development mode.

## Technologies

- Angular 20 standalone components with the modern signals API for local state.
- Angular Material 20 (cards, buttons, spinner).
- RxJS `firstValueFrom` for bridging Angular HTTP observables into async/await flows.
- ASP.NET Core 8, HttpClient factory, and IMemoryCache for resilient Hacker News API access.
- xUnit test project scaffold (`Backend/HackerNewsApi.Tests`) for backend verification.

## Design Decisions & Trade-offs

- **Angular Material + custom theme** – accelerates UI delivery with accessible components while allowing branding via CSS. Drawback: increases bundle size.
- **Standalone Angular architecture with signals** – removes NgModule boilerplate and keeps state co-located inside services.
- **Story card extracted into its own component** – improves readability and reusability of the feed. Adds a small amount of component overhead but makes future variations (e.g., compact cards) easier.
- **Backend caching of Hacker News responses** – reduces external API calls and speeds up repeat requests with a 5-minute cache window. Can serve slightly stale content and increases memory footprint proportional to cached story count.
- **Search via filtered newest stories** – keeps implementation simple and offline-friendly by filtering in-memory results. Limits search coverage to the newest 500 stories and may miss older/archived items.

## Configuration Notes

- The frontend expects the API at `http://localhost:5163/api` (see `Frontend/src/app/shared/config/api.config.ts`). Update the base URL before deploying to another environment.
- CORS is configured to allow `http://localhost:4200`; adjust in `Backend/HackerNewsApi/Program.cs` when hosting elsewhere.
