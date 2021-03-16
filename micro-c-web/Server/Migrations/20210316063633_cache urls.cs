using Microsoft.EntityFrameworkCore.Migrations;

namespace micro_c_web.Server.Migrations
{
    public partial class cacheurls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ItemCache",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ItemCache");
        }
    }
}
