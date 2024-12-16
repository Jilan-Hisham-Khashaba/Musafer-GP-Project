using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GP.Repository.identity.Migrations
{
    public partial class editphoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<string>(
                name: "PhotoPicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPicture",
                table: "AspNetUsers");

            
               
        }
    }
}
