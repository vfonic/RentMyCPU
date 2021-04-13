using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RentMyCPU.Backend.Data.Migrations
{
    public partial class Tasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "Credits",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "RequestorTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    ModificationDate = table.Column<DateTimeOffset>(nullable: false),
                    RequestorId = table.Column<Guid>(nullable: false),
                    WasmB64 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestorTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestorTasks_AspNetUsers_RequestorId",
                        column: x => x.RequestorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    ModificationDate = table.Column<DateTimeOffset>(nullable: false),
                    RequestorTaskId = table.Column<Guid>(nullable: false),
                    ProviderId = table.Column<Guid>(nullable: false),
                    Output = table.Column<string>(nullable: true),
                    Parameter = table.Column<int>(nullable: false),
                    Error = table.Column<string>(nullable: true),
                    IsSuccessful = table.Column<bool>(nullable: true),
                    IsExecuted = table.Column<bool>(nullable: false),
                    ElapsedMilliseconds = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerTasks_AspNetUsers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerTasks_RequestorTasks_RequestorTaskId",
                        column: x => x.RequestorTaskId,
                        principalTable: "RequestorTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestorTasks_RequestorId",
                table: "RequestorTasks",
                column: "RequestorId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerTasks_ProviderId",
                table: "WorkerTasks",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerTasks_RequestorTaskId",
                table: "WorkerTasks",
                column: "RequestorTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerTasks");

            migrationBuilder.DropTable(
                name: "RequestorTasks");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Credits",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "AspNetUsers");
        }
    }
}
