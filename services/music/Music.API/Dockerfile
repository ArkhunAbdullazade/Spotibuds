# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY Spotibuds.sln ./
COPY services/music/Music.API/Music.API.csproj ./services/music/Music.API/
COPY services/music/Music.Domain/Music.Domain.csproj ./services/music/Music.Domain/
COPY shared/Shared/Shared.csproj ./shared/Shared/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY services/music/Music.API/ ./services/music/Music.API/
COPY services/music/Music.Domain/ ./services/music/Music.Domain/
COPY shared/Shared/ ./shared/Shared/

# Build and publish
WORKDIR /app/services/music/Music.API
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/services/music/Music.API/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Music.API.dll"]