# Premier League Squad Finder

This project provides a backend API and a frontend React app to search and display English Premier League team squads.

## Solution Overview

### Backend:

- ASP.NET Core Web API that integrates with the external Football API https://www.api-football.com/ to fetch teams and squad details.
- Uses HttpClient with dependency injection and FluentResults for robust error handling.
- Simple logging to console just for testing purposes
- CORS configured to allow requests from the frontend running on `http://localhost:5173`.
- Swagger/OpenAPI enabled for easy testing and API exploration during development.

### Frontend:

- React app created with vite (https://vite.dev/guide/) that allows searching for a team by name or nickname and displays squad members with photos and details.
- Fetches data from the backend API and handles loading and error states.

## How to Run

### Prerequisites

- .NET 8 SDK installed
- Node.js (for frontend)
- API Key for the Football API (configured in backend `appsettings.json`)

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

bash
Copy
Edit
dotnet build
dotnet run
The backend will start at http://localhost:5231.

Swagger UI is available at http://localhost:5231/swagger for API testing.

Frontend
Navigate to the frontend project directory (e.g., frontend).

Install dependencies:

bash
Copy
Edit
npm install
Run the development server:

bash
Copy
Edit
npm run dev
Open your browser at http://localhost:5173.

Use the search box to enter a team name or nickname and view the squad details.

Important Notes
The backend service uses logging to output errors and important info to the console, helping with debugging.

CORS is configured to allow the frontend to communicate with the backend during development.

The project uses FluentResults in the backend to handle and propagate errors in a clean way.

The solution is designed for ease of maintenance and extensibility.
```
