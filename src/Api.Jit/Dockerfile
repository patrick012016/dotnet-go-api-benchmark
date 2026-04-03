# --- STAGE 1: BUILD ---
# Get build tools (SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env

# Set working directory
WORKDIR /app

# Copy project file (for caching)
COPY ["*.csproj", "./"]

# Restore NuGet packages
RUN dotnet restore

# Copy remaining source code
COPY [".", "./"]

# Build production-ready app (Release), UseAppHost=false only in JIT version
RUN dotnet publish -c Release -o /app/out --no-restore /p:UseAppHost=false


# --- STAGE 2: RUNTIME ---

# Use lightweight, secure runtime (no SDK) without shell (includes globalization/ICU dependencies)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra

# Set working directory
WORKDIR /app

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy built files from Stage 1
COPY --from=build-env ["/app/out", "./"]

# Run as non-root user (security)
USER $APP_UID

# Port the app listens on
EXPOSE 8080

# Launch the application
ENTRYPOINT ["dotnet", "Api.Jit.dll"]
