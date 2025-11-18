# Copilot Instructions for NightstormReborn

## Technology Stack
- Backend: .NET 9
- Frontend: React (planned for later development stages)

## Naming Conventions

Follow Microsoft's .NET naming conventions:

- **Classes, Methods, Properties, Events**: PascalCase (e.g., `UserService`, `GetUserById`, `FirstName`)
- **Interfaces**: PascalCase with `I` prefix (e.g., `IUserRepository`, `IEmailService`)
- **Private fields**: camelCase with `_` prefix (e.g., `_userRepository`, `_connectionString`)
- **Local variables and parameters**: camelCase (e.g., `userId`, `userName`)
- **Constants**: PascalCase (e.g., `MaxRetryCount`, `DefaultTimeout`)
- **Async methods**: Suffix with `Async` (e.g., `GetUserAsync`, `SaveChangesAsync`)

## Architecture & Design Patterns

### Clean Architecture
Organize code into the following layers (dependency direction: outward → inward):

- **Domain**: Entities, Value Objects, Domain Events, Interfaces (no dependencies)
- **Application**: Use Cases, DTOs, CQRS (Commands/Queries), Validators, Application Interfaces
- **Infrastructure**: Repositories, External Services, Database Context, Caching
- **Presentation**: API Controllers, Minimal APIs, gRPC services

### SOLID Principles
- **Single Responsibility**: Each class should have one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes must be substitutable for base classes
- **Interface Segregation**: Many specific interfaces over one general interface
- **Dependency Inversion**: Depend on abstractions, not concretions

### Design Patterns (2025 Best Practices)
- **CQRS** with MediatR for separating reads and writes
- **Repository Pattern** for data access abstraction
- **Unit of Work** for transaction management
- **Result Pattern** for error handling (avoid exceptions for flow control)
- **Options Pattern** for configuration
- **Specification Pattern** for complex queries
- **Domain Events** for decoupling business logic

## Async/Await Patterns
- **All I/O operations must be async**: Database calls, HTTP requests, file operations, etc.
- Use `async Task` for methods that return no value, `async Task<T>` for methods that return a value
- Always use `await` for async operations (avoid `.Result` or `.Wait()`)
- Use `ConfigureAwait(false)` in library code, not in ASP.NET Core (it's unnecessary)
- Prefer `ValueTask<T>` for hot paths where allocations matter

## Dependency Injection
- **Use built-in DI container** - Register all services in `Program.cs`
- **Service lifetimes**:
  - `AddSingleton`: Stateless services, shared across application lifetime
  - `AddScoped`: Per-request services (DbContext, repositories)
  - `AddTransient`: Lightweight, stateless services created each time
- **Constructor injection only** - Avoid service locator pattern
- Register interfaces, not concrete types (follow Dependency Inversion)
- Use extension methods for organizing registrations (e.g., `AddApplicationServices()`, `AddInfrastructureServices()`)

## Error Handling & Validation

### Error Handling
- **Use Result Pattern** for business logic errors (e.g., `Result<T>`, `Result<T, TError>`)
- Reserve exceptions for truly exceptional cases (system failures, programming errors)
- Implement global exception handling middleware for unhandled exceptions
- Return appropriate HTTP status codes:
  - `400 Bad Request` for validation errors
  - `404 Not Found` for missing resources
  - `409 Conflict` for business rule violations
  - `500 Internal Server Error` for unexpected errors
- Use **Problem Details (RFC 7807)** for standardized error responses
- Log all exceptions with context (user ID, correlation ID, request path)

### Validation
- **Use FluentValidation** for input validation in Application layer
- Create validators for all Commands and Queries
- Validate at API boundary before processing
- Return detailed validation errors to clients
- Use MediatR pipeline behaviors for automatic validation
- Domain entities should validate their own invariants in constructors/methods

### Authentication & Authorization
- Use **ASP.NET Core Identity** or **IdentityServer/Duende** for authentication
- Implement **JWT tokens** with refresh tokens for API authentication
- Use role-based and policy-based authorization
- Store sensitive configuration in **Azure Key Vault** or **User Secrets** (development)
- Never commit secrets, API keys, or connection strings to source control

### API Security
- **Enable HTTPS only** - Redirect HTTP to HTTPS
- Configure **CORS** properly - whitelist specific origins, avoid `AllowAnyOrigin()`
- Implement **rate limiting** to prevent abuse
- Use **API versioning** for backward compatibility
- Validate and sanitize all user inputs
- Implement **request size limits** to prevent DoS attacks

### Data Protection
- Use **parameterized queries** or EF Core to prevent SQL injection
- Hash passwords with **BCrypt**, **Argon2**, or ASP.NET Core Identity
- Encrypt sensitive data at rest and in transit
- Implement **CSRF protection** for forms
- Use **HttpOnly and Secure flags** for cookies
- Enable **Strict-Transport-Security (HSTS)** header

### Logging & Monitoring
- Never log sensitive data (passwords, tokens, PII)
- Use structured logging with correlation IDs for request tracing
- Implement security event logging (failed login attempts, authorization failures)
- Set up alerts for suspicious activity patterns