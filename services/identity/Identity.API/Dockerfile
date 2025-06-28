# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY Spotibuds.sln ./
COPY services/identity/Identity.API/Identity.API.csproj ./services/identity/Identity.API/
COPY services/identity/Identity.Domain/Identity.Domain.csproj ./services/identity/Identity.Domain/
COPY shared/Shared/Shared.csproj ./shared/Shared/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY services/identity/Identity.API/ ./services/identity/Identity.API/
COPY services/identity/Identity.Domain/ ./services/identity/Identity.Domain/
COPY shared/Shared/ ./shared/Shared/

# Build and publish
WORKDIR /app/services/identity/Identity.API
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/services/identity/Identity.API/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Identity.API.dll"]