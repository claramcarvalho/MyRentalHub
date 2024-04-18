using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalProperties.Migrations
{
    /// <inheritdoc />
    public partial class CreatingAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitDate",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "ManagerAvailabilities",
                columns: table => new
                {
                    ManagerAvailabilityId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    AvailableSlot = table.Column<DateTime>(type: "datetime2", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerAvailabilities");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "VisitDate",
                table: "Appointments",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
