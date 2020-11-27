using Microsoft.EntityFrameworkCore.Migrations;

namespace Marcware.JudgeMyPhoto.Migrations
{
    public partial class LinkUserToPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotographerId",
                table: "Photographs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photographs_PhotographerId",
                table: "Photographs",
                column: "PhotographerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photographs_AspNetUsers_PhotographerId",
                table: "Photographs",
                column: "PhotographerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photographs_AspNetUsers_PhotographerId",
                table: "Photographs");

            migrationBuilder.DropIndex(
                name: "IX_Photographs_PhotographerId",
                table: "Photographs");

            migrationBuilder.DropColumn(
                name: "PhotographerId",
                table: "Photographs");
        }
    }
}
