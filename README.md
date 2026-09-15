# Task Management API

A production-ready RESTful API developed with **ASP.NET Core (.NET 10)**, **Entity Framework Core**, and **SQL Server**. The platform delivers comprehensive project tracking, scoped task management, and comment moderation enforced by strict Role-Based Access Control (RBAC), standardized RFC 7807 error handling, and automated CI/CD workflows.

---

## Core Features

* **Authentication & Role-Based Access Control (RBAC):**
  * JWT Bearer token authentication with secure password hashing.
  * Three permission tiers: `Admin`, `ProjectManager`, and `Member`.
* **Resource Scoping & Access Restrictions:**
  * Multi-tenant project memberships ensuring users only query or mutate tasks within authorized projects.
  * Granular comment moderation permissions restricted to authors, project managers, and administrators.
* **Advanced Querying & Pagination:**
  * Case-insensitive keyword search matching task titles and descriptions.
  * Filtering by status (`Todo`, `InProgress`, `Completed`, `Cancelled`) and priority (`Low`, `Medium`, `High`, `Critical`).
  * Dynamic sorting across multiple entity properties.
  * Standard offset-based pagination returning complete navigation metadata (`PageNumber`, `PageSize`, `TotalCount`, `TotalPages`).
* **Standardized Global Error Pipeline:**
  * Native .NET `IExceptionHandler` implementation mapping domain exceptions to RFC 7807 `ProblemDetails` responses.
  * Strict custom exception hierarchy (`NotFoundException`, `ConflictException`, `BadRequestException`).
* **Automated CI/CD & Testing:**
  * Comprehensive test suite utilizing `xUnit v3`, `FluentAssertions`, and `EF Core InMemory`.
  * GitHub Actions automated CI workflow executing on every push and pull request.

---

## Tech Stack

* **Framework:** ASP.NET Core Web API (.NET 10)
* **Data Access:** Entity Framework Core
* **Database:** Microsoft SQL Server
* **Testing:** xUnit v3, FluentAssertions, Moq, EF Core InMemory Provider
* **CI/CD:** GitHub Actions
* **API Documentation:** Swagger / OpenAPI

---

## Project Structure

```text
TaskManagementAPI/
├── Controllers/              # API endpoints (Auth, Projects, Tasks, Comments)
├── IServices/                # Service layer contracts
├── Services/                 # Core domain implementations and business logic
├── Models/                   # EF Core domain entities and enums
├── DTOs/                     # Strongly-typed request/response records
├── Exceptions/               # Custom domain exceptions and GlobalExceptionHandler
├── Data/                     # AppDbContext and Fluent API entity configs
└── Program.cs                # Dependency injection and middleware pipeline

TaskManagementAPI.Tests/
├── ProjectServiceTests.cs    # Unit tests for project authorization and memberships
├── TaskManagementTests.cs    # Domain model state and exception contract tests
└── TaskManagementAPI.Tests.csproj
