# Customer Service - Multiple Customers Guide

## ✅ NO PREDEFINED CUSTOMERS - 100% DYNAMIC

Your Customer Service is **designed from scratch to support unlimited customers with NO hardcoded/seed data**. Every customer you create is dynamically stored in the database.

---

## 🏗️ System Architecture for Multiple Customers

### Database Design
- **Table**: `Customers` (one record per customer)
- **Unique Constraints**: Email is unique per customer
- **Auto-Generated**: CustomerCode (CUST-{year}-{sequence})
- **No Seed Data**: Empty on startup - you populate it

### Key Features
✅ **Unlimited Customers** - No maximum limit  
✅ **Unique Identification** - Each gets unique ID + CustomerCode  
✅ **Pagination** - Efficiently handle thousands of records  
✅ **Filtering** - Search by type, status, date range  
✅ **Soft Delete** - Mark inactive (GDPR compliant)  
✅ **No Duplicates** - Email uniqueness enforced  

---

## 📝 Customer Sequence Number Generation

Automatically generates unique customer codes:

```
First customer in 2026:  CUST-2026-001
Second customer in 2026: CUST-2026-002
Third customer in 2026:  CUST-2026-003
...
1000th customer in 2026: CUST-2026-1000

First customer in 2027:  CUST-2027-001
```

**Code Pattern**: `CUST-{Year}-{SequenceNumber}`

---

## 🚀 Creating Multiple Customers - Step by Step

### Step 1: Start the Service
```bash
F5 in Visual Studio
# or
cd src/Services/Customer/CMS.CustomerService.API
dotnet run
```
→ Swagger opens: `http://localhost:5003/swagger`

### Step 2: Create First Customer (No Auth Required)
```bash
POST /api/v1/customers
```
**Request Body**:
```json
{
  "fullName": "John Smith",
  "companyName": "Smith Trading Company",
  "contactPerson": "John Smith",
  "email": "john.smith@example.com",
  "phone": "+1-212-555-0100",
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

**Response** (201 Created):
```json
{
  "success": true,
  "data": {
    "id": "abc123de-f456-7890-ghij-klmnopqrstuv",
    "customerCode": "CUST-2026-001",
    "fullName": "John Smith",
    "email": "john.smith@example.com",
    "type": "Individual",
    "creditLimit": 50000,
    "isActive": true,
    "createdAt": "2026-04-15T10:30:00Z"
  },
  "message": "Success"
}
```

### Step 3: Create Second Customer (Different Email)
```json
{
  "fullName": "Acme Corporation",
  "companyName": "Acme Corporation Inc",
  "contactPerson": "Sarah Johnson",
  "email": "sarah.johnson@acmecorp.com",
  "phone": "+1-213-555-0101",
  "address": "456 Business Avenue",
  "city": "Los Angeles",
  "state": "CA",
  "zipCode": "90001",
  "country": "USA",
  "taxId": "98-7654321",
  "type": "Corporate",
  "creditLimit": 100000,
  "paymentTerms": "Net 60"
}
```

**Auto-Generated CustomerCode**: `CUST-2026-002`

### Step 4: Keep Adding More Customers
Repeat the process with different emails. Each gets:
- ✅ Unique ID (auto-generated)
- ✅ Unique CustomerCode (auto-sequence)
- ✅ Stored in database indefinitely
- ✅ Retrieved anytime via API

---

## 📊 Querying Multiple Customers

### Get ALL Customers (Requires JWT Token)
```bash
GET /api/v1/customers
Authorization: Bearer {your_jwt_token}
```

**Response** (first 20 by default):
```json
{
  "success": true,
  "data": [
    {
      "id": "abc123...",
      "customerCode": "CUST-2026-001",
      "fullName": "John Smith",
      "type": "Individual"
    },
    {
      "id": "def456...",
      "customerCode": "CUST-2026-002",
      "fullName": "Acme Corporation",
      "type": "Corporate"
    },
    ...
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 42,      ← Total customers
  "totalPages": 3,       ← Pages available
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Pagination - Get Page 2
```bash
GET /api/v1/customers?page=2&pageSize=20
Authorization: Bearer {token}
```
→ Returns customers 21-40

### Filter by Type
```bash
# Get Corporate customers only
GET /api/v1/customers?type=Corporate
Authorization: Bearer {token}

# Get Individual customers only
GET /api/v1/customers?type=Individual
Authorization: Bearer {token}

# Get FreightForwarder customers only
GET /api/v1/customers?type=FreightForwarder
Authorization: Bearer {token}
```

### Search by Name/Email/Code
```bash
# Search by name
GET /api/v1/customers?search=john
Authorization: Bearer {token}
→ Returns all customers with "john" in name

# Search by email
GET /api/v1/customers?search=acme
Authorization: Bearer {token}
→ Returns customers with "acme" in any field

# Search by customer code
GET /api/v1/customers?search=CUST-2026-001
Authorization: Bearer {token}
```

### Combined Filters
```bash
# Corporate customers, page 2, 10 per page
GET /api/v1/customers?type=Corporate&page=2&pageSize=10
Authorization: Bearer {token}

# Corporate customers matching "tech"
GET /api/v1/customers?type=Corporate&search=tech
Authorization: Bearer {token}

# Active individual customers
GET /api/v1/customers?type=Individual&isActive=true
Authorization: Bearer {token}
```

### Get Specific Customer
```bash
GET /api/v1/customers/{customerId}
Authorization: Bearer {token}
```

**Response**:
```json
{
  "success": true,
  "data": {
    "id": "abc123de-f456-7890-ghij-klmnopqrstuv",
    "customerCode": "CUST-2026-001",
    "fullName": "John Smith",
    "companyName": "Smith Trading Company",
    "email": "john.smith@example.com",
    "phone": "+1-212-555-0100",
    "address": "123 Main Street",
    "city": "New York",
    "country": "USA",
    "type": "Individual",
    "creditLimit": 50000,
    "isActive": true,
    "kycDocuments": [],
    "createdAt": "2026-04-15T10:30:00Z",
    "updatedAt": "2026-04-15T10:30:00Z"
  }
}
```

---

## ✅ Verification - No Predefined Customers

### How to Verify
1. **Fresh Database**: Delete `CMS_Customer` database
2. **Run Migrations**: 
   ```bash
   Add-Migration InitialCreate
   Update-Database
   ```
3. **Check Database**: Should be empty (Customers table = 0 rows)
4. **Start Service**: `F5`
5. **Create Customer**: POST to `/api/v1/customers`
6. **Query Database**: 
   ```sql
   SELECT COUNT(*) FROM Customers;  → Should be 1
   SELECT COUNT(*) FROM Customers WHERE Year = 2026;  → Should be 1
   ```

### Expected Database State Over Time
```
Initial:        Customers = 0

After 1st POST: Customers = 1 (CUST-2026-001)
After 2nd POST: Customers = 2 (CUST-2026-002)
After 3rd POST: Customers = 3 (CUST-2026-003)
...
After 100 POSTs: Customers = 100 (CUST-2026-001 to CUST-2026-100)
```

### View in SQL Server
```sql
-- See all customers
SELECT * FROM Customers ORDER BY CreatedAt DESC;

-- Count by type
SELECT Type, COUNT(*) as Count FROM Customers GROUP BY Type;

-- Count by year
SELECT YEAR(CreatedAt) as Year, COUNT(*) as Count 
FROM Customers 
GROUP BY YEAR(CreatedAt);

-- Check if duplicates prevented
SELECT Email, COUNT(*) as Count FROM Customers 
GROUP BY Email HAVING COUNT(*) > 1;
```

---

## 🔒 Data Integrity - Constraints

### Unique Constraints
- **Email**: One customer per email address
- **CustomerCode**: One per sequential number per year
- **ID**: Global unique identifier

### Validation Rules
- **FullName**: Required, max 200 chars
- **Email**: Required, unique, valid format
- **Phone**: Required, max 50 chars
- **Address**: Required, max 500 chars
- **City**: Required, max 100 chars
- **Country**: Required, max 100 chars
- **Type**: Required, must be: Individual | Corporate | FreightForwarder

### Prevention of Issues
✅ Duplicate emails rejected:
```
POST /api/v1/customers with email="john.smith@example.com"
Response: 409 Conflict
Message: "A customer with email 'john.smith@example.com' already exists."
```

✅ No predefined conflicts - database empty on init

---

## 📋 Complete Test Workflow

### Scenario: Create 8 Different Customers

**1. Customer 1 - Individual (USA)**
```json
{
  "fullName": "John Smith",
  "email": "john.smith@example.com",
  "phone": "+1-212-555-0100",
  "address": "123 Main Street",
  "city": "New York",
  "country": "USA",
  "type": "Individual"
}
```
→ Response: `CUST-2026-001`

**2. Customer 2 - Corporate (USA)**
```json
{
  "fullName": "Acme Corporation",
  "email": "sarah@acmecorp.com",
  "phone": "+1-213-555-0101",
  "address": "456 Business Avenue",
  "city": "Los Angeles",
  "country": "USA",
  "type": "Corporate"
}
```
→ Response: `CUST-2026-002`

**3. Customer 3 - FreightForwarder (Singapore)**
```json
{
  "fullName": "Asia Express Logistics",
  "email": "raj@asiaexpress.sg",
  "phone": "+65-6555-0105",
  "address": "Shipping Lane",
  "city": "Singapore",
  "country": "Singapore",
  "type": "FreightForwarder"
}
```
→ Response: `CUST-2026-003`

**4-8. Continue pattern...**

### Query Results After Creating 8 Customers
```bash
GET /api/v1/customers
Authorization: Bearer {token}
```

**Response**:
```json
{
  "success": true,
  "data": [
    // 8 customer objects
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 8,
  "totalPages": 1,
  "hasNextPage": false
}
```

**Filter Corporate**:
```bash
GET /api/v1/customers?type=Corporate
```
→ Returns customers 2, 5, 8 (3 corporate)

**Search by Name**:
```bash
GET /api/v1/customers?search=john
```
→ Returns customer 1

---

## 🎯 Key Points Summary

| Feature | Status | Notes |
|---------|--------|-------|
| Predefined Customers | ❌ NONE | Database empty on init |
| Multiple Customers | ✅ YES | Unlimited |
| Auto Customer Code | ✅ YES | CUST-2026-001, 002, etc. |
| Duplicate Prevention | ✅ YES | Email unique constraint |
| Pagination | ✅ YES | page, pageSize parameters |
| Filtering | ✅ YES | By type, status, search |
| Soft Delete | ✅ YES | Mark inactive, not deleted |
| Database Persistence | ✅ YES | All data stored indefinitely |
| Multi-Tenant Ready | ✅ YES | Ready for multiple organizations |

---

## 🔧 How It Works Under the Hood

### Request Flow for Creating Customer
```
1. Client: POST /api/v1/customers with customer data
2. Validation: Check all required fields ✓
3. Check Duplicate: Is email already used? ✗
4. Generate Code: Get next sequence for year → CUST-2026-{seq}
5. Create Entity: Customer.Create(...)
6. Save to DB: await _customerRepository.AddAsync(customer)
7. Return: 201 Created with customer details
```

### Repository Pattern
```csharp
// GetNextSequenceAsync - Auto-increments yearly
var count = await _context.Customers.CountAsync(
    c => c.CreatedAt.Year == year);
return count + 1;  // Returns: 1, 2, 3, ...

// ExistsAsync - Prevents duplicates
await _context.Customers.AnyAsync(
    c => c.Email == email);  // true if duplicate
```

### Database Tables
```
Customers Table:
- Id (GUID, Primary Key)
- CustomerCode (string, Unique) → CUST-2026-001
- FullName (string)
- Email (string, Unique)
- Type (string) → Individual | Corporate | FreightForwarder
- IsActive (bool) → true = active, false = soft-deleted
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- ... (other fields)

KycDocuments Table:
- Id (GUID)
- CustomerId (GUID, FK)
- DocumentType (string)
- BlobReference (string)
```

---

## ✨ Ready to Use!

**Your Customer Service is production-ready for managing unlimited customers:**

1. ✅ No seed data - fresh start
2. ✅ Auto customer code generation
3. ✅ Full CRUD operations
4. ✅ Pagination & filtering
5. ✅ Data persistence
6. ✅ Duplicate prevention
7. ✅ Soft delete support
8. ✅ KYC document attachment

**Start creating customers now!**
