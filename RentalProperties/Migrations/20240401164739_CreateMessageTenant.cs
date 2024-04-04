using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalProperties.Migrations
{
    /// <inheritdoc />
    public partial class CreateMessageTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_UserAccounts_ManagerId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ManagerId",
                table: "Properties");

            migrationBuilder.CreateTable(
                name: "MessagesFromTenants",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    ApartmentId = table.Column<int>(type: "int", nullable: false),
                    MessageSent = table.Column<string>(type: "text", nullable: false),
                    AnswerFromManager = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesFromTenants", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_MessagesFromTenants_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "ApartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessagesFromTenants_UserAccounts_TenantId",
                        column: x => x.TenantId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ManagerId",
                table: "Properties",
                column: "ManagerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessagesFromTenants_ApartmentId",
                table: "MessagesFromTenants",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesFromTenants_TenantId",
                table: "MessagesFromTenants",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_UserAccounts_ManagerId",
                table: "Properties",
                column: "ManagerId",
                principalTable: "UserAccounts",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_UserAccounts_ManagerId",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "MessagesFromTenants");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ManagerId",
                table: "Properties");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ManagerId",
                table: "Properties",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_UserAccounts_ManagerId",
                table: "Properties",
                column: "ManagerId",
                principalTable: "UserAccounts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
