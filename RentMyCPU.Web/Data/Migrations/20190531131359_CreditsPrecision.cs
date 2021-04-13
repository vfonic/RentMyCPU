using Microsoft.EntityFrameworkCore.Migrations;

namespace RentMyCPU.Backend.Data.Migrations
{
    public partial class CreditsPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Credits",
                table: "AspNetUsers",
                type: "decimal(18, 6)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Credits",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 6)");
        }
    }
}
