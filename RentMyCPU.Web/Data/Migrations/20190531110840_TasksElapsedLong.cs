using Microsoft.EntityFrameworkCore.Migrations;

namespace RentMyCPU.Backend.Data.Migrations
{
    public partial class TasksElapsedLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ElapsedMilliseconds",
                table: "WorkerTasks",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ElapsedMilliseconds",
                table: "WorkerTasks",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
