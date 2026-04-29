using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "topgear",
                table: "PurchaseInvoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_CreatedBy",
                schema: "topgear",
                table: "PurchaseInvoices",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseInvoices_AspNetUsers_CreatedBy",
                schema: "topgear",
                table: "PurchaseInvoices",
                column: "CreatedBy",
                principalSchema: "topgear",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseInvoices_AspNetUsers_CreatedBy",
                schema: "topgear",
                table: "PurchaseInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseInvoices_CreatedBy",
                schema: "topgear",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "topgear",
                table: "PurchaseInvoices");
        }
    }
}
