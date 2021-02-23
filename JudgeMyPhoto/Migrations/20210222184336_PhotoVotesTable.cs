using Microsoft.EntityFrameworkCore.Migrations;

namespace Marcware.JudgeMyPhoto.Migrations
{
    public partial class PhotoVotesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhotoVotes",
                columns: table => new
                {
                    PhotoVoteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    PhotoId = table.Column<int>(type: "INTEGER", nullable: true),
                    VoterId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoVotes", x => x.PhotoVoteId);
                    table.ForeignKey(
                        name: "FK_PhotoVotes_AspNetUsers_VoterId",
                        column: x => x.VoterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotoVotes_Photographs_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photographs",
                        principalColumn: "PhotoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoVotes_PhotoId",
                table: "PhotoVotes",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoVotes_VoterId",
                table: "PhotoVotes",
                column: "VoterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoVotes");
        }
    }
}
