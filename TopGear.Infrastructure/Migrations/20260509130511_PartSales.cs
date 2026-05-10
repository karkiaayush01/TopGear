using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PartSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartSales",
                schema: "topgear",
                columns: table => new
                {
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsCredit = table.Column<bool>(type: "boolean", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartSales", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_PartSales_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "topgear",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartSales_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "topgear",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                schema: "topgear",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Make = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    PlateNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "topgear",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartSaleItems",
                schema: "topgear",
                columns: table => new
                {
                    SaleItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartSaleItems", x => x.SaleItemId);
                    table.ForeignKey(
                        name: "FK_PartSaleItems_PartSales_SaleId",
                        column: x => x.SaleId,
                        principalSchema: "topgear",
                        principalTable: "PartSales",
                        principalColumn: "SaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartSaleItems_Parts_PartId",
                        column: x => x.PartId,
                        principalSchema: "topgear",
                        principalTable: "Parts",
                        principalColumn: "PartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleItems_PartId",
                schema: "topgear",
                table: "PartSaleItems",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleItems_SaleId",
                schema: "topgear",
                table: "PartSaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSales_CreatedById",
                schema: "topgear",
                table: "PartSales",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartSales_CustomerId",
                schema: "topgear",
                table: "PartSales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId",
                schema: "topgear",
                table: "Vehicles",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartSaleItems",
                schema: "topgear");

            migrationBuilder.DropTable(
                name: "Vehicles",
                schema: "topgear");

            migrationBuilder.DropTable(
                name: "PartSales",
                schema: "topgear");
        }
    }
}
