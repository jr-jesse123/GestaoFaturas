# Use the official .NET 8 devcontainer image as base
FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm

# Install additional tools and dependencies
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
    # PostgreSQL client tools
    postgresql-client \
    # Build essentials
    make \
    # Network tools
    curl \
    wget \
    netcat-openbsd \
    # Process management
    htop \
    # JSON processing
    jq \
    # Clean up
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Install .NET tools globally
RUN dotnet tool install --global dotnet-ef --version 8.* \
    && dotnet tool install --global dotnet-outdated-tool \
    && dotnet tool install --global dotnet-format

# Install .NET Aspire workload
RUN dotnet workload update \
    && dotnet workload install aspire

# Add .NET tools to PATH
ENV PATH="$PATH:/home/vscode/.dotnet/tools:/usr/local/bin:/home/vscode/.npm-global/bin"

# Install Node.js for frontend tooling (npm, etc)
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm@latest

# Install Claude Code
RUN npm install -g @anthropic-ai/claude-code
RUN claude mcp add playwright npx @playwright/mcp@latest

# Create workspace directory
WORKDIR /workspace

# Pre-cache common .NET packages to speed up restore (as root)
RUN dotnet new webapi -n temp && cd temp && dotnet restore && cd .. && rm -rf temp

# Set up non-root user environment
USER vscode

# Configure git for the vscode user
RUN git config --global core.autocrlf false \
    && git config --global core.eol lf \
    && git config --global init.defaultBranch main \
    && git config --global user.email "jesse.junior@gmail.com" \
    && git config --global user.name "Jessé Junior"

# Switch back to dialog for interactive use
ENV DEBIAN_FRONTEND=dialog
