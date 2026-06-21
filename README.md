# BookCatalog

Production-oriented Clean Architecture sample for a public book catalog with protected administrative writes.

## User Story

As a catalog visitor, I want to browse and search books without signing in, so that I can quickly discover what is available and inspect each book's details.

As a catalog administrator, I want to sign in securely and manage the book list, so that I can create, edit, and remove catalog entries while keeping public users in read-only mode.

Acceptance criteria:

- Public users can list books, search by title or author, and open a book detail page.
- Anonymous users cannot create, update, or delete books.
- Administrators can register, log in, and receive a JWT for protected write operations.
- Book data is validated before persistence: title, author, ISBN, and published year must be valid.
- Duplicate ISBNs are rejected.
- API failures return consistent HTTP responses without leaking stack traces.

```text
+----------------------+        +-------------------------+
| Angular 22 Frontend  | -----> | ASP.NET Core Web API    |
| Signals + zoneless   |        | Controllers + Middleware|
+----------------------+        +-----------+-------------+
                                             |
                                             v
                              +--------------+--------------+
                              | Application                 |
                              | Use cases, DTOs, validation |
                              +--------------+--------------+
                                             |
                                             v
                              +--------------+--------------+
                              | Domain                      |
                              | Entities, interfaces, errors|
                              +--------------+--------------+
                                             ^
                                             |
                              +--------------+--------------+
                              | Infrastructure              |
                              | ADO.NET, JWT, BCrypt        |
                              +--------------+--------------+
                                             |
                                             v
                                   +---------+---------+
                                   | SQL Server 2022   |
                                   +-------------------+
```

## Prerequisites

- Docker Desktop
- .NET 10 SDK
- Node 22
- Angular CLI 22

## Quick Start

```powershell
docker-compose up --build
```

The frontend is available at `http://localhost:4200`, and the API listens on `http://localhost:8080`.

Demo credentials:

- Email: `admin@bookcatalog.io`
- Password: `Admin1234!`

## Manual Local Setup

Start SQL Server locally and run:

```powershell
sqlcmd -S localhost -U sa -P BookCatalog_2024! -i db/01_schema.sql
sqlcmd -S localhost -U sa -P BookCatalog_2024! -i db/02_seed.sql
dotnet run --project src/BookCatalog.API/BookCatalog.API.csproj
```

In another terminal:

```powershell
cd frontend
npm install
ng serve
```

## API Reference

| Method | Path | Auth | Description | Example response |
|---|---|---:|---|---|
| GET | `/api/health` | No | Health probe | `{ "status": "healthy", "timestamp": "2026-06-20T19:00:00Z" }` |
| GET | `/api/books` | No | List books | `[{"id":"...","title":"Dune","author":"Frank Herbert"}]` |
| GET | `/api/books/{id}` | No | Get one book | `{"id":"...","title":"Dune","isbn":"9780441172719"}` |
| POST | `/api/books` | Yes | Create book | `{"id":"...","title":"New Book"}` |
| PUT | `/api/books/{id}` | Yes | Update book | `{"id":"...","title":"Updated"}` |
| DELETE | `/api/books/{id}` | Yes | Delete book | `204 No Content` |
| POST | `/api/auth/register` | No | Register user | `{"id":"...","username":"admin","email":"admin@bookcatalog.io"}` |
| POST | `/api/auth/login` | No | Issue JWT | `{"token":"...","expiresAt":"...","userId":"...","username":"admin"}` |
| GET | `/api/me` | Yes | Read current JWT claims | `{"id":"...","username":"admin","email":"admin@bookcatalog.io"}` |

Swagger is enabled in Development at `/swagger` with JWT Bearer authorization support.

## Tests

```powershell
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

## Architecture Decisions

Raw ADO.NET keeps database access explicit and dependency-light. The repositories use `Microsoft.Data.SqlClient` directly, parameterized `SqlCommand` instances, and manual row mapping; Entity Framework, Dapper, and Mediator are intentionally absent.

Public reads with protected writes match a common catalog workflow: anyone can browse and inspect books, while authenticated users manage inventory through the admin view.

Angular Signals and zoneless change detection keep UI state local, predictable, and efficient. The public list uses signal-based filtering and `httpResource`, while protected write flows use typed services over `HttpClient`.

## Implementation Notes

I approached this as a small production slice rather than a demo with everything in one project. The first decision was to make the use case very explicit: public catalog reads, protected admin writes, and a simple JWT login flow. That gave the architecture something concrete to protect, instead of creating layers just for the sake of having layers.

The backend was built test-first around the application behavior. I started with `BookServiceTests` and `UserServiceTests` because those tests describe the actual business workflows: listing books, validating input, preventing duplicate ISBNs, registering users, and rejecting invalid credentials. Once those tests were red, the Application layer was filled in with DTOs and services until the tests went green.

After that, I tightened the domain boundary. The first version had book validation inside `BookService`, which worked, but it made the Application layer responsible for rules that define what a valid book is. I moved intrinsic book rules into the Domain `Book` entity and kept repository-dependent checks, like duplicate ISBN lookup, in `BookService`. That split feels cleaner: Domain protects invariants, Application coordinates use cases.

The API layer was kept intentionally thin. Controllers only translate HTTP requests into service calls and return HTTP results. The global exception middleware handles the common error mapping, while controller tests cover the response shapes that matter for clients.

Infrastructure came last on the backend side. Repositories use raw `Microsoft.Data.SqlClient` with parameterized commands and manual mapping. That is more verbose than EF or Dapper, but the verbosity is useful here because the exercise explicitly asks for no ORM and no hidden SQL generation.

For the frontend, I kept the first screen as the actual book catalog rather than a landing page. The Angular app uses standalone components, signal-based local state, a functional auth interceptor, and a functional guard for admin routes. The UI is deliberately simple: public users browse cards, administrators work from a table and form.

The final verification step was `dotnet test BookCatalog.sln`, which passed with 29 tests. I could not verify the Angular production build on this machine because Node and Angular CLI were not installed locally, but the frontend project is scaffolded for `npm install` followed by `ng build --configuration=production`.
