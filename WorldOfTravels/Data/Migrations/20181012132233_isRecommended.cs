using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldOfTravels.Data.Migrations
{
    public partial class isRecommended : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Post",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecommended",
                table: "Post",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRecommended",
                table: "Post");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Post",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
