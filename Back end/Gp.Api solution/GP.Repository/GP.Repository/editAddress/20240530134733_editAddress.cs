using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GP.Repository.GP.Repository.editAddress
{
    public partial class editAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NationalId",
                table: "Addresses",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Addresses");
        }
    }
}
