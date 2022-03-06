using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitaPairing.Migrations
{
    public partial class UpdateSemester : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Current",
                table: "Semester",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Current",
                table: "Semester");
        }
    }
}
