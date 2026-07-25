# CareerConnect

CareerConnect is a professional-networking platform (profiles, posts, jobs, real-time chat) built as a set of **.NET 9 / ASP.NET Core** microservices.

---

## Architecture

| Project | Responsibility | Default port |
| :--- | :--- | :--- |
| `CareerConnect.Shared` | Class library shared by every service: JWT auth wiring, current-user claim helpers, global exception handling, the `IFileServiceClient` Refit contract | — |
| `CareerConnect.UsersService` | Auth (register/login/JWT issuing), user profiles, files/resumes, experiences | 8081 |
| `CareerConnect.PostService` | Posts, comments, likes | 8083 |
| `CareerConnect.FileService` | File upload/delete via Cloudinary | 8084 |
| `CareerConnect.CompanyJobService` | Companies, locations, jobs, categories, applications | 8085 |
| `CareerConnect.ChatService` | Chats/messages (REST) + real-time delivery (SignalR) | 8086 |

Services communicate over HTTP using typed Refit clients, with base URLs configured per service (`Services:FileService` in `appsettings.json`). Each service owns its own SQL Server database via EF Core; `FileService` is stateless and delegates storage to Cloudinary.

### Authentication

`CareerConnect.UsersService` is the only token issuer: `POST /auth/register` and `POST /auth/login` hash/verify passwords with BCrypt and return a JWT. Every other service validates that same JWT independently via `CareerConnect.Shared.Auth.AddCareerConnectJwtAuthentication`, using an identical `Jwt:Secret` / `Issuer` / `Audience` configured in each service — no central auth server, just a shared signing secret.

Controllers read the caller's identity through `CareerConnect.Shared.Auth.CurrentUserExtensions`: `HttpContext.GetUserId()`, `GetUserEmail()`, `IsAdmin()`, and `ShouldBeAdmin()` (throws 403 unless the token carries the `ADMIN` role).

---

## Getting started

**Prerequisites:** .NET 9 SDK, SQL Server (LocalDB is fine for local dev), a Cloudinary account (free tier works) for `FileService`.

```bash
dotnet restore
dotnet build
```

### Database setup

Each service with a database needs its own `dotnet ef database update` (install the tool once with `dotnet tool install --global dotnet-ef` if you don't have it):

```bash
dotnet ef database update --project src/CareerConnect.UsersService
dotnet ef database update --project src/CareerConnect.PostService
dotnet ef database update --project src/CareerConnect.CompanyJobService
dotnet ef database update --project src/CareerConnect.ChatService
```

The default connection strings in `appsettings.Development.json` point at `(localdb)\mssqllocaldb`. Change them if you're using a different SQL Server instance.

### Secrets

No secrets are committed anywhere in this repository. `appsettings.json` ships with **empty** placeholders for `Jwt:Secret` (and `Cloudinary:Url` in FileService), and each service refuses to start until a valid secret is configured — a JWT secret of at least 32 characters is enforced at startup.

Configure secrets with `dotnet user-secrets` (stored per-machine outside the repo). The **same** `Jwt:Secret` value must be set for all four JWT-validating services, since tokens issued by UsersService are validated independently by each of them:

```bash
# Pick one strong random value and set it identically for all four services
for p in UsersService PostService CompanyJobService ChatService; do
  dotnet user-secrets set "Jwt:Secret" "<your-32+char-random-secret>" --project src/CareerConnect.$p
done

# Cloudinary credentials for FileService
dotnet user-secrets init --project src/CareerConnect.FileService
dotnet user-secrets set "Cloudinary:Url" "cloudinary://<api_key>:<api_secret>@<cloud_name>" --project src/CareerConnect.FileService
```

In production, supply the same keys as environment variables (e.g. `Jwt__Secret`, `Cloudinary__Url`) instead.

### Running a service

```bash
dotnet run --project src/CareerConnect.UsersService
```

Each service exposes OpenAPI at `/openapi/v1.json` in Development.

---

## API reference

### 🔐 Auth (`CareerConnect.UsersService`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/auth/register` | Register a new user |
| `POST` | `/auth/login` | Authenticate a user and receive a JWT |

### 👤 Users (`CareerConnect.UsersService`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/users` | Get a list of all users (Admin) |
| `GET` | `/api/users/{id}` | Get a specific user by ID |
| `PUT` | `/api/users` | Update current user details |
| `DELETE` | `/api/users/{id}` | Delete a specific user by ID (Admin) |
| `GET` | `/api/users/profile` | Get the profile of the current authenticated user |
| `GET` | `/api/users/profile/{id}` | Get the profile of a specific user by ID |
| `GET` | `/api/users/friends` | Get a list of friends/connections for the current user |
| `POST` | `/api/users/upload` | Upload a user file (e.g., avatar, cover) |
| `GET` | `/api/users/files` | Get all files for the current user |
| `GET` | `/api/users/files/{type}` | Get a specific user file by its type |
| `GET` | `/api/users/resumes` | Get all uploaded resumes for the current user |
| `POST` | `/api/users/experiences` | Create a new experience for the current user |
| `GET` | `/api/users/experiences` | Get all experiences for the current user |
| `GET` | `/api/users/experiences/{id}` | Get a specific experience by its ID |
| `PUT` | `/api/users/experiences/{id}` | Update a specific experience |
| `DELETE` | `/api/users/experiences/{id}` | Delete a specific experience |

### 📝 Posts (`CareerConnect.PostService`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/posts` | Create a new post |
| `POST` | `/api/posts/upload` | Upload a file/media attached to a specific post |
| `GET` | `/api/posts` | Get all posts (feed) |
| `GET` | `/api/posts/{id}` | Get a specific post by ID |
| `DELETE` | `/api/posts/{id}` | Delete a specific post |
| `POST` | `/api/posts/comments` | Add a comment to a post |
| `GET` | `/api/posts/comments/{id}` | Get a specific comment by ID |
| `GET` | `/api/posts/comments/replies/{id}` | Get all replies to a specific comment |
| `GET` | `/api/posts/comments/post/{id}` | Get all comments for a specific post |
| `DELETE` | `/api/posts/comments/{id}` | Delete a specific comment |
| `POST` | `/api/posts/likes` | Like or unlike a post (toggles) |

### 🏢 Company & Jobs (`CareerConnect.CompanyJobService`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/company` | Create a new company page |
| `POST` | `/api/company/upload` | Upload a company file (logo, banner) |
| `GET` | `/api/company` | Get all companies (Admin) |
| `GET` | `/api/company/{id}` | Get a specific company by ID |
| `GET` | `/api/company/detailed/{id}` | Get detailed data for a specific company |
| `PUT` | `/api/company/{id}` | Update company details |
| `DELETE` | `/api/company/{id}` | Delete a specific company |
| `POST` | `/api/company/locations` | Add a location to a company |
| `PUT` | `/api/company/locations/{id}` | Update a specific company location |
| `DELETE` | `/api/company/locations/{id}` | Delete a specific company location |
| `POST` | `/api/jobs` | Create a new job posting |
| `GET` | `/api/jobs` | Get all job postings |
| `GET` | `/api/jobs/{id}` | Get a specific job posting by ID |
| `GET` | `/api/jobs/detailed/{id}` | Get detailed information for a specific job |
| `GET` | `/api/jobs/company/{id}` | Get all jobs posted by a specific company |
| `GET` | `/api/jobs/category/{id}` | Get all jobs under a specific category |
| `GET` | `/api/jobs/sorted/{sortType}` | Get jobs sorted by date (`ascending`/`descending`) |
| `PUT` | `/api/jobs/{id}` | Update a specific job posting |
| `DELETE` | `/api/jobs/{id}` | Delete a specific job posting |
| `POST` | `/api/jobs/applied` | Apply for a job |
| `GET` | `/api/jobs/applied/{id}` | Get all applications for a specific job ID |
| `POST` | `/api/category` | Create a new job category |
| `GET` | `/api/category` | Get all job categories |
| `GET` | `/api/category/{id}` | Get a specific category by ID |
| `PUT` | `/api/category/{id}` | Update a specific category (Admin) |
| `DELETE` | `/api/category/{id}` | Delete a specific category (Admin) |

### 💬 Chat (`CareerConnect.ChatService`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/chats` | Create a new chat session |
| `GET` | `/api/chats` | Get all chats for the current user |
| `GET` | `/api/chats/{id}` | Get a specific chat by ID |
| `DELETE` | `/api/chats/{id}` | Delete a specific chat |
| `POST` | `/api/chats/messages` | Send a new message in a chat |
| `PUT` | `/api/chats/messages/{id}` | Update a specific message |
| `DELETE` | `/api/chats/messages/{id}` | Delete a specific message |

Real-time delivery: connect to the SignalR hub at `/hubs/chat` with the JWT passed as `?access_token=<token>` (SignalR's browser client can't set an `Authorization` header on the websocket handshake). Clients receive `MessageReceived`, `MessageUpdated`, and `MessageDeleted` events for chats they belong to.

### 📁 Files (`CareerConnect.FileService`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/files` | Upload a file directly and return its URL |
| `POST` | `/api/files/batch-delete` | Batch delete multiple files |
| `DELETE` | `/api/files/{id}` | Delete a specific file by its ID |
