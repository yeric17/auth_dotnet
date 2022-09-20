using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThulloWebAPI.Migrations
{
    public partial class Initial05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Expires",
                table: "Tokens",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expires",
                table: "Tokens");
        }
    }
}
