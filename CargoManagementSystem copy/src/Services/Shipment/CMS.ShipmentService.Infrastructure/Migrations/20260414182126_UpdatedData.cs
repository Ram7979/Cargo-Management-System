using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.ShipmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LocationLatitude",
                table: "ShipmentStatusHistories",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                table: "ShipmentStatusHistories",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LocationRecordedAt",
                table: "ShipmentStatusHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Shipments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CargoType",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryOfOrigin",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredValue",
                table: "Shipments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryDate",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HsCode",
                table: "Shipments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDangerousGoods",
                table: "Shipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastFailureReason",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMode",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Shipments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReDeliveryScheduledAt",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientCity",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientContact",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientCountry",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "Shipments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientZip",
                table: "Shipments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderCity",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderContact",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderCountry",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Shipments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderZip",
                table: "Shipments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeCbm",
                table: "Shipments",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationLatitude",
                table: "ShipmentStatusHistories");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                table: "ShipmentStatusHistories");

            migrationBuilder.DropColumn(
                name: "LocationRecordedAt",
                table: "ShipmentStatusHistories");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CargoType",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CountryOfOrigin",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "HsCode",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "IsDangerousGoods",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "LastFailureReason",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ReDeliveryScheduledAt",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "RecipientCity",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "RecipientContact",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "RecipientCountry",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "RecipientZip",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SenderCity",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SenderContact",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SenderCountry",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SenderZip",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "VolumeCbm",
                table: "Shipments");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
