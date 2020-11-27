using Microsoft.EntityFrameworkCore.Migrations;

namespace Marcware.JudgeMyPhoto.Migrations
{
    public partial class OrientationAndPhotoNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Orientation",
                table: "Photographs",
                type: "TEXT",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "UserCategoryPhotoNumber",
                table: "Photographs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "Photographs");

            migrationBuilder.DropColumn(
                name: "UserCategoryPhotoNumber",
                table: "Photographs");
        }
    }
}
