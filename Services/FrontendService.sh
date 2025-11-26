#!/bin/bash

# React Server Startup Script

set -e  # Exit on any error

# Navigate to the React application directory
echo "Navigating to React frontend directory..."
cd "$HOME/Complex-IT/react-frontend" || { echo "Error: Cannot navigate to react-frontend directory"; exit 1; }

# Install dependencies
echo "Installing npm dependencies..."
npm install || { echo "Error: npm install failed"; exit 1; }

# Build the application
echo "Building React application..."
npm run build || { echo "Error: Build failed"; exit 1; }

# Start the server
echo "Starting React server on port 3000..."
serve build -p 3000 -s

# End of React Server Startup Script