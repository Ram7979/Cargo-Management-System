using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.ShipmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DestinationAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CargoDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BolDocumentUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PodImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PodSignatureData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RefundEligible = table.Column<bool>(type: "bit", nullable: false),
                    LastLocationLatitude = table.Column<double>(type: "float", nullable: true),
                    LastLocationLongitude = table.Column<double>(type: "float", nullable: true),
                    LastLocationRecordedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentStatusHistories_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentStatusHistories_ShipmentId",
                table: "ShipmentStatusHistories",
                column: "ShipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentStatusHistories");

            migrationBuilder.DropTable(
                name: "Shipments");
        }
    }
}
