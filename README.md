# Bloggie

Bloggie is a full-stack blogging platform built with ASP.NET Core MVC. It combines a server-rendered Razor interface with role-based administration, SQL Server persistence through Entity Framework Core, and ImageKit-powered image uploads.

## Features

- Browse published blog posts and explore their tags.
- View post details, including comments and likes.
- Register, sign in, and manage authenticated user interactions.
- Manage blog posts, tags, and user accounts through role-protected admin pages.
- Upload and host post images with ImageKit.

## Architecture

The solution is organized as a single web application with clear MVC and repository boundaries:

```text
Bloggie.sln
└── Bloggie.Web/
    ├── Controllers/     MVC endpoints for public, account, admin, and API workflows
    ├── Data/            Blog and ASP.NET Core Identity EF Core contexts
    ├── Models/
    │   ├── Domain/      Blog posts, tags, comments, and likes
    │   └── ViewModels/  Request and page models
    ├── Repositories/    Data access and ImageKit integration
    ├── Views/           Server-rendered Razor pages
    └── wwwroot/         Static assets
```

`BloggieDbContext` stores blog content and interactions. `AuthDbContext` stores ASP.NET Core Identity users and roles. Controllers use repository interfaces for blog data, tags, comments, likes, users, and image uploads. Both database contexts are configured for SQL Server, and their EF Core migrations are applied during application startup.

## Technology

- C# and .NET 7
- ASP.NET Core MVC and Razor Views
- ASP.NET Core Identity with role-based authorization
- Entity Framework Core 7 and SQL Server
- ImageKit .NET SDK for image uploads
- HTML, CSS, and JavaScript

## Run Locally

### Prerequisites

- .NET 7 SDK
- SQL Server LocalDB or another SQL Server instance
- An ImageKit account for image uploads

### Configure settings

The application expects two SQL Server connection strings and ImageKit credentials. Configure them with .NET User Secrets for local development rather than committing credentials:

```powershell
dotnet user-secrets init --project Bloggie.Web
dotnet user-secrets set "ConnectionStrings:BloggieDbConnectionString" "<blog-database-connection-string>" --project Bloggie.Web
dotnet user-secrets set "ConnectionStrings:BloggieAuthDbConnectionString" "<identity-database-connection-string>" --project Bloggie.Web
dotnet user-secrets set "ImageKit:PublicKey" "<imagekit-public-key>" --project Bloggie.Web
dotnet user-secrets set "ImageKit:PrivateKey" "<imagekit-private-key>" --project Bloggie.Web
dotnet user-secrets set "ImageKit:UrlEndPoint" "<imagekit-url-endpoint>" --project Bloggie.Web
```

The blog and identity connection strings can point to the same SQL Server instance, but the application uses separate databases by default. The configured SQL Server account must be able to create or update the databases because migrations run at startup.

### Build and start

Run these commands from the repository root:

```bash
dotnet restore Bloggie.sln
dotnet build Bloggie.sln
dotnet run --project Bloggie.Web
```

Open the local URL printed by `dotnet run` (the included launch profile uses `https://localhost:7161` and `http://localhost:5161`).

## Security Notes

- Replace and rotate any credentials that have been committed before publishing or deploying this project. Keep connection strings and ImageKit private keys in User Secrets or a deployment secret store.
- The auth database model seeds a SuperAdmin account for development. Replace or remove demo seed credentials and review role assignments before using the application in a shared or production environment.

## Project Status

This repository targets .NET 7. Review the target framework and dependency support lifecycle before choosing it for a new production deployment.