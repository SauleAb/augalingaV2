using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace augalinga.Data.Migrations
{
    /// <inheritdoc />
    public partial class DB020 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Background",
                table: "Meetings");

            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Background",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
