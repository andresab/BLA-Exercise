# Task Management API

Production-ready RESTful API for a Task Management System built with .NET 10, ASP.NET Core, Entity Framework Core, PostgreSQL, JWT authentication, FluentValidation, xUnit, Docker, and Clean Architecture.

## Architecture

The solution is split into four layers:

- `TaskManagement.Domain`: Entities, enum, and domain validation.
- `TaskManagement.Application`: Use cases, DTOs, FluentValidation validators, service contracts, repository abstractions, and auth/token abstractions.
- `TaskManagement.Infrastructure`: PostgreSQL EF Core `DbContext`, repository implementations, JWT token generation, entity configuration, migrations, and sample-data seeding.
- `TaskManagement.API`: Controllers, JWT authentication, Swagger/OpenAPI, JSON configuration, validation filter, and exception handling middleware.

The service layer is independent of EF Core. Application services depend on repository and unit-of-work abstractions, while Infrastructure provides EF-backed implementations. Controllers are intentionally thin and delegate business workflows to Application services.

## Requirements

- .NET SDK 10
- PostgreSQL

Default connection string:

```json
"TaskManagementDb": "Host=localhost;Port=5432;Database=task_management;Username=postgres;Password=postgres"
```

Override it with configuration or environment variables before running in real environments.

## Run

```powershell
cd TaskManagement
dotnet restore
dotnet build
dotnet run --project src/TaskManagement.API/TaskManagement.API.csproj
```

Swagger UI is enabled in Development at:

```text
https://localhost:<port>/swagger
```

## Run With Docker

```powershell
cd TaskManagement
docker compose up -d --build
```

The Docker stack starts:

- PostgreSQL on `localhost:5432`
- API on `http://localhost:8081`

The API applies EF migrations at startup. The seed migration creates sample users and tasks.

## Database Migrations

Migrations are included under `src/TaskManagement.Infrastructure/Persistence/Migrations`, including sample data for:

- `ada@example.com`
- `grace@example.com`

Install or restore EF tooling, then update the database:

```powershell
dotnet tool restore
dotnet ef database update --project src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj --startup-project src/TaskManagement.API/TaskManagement.API.csproj
```

If local tool resolution is unavailable on the machine, install `dotnet-ef` globally or invoke the restored EF tool directly.

## API Endpoints

- `POST /api/auth/token`: Create JWT for a seeded user email
- `POST /api/tasks`: Create Task
- `GET /api/tasks/{id}`: Get Task By Id
- `GET /api/tasks`: Get All Tasks
- `PUT /api/tasks/{id}`: Update Task
- `DELETE /api/tasks/{id}`: Delete Task
- `GET /api/tasks/by-user/{userId}`: Get Tasks By User

Task endpoints require a bearer token. Users can only create, read, update, and delete their own tasks.

## Authentication Example

```http
POST /api/auth/token
Content-Type: application/json

{
  "email": "ada@example.com"
}
```

Use the returned `accessToken` as:

```http
Authorization: Bearer <accessToken>
```

## Validation And Errors

Request validation uses FluentValidation:

- Required fields are validated.
- Task titles are limited to 200 characters.
- Task descriptions are limited to 2000 characters.
- `DueDate` cannot be in the past when creating a task.

Validation, authorization, not-found, conflict, and unexpected errors are returned as RFC7807 ProblemDetails responses.

## Example Request

```http
POST /api/tasks
Content-Type: application/json
Authorization: Bearer <accessToken>

{
  "title": "Prepare sprint planning",
  "description": "Review backlog and define priorities.",
  "status": "Pending",
  "dueDate": "2026-07-01T14:00:00Z",
  "userId": "11111111-1111-1111-1111-111111111111"
}
```

## Example Response

```json
{
  "id": "22222222-2222-2222-2222-222222222222",
  "title": "Prepare sprint planning",
  "description": "Review backlog and define priorities.",
  "status": "Pending",
  "dueDate": "2026-07-01T14:00:00Z",
  "userId": "11111111-1111-1111-1111-111111111111",
  "user": {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Ada Lovelace",
    "email": "ada@example.com"
  }
}
```

## Tests

```powershell
dotnet test
```

The test suite includes:

- Unit tests for domain validation.
- Integration tests for task API endpoints using `WebApplicationFactory` and an isolated EF Core in-memory database.

## First Prompt

```text
Act as a Senior .NET Architect and Software Engineer.

Generate a production-ready RESTful API for a Task Management System using .NET 10 and Entity Framework in a separate solution. Create a proper README.md file including this prompt.

Requirements:

Architecture:

* Use Clean Architecture principles.
* Separate projects/layers:
  * API
  * Application
  * Domain
  * Infrastructure
* Use Dependency Injection.
* Follow SOLID principles.
* Implement Repository Pattern only if justified; otherwise leverage EF Core directly through abstractions.

Database:

* Use PostgreSQL.
* Configure EF Core migrations.
* Create Task and User entities.

Domain Model:

User:

* Id (Guid)
* Name (string)
* Email (string)

Task:

* Id (Guid)
* Title (string, required, max 200 chars)
* Description (string, optional, max 2000 chars)
* Status (enum: Pending, InProgress, Completed)
* DueDate (DateTime)
* UserId (Guid)
* User navigation property

API Features:

* Create Task
* Get Task By Id
* Get All Tasks
* Update Task
* Delete Task
* Get Tasks By User

Testing:

* Create unit tests using xUnit.
* Include integration tests for API endpoints.

Documentation:

* Configure Swagger/OpenAPI.
* Include request and response examples.
```

## Second Prompt

```text
Apply following changes in TaskManagement solution and include this prompt in README.md

1. Fix NuGet packages vulnerabilities.

2. Provide Docker support. Include following files: 
* Dockerfile
* docker-compose.yml

3. Create sample data using EF Migrations.

4. Implement following features:

Validation:
* Use FluentValidation.
* Validate required fields.
* Validate title length.
* DueDate cannot be in the past when creating a task.

Authentication & Authorization:
* Implement JWT authentication.
* Users can only access their own tasks.

Error Handling:
* Global exception middleware.
* Return RFC7807 ProblemDetails responses.
```

## Third Prompt

```text
Apply following code improvements and save this prompt in README.md file:
1. Use primary controller in all the classes if possible.
2. Make service layer independent of EF, applying repository pattern for task and auth features.
3. Make AuthController light: Move auth logic to application and infrastructure layers following Clean Architecture and SOLID principles.
```
