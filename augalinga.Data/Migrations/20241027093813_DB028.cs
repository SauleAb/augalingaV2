using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace augalinga.Data.Migrations
{
    /// <inheritdoc />
    public partial class DB028 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForUserId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ForUserId",
                table: "Notifications",
                column: "ForUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_ForUserId",
                table: "Notifications",
                column: "ForUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_ForUserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ForUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ForUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");
        }
    }
}
