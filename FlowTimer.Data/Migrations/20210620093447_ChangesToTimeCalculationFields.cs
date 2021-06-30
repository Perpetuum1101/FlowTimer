using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlowTimer.Data.Migrations
{
    public partial class ChangesToTimeCalculationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentTimeSeconds",
                table: "Records",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Records",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalTimeMinutes",
                table: "Records",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTimeSeconds",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "TotalTimeMinutes",
                table: "Records");
        }
    }
}
