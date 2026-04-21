FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY ["CoursePlatform.API/CoursePlatform.API.csproj", "CoursePlatform.API/"]
COPY ["CoursePlatform.Application/CoursePlatform.Application.csproj", "CoursePlatform.Application/"]
COPY ["CoursePlatform.Domain/CoursePlatform.Domain.csproj", "CoursePlatform.Domain/"]
COPY ["CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj", "CoursePlatform.Infrastructure/"]

RUN dotnet restore

COPY . .

WORKDIR "/src/CoursePlatform.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "CoursePlatform.API.dll"]