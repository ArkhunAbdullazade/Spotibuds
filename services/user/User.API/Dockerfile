# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and all project files
COPY Spotibuds.sln ./
COPY services/identity/Identity.API/Identity.API.csproj ./services/identity/Identity.API/
COPY services/identity/Identity.Domain/Identity.Domain.csproj ./services/identity/Identity.Domain/
COPY services/music/Music.API/Music.API.csproj ./services/music/Music.API/
COPY services/music/Music.Domain/Music.Domain.csproj ./services/music/Music.Domain/
COPY services/user/User.API/User.API.csproj ./services/user/User.API/
COPY services/user/User.Domain/User.Domain.csproj ./services/user/User.Domain/
COPY shared/Shared/Shared.csproj ./shared/Shared/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY services/identity/Identity.API/ ./services/identity/Identity.API/
COPY services/identity/Identity.Domain/ ./services/identity/Identity.Domain/
COPY services/music/Music.API/ ./services/music/Music.API/
COPY services/music/Music.Domain/ ./services/music/Music.Domain/
COPY services/user/User.API/ ./services/user/User.API/
COPY services/user/User.Domain/ ./services/user/User.Domain/
COPY shared/Shared/ ./shared/Shared/

# Build and publish
WORKDIR /app/services/user/User.API
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/services/user/User.API/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "User.API.dll"]