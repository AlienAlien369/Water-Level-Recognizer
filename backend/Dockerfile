# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies (layer caching)
COPY ["src/WLR.API/WLR.API.csproj", "src/WLR.API/"]
COPY ["src/WLR.Application/WLR.Application.csproj", "src/WLR.Application/"]
COPY ["src/WLR.Infrastructure/WLR.Infrastructure.csproj", "src/WLR.Infrastructure/"]
COPY ["src/WLR.Domain/WLR.Domain.csproj", "src/WLR.Domain/"]

RUN dotnet restore "src/WLR.API/WLR.API.csproj"

# Copy all source code
COPY . .

WORKDIR "/src/src/WLR.API"
RUN dotnet build "WLR.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "WLR.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Security: non-root user
RUN addgroup --system wlrgroup && adduser --system --ingroup wlrgroup wlruser

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
RUN mkdir -p /app/logs && chown -R wlruser:wlrgroup /app

USER wlruser
EXPOSE 8080
EXPOSE 8081

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WLR.API.dll"]
