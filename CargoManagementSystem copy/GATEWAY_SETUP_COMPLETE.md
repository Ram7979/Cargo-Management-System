# ✅ CMS API Gateway - Complete Working Setup

## Overview
Your API Gateway is configured to aggregate Swagger documentation from all 8 microservices into a single UI with a dropdown service selector. This document confirms the configuration and provides instructions to run everything without errors.

## Architecture
```
┌─────────────────────────────────────────────────────────────┐
│           CMS API Gateway (Port 5000)                        │
│   - SwaggerForOcelot: Aggregates all service Swagger docs   │
│   - Ocelot: Routes all requests to appropriate services     │
│   - JWT Authentication: Validates tokens from Identity      │
└─────────────────────────────────────────────────────────────┘
         ↓ Routes ↓ & ↓ Swagger ↓
┌──────────────────────────────────────────────────────────────────────────┐
│  Identity (5001)  Shipment (5002)  Customer (5003)  Fleet (5004)          │
│  Warehouse (5005)  Billing (5006)  Notification (5007)  Reporting (5008)  │
│  - Each service has Swagger enabled on /swagger/v1/swagger.json           │
│  - Each service validates JWT tokens via API Gateway                       │
└──────────────────────────────────────────────────────────────────────────┘
```

## ✅ Configuration Status

### Services Configuration
| Service | Port | Swagger Endpoint | Status |
|---------|------|------------------|--------|
| Identity Service | 5001 | http://localhost:5001/swagger/v1/swagger.json | ✅ Configured |
| Shipment Service | 5002 | http://localhost:5002/swagger/v1/swagger.json | ✅ Configured |
| Customer Service | 5003 | http://localhost:5003/swagger/v1/swagger.json | ✅ Configured |
| Fleet Service | 5004 | http://localhost:5004/swagger/v1/swagger.json | ✅ Configured |
| Warehouse Service | 5005 | http://localhost:5005/swagger/v1/swagger.json | ✅ Configured |
| Billing Service | 5006 | http://localhost:5006/swagger/v1/swagger.json | ✅ Configured |
| Notification Service | 5007 | http://localhost:5007/swagger/v1/swagger.json | ✅ Configured |
| Reporting Service | 5008 | http://localhost:5008/swagger/v1/swagger.json | ✅ Configured |

### Gateway Configuration
| Component | Configuration | Status |
|-----------|----------------|--------|
| SwaggerForOcelot | 8 services in SwaggerEndPoints | ✅ Complete |
| Ocelot Routes | All endpoints routed | ✅ Complete |
| CORS | AllowAnyOrigin, AllowAnyMethod, AllowAnyHeader | ✅ Enabled |
| JWT Authentication | Bearer token validation | ✅ Enabled |
| Rate Limiting | 100 requests/minute per service | ✅ Enabled |

## 🚀 How to Run (Without Errors)

### Prerequisites
- .NET 10.0 SDK installed
- SQL Server or LocalDB (for each service database)
- Seq (optional, for logging aggregation on http://localhost:5341)

### Step 1: Start Services in Order
Open separate terminals and run each service:

```bash
# Terminal 1: API Gateway (must start last after services are ready)
cd src\Gateway\CMS.ApiGateway
dotnet run

# Terminal 2: Identity Service (start first)
cd src\Services\Identity\CMS.IdentityService.API
dotnet run

# Terminal 3: Shipment Service
cd src\Services\Shipment\CMS.ShipmentService.API
dotnet run

# Terminal 4: Customer Service
cd src\Services\Customer\CMS.CustomerService.API
dotnet run

# Terminal 5: Fleet Service
cd src\Services\Fleet\CMS.FleetService.API
dotnet run

# Terminal 6: Warehouse Service
cd src\Services\Warehouse\CMS.WarehouseService.API
dotnet run

# Terminal 7: Billing Service
cd src\Services\Billing\CMS.BillingService.API
dotnet run

# Terminal 8: Notification Service
cd src\Services\Notification\CMS.NotificationService.API
dotnet run

# Terminal 9: Reporting Service
cd src\Services\Reporting\CMS.ReportingService.API
dotnet run
```

### Step 2: Access the Gateway
1. Open browser: `http://localhost:5000/swagger`
2. You'll see a dropdown showing all 8 services
3. Select a service from the dropdown to view its API documentation
4. Use the service's API endpoints directly or through the gateway

## 🔑 JWT Authentication

All protected endpoints require a Bearer token. Example:

```bash
# 1. Login to get a token
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}'

# Response: {"token":"eyJhbGc...","expiresIn":3600}

# 2. Use token in requests
curl -X GET http://localhost:5000/api/v1/customers \
  -H "Authorization: Bearer eyJhbGc..."
```

## 📊 Gateway Swagger UI Features

When you access `http://localhost:5000/swagger`:

1. **Service Dropdown**: Shows all 8 available services
   - Identity Service (Auth)
   - Shipment Service
   - Customer Service
   - Fleet Service
   - Warehouse Service
   - Billing Service
   - Notification Service
   - Reporting Service

2. **JWT Authentication**: Authorize button to add Bearer token

3. **Request Routing**: Try-it-out functionality automatically routes to correct service

## 🔄 Request Flow

```
Client Browser (http://localhost:5000/swagger)
        ↓
Select Service from Dropdown
        ↓
View Service API Documentation (via SwaggerForOcelot)
        ↓
Click "Try it out" on endpoint
        ↓
Gateway routes request:
  /api/v1/customers/1 → http://localhost:5003/api/v1/customers/1
  /api/v1/shipments/1 → http://localhost:5002/api/v1/shipments/1
        ↓
Service receives request with JWT Bearer token
        ↓
Service validates token against Identity Service
        ↓
Service processes request and returns response
        ↓
Gateway passes response back to client
```

## ⚙️ Configuration Details

### Gateway Middleware Order (Important!)
1. CORS
2. CorrelationId middleware
3. GlobalException middleware
4. Authentication
5. Authorization
6. Swagger
7. SwaggerForOcelot UI ← Must be before Ocelot
8. Ocelot ← Must be last

### Port Assignments
- **5000**: API Gateway
- **5001**: Identity Service
- **5002**: Shipment Service
- **5003**: Customer Service
- **5004**: Fleet Service
- **5005**: Warehouse Service
- **5006**: Billing Service
- **5007**: Notification Service
- **5008**: Reporting Service

### JWT Configuration (appsettings.json)
```json
{
  "Jwt": {
    "Secret": "CMS-Super-Secret-Key-That-Is-At-Least-32-Characters-Long!",
    "Issuer": "CMS.IdentityService",
    "Audience": "CMS.Clients"
  }
}
```

## 🛠️ Troubleshooting

### Issue: Swagger dropdown is empty
**Solution**: 
- Ensure all services are running on correct ports
- Check that each service has `/swagger/v1/swagger.json` accessible
- Verify ocelot.json SwaggerEndPoints URLs are correct

### Issue: 404 when accessing service endpoints
**Solution**:
- Verify the service is running on correct port
- Check ocelot.json Routes to ensure endpoint is configured
- Verify URL path matches UpstreamPathTemplate

### Issue: JWT token validation fails
**Solution**:
- Ensure JWT Secret matches across all services
- Verify token is from Identity Service on port 5001
- Check token hasn't expired (default 1 hour)
- Include "Bearer " prefix in Authorization header

### Issue: CORS errors
**Solution**:
- Gateway has CORS enabled with AllowAnyOrigin
- All services should also have CORS enabled
- Verify Content-Type headers are set correctly

## 📝 Service Endpoints (Examples)

### Identity Service
- POST `/api/v1/auth/login` - Login
- POST `/api/v1/auth/logout` - Logout
- GET `/api/v1/users` - List users (requires auth)

### Shipment Service
- GET `/api/v1/shipments` - List shipments
- POST `/api/v1/shipments` - Create shipment
- GET `/api/v1/shipments/{id}` - Get shipment details

### Customer Service
- GET `/api/v1/customers` - List customers
- POST `/api/v1/customers` - Create customer
- GET `/api/v1/customers/{id}` - Get customer details

### Similar patterns for Fleet, Warehouse, Billing, Notification, Reporting

## ✨ Key Features Enabled

✅ **SwaggerForOcelot Integration**: Aggregates all service swagger docs  
✅ **API Gateway Routing**: Single entry point for all services  
✅ **JWT Bearer Authentication**: Secure token-based access  
✅ **Rate Limiting**: 100 requests/minute protection  
✅ **CORS**: Cross-origin resource sharing enabled  
✅ **Correlation IDs**: Request tracking across services  
✅ **Global Exception Handling**: Consistent error responses  
✅ **Serilog Logging**: Structured logging to console and Seq  

## 🎯 Next Steps

1. Start all services as shown in "How to Run" section
2. Access Gateway at http://localhost:5000/swagger
3. Select a service from dropdown
4. Get JWT token via `/api/v1/auth/login`
5. Click "Authorize" button and enter token
6. Start testing endpoints

## 📞 Support

If you encounter issues:
1. Check logs in each terminal for specific error messages
2. Verify all services are running: `netstat -an | findstr 500`
3. Ensure databases are accessible
4. Check firewall isn't blocking ports
5. Verify JWT secret matches across all services

---

**Status**: ✅ Gateway fully configured and ready to use  
**Last Updated**: April 2026  
**Configuration Version**: 1.0
