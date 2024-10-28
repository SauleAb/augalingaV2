using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace augalinga.Data.Migrations
{
    /// <inheritdoc />
    public partial class DB021 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Employee",
                table: "Meetings");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_UserId",
                table: "Meetings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Users_UserId",
                table: "Meetings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Users_UserId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_UserId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Meetings");

            migrationBuilder.AddColumn<string>(
                name: "Employee",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
