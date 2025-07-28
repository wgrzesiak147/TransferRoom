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
```

### Build and run the backend API:

bash
Copy
Edit
dotnet build
dotnet run
The backend will start at:
👉 http://localhost:5231
Swagger UI available at:
👉 http://localhost:5231/swagger

### 🌐 Frontend Setup

Navigate to the frontend project directory:

bash
Copy
Edit
cd frontend
Install dependencies:

bash
Copy
Edit
npm install
Start the development server:

bash
Copy
Edit
npm run dev
Open your browser at:
👉 http://localhost:5173

Use the search box to enter a team name or nickname and view the squad details.
A season dropdown is also available.

### 🧠 Important Notes

The backend uses console logging via ILogger to help debug API calls and errors.

CORS is configured to allow cross-origin requests from the frontend (localhost:5173).

The project uses FluentResults for clean and consistent error handling.

### In-memory caching is used to:

Store the list of Premier League teams (from the /teams API).

Store each team's squad (from the /players or /squads API).

Caching reduces external API usage, improves performance, and adds resiliency.

⏱ Cache expiration is currently set to 1 hour.
