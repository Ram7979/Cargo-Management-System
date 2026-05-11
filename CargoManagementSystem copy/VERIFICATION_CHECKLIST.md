# ✅ CMS API Gateway - Verification Checklist

## Pre-Startup Verification

### Environment Check
- [ ] .NET 10.0 SDK installed: `dotnet --version`
- [ ] PowerShell or CMD available
- [ ] SQL Server or LocalDB configured
- [ ] Ports 5000-5008 are available (not in use)
- [ ] Firewall allows local connections on these ports

### File Structure Verification
- [ ] Gateway exists: `src\Gateway\CMS.ApiGateway\Program.cs`
- [ ] All 8 services exist:
  - [ ] `src\Services\Identity\CMS.IdentityService.API`
  - [ ] `src\Services\Shipment\CMS.ShipmentService.API`
  - [ ] `src\Services\Customer\CMS.CustomerService.API`
  - [ ] `src\Services\Fleet\CMS.FleetService.API`
  - [ ] `src\Services\Warehouse\CMS.WarehouseService.API`
  - [ ] `src\Services\Billing\CMS.BillingService.API`
  - [ ] `src\Services\Notification\CMS.NotificationService.API`
  - [ ] `src\Services\Reporting\CMS.ReportingService.API`

### Configuration Files Verification
- [ ] Gateway `appsettings.json` exists
- [ ] Gateway `ocelot.json` exists with all 8 services in SwaggerEndPoints
- [ ] Each service has `launchSettings.json` with correct port
- [ ] All services have same JWT secret in `appsettings.json`:
  - Expected: `"CMS-Super-Secret-Key-That-Is-At-Least-32-Characters-Long!"`

---

## Startup Verification

### 1. Start Services

Option A (Batch Script):
```bash
start-all-services.bat
```

Option B (PowerShell Script):
```powershell
.\start-all-services.ps1
```

Option C (Manual):
```bash
# Terminal 1
cd src\Services\Identity\CMS.IdentityService.API
dotnet run

# Terminal 2
cd src\Services\Shipment\CMS.ShipmentService.API
dotnet run

# Terminal 3
cd src\Services\Customer\CMS.CustomerService.API
dotnet run

# Terminal 4
cd src\Services\Fleet\CMS.FleetService.API
dotnet run

# Terminal 5
cd src\Services\Warehouse\CMS.WarehouseService.API
dotnet run

# Terminal 6
cd src\Services\Billing\CMS.BillingService.API
dotnet run

# Terminal 7
cd src\Services\Notification\CMS.NotificationService.API
dotnet run

# Terminal 8
cd src\Services\Reporting\CMS.ReportingService.API
dotnet run

# Terminal 9 (last)
cd src\Gateway\CMS.ApiGateway
dotnet run
```

### 2. Service Startup Indicators

Each service should display:
```
✅ [Service Name] started
📊 Swagger: http://localhost:PORT/swagger
```

Watch for errors in first 30 seconds.

### 3. Gateway Startup Indicators

Gateway should display:
```
✅ CMS API Gateway started successfully
📊 Access Swagger UI with service dropdown at: http://localhost:5000/swagger
🔗 Gateway will route requests to all available microservices
```

---

## Functional Verification

### 1. Access Gateway Swagger UI
- [ ] Open browser: `http://localhost:5000/swagger`
- [ ] Page loads without errors
- [ ] Gateway title shows "CMS API Gateway"

### 2. Verify Service Dropdown
- [ ] Dropdown exists with label "Select a definition"
- [ ] All 8 services visible in dropdown:
  - [ ] Identity Service
  - [ ] Shipment Service
  - [ ] Customer Service
  - [ ] Fleet Service
  - [ ] Warehouse Service
  - [ ] Billing Service
  - [ ] Notification Service
  - [ ] Reporting Service

### 3. Test Each Service
For each service in the dropdown:
- [ ] Click service name
- [ ] Swagger documentation loads
- [ ] API endpoints are visible
- [ ] No 404 or connection errors

### 4. Direct Service Access
- [ ] Identity: `http://localhost:5001/swagger`
- [ ] Shipment: `http://localhost:5002/swagger`
- [ ] Customer: `http://localhost:5003/swagger`
- [ ] Fleet: `http://localhost:5004/swagger`
- [ ] Warehouse: `http://localhost:5005/swagger`
- [ ] Billing: `http://localhost:5006/swagger`
- [ ] Notification: `http://localhost:5007/swagger`
- [ ] Reporting: `http://localhost:5008/swagger`

---

## API Testing Verification

### 1. Public Endpoint (No Auth Required)
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'
```
- [ ] Request returns JWT token
- [ ] Response includes `token` and `expiresIn` fields

### 2. Protected Endpoint (Auth Required)
```bash
# Get the token from step 1, then:
curl -X GET http://localhost:5000/api/v1/users \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```
- [ ] Request succeeds with 200 status
- [ ] Returns user list or appropriate data

### 3. Gateway Routing
Test each service endpoint through gateway:
- [ ] Shipment endpoint: `/api/v1/shipments` → port 5002
- [ ] Customer endpoint: `/api/v1/customers` → port 5003
- [ ] Vehicle endpoint: `/api/v1/vehicles` → port 5004
- [ ] Warehouse endpoint: `/api/v1/warehouse` → port 5005
- [ ] Invoice endpoint: `/api/v1/invoices` → port 5006
- [ ] Notification endpoint: `/api/v1/notifications` → port 5007

---

## Troubleshooting Checklist

### Issue: Swagger UI won't load
- [ ] Verify Gateway is running on port 5000
- [ ] Check firewall isn't blocking port 5000
- [ ] Clear browser cache: Ctrl+Shift+Delete
- [ ] Try different browser (Chrome, Firefox, Edge)
- [ ] Check browser console for errors: F12

### Issue: Service dropdown is empty
- [ ] Verify all 8 services are running
- [ ] Check ocelot.json SwaggerEndPoints URLs
- [ ] Verify each service's `/swagger/v1/swagger.json` is accessible
- [ ] Test: `curl http://localhost:5001/swagger/v1/swagger.json`
- [ ] Check Gateway logs for connection errors

### Issue: 404 when selecting service
- [ ] Verify service is running on correct port
- [ ] Check ocelot.json Route is configured for endpoint
- [ ] Test direct service: `http://localhost:5001/swagger`
- [ ] Check service error logs

### Issue: JWT token validation fails
- [ ] Verify token from Identity Service (port 5001)
- [ ] Check JWT secret matches in all services
- [ ] Ensure "Bearer " prefix is included in header
- [ ] Verify token hasn't expired
- [ ] Test with unprotected endpoint first

### Issue: Database connection errors
- [ ] Verify SQL Server is running
- [ ] Check connection strings in appsettings.json
- [ ] Verify user has database permissions
- [ ] Check firewall allows SQL Server connections
- [ ] Try creating database manually if needed

### Issue: Port already in use
**Fix**:
```bash
# Find process using port 5000:
netstat -ano | findstr :5000

# Kill process:
taskkill /PID PROCESS_ID /F

# Or change ports in launchSettings.json
```

---

## Performance Verification

### Response Times
- [ ] Gateway response time: < 500ms
- [ ] Direct service response time: < 200ms
- [ ] Authentication: < 300ms

### Resource Usage
- [ ] Each service uses < 200MB RAM
- [ ] Gateway uses < 150MB RAM
- [ ] No memory leaks after 5 minutes runtime

### Rate Limiting
- [ ] After 100 requests/minute: `429 Too Many Requests`
- [ ] Rate limit resets after timeout

---

## Final Verification Checklist

### Everything Working?
- [ ] All 9 services running without errors
- [ ] Gateway Swagger UI accessible at http://localhost:5000/swagger
- [ ] Service dropdown showing all 8 services
- [ ] Can select and view each service's API
- [ ] Can authenticate and access protected endpoints
- [ ] JWT tokens valid and authenticated properly
- [ ] CORS working (no cross-origin errors)
- [ ] Rate limiting active

### Documentation
- [ ] Read `GATEWAY_SETUP_COMPLETE.md`
- [ ] Understood service architecture
- [ ] Noted JWT authentication requirements
- [ ] Reviewed endpoint routing configuration

---

## Success Indicators

✅ **You'll know everything is working when:**

1. **Dropdown appears**: 8 services listed in gateway Swagger UI
2. **Service switches**: Can click each service and see its endpoints
3. **API calls work**: Can make authenticated and unauthenticated requests
4. **Tokens valid**: JWT bearer tokens accepted by all services
5. **Routing works**: Requests to `/api/v1/customers` reach port 5003
6. **Logs clean**: No 404, 500, or connection errors in service logs

---

## Quick Reference Commands

```bash
# Check .NET version
dotnet --version

# Check port availability (Windows)
netstat -ano | findstr :5000

# Kill process on port (Windows)
taskkill /PID PROCESS_ID /F

# Build all projects
dotnet build

# Run migrations (if using EF Core)
dotnet ef database update

# Check service health
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
# etc.

# Get JWT token
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'

# Test gateway route
curl http://localhost:5000/api/v1/shipments
```

---

## Support Resources

- **Gateway Setup**: See `GATEWAY_SETUP_COMPLETE.md`
- **Startup Scripts**: `start-all-services.bat` or `start-all-services.ps1`
- **Service Logs**: Check individual service terminal output
- **Network Issues**: Use `netstat` and `telnet` for debugging

---

**Status**: ✅ Verification checklist complete  
**Last Updated**: April 2026
