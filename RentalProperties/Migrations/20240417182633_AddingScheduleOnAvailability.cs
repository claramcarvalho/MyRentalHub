using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalProperties.Migrations
{
    /// <inheritdoc />
    public partial class AddingScheduleOnAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlreadyScheduled",
                table: "ManagerAvailabilities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlreadyScheduled",
                table: "ManagerAvailabilities");
        }
    }
}
