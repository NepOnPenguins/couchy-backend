# Couchy backend (.NET)

ASP.NET Core Web API port of the Spring Boot `couchy-backend`. It keeps the same
URLs, JSON shapes, database tables and JWT format, so the frontend and an
existing database work with either backend.

- .NET 8, ASP.NET Core controllers
- Entity Framework Core + PostgreSQL (Npgsql)
- JWT bearer authentication (HS256), BCrypt password hashes
- Swagger UI in development

## Run it

Requirements: the .NET SDK (8 or newer) and PostgreSQL.

**Option A: database in Docker, API from the SDK**

```
docker compose up -d postgres
dotnet run --project Couchy.Api
```

**Option B: everything in Docker**

```
docker compose up --build
```

The API listens on http://localhost:8080 and Swagger UI is at
http://localhost:8080/swagger. In Swagger, call `/api/v1/auth/login`, press
**Authorize** and paste the token to try the protected endpoints.

Settings live in `Couchy.Api/appsettings.json`: the connection string, the JWT
secret and lifetime, and the allowed frontend origin (`http://localhost:5173`).

The tables are created on first start if the database is empty.

## Endpoints

| Method | Path | Auth | What it does |
|---|---|---|---|
| POST | `/api/v1/auth/register` | no | Create an account, returns `{ token }` |
| POST | `/api/v1/auth/login` | no | Log in, returns `{ token }` |
| POST | `/api/v1/profiles/me/create` | yes | Create my profile |
| GET | `/api/v1/profiles/me` | yes | My profile |
| GET | `/api/v1/profiles/search?city=&hostingStatus=` | no | List profiles, optional filters |
| GET | `/api/v1/profiles/{id}` | no | One profile |
| PATCH | `/api/v1/profiles/edit/about` | yes | Edit name, about, gender, pronouns, languages |
| PATCH | `/api/v1/profiles/edit/home` | yes | Edit location, home details, hosting and meeting status |
| GET | `/api/v1/messages/surfing` | yes | Stay requests I sent |
| GET | `/api/v1/messages/hosting` | yes | Stay requests I received |
| POST | `/api/v1/messages` | yes | Send a stay request to a host |
| POST | `/api/v1/messages/{id}/accept` | yes | Host accepts a pending request |
| POST | `/api/v1/messages/{id}/reject` | yes | Host rejects a pending request |
| GET | `/api/v1/users` | yes | List users |
| POST | `/api/v1/users` | yes | Add a user |
| PUT | `/api/v1/users/{id}?username=&email=&password=` | yes | Update a user |
| DELETE | `/api/v1/users/{id}` | yes | Soft-delete a user |

"Auth: yes" means the request needs an `Authorization: Bearer <token>` header.

## Where the Spring code went

| Spring Boot | .NET |
|---|---|
| `controller/*` | `Controllers/*` |
| `service/*` | `Services/*` |
| `repository/*` (Spring Data JPA) | `Data/CouchyDbContext.cs` (EF Core `DbSet`s and LINQ) |
| `entity/*`, `@Embeddable` | `Entities/*`, EF Core owned types |
| `enums/*` | `Enums/*` |
| `dto/request`, `dto/response` | `Dtos/*` |
| `mapper/*` | `Mappers/*` (extension methods) |
| `SecurityConfig`, `JWTAuthFilter`, `AppConfig` | JWT bearer setup in `Program.cs`, `[Authorize]` / `[AllowAnonymous]` |
| `JwtService` | `Services/JwtService.cs`, `Auth/JwtSettings.cs` |
| `BCryptPasswordEncoder` | `Auth/PasswordHasher.cs` |
| `@EnableJpaAuditing` | `SaveChanges` override in `CouchyDbContext` |
| CORS bean in `CouchyBackendApplication` | `AddCors` in `Program.cs` |
| `application.properties` | `appsettings.json` |
| `ProfileNotFoundException` | `Common/ApiException.cs` + `Middleware/ExceptionMiddleware.cs` |

## Differences from the Spring version

- **Error responses.** Spring answered most business errors with 500 or 403.
  Here they are 400, 401, 403, 404 or 409 with a `{ "message": "..." }` body.
  A wrong password is 401, a missing profile is 404.
- **Users endpoints never return the password hash**, and passwords sent to
  `POST` / `PUT /api/v1/users` are hashed before they are stored.
- **Register rejects duplicates** with 409 instead of failing in the database.
- **Required fields.** Creating a profile needs first name, last name and about;
  a stay request needs a host id and a message. The validation annotations in
  the Spring DTOs were never active (no `@Valid`), so the "50 characters" rule on
  `about` is not enforced here either.
- `StayRequestDTO` was unused and was not ported.

## Open decisions

- **Token lifetime is 24 minutes**, as in the Spring code (`1000 * 60 * 24` ms).
  If 24 hours was intended, set `Jwt:ExpirationMinutes` to `1440`.
- **`/api/v1/users` is open to every logged-in user**, including updating and
  deleting other users. That is how the Spring version behaved; consider
  restricting it to `ROLE_ADMIN` or to the user's own account.
- **The JWT secret is in `appsettings.json`** (it was hardcoded in `JwtService.java`).
  Fine for a school project, but do not publish it in a real deployment.
- **Deleted users can still log in** and still appear in search, as before.
- **Schema changes.** `EnsureCreated` only creates tables in an empty database.
  If you change the entities later, use EF Core migrations
  (`dotnet ef migrations add ...`).
