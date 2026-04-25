using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PartsMangementFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendors",
                schema: "topgear",
                columns: table => new
                {
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.VendorId);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                schema: "topgear",
                columns: table => new
                {
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartName = table.Column<string>(type: "text", nullable: false),
                    PartPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.PartId);
                    table.ForeignKey(
                        name: "FK_Parts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "topgear",
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_VendorId",
                schema: "topgear",
                table: "Parts",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parts",
                schema: "topgear");

            migrationBuilder.DropTable(
                name: "Vendors",
                schema: "topgear");
        }
    }
}
