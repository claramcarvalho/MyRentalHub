using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalProperties.Migrations
{
    /// <inheritdoc />
    public partial class correctingSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerSlots_UserAccounts_SlotId",
                table: "ManagerSlots");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerSlots_ManagerId",
                table: "ManagerSlots",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerSlots_UserAccounts_ManagerId",
                table: "ManagerSlots",
                column: "ManagerId",
                principalTable: "UserAccounts",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerSlots_UserAccounts_ManagerId",
                table: "ManagerSlots");

            migrationBuilder.DropIndex(
                name: "IX_ManagerSlots_ManagerId",
                table: "ManagerSlots");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerSlots_UserAccounts_SlotId",
                table: "ManagerSlots",
                column: "SlotId",
                principalTable: "UserAccounts",
                principalColumn: "UserId");
        }
    }
}
