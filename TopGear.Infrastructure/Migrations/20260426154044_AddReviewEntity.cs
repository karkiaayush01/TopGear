using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartPrice",
                schema: "topgear",
                table: "Parts",
                newName: "SellingPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                schema: "topgear",
                table: "Parts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "topgear",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewType = table.Column<int>(type: "integer", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "topgear",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Parts_PartId",
                        column: x => x.PartId,
                        principalSchema: "topgear",
                        principalTable: "Parts",
                        principalColumn: "PartId");
                    table.ForeignKey(
                        name: "FK_Reviews_ServiceAppointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "topgear",
                        principalTable: "ServiceAppointments",
                        principalColumn: "AppointmentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AppointmentId",
                schema: "topgear",
                table: "Reviews",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                schema: "topgear",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PartId",
                schema: "topgear",
                table: "Reviews",
                column: "PartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "topgear");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                schema: "topgear",
                table: "Parts");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                schema: "topgear",
                table: "Parts",
                newName: "PartPrice");
        }
    }
}
