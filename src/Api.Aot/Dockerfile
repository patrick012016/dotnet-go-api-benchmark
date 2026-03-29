# --- STAGE 1: BUILD ---
# Get build tools (SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env

# Installing system dependencies for Native AOT (clang and zlib, C++ compiler and linker tools)
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       clang zlib1g-dev

# Set working directory
WORKDIR /app

# Copy project file (for caching)
COPY *.csproj ./

# Restore NuGet packages
RUN dotnet restore

# Copy remaining source code
COPY . ./

# Build production-ready app (Release)
RUN dotnet publish -c Release -o /app/out --no-restore


# --- STAGE 2: RUNTIME ---

# Use OS dependencies only (NO .NET runtime, NO SDK) for self-contained/AOT apps
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0

# Set working directory
WORKDIR /app

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy built files from Stage 1

COPY --from=build-env /app/out .

# Run as non-root user (security)
USER $APP_UID

# Port the app listens on
EXPOSE 8080

# Launch the application
ENTRYPOINT ["./Api.Aot"]
