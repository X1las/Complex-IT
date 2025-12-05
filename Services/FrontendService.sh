#!/bin/bash

# React Server Startup Script

set -e  # Exit on any error

# Navigate to the React application directory
echo "Navigating to React frontend directory..."
cd "/home/xilas/Complex-IT/Frontend" || { echo "Error: Cannot navigate to Frontend directory"; exit 1; }

# Install dependencies
echo "Installing npm dependencies..."
npm install || { echo "Error: npm install failed"; exit 1; }

# Build the application using npx to ensure vite is found
echo "Building React application..."
npx vite build || { echo "Error: Build failed"; exit 1; }

# Start the server with HTTPS
echo "Starting React server on HTTPS port 443..."
sudo serve -s build -l 443 \
  --ssl-cert /etc/letsencrypt/live/www.newtlike.com/fullchain.pem \
  --ssl-key /etc/letsencrypt/live/www.newtlike.com/privkey.pem

