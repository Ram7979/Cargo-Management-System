# Customer Service - Complete Setup & Testing Guide

## ✅ What is ValidationException?

The **ValidationException you're seeing is NOT an error** - it's the expected behavior when request validation fails. The system is working correctly!

### How it works:
1. ✅ Request comes to API
2. ✅ ValidationBehavior checks all required fields
3. ✅ Missing/invalid fields → ValidationException thrown
4. ✅ GlobalExceptionMiddleware catches it
5. ✅ Returns **HTTP 400 Bad Request** with error details
6. ✅ Client sees validation error messages

---

## 📋 Prerequisites

Before running Customer Service, ensure:

1. **SQL Server is running**
   - Instance: `localhost\sqlexpress`
   - Check in Services.msc or SQL Server Management Studio
   - Create database: `CMS_Customer` (if not exists)

2. **Databases exist**: Run migrations for each service
   ```bash
   # In Package Manager Console
   Add-Migration InitialCreate
   Update-Database
   ```

3. **Identity Service is running** (port 5001)
   - Needed for JWT validation
   - Or disable Authorization temporarily for testing

4. **Redis is running** (port 6379) - Optional but recommended
   - For caching functionality
   - Run: `redis-server` or use Docker

---

## 🚀 Running Customer Service

### Option 1: Visual Studio (Recommended)
1. Open solution in Visual Studio
2. Set as **Startup Project**: Right-click `CMS.CustomerService.API` → "Set as Startup Project"
3. **Set Default Project** (Package Manager Console): `CMS.CustomerService.Infrastructure`
4. Press **F5** (Debug) or **Ctrl+F5** (Run without Debug)
5. Swagger should automatically open at `http://localhost:5003/swagger`

### Option 2: Command Line
```bash
cd src\Services\Customer\CMS.CustomerService.API
dotnet run
```
Then open: `http://localhost:5003/swagger`

### Option 3: From .http file
1. Open `CMS.CustomerService.API.http` in Visual Studio
2. Click "Send Request" on any request
3. Valid requests will succeed; invalid ones will return validation errors

---

## 📝 Required Fields for Customer Registration

When registering a customer, ALL these fields are required:

| Field | Type | Format | Example |
|-------|------|--------|---------|
| **fullName** | string | Non-empty | "John Smith" |
| **email** | string | Valid email | "john@example.com" |
| **phone** | string | Non-empty | "+1-555-0100" |
| **address** | string | Non-empty | "123 Main St" |
| **city** | string | Non-empty | "New York" |
| **country** | string | Non-empty | "USA" |
| **type** | string | One of: Individual, Corporate, FreightForwarder | "Individual" |
| companyName | string | Optional | "Smith Trading" |
| contactPerson | string | Optional | "John Smith" |
| state | string | Optional | "NY" |
| zipCode | string | Optional | "10001" |
| taxId | string | Optional | "12-3456789" |
| creditLimit | decimal | Optional | 50000 |
| paymentTerms | string | Optional | "Net 30" |
| kycDocuments | array | Optional | [] |

---

## ✅ Valid Test Requests

### ✓ WILL SUCCEED - Register Individual Customer
```json
POST /api/v1/customers
{
  "fullName": "John Smith",
  "companyName": "Smith Trading",
  "contactPerson": "John Smith",
  "email": "john.smith@gmail.com",
  "phone": "+1234567890",
  "address": "123 Main Street",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "country": "USA",
  "taxId": "12-3456789",
  "type": "Individual",
  "creditLimit": 50000,
  "paymentTerms": "Net 30"
}

Response: 201 Created ✓
```

### ✗ WILL FAIL - Missing Required Fields
```json
POST /api/v1/customers
{
  "fullName": "Jane Doe",
  "email": "jane@example.com"
}

Response: 400 Bad Request
{
  "success": false,
  "data": null,
  "message": "One or more validation errors occurred.",
  "errors": [
    "Phone is required.",
    "Address is required.",
    "City is required.",
    "Country is required.",
    "Customer type is required."
  ]
}
```

### ✗ WILL FAIL - Invalid Email Format
```json
POST /api/v1/customers
{
  "fullName": "Test User",
  "companyName": "Test Co",
  "contactPerson": "Test User",
  "email": "not-an-email",  // ← Invalid!
  "phone": "+1-555-0200",
  "address": "789 Oak Street",
  "city": "Boston",
  "country": "USA",
  "type": "Individual",
  "creditLimit": 75000,
  "paymentTerms": "Net 30"
}

Response: 400 Bad Request
{
  "success": false,
  "errors": ["A valid email is required."]
}
```

### ✗ WILL FAIL - Invalid Customer Type
```json
POST /api/v1/customers
{
  "fullName": "Test User",
  "companyName": "Test Co",
  "contactPerson": "Test User",
  "email": "test@example.com",
  "phone": "+1-555-0200",
  "address": "999 Test Ave",
  "city": "Denver",
  "country": "USA",
  "type": "InvalidType",  // ← Must be: Individual, Corporate, or FreightForwarder
  "creditLimit": 50000,
  "paymentTerms": "Net 30"
}

Response: 400 Bad Request
{
  "success": false,
  "errors": ["Type must be one of: Individual, Corporate, FreightForwarder."]
}
```

---

## 🧪 Testing Endpoints

### 1. Register Customer (No Auth Required)
```http
POST http://localhost:5003/api/v1/customers
Content-Type: application/json

{
  "fullName": "John Smith",
  "companyName": "Smith Trading",
  "contactPerson": "John Smith",
  "email": "john.smith@gmail.com",
  "phone": "+1234567890",
  "address": "123 Main Street",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "country": "USA",
  "taxId": "12-3456789",
  "type": "Individual",
  "creditLimit": 50000,
  "paymentTerms": "Net 30"
}
```

### 2. Get All Customers (Auth Required)
```http
GET http://localhost:5003/api/v1/customers
Authorization: Bearer {your_jwt_token}
```

### 3. Get Customers by Type
```http
GET http://localhost:5003/api/v1/customers?type=Corporate
Authorization: Bearer {your_jwt_token}
```

### 4. Search Customers
```http
GET http://localhost:5003/api/v1/customers?search=john
Authorization: Bearer {your_jwt_token}
```

### 5. Get Specific Customer
```http
GET http://localhost:5003/api/v1/customers/{customerId}
Authorization: Bearer {your_jwt_token}
```

---

## 🔑 Getting JWT Token for Authorization

1. **Start Identity Service** (port 5001)
2. **Open Swagger**: `http://localhost:5001/swagger`
3. **POST** `/api/v1/auth/login`
4. **Body**:
   ```json
   {
     "email": "admin@cms.com",
     "password": "Admin@123"
   }
   ```
5. **Copy token** from response → Use in `Authorization: Bearer {token}` header

---

## ❌ Common Issues & Solutions

### Issue: "Connection to SQL Server failed"
**Solution**:
- Start SQL Server: `sc start MSSQLSERVER`
- Verify connection string in `appsettings.json`
- Check database exists: `CMS_Customer`

### Issue: "Swagger not opening"
**Solution**:
- Manual URL: `http://localhost:5003/swagger`
- Check launchSettings.json has `"launchUrl": "swagger"`
- Verify port 5003 is not in use

### Issue: "ValidationException when registering customer"
**Solution**:
- This is EXPECTED - check which fields are missing
- Review "Required Fields" section above
- Ensure email format is valid (contains @)
- Ensure Type is one of: Individual, Corporate, FreightForwarder

### Issue: "Unauthorized - 401"
**Solution**:
- Get JWT token from Identity Service first
- Include token in Authorization header: `Bearer {token}`
- For GET endpoints, most require authorization
- For POST registration, authorization is NOT required

### Issue: "Port already in use"
**Solution**:
- Find process: `netstat -ano | findstr :5003`
- Kill process: `taskkill /PID {pid} /F`
- Or change port in launchSettings.json

---

## 🎯 Complete Workflow

### 1. Start Services (in order)
```
1. SQL Server ✓
2. Redis (optional) ✓
3. Seq Logging (optional) ✓
4. Identity Service (port 5001) ✓
5. Customer Service (port 5003) ✓
```

### 2. Get JWT Token
- Call: `POST http://localhost:5001/api/v1/auth/login`
- Use credentials to get token

### 3. Register Customer
- Call: `POST http://localhost:5003/api/v1/customers`
- Body: Valid customer data (see examples above)
- Response: 201 Created with customer ID

### 4. Query Customers
- Call: `GET http://localhost:5003/api/v1/customers`
- Header: `Authorization: Bearer {token}`
- Response: List of customers

---

## 📊 API Response Format

### Success Response (201 Created)
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "John Smith",
    "email": "john.smith@gmail.com",
    "type": "Individual",
    "isActive": true,
    "createdAt": "2024-04-15T10:30:00Z"
  },
  "message": "Success",
  "errors": null
}
```

### Validation Error Response (400 Bad Request)
```json
{
  "success": false,
  "data": null,
  "message": "One or more validation errors occurred.",
  "errors": [
    "Full name is required.",
    "A valid email is required.",
    "Phone is required."
  ]
}
```

### Not Found Response (404)
```json
{
  "success": false,
  "data": null,
  "message": "Customer not found.",
  "errors": ["Customer not found."]
}
```

---

## 🔍 Debugging Tips

1. **Check Console Output**
   - Look for SQL connection errors
   - Check JWT validation messages
   - View correlation ID for tracing

2. **Use Swagger UI**
   - Try endpoints directly
   - See response codes and bodies
   - Test authentication workflows

3. **Use .http File**
   - Open `CMS.CustomerService.API.http`
   - Click "Send Request"
   - See real HTTP responses

4. **Monitor Logs**
   - Seq: `http://localhost:5341`
   - Console output
   - Check service name = "CustomerService"

---

## ✨ Summary

✅ ValidationException = **Expected behavior** (not an error)
✅ Ensure **ALL required fields** are provided
✅ Use **valid email format** (must contain @)
✅ Use **valid Type** (Individual, Corporate, FreightForwarder)
✅ Get **JWT token** from Identity Service for authorized endpoints
✅ Include **Bearer token** in Authorization header

**The Customer Service should now work perfectly!**
