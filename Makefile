.PHONY: dev prod build test clean

# Development
dev:
docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build

dev-down:
docker compose -f docker-compose.yml -f docker-compose.dev.yml down

# Production
prod:
docker compose up -d --build

prod-down:
docker compose down

# Build
build-backend:
cd backend && dotnet build -c Release

build-frontend:
cd frontend && npm run build

# Test
test-backend:
cd backend && dotnet test --verbosity normal

test-frontend:
cd frontend && npm test

# Database
migrate:
cd backend && dotnet ef database update --project src/WLR.Infrastructure --startup-project src/WLR.API

migration-add:
cd backend && dotnet ef migrations add $(name) --project src/WLR.Infrastructure --startup-project src/WLR.API

# Clean
clean:
cd backend && dotnet clean
cd frontend && rm -rf dist node_modules
docker compose down -v

# Logs
logs:
docker compose logs -f

logs-backend:
docker compose logs -f backend

logs-frontend:
docker compose logs -f frontend
