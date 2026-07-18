# Task Management App

Full-stack task management application: a Vue 3 frontend, an ASP.NET Core Web API backend, and an xUnit test project.

**Frontend**: Vue 3 + TypeScript + Pinia  
**Backend**: ASP.NET Core 8 Web API + EF Core + SQLite + JWT Auth  
**Tests**: xUnit + WebApplicationFactory

## Project Structure

```
TaskManagementApp/
├── task-management-frontend/   # Vue 3 frontend
├── TaskManagementApi/           # ASP.NET Core Web API
├── TaskManagementApi.Tests/     # xUnit test project
└── TaskManagement.sln           # Solution file (opens both API + Tests in Visual Studio)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS version) and npm
- Visual Studio 2022 (or VS Code with the C# Dev Kit extension)

## Running the Backend API

The API uses SQLite, and the database file is created automatically on first run — no manual migration step needed.

### Option A: Visual Studio

1. Open `TaskManagement.sln` in Visual Studio.
2. Set `TaskManagementApi` as the startup project (right-click it in Solution Explorer → **Set as Startup Project**).
3. Press **F5** (or **Ctrl+F5** to run without debugging).
4. Visual Studio will open a browser to the Swagger UI. Note the base URL shown in the address bar (e.g. `https://localhost:5155`)

### Option B: Command line

```bash
cd TaskManagementApi
dotnet run
```

The console output will print the URL(s) the API is listening on.

## Running the Tests

### Option A: Visual Studio

Open **Test Explorer** (Test → Test Explorer) and click **Run All**.

### Option B: Command line

```bash
dotnet test
```

(Can be run from the repo root, since it will discover the test project via the solution.)

## Running the Frontend

```bash
cd task-management-frontend
npm install
npm run dev
```

This starts the Vue dev server (Vite) — the terminal will print the local URL (typically `http://localhost:5173`).

**Important:** Confirm the frontend is pointed at the correct API URL/port from the step above. It's currently set at 5155 but if your API ran at a different port you'll have to update the frontend baseURL at src/services/api.ts.

## Configuration Notes

- `TaskManagementApi/appsettings.json` contains a development JWT signing key and the SQLite connection string.
- CORS is configured to allow any origin in development, so the frontend dev server should be able to reach the API regardless of which port Vite picks.

## Features

- User registration & login (JWT)
- Create, read, update, delete tasks
- Mark tasks as complete
- Responsive UI with modals

## Quick Start Summary

1. `cd TaskManagementApi && dotnet run` (note the printed URL - should be 5155)
2. In a separate terminal: `cd task-management-frontend && npm install && npm run dev`
3. Confirm frontend's API base URL matches step 1
4. Open the frontend URL in your browser