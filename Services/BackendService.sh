#!/bin/bash
# Backend Server Startup Script for C#

set -e  # Exit on any error

# Navigate to the WebApplication directory
echo "Navigating to WebApplication directory..."
cd "$HOME/Complex-IT/Backend/WebApplication" || { echo "Error: Cannot navigate to WebApplication directory"; exit 1; }

# Restore dependencies and build project
echo "Restoring .NET dependencies..."
dotnet restore || { echo "Error: dotnet restore failed"; exit 1; }

echo "Building .NET project..."
dotnet build || { echo "Error: Build failed"; exit 1; }

# Run the application
echo "Starting .NET backend server on port 5000..."
dotnet run --urls "http://localhost:5000"

# End of Backend Server Startup Script