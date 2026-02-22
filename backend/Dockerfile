# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY StoryFirst.Api/*.csproj ./StoryFirst.Api/
RUN dotnet restore "./StoryFirst.Api/StoryFirst.Api.csproj"

# Copy everything else and build
COPY StoryFirst.Api/. ./StoryFirst.Api/
WORKDIR "/src/StoryFirst.Api"
RUN dotnet build "StoryFirst.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "StoryFirst.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoryFirst.Api.dll"]
