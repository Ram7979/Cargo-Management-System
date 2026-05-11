# CMS Cargo Management System - Start All Services (PowerShell)
# Usage: .\start-all-services.ps1

Write-Host @"
========================================
CMS API Gateway - Service Launcher
========================================

This script will start all services in separate PowerShell windows
Ensure .NET 10.0 SDK is installed and accessible

Services to start:
  1. Identity Service (5001)
  2. Shipment Service (5002)
  3. Customer Service (5003)
  4. Fleet Service (5004)
  5. Warehouse Service (5005)
  6. Billing Service (5006)
  7. Notification Service (5007)
  8. Reporting Service (5008)
  9. API Gateway (5000)

"@ -ForegroundColor Cyan

Read-Host "Press Enter to start all services"

# Get the root directory
$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Service paths
$services = @(
    @{
        Name = "Identity Service (5001)"
        Path = "src\Services\Identity\CMS.IdentityService.API"
        Port = "5001"
    },
    @{
        Name = "Shipment Service (5002)"
        Path = "src\Services\Shipment\CMS.ShipmentService.API"
        Port = "5002"
    },
    @{
        Name = "Customer Service (5003)"
        Path = "src\Services\Customer\CMS.CustomerService.API"
        Port = "5003"
    },
    @{
        Name = "Fleet Service (5004)"
        Path = "src\Services\Fleet\CMS.FleetService.API"
        Port = "5004"
    },
    @{
        Name = "Warehouse Service (5005)"
        Path = "src\Services\Warehouse\CMS.WarehouseService.API"
        Port = "5005"
    },
    @{
        Name = "Billing Service (5006)"
        Path = "src\Services\Billing\CMS.BillingService.API"
        Port = "5006"
    },
    @{
        Name = "Notification Service (5007)"
        Path = "src\Services\Notification\CMS.NotificationService.API"
        Port = "5007"
    },
    @{
        Name = "Reporting Service (5008)"
        Path = "src\Services\Reporting\CMS.ReportingService.API"
        Port = "5008"
    }
)

$gateway = @{
    Name = "API Gateway (5000)"
    Path = "src\Gateway\CMS.ApiGateway"
    Port = "5000"
}

Write-Host "Starting services..." -ForegroundColor Green
Write-Host ""

$counter = 1

# Start all services
foreach ($service in $services) {
    Write-Host "[$counter/9] Starting $($service.Name)..." -ForegroundColor Yellow
    
    $servicePath = Join-Path $rootPath $service.Path
    
    # Check if path exists
    if (-not (Test-Path $servicePath)) {
        Write-Host "  ❌ Path not found: $servicePath" -ForegroundColor Red
        $counter++
        continue
    }
    
    # Start service in new PowerShell window
    $scriptBlock = {
        param($path, $name)
        Set-Location $path
        Write-Host "🚀 Starting $name..." -ForegroundColor Cyan
        dotnet run
    }
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "& {param(`$path); Set-Location `$path; Write-Host '🚀 Starting $($service.Name)...' -ForegroundColor Cyan; dotnet run}" -ArgumentList @($servicePath) -WindowStyle Normal
    
    Start-Sleep -Seconds 2
    $counter++
}

# Start Gateway (wait a bit longer)
Write-Host "[9/9] Starting API Gateway..." -ForegroundColor Yellow
$gatewayPath = Join-Path $rootPath $gateway.Path

if (Test-Path $gatewayPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "& {param(`$path); Set-Location `$path; Write-Host '🌐 Starting $($gateway.Name)...' -ForegroundColor Cyan; Write-Host 'Access Swagger at: http://localhost:5000/swagger' -ForegroundColor Green; dotnet run}" -ArgumentList @($gatewayPath) -WindowStyle Normal
    Start-Sleep -Seconds 3
} else {
    Write-Host "  ❌ Gateway path not found: $gatewayPath" -ForegroundColor Red
}

Write-Host ""
Write-Host @"
========================================
All services are starting!
========================================

Wait 10-15 seconds for all services to initialize, then access:
  🌐 Gateway Swagger UI: http://localhost:5000/swagger

The dropdown will show all 8 available services:
  - Identity Service
  - Shipment Service
  - Customer Service
  - Fleet Service
  - Warehouse Service
  - Billing Service
  - Notification Service
  - Reporting Service

Each service terminal will show its initialization logs.
Check logs if any service fails to start.

"@ -ForegroundColor Cyan

Read-Host "Press Enter to exit"
