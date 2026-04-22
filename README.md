# Portfolio Backend (Clean Architecture)

Production-grade ASP.NET Core Web API for a developer portfolio and admin dashboard.

## Architecture

- `src/Domain`: entities, enums, core business models
- `src/Application`: CQRS with MediatR, validators, DTOs, mapping, interfaces
- `src/Infrastructure`: EF Core, repositories, unit of work, JWT/password/file/email services, seeding
- `src/API`: controllers, middleware, auth/authorization, versioning, Swagger, rate limit, health checks

## Security Features

- JWT access tokens + refresh token persistence
- Role and policy authorization (`AdminOnly`)
- BCrypt password hashing
- Global exception handling with standardized error response
- Request/response logging middleware
- CORS policy for frontend origins
- IP rate limiting (general + stricter login endpoint)
- Over-posting prevention through command DTOs

## Core Endpoints (v1)

- Public
  - `GET /api/v1/projects`
  - `GET /api/v1/projects/{id}`
  - `GET /api/v1/education`
  - `POST /api/v1/contact`
- Auth
  - `POST /api/v1/auth/login`
  - `POST /api/v1/auth/refresh`
  - `GET /api/v1/auth/me`
- Admin (JWT Admin role required)
  - `POST /api/v1/admin/projects`
  - `DELETE /api/v1/admin/projects/{id}`
  - `POST /api/v1/admin/projects/{id}/image`
  - `POST /api/v1/admin/education`
  - `DELETE /api/v1/admin/education/{id}`
  - `GET /api/v1/admin/stats`

## Database

- Provider: SQL Server (LocalDB by default, configurable via connection string)
- EF Core migrations included in `src/Infrastructure/Persistence/Migrations`
- Seeded admin user:
  - Email: `zeyad.shosha@outlook.com`
  - Password: `admin1234`

## Run Locally

1. Set SQL Server connection in `src/API/appsettings.json` (`ConnectionStrings:DefaultConnection`).
2. From `Backend` run:
   - `dotnet restore`
   - `dotnet ef database update --project src/Infrastructure --startup-project src/API`
   - `dotnet run --project src/API`
3. Open Swagger at:
   - `https://localhost:5001/swagger` or assigned launch URL.

## Notes for Frontend Integration

- Replace mock calls with the backend base URL and versioned routes:
  - `http://localhost:{port}/api/v1/...`
- Keep JWT in secure client storage strategy (prefer memory + refresh flow; avoid long-lived local storage in production).
