using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopGear.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ForgotPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "ForgotPasswordRequests",
                schema: "topgear",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    VerificationCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgotPasswordRequests", x => x.RequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForgotPasswordRequests_UserEmail",
                schema: "topgear",
                table: "ForgotPasswordRequests",
                column: "UserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForgotPasswordRequests",
                schema: "topgear");
        }
    }
}
