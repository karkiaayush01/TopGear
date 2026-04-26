using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVendorEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VendorName",
                schema: "topgear",
                table: "Vendors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "topgear",
                table: "Vendors",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                schema: "topgear",
                table: "Vendors",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                schema: "topgear",
                table: "Vendors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "topgear",
                table: "Vendors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "topgear",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "topgear",
                table: "Vendors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "topgear",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "topgear",
                table: "Vendors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.AlterColumn<string>(
                name: "VendorName",
                schema: "topgear",
                table: "Vendors",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
