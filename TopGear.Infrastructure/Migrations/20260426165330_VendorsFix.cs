using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VendorsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Vendor_VendorId",
                schema: "topgear",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseInvoices_Vendor_VendorId",
                schema: "topgear",
                table: "PurchaseInvoices");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseInvoices_Vendors_VendorId",
                schema: "topgear",
                table: "PurchaseInvoices",
                column: "VendorId",
                principalSchema: "topgear",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Vendors_VendorId",
                schema: "topgear",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseInvoices_Vendors_VendorId",
                schema: "topgear",
                table: "PurchaseInvoices");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Vendor_VendorId",
                schema: "topgear",
                table: "Parts",
                column: "VendorId",
                principalSchema: "topgear",
                principalTable: "Vendor",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseInvoices_Vendor_VendorId",
                schema: "topgear",
                table: "PurchaseInvoices",
                column: "VendorId",
                principalSchema: "topgear",
                principalTable: "Vendor",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
