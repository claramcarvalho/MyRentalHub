using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalProperties.Migrations
{
    /// <inheritdoc />
    public partial class slots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagerSlots",
                columns: table => new
                {
                    SlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    AvailableSlot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAlreadyScheduled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerSlots", x => x.SlotId);
                    table.ForeignKey(
                        name: "FK_ManagerSlots_UserAccounts_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerSlots_ManagerId",
                table: "ManagerSlots",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerSlots");
        }
    }
}
