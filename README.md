# nextech-interview-assessment
Nextech coding challenge

## Project Overview

This is a full-stack application with an **Angular frontend** and a **C# .NET Core backend**. The application demonstrates a weather dashboard that fetches weather forecast data from the backend API and displays it in an interactive Angular interface.

### Architecture

- **Frontend**: Angular 20.3.3 with TypeScript, standalone components, and modern Angular features
- **Backend**: .NET Core 9.0 Web API with CORS enabled for cross-origin requests
- **Communication**: RESTful API communication between frontend and backend

## Prerequisites

Before running this application, ensure you have the following installed:

- [.NET Core SDK 9.0+](https://dotnet.microsoft.com/download)
- [Node.js 20.19+ and npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) (will be installed automatically via npx)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd nextech-interview-assessment
```

### 2. Backend Setup (.NET Core API)

Navigate to the backend directory and run the API:

```bash
cd BackendApi
dotnet restore
dotnet build
dotnet run
```

The backend API will be available at:
- HTTPS: `https://localhost:7002`
- HTTP: `http://localhost:5176`

**API Endpoints:**
- `GET /WeatherForecast` - Returns a list of weather forecast data

### 3. Frontend Setup (Angular)

In a new terminal window, navigate to the frontend directory:

```bash
cd FrontendApp
npm install
npm start
# or use: ng serve
```

The Angular application will be available at: `http://localhost:4200`

## Usage

1. Start the backend API first (Step 2 above)
2. Start the frontend application (Step 3 above)
3. Open your browser to `http://localhost:4200`
4. The weather dashboard will automatically load data from the backend API
5. Use the "Refresh Weather Data" button to get new weather forecasts

## Features

- **Responsive Design**: The application works on desktop and mobile devices
- **Error Handling**: Graceful error handling with retry functionality
- **Loading States**: Visual feedback during data loading
- **Modern Angular**: Uses latest Angular features including signals, control flow, and standalone components
- **CORS Enabled**: Backend properly configured for frontend communication
- **Clean Architecture**: Separation of concerns between frontend and backend

## Project Structure

```
nextech-interview-assessment/
├── BackendApi/                 # .NET Core Web API
│   ├── Controllers/            # API controllers
│   ├── Program.cs             # Main application entry point
│   └── WeatherForecast.cs     # Weather data model
├── FrontendApp/               # Angular application
│   ├── src/app/               # Angular application code
│   │   ├── app.ts            # Main application component
│   │   ├── app.html          # Application template
│   │   ├── app.css           # Application styles
│   │   └── weather.service.ts # Weather API service
│   └── package.json          # Frontend dependencies
└── README.md                 # This file
```

## Development

### Backend Development

- The backend uses the default Weather Forecast controller that comes with .NET Core Web API template
- CORS is configured to allow requests from the Angular frontend
- API runs on both HTTP and HTTPS endpoints

### Frontend Development

- The Angular app uses standalone components (modern Angular approach)
- Weather service handles API communication with proper error handling
- Responsive grid layout for weather cards
- Modern CSS with hover effects and smooth transitions

## Building for Production

### Backend
```bash
cd BackendApi
dotnet publish -c Release
```

### Frontend
```bash
cd FrontendApp
npm run build
```

The built files will be in `FrontendApp/dist/FrontendApp/`.

## Troubleshooting

1. **CORS Issues**: Ensure the backend is running before starting the frontend
2. **Port Conflicts**: Make sure ports 4200 (frontend) and 7002/5176 (backend) are available
3. **SSL Certificate Issues**: The backend uses HTTPS by default; you may need to accept the development certificate

## Technologies Used

- **Backend**: C#, .NET Core 9.0, ASP.NET Core Web API, CORS
- **Frontend**: Angular 20.3.3, TypeScript, RxJS, CSS Grid, Flexbox
- **Development**: Node.js, npm, Angular CLI, .NET CLI
