#!/bin/bash
set -e

echo "🚀 Setting up development environment..."

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[0;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_step() {
    echo -e "${BLUE}➜${NC} $1"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_info() {
    echo -e "${YELLOW}ℹ${NC} $1"
}

# Configure git safe directory (needed for devcontainers)
print_step "Configuring git for devcontainer..."
git config --global --add safe.directory /workspace
print_success "Git configured"

# Check if .NET Aspire workload is installed
print_step "Checking .NET Aspire workload..."
if dotnet workload list | grep -q aspire; then
    print_success ".NET Aspire workload is installed"
else
    print_info ".NET Aspire workload not found. You may need to install it with: dotnet workload install aspire"
fi

# Restore packages if solution exists
if [ -f "GestaoFaturas.sln" ]; then
    print_step "Restoring NuGet packages..."
    dotnet restore
    print_success "NuGet packages restored"
else
    print_info "No solution found. Run 'dotnet new sln' to create one after container starts"
fi


echo ""
echo "========================================="
echo -e "${GREEN}✨ Development environment ready!${NC}"
echo "========================================="
echo ""
echo "Container tools available:"
echo "  • .NET 8.0 SDK"
echo "  • Entity Framework Core CLI (dotnet ef)"
echo "  • .NET Aspire workload"
echo "  • PostgreSQL client tools (psql)"
echo "  • Node.js 20 & npm"
echo ""
print_info "Start developing! The workspace is mounted at /workspace"
