# Premier League Squad Finder

This project provides a backend API and a frontend React app to search and display English Premier League team squads.

## Solution Overview

### Backend:

- ASP.NET Core Web API that integrates with the external [API-Football](https://www.api-football.com/) to fetch teams and squad details.
- Uses `HttpClient` with dependency injection and **FluentResults** for robust error handling.
- **In-memory caching** has been added to avoid unnecessary calls to the 3rd party API for data that changes infrequently (e.g., team list and squad).
- Simple logging to console (via `ILogger`) for development and debugging purposes.
- CORS configured to allow requests from the frontend running on `http://localhost:5173`.
- Swagger/OpenAPI enabled for easy testing and API exploration during development.

### Frontend:

- React app created with [Vite] + Typescript (https://vite.dev/guide/) that allows searching for a team by name or nickname and displays squad members with photos and details.
- Fetches data from the backend API and handles loading and error states gracefully.
- Responsive and lightweight UI for fast development and testing.

---

## How to Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) installed
- [Node.js](https://nodejs.org/en) installed (for frontend)
- API Key for the Football API (configured in backend `appsettings.json`)

---

### Backend

1. Clone the repository and open the backend project directory.

2. Update `appsettings.json` or `appsettings.Development.json` with your Football API key under the `ApiFootball` section:

```json
{
  "ApiFootball": {
    "BaseUrl": "https://api-football-v1.p.rapidapi.com/v3/",
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
Build and run the backend API:

dotnet build
dotnet run
The backend will start at: http://localhost:5231

Swagger UI will be available at: http://localhost:5231/swagger

Frontend
Navigate to the frontend project directory (e.g., frontend):

cd frontend
Install dependencies:

npm install
Run the development server:

npm run dev
Open your browser at: http://localhost:5173

Use the search box to enter a team name or nickname and view the squad details. There is also dropdown to select season

Important Notes
The backend uses console logging to output errors and HTTP request issues, aiding debugging during development.

CORS is configured to allow cross-origin requests from the frontend to the backend (localhost).

The project leverages FluentResults to cleanly handle and propagate error cases in the backend logic.

In-memory caching has been added for:

The list of Premier League teams (fetched from the teams API).

Each team's squad (fetched from the players/squads API).

This reduces external API usage, improves response time, and provides resiliency if the external service becomes unavailable temporarily. Cache expiration is currently set to 1 hour.

```
