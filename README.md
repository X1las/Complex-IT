# Complex IT Linux Services Repository

## Commands

```bash
cd /home/xilas/Complex-IT/Services
sudo chmod +x AutoPull.sh
sudo chmod +x FrontendService.sh
sudo chmod +x BackendService.sh
```

## Notes

### Services are located in:
- **Service Scripts:** `/home/xilas/Complex-IT/Services/`
- **Systemd Service Files:** `/etc/systemd/system/`
- **Service Names:**
  - `complex-it-backend.service`
  - `complex-it-frontend.service` 
  - `complex-it-autopull.service`

### Restarting a service:
```bash
# Restart individual services
sudo systemctl restart complex-it-backend.service
sudo systemctl restart complex-it-frontend.service
sudo systemctl restart complex-it-autopull.service

# Or use the management script (if installed)
bash /home/xilas/Complex-IT/Services/manage-services.sh restart

# Check service status
sudo systemctl status complex-it-backend.service
```

### Looking at Journal:
```bash
# View logs for specific service
sudo journalctl -u complex-it-backend.service -f
sudo journalctl -u complex-it-frontend.service -f
sudo journalctl -u complex-it-autopull.service -f

# View recent logs (last hour)
sudo journalctl -u complex-it-backend.service --since "1 hour ago"

# View logs with specific number of lines
sudo journalctl -u complex-it-backend.service -n 50

# View all service logs together
sudo journalctl -u complex-it-*.service -f
```

## Service Management

### Manual Control:
```bash
# Start services
sudo systemctl start complex-it-backend.service
sudo systemctl start complex-it-frontend.service

# Stop services  
sudo systemctl stop complex-it-backend.service
sudo systemctl stop complex-it-frontend.service

# Enable/disable auto-start on boot
sudo systemctl enable complex-it-backend.service
sudo systemctl disable complex-it-backend.service
```

## Application URLs:
- **Frontend:** https://newtlike.com (port 443)
- **Backend API:** https://newtlike.com:3000
- **Local Development:** http://localhost:5173 (npm run dev)

## Troubleshooting:
```bash
# Check if ports are in use
sudo lsof -i :443
sudo lsof -i :3000

# Check service status
systemctl --failed

# Reload systemd after changes
sudo systemctl daemon-reload

# Make scripts executable
cd /home/xilas/Complex-IT/Services
sudo chmod +x *.sh

# Test scripts manually
sudo -u xilas bash ./BackendService.sh
sudo -u root bash ./FrontendService.sh
sudo -u xilas bash ./AutoPull.sh
```