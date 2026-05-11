""# CMS Full Stack Startup Script (No Cleanup)
# Usage: .\start-cms-no-cleanup.ps1
# Run from: D:\Files\CMS\CargoManagementSystem

Write-Host "=== CMS Startup Script ===" -ForegroundColor Cyan

# Step 2: Start all backend services
Write-Host "`n[1/2] Starting backend services..." -ForegroundColor Yellow

$root = $PSScriptRoot

$services = @(
    @{ Name="Identity";   Path="src\Services\Identity\CMS.IdentityService.API\CMS.IdentityService.API.csproj";     Port=5001 },
    @{ Name="Shipment";   Path="src\Services\Shipment\CMS.ShipmentService.API\CMS.ShipmentService.API.csproj";     Port=5002 },
    @{ Name="Customer";   Path="src\Services\Customer\CMS.CustomerService.API\CMS.CustomerService.API.csproj";     Port=5003 },
    @{ Name="Fleet";      Path="src\Services\Fleet\CMS.FleetService.API\CMS.FleetService.API.csproj";               Port=5004 },
    @{ Name="Warehouse";  Path="src\Services\Warehouse\CMS.WarehouseService.API\CMS.WarehouseService.API.csproj";       Port=5005 },
    @{ Name="Billing";    Path="src\Services\Billing\CMS.BillingService.API\CMS.BillingService.API.csproj";         Port=5006 },
    @{ Name="Notification";Path="src\Services\Notification\CMS.NotificationService.API\CMS.NotificationService.API.csproj"; Port=5007 },
    @{ Name="Reporting";  Path="src\Services\Reporting\CMS.ReportingService.API\CMS.ReportingService.API.csproj";   Port=5008 },
    @{ Name="Gateway";    Path="src\Gateway\CMS.ApiGateway\CMS.ApiGateway.csproj";                                  Port=5000 }
)

foreach ($svc in $services) {
    $projPath = Join-Path $root $svc.Path
    Start-Process "dotnet" -ArgumentList "run --project `"$projPath`" --no-build" -WindowStyle Hidden
    Write-Host "  Started $($svc.Name) (port $($svc.Port))" -ForegroundColor Gray
    Start-Sleep -Milliseconds 800
}

# Step 3: Wait for services to be ready, then start frontend
Write-Host "`n[2/2] Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 18

$healthy = 0
foreach ($svc in $services) {
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:$($svc.Port)/swagger/index.html" -UseBasicParsing -TimeoutSec 5
        Write-Host "  ✅ $($svc.Name) (port $($svc.Port)) - OK" -ForegroundColor Green
        $healthy++
    } catch {
        Write-Host "  ⚠️  $($svc.Name) (port $($svc.Port)) - Still starting..." -ForegroundColor DarkYellow
    }
}

Write-Host "`n$healthy/$($services.Count) services healthy." -ForegroundColor Cyan

# Start Angular frontend
Write-Host "`nStarting Angular frontend at http://localhost:4200 ..." -ForegroundColor Yellow
$clientPath = Join-Path $root "src\ClientApp"
Start-Process "powershell" -ArgumentList "-NoExit -Command `"cd '$clientPath'; npm start`"" -WindowStyle Normal

Write-Host "`n=== CMS is starting up ===" -ForegroundColor Cyan
Write-Host "Frontend:  http://localhost:4200" -ForegroundColor White
Write-Host "Gateway:   http://localhost:5000/swagger" -ForegroundColor White
Write-Host "Login:     superadmin@cms.com / Admin@123" -ForegroundColor White
