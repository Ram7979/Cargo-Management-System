@echo off
REM CMS Cargo Management System - Start All Services

echo.
echo ========================================
echo CMS API Gateway - Service Launcher
echo ========================================
echo.
echo This script will start all services in separate terminal windows
echo Ensure .NET 10.0 SDK is installed and accessible
echo.
echo Services to start:
echo   1. Identity Service (5001)
echo   2. Shipment Service (5002)
echo   3. Customer Service (5003)
echo   4. Fleet Service (5004)
echo   5. Warehouse Service (5005)
echo   6. Billing Service (5006)
echo   7. Notification Service (5007)
echo   8. Reporting Service (5008)
echo   9. API Gateway (5000)
echo.
pause

setlocal enabledelayedexpansion

REM Get the root directory of the solution
cd /d "%~dp0"

REM Define paths
set IDENTITY_PATH=src\Services\Identity\CMS.IdentityService.API
set SHIPMENT_PATH=src\Services\Shipment\CMS.ShipmentService.API
set CUSTOMER_PATH=src\Services\Customer\CMS.CustomerService.API
set FLEET_PATH=src\Services\Fleet\CMS.FleetService.API
set WAREHOUSE_PATH=src\Services\Warehouse\CMS.WarehouseService.API
set BILLING_PATH=src\Services\Billing\CMS.BillingService.API
set NOTIFICATION_PATH=src\Services\Notification\CMS.NotificationService.API
set REPORTING_PATH=src\Services\Reporting\CMS.ReportingService.API
set GATEWAY_PATH=src\Gateway\CMS.ApiGateway

REM Colors and titles
set TITLE_IDENTITY=CMS Identity Service (5001)
set TITLE_SHIPMENT=CMS Shipment Service (5002)
set TITLE_CUSTOMER=CMS Customer Service (5003)
set TITLE_FLEET=CMS Fleet Service (5004)
set TITLE_WAREHOUSE=CMS Warehouse Service (5005)
set TITLE_BILLING=CMS Billing Service (5006)
set TITLE_NOTIFICATION=CMS Notification Service (5007)
set TITLE_REPORTING=CMS Reporting Service (5008)
set TITLE_GATEWAY=CMS API Gateway (5000)

echo Starting services...
echo.

REM Start services with small delays between each to avoid resource contention
echo [1/9] Starting Identity Service...
start "%TITLE_IDENTITY%" cmd /k "cd /d "%CD%\%IDENTITY_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [2/9] Starting Shipment Service...
start "%TITLE_SHIPMENT%" cmd /k "cd /d "%CD%\%SHIPMENT_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [3/9] Starting Customer Service...
start "%TITLE_CUSTOMER%" cmd /k "cd /d "%CD%\%CUSTOMER_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [4/9] Starting Fleet Service...
start "%TITLE_FLEET%" cmd /k "cd /d "%CD%\%FLEET_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [5/9] Starting Warehouse Service...
start "%TITLE_WAREHOUSE%" cmd /k "cd /d "%CD%\%WAREHOUSE_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [6/9] Starting Billing Service...
start "%TITLE_BILLING%" cmd /k "cd /d "%CD%\%BILLING_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [7/9] Starting Notification Service...
start "%TITLE_NOTIFICATION%" cmd /k "cd /d "%CD%\%NOTIFICATION_PATH%" && dotnet run"
timeout /t 2 /nobreak

echo [8/9] Starting Reporting Service...
start "%TITLE_REPORTING%" cmd /k "cd /d "%CD%\%REPORTING_PATH%" && dotnet run"
timeout /t 3 /nobreak

echo [9/9] Starting API Gateway...
start "%TITLE_GATEWAY%" cmd /k "cd /d "%CD%\%GATEWAY_PATH%" && dotnet run"

echo.
echo ========================================
echo All services are starting!
echo ========================================
echo.
echo Wait 10-15 seconds for all services to initialize, then access:
echo   🌐 Gateway Swagger UI: http://localhost:5000/swagger
echo.
echo The dropdown will show all 8 available services:
echo   - Identity Service
echo   - Shipment Service
echo   - Customer Service
echo   - Fleet Service
echo   - Warehouse Service
echo   - Billing Service
echo   - Notification Service
echo   - Reporting Service
echo.
echo Each service terminal will show its initialization logs.
echo Check logs if any service fails to start.
echo.
pause
