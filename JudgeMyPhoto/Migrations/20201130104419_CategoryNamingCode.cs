using Microsoft.EntityFrameworkCore.Migrations;

namespace Marcware.JudgeMyPhoto.Migrations
{
    public partial class CategoryNamingCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoNamingThemeCode",
                table: "Category",
                type: "TEXT",
                maxLength: 3,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoNamingThemeCode",
                table: "Category");
        }
    }
}
