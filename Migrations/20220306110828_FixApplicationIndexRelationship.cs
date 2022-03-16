using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitaPairing.Migrations
{
    public partial class FixApplicationIndexRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indexes_Applications_ApplicationDataId",
                table: "Indexes");

            migrationBuilder.DropIndex(
                name: "IX_Indexes_ApplicationDataId",
                table: "Indexes");

            migrationBuilder.DropColumn(
                name: "ApplicationDataId",
                table: "Indexes");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationIndexes");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationDataId",
                table: "Indexes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Indexes_ApplicationDataId",
                table: "Indexes",
                column: "ApplicationDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Indexes_Applications_ApplicationDataId",
                table: "Indexes",
                column: "ApplicationDataId",
                principalTable: "Applications",
                principalColumn: "Id");
        }
    }
}
