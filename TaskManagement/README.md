# Task Management API

Production-ready RESTful API for a Task Management System built with .NET 10, ASP.NET Core, Entity Framework Core, PostgreSQL, xUnit, and Clean Architecture.

## Architecture

The solution is split into four layers:

- `TaskManagement.Domain`: Entities, enum, and domain validation.
- `TaskManagement.Application`: Use cases, DTOs, service contracts, and the `IApplicationDbContext` EF abstraction.
- `TaskManagement.Infrastructure`: PostgreSQL EF Core `DbContext`, entity configuration, and migrations.
- `TaskManagement.API`: Controllers, Swagger/OpenAPI, JSON configuration, and exception handling middleware.

EF Core is used directly through the `IApplicationDbContext` abstraction. A repository layer was intentionally not added because the current use cases are straightforward CRUD/query workflows and EF already provides the needed unit-of-work/query abstraction.

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

## Database Migrations

An initial migration is included under `src/TaskManagement.Infrastructure/Persistence/Migrations`.

Install or restore EF tooling, then update the database:

```powershell
dotnet tool restore
dotnet ef database update --project src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj --startup-project src/TaskManagement.API/TaskManagement.API.csproj
```

If local tool resolution is unavailable on the machine, install `dotnet-ef` globally or invoke the restored EF tool directly.

## API Endpoints

- `POST /api/tasks`: Create Task
- `GET /api/tasks/{id}`: Get Task By Id
- `GET /api/tasks`: Get All Tasks
- `PUT /api/tasks/{id}`: Update Task
- `DELETE /api/tasks/{id}`: Delete Task
- `GET /api/tasks/by-user/{userId}`: Get Tasks By User

## Example Request

```http
POST /api/tasks
Content-Type: application/json

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

## Original Prompt

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
