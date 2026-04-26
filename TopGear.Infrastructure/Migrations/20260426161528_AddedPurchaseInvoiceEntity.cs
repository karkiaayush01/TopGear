using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPurchaseInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Vendors_VendorId",
                schema: "topgear",
                table: "Parts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendors",
                schema: "topgear",
                table: "Vendors");

            migrationBuilder.RenameTable(
                name: "Vendors",
                schema: "topgear",
                newName: "Vendor",
                newSchema: "topgear");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendor",
                schema: "topgear",
                table: "Vendor",
                column: "VendorId");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                schema: "topgear",
                columns: table => new
                {
                    PurchaseInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoices", x => x.PurchaseInvoiceId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "topgear",
                        principalTable: "Vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceItems",
                schema: "topgear",
                columns: table => new
                {
                    PurchaseInvoiceItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceItems", x => x.PurchaseInvoiceItemId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceItems_Parts_PartId",
                        column: x => x.PartId,
                        principalSchema: "topgear",
                        principalTable: "Parts",
                        principalColumn: "PartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceItems_PurchaseInvoices_PurchaseInvoiceId",
                        column: x => x.PurchaseInvoiceId,
                        principalSchema: "topgear",
                        principalTable: "PurchaseInvoices",
                        principalColumn: "PurchaseInvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceItems_PartId",
                schema: "topgear",
                table: "PurchaseInvoiceItems",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceItems_PurchaseInvoiceId",
                schema: "topgear",
                table: "PurchaseInvoiceItems",
                column: "PurchaseInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_VendorId",
                schema: "topgear",
                table: "PurchaseInvoices",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Vendor_VendorId",
                schema: "topgear",
                table: "Parts",
                column: "VendorId",
                principalSchema: "topgear",
                principalTable: "Vendor",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Vendor_VendorId",
                schema: "topgear",
                table: "Parts");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceItems",
                schema: "topgear");

            migrationBuilder.DropTable(
                name: "PurchaseInvoices",
                schema: "topgear");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendor",
                schema: "topgear",
                table: "Vendor");

            migrationBuilder.RenameTable(
                name: "Vendor",
                schema: "topgear",
                newName: "Vendors",
                newSchema: "topgear");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendors",
                schema: "topgear",
                table: "Vendors",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Vendors_VendorId",
                schema: "topgear",
                table: "Parts",
                column: "VendorId",
                principalSchema: "topgear",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
