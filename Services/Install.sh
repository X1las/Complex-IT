#!/bin/bash
# Services Installation Script
# Installs AutoPull, BackendService, and FrontendService as systemd services

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Project paths
PROJECT_DIR="$HOME/Complex-IT"
SERVICES_DIR="$PROJECT_DIR/Services"

echo -e "${GREEN}Complex-IT Services Installation Script${NC}"
echo "==========================================="

# Check if running as root for systemd service installation
if [[ $EUID -eq 0 ]]; then
    SERVICE_DIR="/etc/systemd/system"
    USER_NAME="${SUDO_USER:-$USER}"
    echo -e "${YELLOW}Running as root - installing system-wide services${NC}"
else
    SERVICE_DIR="$HOME/.config/systemd/user"
    USER_NAME="$USER"
    echo -e "${YELLOW}Running as user - installing user services${NC}"
    mkdir -p "$SERVICE_DIR"
fi

# Function to create service files
create_service_file() {
    local service_name=$1
    local script_path=$2
    local description=$3
    local service_file="$SERVICE_DIR/${service_name}.service"

    echo "Creating $service_name service file..."
    
    if [[ $EUID -eq 0 ]]; then
        cat > "$service_file" << EOF
[Unit]
Description=$description
After=network.target

[Service]
Type=simple
User=$USER_NAME
WorkingDirectory=$PROJECT_DIR
ExecStart=/bin/bash $script_path
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF
    else
        cat > "$service_file" << EOF
[Unit]
Description=$description
After=network.target

[Service]
Type=simple
WorkingDirectory=$PROJECT_DIR
ExecStart=/bin/bash $script_path
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=default.target
EOF
    fi

    echo -e "${GREEN}✓${NC} Created $service_name service file"
}

# Create systemd service files
echo -e "\n${YELLOW}Creating systemd service files...${NC}"

create_service_file "complex-it-autopull" "$SERVICES_DIR/AutoPull.sh" "Complex-IT Auto Pull Service"
create_service_file "complex-it-backend" "$SERVICES_DIR/BackendService.sh" "Complex-IT Backend Service"
create_service_file "complex-it-frontend" "$SERVICES_DIR/FrontendService.sh" "Complex-IT Frontend Service"

# Make scripts executable
echo -e "\n${YELLOW}Making service scripts executable...${NC}"
chmod +x "$SERVICES_DIR/AutoPull.sh"
chmod +x "$SERVICES_DIR/BackendService.sh" 
chmod +x "$SERVICES_DIR/FrontendService.sh"
echo -e "${GREEN}✓${NC} Scripts made executable"

# Create a timer for AutoPull service (runs every 5 minutes)
echo -e "\n${YELLOW}Creating AutoPull timer...${NC}"
TIMER_FILE="$SERVICE_DIR/complex-it-autopull.timer"

if [[ $EUID -eq 0 ]]; then
    cat > "$TIMER_FILE" << EOF
[Unit]
Description=Complex-IT Auto Pull Timer
Requires=complex-it-autopull.service

[Timer]
OnBootSec=2min
OnUnitActiveSec=5min
Unit=complex-it-autopull.service

[Install]
WantedBy=timers.target
EOF
else
    cat > "$TIMER_FILE" << EOF
[Unit]
Description=Complex-IT Auto Pull Timer
Requires=complex-it-autopull.service

[Timer]
OnBootSec=2min
OnUnitActiveSec=5min
Unit=complex-it-autopull.service

[Install]
WantedBy=timers.target
EOF
fi

echo -e "${GREEN}✓${NC} Created AutoPull timer"

# Reload systemd and enable services
echo -e "\n${YELLOW}Configuring systemd services...${NC}"

if [[ $EUID -eq 0 ]]; then
    systemctl daemon-reload
    systemctl enable complex-it-backend.service
    systemctl enable complex-it-frontend.service
    systemctl enable complex-it-autopull.timer
    echo -e "${GREEN}✓${NC} System services enabled"
else
    systemctl --user daemon-reload
    systemctl --user enable complex-it-backend.service
    systemctl --user enable complex-it-frontend.service
    systemctl --user enable complex-it-autopull.timer
    echo -e "${GREEN}✓${NC} User services enabled"
fi

# Create management script
echo -e "\n${YELLOW}Creating service management script...${NC}"
MGMT_SCRIPT="$SERVICES_DIR/manage-services.sh"

cat > "$MGMT_SCRIPT" << 'EOF'
#!/bin/bash
# Service Management Script

SERVICE_PREFIX="complex-it"
SYSTEMCTL_CMD="systemctl"

if [[ $EUID -ne 0 ]]; then
    SYSTEMCTL_CMD="systemctl --user"
fi

case "$1" in
    start)
        echo "Starting Complex-IT services..."
        $SYSTEMCTL_CMD start ${SERVICE_PREFIX}-backend.service
        $SYSTEMCTL_CMD start ${SERVICE_PREFIX}-frontend.service
        $SYSTEMCTL_CMD start ${SERVICE_PREFIX}-autopull.timer
        echo "Services started"
        ;;
    stop)
        echo "Stopping Complex-IT services..."
        $SYSTEMCTL_CMD stop ${SERVICE_PREFIX}-autopull.timer
        $SYSTEMCTL_CMD stop ${SERVICE_PREFIX}-frontend.service
        $SYSTEMCTL_CMD stop ${SERVICE_PREFIX}-backend.service
        echo "Services stopped"
        ;;
    restart)
        echo "Restarting Complex-IT services..."
        $SYSTEMCTL_CMD restart ${SERVICE_PREFIX}-backend.service
        $SYSTEMCTL_CMD restart ${SERVICE_PREFIX}-frontend.service
        $SYSTEMCTL_CMD restart ${SERVICE_PREFIX}-autopull.timer
        echo "Services restarted"
        ;;
    status)
        echo "Complex-IT Services Status:"
        echo "=========================="
        $SYSTEMCTL_CMD status ${SERVICE_PREFIX}-backend.service --no-pager -l
        echo ""
        $SYSTEMCTL_CMD status ${SERVICE_PREFIX}-frontend.service --no-pager -l
        echo ""
        $SYSTEMCTL_CMD status ${SERVICE_PREFIX}-autopull.timer --no-pager -l
        ;;
    logs)
        SERVICE=${2:-backend}
        echo "Showing logs for ${SERVICE} service..."
        $SYSTEMCTL_CMD logs -f ${SERVICE_PREFIX}-${SERVICE}.service
        ;;
    *)
        echo "Usage: $0 {start|stop|restart|status|logs [service]}"
        echo "Services: backend, frontend, autopull"
        exit 1
        ;;
esac
EOF

chmod +x "$MGMT_SCRIPT"
echo -e "${GREEN}✓${NC} Created service management script at $MGMT_SCRIPT"

# Installation complete
echo -e "\n${GREEN}============================================${NC}"
echo -e "${GREEN}Installation completed successfully!${NC}"
echo -e "${GREEN}============================================${NC}"
echo ""
echo "Available commands:"
echo "  Start services:   bash $MGMT_SCRIPT start"
echo "  Stop services:    bash $MGMT_SCRIPT stop"
echo "  Restart services: bash $MGMT_SCRIPT restart"
echo "  Check status:     bash $MGMT_SCRIPT status"
echo "  View logs:        bash $MGMT_SCRIPT logs [backend|frontend|autopull]"
echo ""
echo "Services installed:"
echo "  • Backend Service  - Runs .NET WebApplication"
echo "  • Frontend Service - Runs React application"
echo "  • AutoPull Service - Checks for git updates every 5 minutes"
echo ""
echo -e "${YELLOW}Note: Services are enabled but not started. Use the management script to control them.${NC}"