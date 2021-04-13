using Microsoft.EntityFrameworkCore.Migrations;

namespace RentMyCPU.Web.Data.Migrations
{
    public partial class WorkerConnexionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnexionId",
                table: "Workers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnexionId",
                table: "Workers");
        }
    }
}
