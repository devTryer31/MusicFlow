using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicFlow.DAL.SQLite.Migrations
{
    public partial class refresh_addons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Chats",
                type: "long",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpiresIn",
                table: "Chats",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ExpiresIn",
                table: "Chats");
        }
    }
}
