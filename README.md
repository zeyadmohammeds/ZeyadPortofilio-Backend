# Portfolio Backend API

An ASP.NET Core Web API for a developer portfolio and administration dashboard, organized with Clean Architecture principles.

> **Security note:** Never publish real credentials, access tokens, connection strings, or production secrets in documentation or source control. Configure secrets through environment variables or a local, untracked configuration file.

## Architecture

- `src/Domain` — core entities, enums, and business models
- `src/Application` — use cases, CQRS/MediatR, validation, DTOs, mapping, and interfaces
- `src/Infrastructure` — Entity Framework Core, persistence, repositories, and external-service implementations
- `src/API` — controllers, middleware, authentication, authorization, Swagger, rate limiting, and health checks

## Capabilities

- JWT authentication with refresh-token support
- Role- and policy-based authorization
- BCrypt password hashing
- Centralized exception handling
- Request/response logging middleware
- Configurable CORS policy
- IP-based rate limiting, including stricter login limits
- DTO-based request handling to reduce over-posting risks

## API overview

The following routes are documented by this project; confirm route availability against the current implementation and Swagger output.

| Area | Method | Route |
|---|---|---|
| Projects | GET | `/api/v1/projects` |
| Project details | GET | `/api/v1/projects/{id}` |
| Education | GET | `/api/v1/education` |
| Contact | POST | `/api/v1/contact` |
| Login | POST | `/api/v1/auth/login` |
| Refresh token | POST | `/api/v1/auth/refresh` |
| Current user | GET | `/api/v1/auth/me` |
| Admin statistics | GET | `/api/v1/admin/stats` |

Admin routes require the appropriate authenticated role or policy.

## Technology

- ASP.NET Core / C#
- Entity Framework Core
- SQL Server
- MediatR and validation
- JWT-based authentication
- Swagger / OpenAPI

## Run locally

Prerequisites: a compatible .NET SDK and a reachable SQL Server instance.

1. Configure `ConnectionStrings:DefaultConnection` in your local configuration.
2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Apply migrations using the repository's configured startup and infrastructure projects:

   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/API
   ```

4. Start the API:

   ```bash
   dotnet run --project src/API
   ```

5. Open the Swagger URL printed by the application.

## Configuration and security

- Do not commit real passwords, signing keys, API keys, or production connection strings.
- Use user secrets or environment variables for local development and your hosting provider's secret manager in deployment.
- Create or reset the initial administrator through a secure setup process; do not rely on credentials stored in a README.
- Restrict CORS origins to trusted frontends before deployment.
- Use HTTPS in production and rotate any secret that may previously have been exposed.

## Frontend integration

Set the frontend's API base URL to the running backend and use the versioned routes above. Follow the application's authentication and refresh-token flow; avoid storing long-lived credentials in insecure client-side storage.

## Project status

This repository is provided as a portfolio project. Review configuration, migrations, and deployment settings for your environment before using it in production.

## Author

**Zeyad Mohammed** — [GitHub](https://github.com/zeyadmohammeds)
