using Microsoft.EntityFrameworkCore.Migrations;

namespace micro_c_web.Server.Migrations
{
    public partial class itemcachename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ItemCache",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ItemCache");
        }
    }
}
