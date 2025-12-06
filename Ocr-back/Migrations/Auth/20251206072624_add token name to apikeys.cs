using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ocr_back.Migrations.Auth
{
    /// <inheritdoc />
    public partial class addtokennametoapikeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                table: "AccessTokens",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenName",
                table: "AccessTokens");
        }
    }
}
