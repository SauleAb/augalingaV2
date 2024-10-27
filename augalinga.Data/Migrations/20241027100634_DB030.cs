using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace augalinga.Data.Migrations
{
    /// <inheritdoc />
    public partial class DB030 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageName",
                table: "Notifications",
                newName: "Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "PageName");
        }
    }
}
