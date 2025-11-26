#!/bin/bash
# Service Uninstallation Script
# Removes AutoPull, BackendService, and FrontendService systemd services

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Project paths
PROJECT_DIR="$HOME/Complex-IT"
SERVICES_DIR="$PROJECT_DIR/Services"

echo -e "${RED}Complex-IT Services Uninstallation Script${NC}"
echo "============================================="

# Determine service directory and systemctl command
if [[ $EUID -eq 0 ]]; then
    SERVICE_DIR="/etc/systemd/system"
    SYSTEMCTL_CMD="systemctl"
    echo -e "${YELLOW}Running as root - removing system-wide services${NC}"
else
    SERVICE_DIR="$HOME/.config/systemd/user"
    SYSTEMCTL_CMD="systemctl --user"
    echo -e "${YELLOW}Running as user - removing user services${NC}"
fi

# Service names
SERVICES=("complex-it-backend" "complex-it-frontend" "complex-it-autopull")

echo -e "\n${YELLOW}Stopping and disabling services...${NC}"

# Stop and disable services
for service in "${SERVICES[@]}"; do
    echo "Processing $service..."
    
    # Stop the service if it's running
    if $SYSTEMCTL_CMD is-active --quiet "$service.service" 2>/dev/null; then
        echo "  Stopping $service.service..."
        $SYSTEMCTL_CMD stop "$service.service" || echo "  Warning: Failed to stop $service.service"
    else
        echo "  $service.service is not running"
    fi
    
    # Disable the service if it's enabled
    if $SYSTEMCTL_CMD is-enabled --quiet "$service.service" 2>/dev/null; then
        echo "  Disabling $service.service..."
        $SYSTEMCTL_CMD disable "$service.service" || echo "  Warning: Failed to disable $service.service"
    else
        echo "  $service.service is not enabled"
    fi
done

# Handle AutoPull timer separately
echo "Processing complex-it-autopull timer..."
if $SYSTEMCTL_CMD is-active --quiet "complex-it-autopull.timer" 2>/dev/null; then
    echo "  Stopping complex-it-autopull.timer..."
    $SYSTEMCTL_CMD stop "complex-it-autopull.timer" || echo "  Warning: Failed to stop timer"
fi

if $SYSTEMCTL_CMD is-enabled --quiet "complex-it-autopull.timer" 2>/dev/null; then
    echo "  Disabling complex-it-autopull.timer..."
    $SYSTEMCTL_CMD disable "complex-it-autopull.timer" || echo "  Warning: Failed to disable timer"
fi

echo -e "${GREEN}✓${NC} Services stopped and disabled"

# Remove service files
echo -e "\n${YELLOW}Removing service files...${NC}"

for service in "${SERVICES[@]}"; do
    service_file="$SERVICE_DIR/$service.service"
    if [[ -f "$service_file" ]]; then
        echo "  Removing $service_file..."
        rm -f "$service_file"
        echo -e "  ${GREEN}✓${NC} Removed $service.service"
    else
        echo "  $service.service file not found"
    fi
done

# Remove timer file
timer_file="$SERVICE_DIR/complex-it-autopull.timer"
if [[ -f "$timer_file" ]]; then
    echo "  Removing $timer_file..."
    rm -f "$timer_file"
    echo -e "  ${GREEN}✓${NC} Removed autopull timer"
else
    echo "  AutoPull timer file not found"
fi

# Remove management script
mgmt_script="$SERVICES_DIR/manage-services.sh"
if [[ -f "$mgmt_script" ]]; then
    echo "  Removing management script..."
    rm -f "$mgmt_script"
    echo -e "  ${GREEN}✓${NC} Removed management script"
else
    echo "  Management script not found"
fi

# Reload systemd
echo -e "\n${YELLOW}Reloading systemd configuration...${NC}"
$SYSTEMCTL_CMD daemon-reload
echo -e "${GREEN}✓${NC} Systemd configuration reloaded"

# Kill any remaining processes
echo -e "\n${YELLOW}Cleaning up any remaining processes...${NC}"

# Kill processes by script name
for script in "AutoPull.sh" "BackendService.sh" "FrontendService.sh"; do
    if pgrep -f "$script" > /dev/null; then
        echo "  Terminating $script processes..."
        pkill -f "$script" || echo "  Warning: Failed to kill $script processes"
    else
        echo "  No $script processes found"
    fi
done

# Kill dotnet processes from WebApplication
if pgrep -f "WebApplication.*dotnet" > /dev/null; then
    echo "  Terminating WebApplication dotnet processes..."
    pkill -f "WebApplication.*dotnet" || echo "  Warning: Failed to kill WebApplication processes"
fi

# Kill serve processes for React frontend
if pgrep -f "serve.*build" > /dev/null; then
    echo "  Terminating React serve processes..."
    pkill -f "serve.*build" || echo "  Warning: Failed to kill serve processes"
fi

# Kill any Vite dev server processes
if pgrep -f "vite.*dev" > /dev/null; then
    echo "  Terminating Vite dev server processes..."
    pkill -f "vite.*dev" || echo "  Warning: Failed to kill Vite processes"
fi

echo -e "${GREEN}✓${NC} Process cleanup completed"

# Optional: Ask if user wants to remove log files
echo -e "\n${YELLOW}Cleanup options:${NC}"
read -p "Remove systemd journal logs for Complex-IT services? [y/N]: " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "Removing journal logs..."
    $SYSTEMCTL_CMD reset-failed complex-it-backend.service 2>/dev/null || true
    $SYSTEMCTL_CMD reset-failed complex-it-frontend.service 2>/dev/null || true
    $SYSTEMCTL_CMD reset-failed complex-it-autopull.service 2>/dev/null || true
    if [[ $EUID -eq 0 ]]; then
        journalctl --vacuum-time=1s --unit=complex-it-*.service 2>/dev/null || true
    else
        journalctl --user --vacuum-time=1s --unit=complex-it-*.service 2>/dev/null || true
    fi
    echo -e "${GREEN}✓${NC} Journal logs cleaned"
fi

# Completion message
echo -e "\n${GREEN}============================================${NC}"
echo -e "${GREEN}Uninstallation completed successfully!${NC}"
echo -e "${GREEN}============================================${NC}"
echo ""
echo "Removed services:"
echo "  • complex-it-backend.service"
echo "  • complex-it-frontend.service" 
echo "  • complex-it-autopull.service"
echo "  • complex-it-autopull.timer"
echo ""
echo -e "${YELLOW}Note: Service scripts in $SERVICES_DIR are preserved.${NC}"
echo -e "${YELLOW}To reinstall services, run: bash $SERVICES_DIR/Install.sh${NC}"
