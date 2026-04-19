using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.ReportingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dataUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipmentReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestinationAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DestinationCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestinationCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CargoType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VehicleId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_CargoType",
                table: "ShipmentReadModels",
                column: "CargoType");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_CreatedAt",
                table: "ShipmentReadModels",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_CustomerCode",
                table: "ShipmentReadModels",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_DestinationCity_DestinationCountry",
                table: "ShipmentReadModels",
                columns: new[] { "DestinationCity", "DestinationCountry" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_DriverId",
                table: "ShipmentReadModels",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_OriginCity_OriginCountry",
                table: "ShipmentReadModels",
                columns: new[] { "OriginCity", "OriginCountry" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_Status",
                table: "ShipmentReadModels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentReadModels_TrackingNumber",
                table: "ShipmentReadModels",
                column: "TrackingNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentReadModels");
        }
    }
}
