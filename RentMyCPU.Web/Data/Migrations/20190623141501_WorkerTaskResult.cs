using Microsoft.EntityFrameworkCore.Migrations;

namespace RentMyCPU.Web.Data.Migrations
{
    public partial class WorkerTaskResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "WorkerTasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "WorkerTasks");
        }
    }
}
