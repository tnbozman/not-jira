# Use Microsoft's devcontainer base image with both Node.js and .NET
FROM mcr.microsoft.com/devcontainers/dotnet:10.0

# Install Node.js 20.x
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm@latest

# Install PostgreSQL client tools and Git
RUN apt-get update && apt-get install -y postgresql-client git

# Install global npm packages for development
RUN npm install -g @vue/cli vite

# Set up user
ARG USERNAME=vscode
USER $USERNAME

WORKDIR /workspace
