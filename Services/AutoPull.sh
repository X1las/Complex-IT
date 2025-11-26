#!/bin/bash
# Automatic GitHub Pull Script
# Automatically pulls on changes to the repo and restarts server services

set -e  # Exit on any error

# Navigate to project directory
cd "$HOME/Complex-IT" || { echo "Error: Cannot navigate to project directory"; exit 1; }

# Get current branch name
CURRENT_BRANCH=$(git branch --show-current)
echo "Working on branch: $CURRENT_BRANCH"

# Fetch latest changes
echo "Fetching latest changes..."
git fetch origin "$CURRENT_BRANCH" || { echo "Error: Failed to fetch from origin"; exit 1; }

LOCAL=$(git rev-parse HEAD)
REMOTE=$(git rev-parse "origin/$CURRENT_BRANCH")

if [ "$LOCAL" != "$REMOTE" ]; then
    echo "Changes detected. Pulling latest changes..."
    git pull origin "$CURRENT_BRANCH" || { echo "Error: Failed to pull changes"; exit 1; }

    echo "Restarting Frontend Service..."
    pkill -f Services/FrontendService.sh || true  # Don't fail if process not found
    sleep 2
    bash "$HOME/Complex-IT/Services/FrontendService.sh" &
    FRONTEND_PID=$!
    echo "Frontend service started with PID: $FRONTEND_PID"

    echo "Restarting Backend Service..."
    pkill -f Services/BackendService.sh || true  # Don't fail if process not found
    sleep 2
    bash "$HOME/Complex-IT/Services/BackendService.sh" &
    BACKEND_PID=$!
    echo "Backend service started with PID: $BACKEND_PID"

    echo "Services restarted successfully."
else
    echo "No changes detected. No action taken."
fi

# End of Automatic GitHub Pull Script