#!/bin/bash
# Backend Server Startup Script for C#

set -e  # Exit on any error

# Navigate to the WebApplication directory
echo "Navigating to WebApplication directory..."
cd "$HOME/Complex-IT/Backend/WebApplication" || { echo "Error: Cannot navigate to WebApplication directory"; exit 1; }

# Set environment variables for HTTPS
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_HTTPS_PORT=3000

# Certificate configuration (choose one method below):

# METHOD 1: If using Let's Encrypt certificates
export ASPNETCORE_Kestrel__Certificates__Default__Path="/etc/letsencrypt/live/newtlike.com/fullchain.pem"
export ASPNETCORE_Kestrel__Certificates__Default__KeyPath="/etc/letsencrypt/live/newtlike.com/privkey.pem"

# METHOD 2: If using .pfx certificate file (comment out METHOD 1 and uncomment this)
# export ASPNETCORE_Kestrel__Certificates__Default__Path="/etc/ssl/certs/newtlike.pfx"
# export ASPNETCORE_Kestrel__Certificates__Default__Password="your-certificate-password"

# METHOD 3: If using development certificate (comment out METHOD 1 and uncomment this)
# export ASPNETCORE_Kestrel__Certificates__Default__Path="/home/xilas/.aspnet/https/aspnetapp.pfx"
# export ASPNETCORE_Kestrel__Certificates__Default__Password="your-dev-cert-password"

# Restore dependencies and build project
echo "Restoring .NET dependencies..."
dotnet restore || { echo "Error: dotnet restore failed"; exit 1; }

echo "Building .NET project..."
dotnet build || { echo "Error: Build failed"; exit 1; }

# Run the application with HTTPS
echo "Starting .NET backend server on HTTPS port 3000..."
dotnet run --urls "https://0.0.0.0:3000"

# End of Backend Server Startup Script