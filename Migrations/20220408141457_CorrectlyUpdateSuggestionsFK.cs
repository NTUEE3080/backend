using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitaPairing.Migrations
{
    public partial class CorrectlyUpdateSuggestionsFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post1DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post2DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post3DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post1DataId",
                table: "TwoWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post2DataId",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_Post1DataId",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_Post2DataId",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ThreeWaySuggestions_Post1DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ThreeWaySuggestions_Post2DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ThreeWaySuggestions_Post3DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropColumn(
                name: "Post1DataId",
                table: "TwoWaySuggestions");

            migrationBuilder.DropColumn(
                name: "Post2DataId",
                table: "TwoWaySuggestions");

            migrationBuilder.DropColumn(
                name: "Post1DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropColumn(
                name: "Post2DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropColumn(
                name: "Post3DataId",
                table: "ThreeWaySuggestions");

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_Post1Id",
                table: "TwoWaySuggestions",
                column: "Post1Id");

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_Post2Id",
                table: "TwoWaySuggestions",
                column: "Post2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_Post1Id",
                table: "ThreeWaySuggestions",
                column: "Post1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_Post2Id",
                table: "ThreeWaySuggestions",
                column: "Post2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThreeWaySuggestions_Post3Id",
                table: "ThreeWaySuggestions",
                column: "Post3Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post1Id",
                table: "ThreeWaySuggestions",
                column: "Post1Id",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post2Id",
                table: "ThreeWaySuggestions",
                column: "Post2Id",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post3Id",
                table: "ThreeWaySuggestions",
                column: "Post3Id",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post1Id",
                table: "TwoWaySuggestions",
                column: "Post1Id",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post2Id",
                table: "TwoWaySuggestions",
                column: "Post2Id",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post1Id",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post2Id",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post3Id",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post1Id",
                table: "TwoWaySuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post2Id",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_Post1Id",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_TwoWaySuggestions_Post2Id",
                table: "TwoWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ThreeWaySuggestions_Post1Id",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ThreeWaySuggestions_Post2Id",
                table: "ThreeWaySuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ThreeWaySuggestions_Post3Id",
                table: "ThreeWaySuggestions");

            migrationBuilder.AddColumn<Guid>(
                name: "Post1DataId",
                table: "TwoWaySuggestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Post2DataId",
                table: "TwoWaySuggestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Post1DataId",
                table: "ThreeWaySuggestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Post2DataId",
                table: "ThreeWaySuggestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Post3DataId",
                table: "ThreeWaySuggestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_Post1DataId",
                table: "TwoWaySuggestions",
                column: "Post1DataId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoWaySuggestions_Post2DataId",
                table: "TwoWaySuggestions",
                column: "Post2DataId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post1DataId",
                table: "ThreeWaySuggestions",
                column: "Post1DataId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post2DataId",
                table: "ThreeWaySuggestions",
                column: "Post2DataId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThreeWaySuggestions_Posts_Post3DataId",
                table: "ThreeWaySuggestions",
                column: "Post3DataId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post1DataId",
                table: "TwoWaySuggestions",
                column: "Post1DataId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TwoWaySuggestions_Posts_Post2DataId",
                table: "TwoWaySuggestions",
                column: "Post2DataId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
