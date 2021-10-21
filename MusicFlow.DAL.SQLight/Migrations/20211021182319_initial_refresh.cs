using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicFlow.DAL.SQLite.Migrations
{
    public partial class initial_refresh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Chats",
                newName: "TelegramChatId");

            migrationBuilder.AddColumn<long>(
                name: "HostUserId",
                table: "Chats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Chats",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostUserId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "TelegramChatId",
                table: "Chats",
                newName: "ChatId");
        }
    }
}
