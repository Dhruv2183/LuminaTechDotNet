# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (for better Docker layer caching)
COPY LuminaTech.sln .
COPY LuminaTech.API/LuminaTech.API.csproj LuminaTech.API/
COPY LuminaTech.Data/LuminaTech.Data.csproj LuminaTech.Data/
COPY LuminaTech.Services/LuminaTech.Services.csproj LuminaTech.Services/
COPY LuminaTech.Tests/LuminaTech.Tests.csproj LuminaTech.Tests/

# Restore dependencies
RUN dotnet restore LuminaTech.sln

# Copy everything else and publish
COPY . .
RUN dotnet publish LuminaTech.API/LuminaTech.API.csproj -c Release -o /app/publish

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render uses port 10000 by default
EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "LuminaTech.API.dll"]
