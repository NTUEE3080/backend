using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitaPairing.Migrations
{
    public partial class AddedThreeWaySuggestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_UniqueChecker",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_UserId",
                table: "TwoWaySuggestions");

            migrationBuilder.CreateTable(
                name: "ThreeWaySuggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Counter = table.Column<long>(type: "bigint", nullable: false),
                    UniqueChecker = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Post1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Post1DataId = table.Column<Guid>(type: "uuid", nullable: true),
                    Post2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Post2DataId = table.Column<Guid>(type: "uuid", nullable: true),
                    Post3Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Post3DataId = table.Column<Guid>(type: "uuid", nullable: true),
                    TimeStamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreeWaySuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreeWaySuggestions_Posts_Post1DataId",
                        column: x => x.Post1DataId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThreeWaySuggestions_Posts_Post2DataId",
                        column: x => x.Post2DataId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThreeWaySuggestions_Posts_Post3DataId",
                        column: x => x.Post3DataId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThreeWaySuggestions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_UserId_UniqueChecker",
                table: "TwoWaySuggestions",
                columns: new[] { "UserId", "UniqueChecker" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_Post1DataId",
                table: "ThreeWaySuggestions",
                column: "Post1DataId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_Post2DataId",
                table: "ThreeWaySuggestions",
                column: "Post2DataId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_Post3DataId",
                table: "ThreeWaySuggestions",
                column: "Post3DataId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_UserId_UniqueChecker",
                table: "ThreeWaySuggestions",
                columns: new[] { "UserId", "UniqueChecker" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreeWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_UserId_UniqueChecker",
                table: "TwoWaySuggestions");

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_UniqueChecker",
                table: "TwoWaySuggestions",
                column: "UniqueChecker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_UserId",
                table: "TwoWaySuggestions",
                column: "UserId");
        }
    }
}
