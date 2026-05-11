# Seed Data via API
$baseUrl = "http://localhost:5000/api/v1"

# 1. Login to get token
$loginPayload = @{
    email = "superadmin@cms.com"
    password = "Admin@123"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $loginPayload -ContentType "application/json"
$token = $loginResponse.data.token
$headers = @{ Authorization = "Bearer $token" }

Write-Host "Logged in. Token acquired."

# 2. Seed Customers
$customers = @(
    @{ name="Acme Corp"; email="logistics@acme.com"; phone="+1234567890"; address="123 Industrial Way" },
    @{ name="Global Freight"; email="shipping@globalfreight.com"; phone="+1987654321"; address="456 Port Road" }
)

$customerIds = @()
foreach ($c in $customers) {
    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/customers" -Method Post -Headers $headers -Body ($c | ConvertTo-Json) -ContentType "application/json"
        $customerIds += $res.data.id
        Write-Host "Created customer: $($c.name)"
    } catch {
        Write-Host "Failed to create customer: $($c.name)"
    }
}

# 3. Seed Shipments
if ($customerIds.Length -gt 0) {
    $shipments = @(
        @{ customerId=$customerIds[0]; origin="New York, NY"; destination="Los Angeles, CA"; weight=1500; dimensions="10x10x10"; serviceType="Express"; contentsDescription="Electronics" },
        @{ customerId=$customerIds[0]; origin="Chicago, IL"; destination="Houston, TX"; weight=2000; dimensions="20x20x20"; serviceType="Standard"; contentsDescription="Furniture" },
        @{ customerId=$customerIds[1]; origin="Miami, FL"; destination="Seattle, WA"; weight=500; dimensions="5x5x5"; serviceType="NextDay"; contentsDescription="Medical Supplies" }
    )

    $shipmentIds = @()
    foreach ($s in $shipments) {
        try {
            $res = Invoke-RestMethod -Uri "$baseUrl/shipments" -Method Post -Headers $headers -Body ($s | ConvertTo-Json) -ContentType "application/json"
            $shipmentIds += $res.data.id
            Write-Host "Created shipment: $($res.data.trackingNumber)"
        } catch {
            Write-Host "Failed to create shipment"
        }
    }

    # 4. Update Shipment Status to mock active/delivered shipments
    if ($shipmentIds.Length -ge 3) {
        # Shipment 1: InTransit
        $statusPayload = @{ status="InTransit"; notes="Departed facility" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$baseUrl/shipments/$($shipmentIds[0])/status" -Method Patch -Headers $headers -Body $statusPayload -ContentType "application/json"
        
        # Shipment 2: Delivered
        $statusPayload2 = @{ status="Delivered"; notes="Signed by receiver"; podSignatureData="mock-signature" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$baseUrl/shipments/$($shipmentIds[1])/status" -Method Patch -Headers $headers -Body $statusPayload2 -ContentType "application/json"
        
        Write-Host "Updated shipment statuses"
    }
}

Write-Host "Seeding complete."
