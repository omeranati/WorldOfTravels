using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldOfTravels.Data.Migrations
{
    public partial class Initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploaderUserName",
                table: "Comment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploaderUserName",
                table: "Comment");
        }
    }
}
