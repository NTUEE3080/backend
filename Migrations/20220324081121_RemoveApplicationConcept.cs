using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitaPairing.Migrations
{
    public partial class RemoveApplicationConcept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "ApplicationIndexes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Applications",
                newName: "ApplierPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                newName: "IX_Applications_ApplierPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_PostId_UserId",
                table: "Applications",
                newName: "IX_Applications_PostId_ApplierPostId");

            migrationBuilder.AddColumn<Guid>(
                name: "IndexDataId",
                table: "Applications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_IndexDataId",
                table: "Applications",
                column: "IndexDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Indexes_IndexDataId",
                table: "Applications",
                column: "IndexDataId",
                principalTable: "Indexes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Posts_ApplierPostId",
                table: "Applications",
                column: "ApplierPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Indexes_IndexDataId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Posts_ApplierPostId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_IndexDataId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IndexDataId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "ApplierPostId",
                table: "Applications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_PostId_ApplierPostId",
                table: "Applications",
                newName: "IX_Applications_PostId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_ApplierPostId",
                table: "Applications",
                newName: "IX_Applications_UserId");

            migrationBuilder.CreateTable(
                name: "ApplicationIndexes",
                columns: table => new
                {
                    OffersId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedApplicationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationIndexes", x => new { x.OffersId, x.RelatedApplicationsId });
                    table.ForeignKey(
                        name: "FK_ApplicationIndexes_Applications_RelatedApplicationsId",
                        column: x => x.RelatedApplicationsId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationIndexes_Indexes_OffersId",
                        column: x => x.OffersId,
                        principalTable: "Indexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationIndexes_RelatedApplicationsId",
                table: "ApplicationIndexes",
                column: "RelatedApplicationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
