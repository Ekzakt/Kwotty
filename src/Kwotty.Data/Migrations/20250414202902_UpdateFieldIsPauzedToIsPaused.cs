using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kwotty.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldIsPauzedToIsPaused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPauzed",
                schema: "kwotty",
                table: "UserSettings",
                newName: "IsPaused");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPaused",
                schema: "kwotty",
                table: "UserSettings",
                newName: "IsPauzed");
        }
    }
}
