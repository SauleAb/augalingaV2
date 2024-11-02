using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace augalinga.Data.Migrations
{
    /// <inheritdoc />
    public partial class DB033 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAssignedToAllUsers",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectedUserIds",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssignedToAllUsers",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "SelectedUserIds",
                table: "Meetings");
        }
    }
}
