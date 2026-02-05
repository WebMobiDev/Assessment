# Assessment – Users API + Web UI (.NET)

This solution implements:
- A **REST Web API** for managing Users (CRUD)
- A simple **Web UI** (MVC) that consumes the API for all user operations
- **Unit tests** (validators) + **Integration tests** (API endpoints via HTTP)

-------------------------------------------------------------------------------------------------------------------

## Tech Stack
- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC (Web UI)
- EF Core (SQL Server in runtime)
- FluentValidation (request validation)
- xUnit (tests)
- Docker Compose (SQL + API + Web)

-------------------------------------------------------------------------------------------------------------------

## Solution Structure
/src
/Api -> Web API (Users endpoints)
/Web -> MVC UI (calls API via HttpClient)
/Infrastructure -> EF Core DbContext, database persistence
/Domain -> Domain models

/tests
/Assessment.Api.UnitTests -> Unit tests (validators / pure logic)
/Assessment.Api.IntegrationTests -> Integration tests (API over HTTP, SQLite)

/docker-compose.yml -> Runs SQL Server + API + Web
/Assessment.sln

-------------------------------------------------------------------------------------------------------------------

## Key Technical Decisions

### 1) Separate API and Web projects (two services)
The Web UI is intentionally a separate app that **only talks to the API**.  
This demonstrates clean separation of concerns and ensures the UI uses the API for **all data operations** (as required).

### 2) Validation via FluentValidation
Request DTO validation is implemented using FluentValidation:
- Invalid email / missing name → **400 Bad Request**
- Validation errors are surfaced in the UI via standard MVC validation.

### 3) Database: SQL Server for runtime, SQLite for integration tests
- Runtime uses **SQL Server** (Docker container or local SQL Server)
- Integration tests use **in-memory SQLite** (fast + isolated, no external dependency)

### 4) Auto migrations on startup (development convenience)
The API applies EF migrations on startup for quick local setup.

-------------------------------------------------------------------------------------------------------------------

## Running with Docker (Recommended)

### Prerequisites
Install Docker Desktop:
- Windows/Mac: Docker Desktop
- Linux: Docker Engine + Compose plugin

> If Docker is not installed, skip to **Run without Docker** below.

### Start the full system
From the repo root:

```bash
docker compose up --build
```
### Expected URLs

- Web UI: http://localhost:5158

- API: http://localhost:5247

- API Swagger: http://localhost:5247/swagger

### Stop containers

```bash
docker compose down
```

-------------------------------------------------------------------------------------------------------------------

## Run without Docker (Alternative)

### Prerequisites

- .NET SDK 8 installed

1) Configure API connection string

*** Update src/Api/appsettings.Development.json (or user secrets) with your SQL connection string: ***

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=AssessmentDb;User Id=...;Password=...;TrustServerCertificate=True;"
  }
}

2) Run the API
``` bash
dotnet run --project .\src\Api --urls http://localhost:5247
```

3) Run the Web UI
``` Bash
dotnet run --project .\src\Web --urls http://localhost:5158
```

### Web UI:

- http://localhost:5158

### Swagger:

- http://localhost:5247/swagger

-------------------------------------------------------------------------------------------------------------------

### Running Tests
## Run all tests (recommended)
```Bash
dotnet test .\Assessment.sln
```
## Run unit tests only
```Bash
dotnet test .\tests\Assessment.Api.UnitTests\Assessment.Api.UnitTests.csproj
```

## Run integration tests only
```Bash
dotnet test .\tests\Assessment.Api.IntegrationTests\Assessment.Api.IntegrationTests.csproj
```
***Notes about tests***

>Unit tests validate DTO validators (no HTTP, no DB).
>Integration tests spin up the API using WebApplicationFactory, and use in-memory SQLite for persistence.

-------------------------------------------------------------------------------------------------------------------

### SQL Server (Docker) – Connection Details (SSMS)

>If you want to inspect the database using SQL Server Management Studio:

- Server: localhost,14333

- Authentication: SQL Login

- User: sa

- Password: Passw0rd!12345

**Note: If port 1433 is already used on your machine, the compose file maps SQL to 14333 to avoid clashes.**
-------------------------------------------------------------------------------------------------------------------

### API endpoints (CRUD):

- GET /api/users

- GET /api/users/{id}

- POST /api/users

- PUT /api/users/{id}

- DELETE /api/users/{id}

-------------------------------------------------------------------------------------------------------------------

### MVC UI:
## Users list

1) Add user

2) Edit user

3) Delete user

-------------------------------------------------------------------------------------------------------------------

### Server + client validation messages
## Tests:

- Unit tests for FluentValidation validators
- Integration tests for API behavior via HTTP