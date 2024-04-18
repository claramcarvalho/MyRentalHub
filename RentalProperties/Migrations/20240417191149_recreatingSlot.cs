using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalProperties.Migrations
{
    /// <inheritdoc />
    public partial class recreatingSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerAvailabilities");

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
                        name: "FK_ManagerSlots_UserAccounts_SlotId",
                        column: x => x.SlotId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserId");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerSlots");

            migrationBuilder.CreateTable(
                name: "ManagerAvailabilities",
                columns: table => new
                {
                    ManagerAvailabilityId = table.Column<int>(type: "int", nullable: false),
                    AvailableSlot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAlreadyScheduled = table.Column<bool>(type: "bit", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerAvailabilities", x => x.ManagerAvailabilityId);
                    table.ForeignKey(
                        name: "FK_ManagerAvailabilities_UserAccounts_ManagerAvailabilityId",
                        column: x => x.ManagerAvailabilityId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
