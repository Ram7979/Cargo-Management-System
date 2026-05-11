# ⚡ CMS API Gateway - Quick Reference Card

## 🚀 START ALL SERVICES (Choose One)

### Windows Batch (Easiest)
```bash
start-all-services.bat
```

### Windows PowerShell
```powershell
.\start-all-services.ps1
```

### Manual (Terminal per Service)
```bash
# 9 separate terminals needed
# Order: Identity → Shipment → Customer → Fleet → Warehouse → Billing → Notification → Reporting → Gateway

cd src\Services\Identity\CMS.IdentityService.API && dotnet run
cd src\Services\Shipment\CMS.ShipmentService.API && dotnet run
cd src\Services\Customer\CMS.CustomerService.API && dotnet run
cd src\Services\Fleet\CMS.FleetService.API && dotnet run
cd src\Services\Warehouse\CMS.WarehouseService.API && dotnet run
cd src\Services\Billing\CMS.BillingService.API && dotnet run
cd src\Services\Notification\CMS.NotificationService.API && dotnet run
cd src\Services\Reporting\CMS.ReportingService.API && dotnet run
cd src\Gateway\CMS.ApiGateway && dotnet run
```

---

## 📊 SERVICE PORTS

| Service | Port | Swagger |
|---------|------|---------|
| API Gateway | **5000** | http://localhost:5000/swagger |
| Identity | 5001 | http://localhost:5001/swagger |
| Shipment | 5002 | http://localhost:5002/swagger |
| Customer | 5003 | http://localhost:5003/swagger |
| Fleet | 5004 | http://localhost:5004/swagger |
| Warehouse | 5005 | http://localhost:5005/swagger |
| Billing | 5006 | http://localhost:5006/swagger |
| Notification | 5007 | http://localhost:5007/swagger |
| Reporting | 5008 | http://localhost:5008/swagger |

---

## 🔗 GATEWAY ACCESS

### Swagger UI with Service Dropdown
```
http://localhost:5000/swagger
```

### What You'll See
- Dropdown: "Select a definition"
- 8 services listed
- Switch between services instantly
- Test API endpoints directly

---

## 🔐 JWT AUTHENTICATION

### Get Token
```bash
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'
```

### Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

### Use Token
```bash
curl -X GET http://localhost:5000/api/v1/users \
  -H "Authorization: Bearer eyJhbGc..."
```

### In Swagger UI
1. Click "Authorize" button
2. Paste: `Bearer YOUR_TOKEN_HERE`
3. Click "Authorize"
4. Click "Close"
5. Now test protected endpoints

---

## ✅ VERIFY IT'S WORKING

### Check Gateway
```bash
curl http://localhost:5000/swagger
# Should return HTML page (Swagger UI)
```

### Check Service
```bash
curl http://localhost:5001/swagger/v1/swagger.json
# Should return JSON with API schema
```

### Check Dropdown Works
```bash
curl http://localhost:5000/swagger/docs
# Gateway should fetch all service swagger docs
```

---

## 🧪 QUICK TEST SEQUENCE

1. **Start all services**
   ```bash
   start-all-services.bat
   ```
   Wait 30-45 seconds ⏳

2. **Open Gateway**
   ```
   http://localhost:5000/swagger
   ```

3. **Get Token**
   - Select "Identity Service" from dropdown
   - Find `POST /api/v1/auth/login`
   - Click "Try it out"
   - Click "Execute"
   - Copy the `token` from response

4. **Authorize in Gateway**
   - Click "Authorize" button at top
   - Paste `Bearer TOKEN` (including "Bearer ")
   - Click "Authorize"

5. **Test Protected Endpoint**
   - Select "Customer Service" from dropdown
   - Find `GET /api/v1/customers`
   - Click "Try it out"
   - Click "Execute"
   - See the customer list! ✅

---

## 🐛 QUICK FIXES

### Service won't start
```bash
# Check if port is in use
netstat -ano | findstr :5001

# Kill process (replace PID)
taskkill /PID 12345 /F
```

### Swagger UI blank
- Refresh browser: `Ctrl+F5`
- Wait another 10 seconds
- Verify all services running

### Token not working
- Ensure "Bearer " is included (with space)
- Token must be from Identity Service
- Check token hasn't expired (max 1 hour)

### CORS error
- All services now have CORS enabled
- Should not occur with current setup
- Restart services if issue persists

---

## 📁 IMPORTANT FILES

| File | Purpose |
|------|---------|
| `IMPLEMENTATION_COMPLETE.md` | Full setup details |
| `GATEWAY_SETUP_COMPLETE.md` | Architecture guide |
| `VERIFICATION_CHECKLIST.md` | Verify everything works |
| `start-all-services.bat` | Start all in Windows |
| `start-all-services.ps1` | Start all in PowerShell |
| `ocelot.json` | Gateway routing config |

---

## 🌐 API ENDPOINTS (Examples)

### Via Gateway (Recommended)
```bash
# Identity
POST http://localhost:5000/api/v1/auth/login
GET http://localhost:5000/api/v1/users

# Shipment
GET http://localhost:5000/api/v1/shipments
POST http://localhost:5000/api/v1/shipments

# Customer
GET http://localhost:5000/api/v1/customers
POST http://localhost:5000/api/v1/customers

# Fleet
GET http://localhost:5000/api/v1/vehicles
POST http://localhost:5000/api/v1/assignments

# Warehouse
GET http://localhost:5000/api/v1/warehouse
POST http://localhost:5000/api/v1/warehouse

# Billing
GET http://localhost:5000/api/v1/invoices
POST http://localhost:5000/api/v1/payments

# Notification
GET http://localhost:5000/api/v1/notifications
POST http://localhost:5000/api/v1/notifications

# Reporting
GET http://localhost:5000/api/v1/reports
GET http://localhost:5000/api/v1/dashboard
```

### Direct to Service
```bash
# Same endpoints but use service port instead of gateway
# Example: http://localhost:5002/api/v1/shipments (direct to Shipment Service)
```

---

## ⚙️ CONFIGURATION

### JWT Secret (Same on All Services)
```
CMS-Super-Secret-Key-That-Is-At-Least-32-Characters-Long!
```

### JWT Issuer
```
CMS.IdentityService
```

### JWT Audience
```
CMS.Clients
```

### Rate Limit
```
100 requests per minute per service
Status: 429 if exceeded
```

---

## 📋 COMMON SCENARIOS

### Scenario 1: Access Customer API
1. Get token from Identity Service
2. Select "Customer Service" in gateway
3. Test `GET /api/v1/customers` endpoint
4. Token validates automatically
5. Returns customer list

### Scenario 2: Create New Shipment
1. Ensure authenticated with Bearer token
2. Select "Shipment Service" in gateway
3. Find `POST /api/v1/shipments`
4. Click "Try it out"
5. Enter shipment details
6. Execute
7. See confirmation with shipment ID

### Scenario 3: Check System Health
1. Check gateway is running: `netstat -ano | findstr :5000`
2. Open Swagger: `http://localhost:5000/swagger`
3. Dropdown should show 8 services
4. All should be clickable and responsive
5. Try any public endpoint to verify

---

## 📞 NEED HELP?

Check these docs in order:
1. **Quick issue**: This card (you are here!)
2. **Setup questions**: `GATEWAY_SETUP_COMPLETE.md`
3. **Testing**: `VERIFICATION_CHECKLIST.md`
4. **Full details**: `IMPLEMENTATION_COMPLETE.md`

---

## ✨ THAT'S IT!

Your CMS API Gateway is fully configured. The dropdown in the Swagger UI at `http://localhost:5000/swagger` is your single access point to all 8 microservices.

**Status**: ✅ Ready to Use  
**Services**: 8 microservices + 1 gateway = Full system
