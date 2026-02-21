# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY NotJira.Api/*.csproj ./NotJira.Api/
RUN dotnet restore "./NotJira.Api/NotJira.Api.csproj"

# Copy everything else and build
COPY NotJira.Api/. ./NotJira.Api/
WORKDIR "/src/NotJira.Api"
RUN dotnet build "NotJira.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NotJira.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotJira.Api.dll"]
