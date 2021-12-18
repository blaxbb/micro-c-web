using Microsoft.EntityFrameworkCore.Migrations;

namespace micro_c_web.Server.Migrations
{
    public partial class itemcacheupc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "ItemCache",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UPC",
                table: "ItemCache",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemCache_SKU",
                table: "ItemCache",
                column: "SKU");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCache_UPC",
                table: "ItemCache",
                column: "UPC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemCache_SKU",
                table: "ItemCache");

            migrationBuilder.DropIndex(
                name: "IX_ItemCache_UPC",
                table: "ItemCache");

            migrationBuilder.DropColumn(
                name: "UPC",
                table: "ItemCache");

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "ItemCache",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
