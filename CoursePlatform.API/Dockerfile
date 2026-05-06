# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (for caching restore)
COPY ["CoursePlatform.API/CoursePlatform.API.csproj", "CoursePlatform.API/"]
COPY ["CoursePlatform.Application/CoursePlatform.Application.csproj", "CoursePlatform.Application/"]
COPY ["CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj", "CoursePlatform.Infrastructure/"]
COPY ["CoursePlatform.Domain/CoursePlatform.Domain.csproj", "CoursePlatform.Domain/"]

# Restore dependencies
RUN dotnet restore "CoursePlatform.API/CoursePlatform.API.csproj"

# Copy full source
COPY . .

# Build project
WORKDIR /src/CoursePlatform.API
RUN dotnet build "CoursePlatform.API.csproj" -c Release -o /app/build --no-restore

# Publish
FROM build AS publish
RUN dotnet publish "CoursePlatform.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Final runtime image
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Ensure uploads folder exists
RUN mkdir -p /app/wwwroot/uploads

# Render will inject PORT env variable
ENTRYPOINT ["dotnet", "CoursePlatform.API.dll"]