# WaterLevelRecognizer

**Enterprise-grade SaaS platform for water motor management across multiple centers and locations.**

[![CI/CD](https://github.com/AlienAlien369/Water-Level-Recognizer/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/AlienAlien369/Water-Level-Recognizer/actions/workflows/ci-cd.yml)

---

## Architecture

```
WaterLevelRecognizer/
+-- backend/          # ASP.NET Core 9 -- Clean Architecture + DDD + CQRS
|   +-- src/
|   |   +-- WLR.Domain/         # Entities, Value Objects, Domain Events
|   |   +-- WLR.Application/    # CQRS Handlers, Validators, DTOs
|   |   +-- WLR.Infrastructure/ # EF Core, PostgreSQL, Redis, SignalR
|   |   +-- WLR.API/            # Controllers, Middleware, Swagger
|   +-- tests/
+-- frontend/         # React 18 + TypeScript + Vite + Tailwind + Shadcn
+-- docker/           # NGINX + PostgreSQL configs
+-- .github/          # CI/CD Workflows
```

## Quick Start

### Prerequisites
- Docker Desktop
- Git

### 1. Clone & Configure
```powershell
git clone https://github.com/AlienAlien369/Water-Level-Recognizer.git
cd WaterLevelRecognizer
Copy-Item .env.example .env
# Edit .env with your configuration
```

### 2. Run with Docker Compose (Development)
```powershell
docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

**Access points:**
| Service    | URL                          |
|------------|------------------------------|
| Frontend   | http://localhost:5173         |
| Backend API| http://localhost:8080         |
| Swagger    | http://localhost:8080/swagger |
| Hangfire   | http://localhost:8080/hangfire|

### 3. Get OTP (Development)

OTP is printed to the backend log. Use PowerShell (Windows) or grep (Linux/Mac):

```powershell
# Windows PowerShell
docker logs wlr-backend 2>&1 | Select-String "OTP for"

# Linux / Mac
docker logs wlr-backend 2>&1 | grep "OTP for"
```

## Deployment

### Frontend → Vercel
Live at: **https://water-level-recognizer.vercel.app**

Vercel auto-deploys from the `main` branch. Set these in the Vercel dashboard under **Project Settings → Environment Variables**:
| Variable | Value |
|---|---|
| `VITE_API_URL` | Your Render backend URL (e.g. `https://wlr-backend.onrender.com`) |
| `VITE_SIGNALR_URL` | `https://wlr-backend.onrender.com/hubs` |

### Backend → Render (Docker)
1. Go to [render.com](https://render.com) → **New → Blueprint**
2. Connect the GitHub repo `AlienAlien369/Water-Level-Recognizer`
3. Render will detect `render.yaml` and provision:
   - `wlr-backend` — ASP.NET Core 9 Docker web service
   - `wlr-postgres` — PostgreSQL 16 database
   - `wlr-redis` — Redis cache
4. Set `ALLOWED_ORIGINS` env var in Render to your Vercel URL after first deploy

### GitHub Actions Secrets Required
Add these secrets in **GitHub → Settings → Secrets → Actions**:

| Secret | Description |
|---|---|
| `VERCEL_TOKEN` | Your Vercel API token |
| `RENDER_DEPLOY_HOOK_URL` | Render deploy hook URL (from service Settings) |

---



## Default Credentials

| Role        | Mobile        | OTP           |
|-------------|---------------|---------------|
| Super Admin | +919999999999 | (Console log) |

## Role Hierarchy

| Role              | Capabilities                                              |
|-------------------|-----------------------------------------------------------|
| **Super Admin**   | Full platform access, manage centers, promote users       |
| **Admin**         | Manage locations, motors, assign Sewadars in their center |
| **Sewadar (User)**| View & operate assigned motors only                       |

## Real-time Features
- SignalR WebSocket for live motor status
- Auto-refresh dashboard (every 30s)
- Real-time notifications

## Authentication Flow
1. Enter mobile number -> Receive OTP (logged to console in dev)
2. Enter OTP -> Get JWT access token + refresh token
3. Access token (1 hour) auto-refreshed using refresh token (30 days)

## Tech Stack

**Backend:** ASP.NET Core 9, C#, EF Core, PostgreSQL, Redis, SignalR, MediatR, FluentValidation, Serilog, Hangfire

**Frontend:** React 18, TypeScript, Vite, Tailwind CSS, Shadcn UI, TanStack Query, Zustand, Recharts

**DevOps:** Docker, NGINX, GitHub Actions CI/CD

## API Documentation
Available at `/swagger` when running. Versioned at `/api/v1/`.

## Running Tests
```powershell
# Backend (PowerShell)
Set-Location backend
dotnet test

# Frontend (PowerShell)
Set-Location frontend
npm test
```

## Database Migrations
```powershell
# Apply migrations (PowerShell)
Set-Location backend
dotnet ef database update --project src/WLR.Infrastructure --startup-project src/WLR.API

# Or inside Docker
docker exec wlr-backend sh -c "export PATH=`$PATH:/root/.dotnet/tools && cd /src && dotnet-ef migrations add <MigrationName> --project src/WLR.Infrastructure --startup-project src/WLR.API --output-dir Persistence/Migrations"
```
