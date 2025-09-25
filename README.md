# nextech-interview-assessment

Nextech coding challenge.

## Backend

- C# .NET 8 Web API lives in `Backend`.
- Requires the .NET 8 SDK (`dotnet --list-sdks` should show 8.x).

### Run locally

1. `cd Backend`
2. Restore dependencies on first run: `dotnet restore`
3. Start the API: `dotnet run`

## Frontend

- Angular 20 application lives in `Frontend`.
- Requires Node.js 20 (or later 18.x LTS) and npm.

### Run locally

1. Install dependencies: `cd Frontend && npm install`
2. Start the dev server: `npm start`
3. Open `http://localhost:4200` in your browser.

Use `npm run build` for a production build and `npm test` for unit tests.\

The API listens on the port printed to the console and exposes `GET /random` to return a JSON payload with a random number.
