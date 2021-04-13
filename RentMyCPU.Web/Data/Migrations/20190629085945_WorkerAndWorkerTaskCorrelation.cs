using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RentMyCPU.Web.Data.Migrations
{
    public partial class WorkerAndWorkerTaskCorrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerTasks_AspNetUsers_ProviderId",
                table: "WorkerTasks");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId",
                table: "WorkerTasks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkerTasks_WorkerId",
                table: "WorkerTasks",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerTasks_AspNetUsers_ProviderId",
                table: "WorkerTasks",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerTasks_Workers_WorkerId",
                table: "WorkerTasks",
                column: "WorkerId",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerTasks_AspNetUsers_ProviderId",
                table: "WorkerTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerTasks_Workers_WorkerId",
                table: "WorkerTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkerTasks_WorkerId",
                table: "WorkerTasks");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "WorkerTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerTasks_AspNetUsers_ProviderId",
                table: "WorkerTasks",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
