using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiAiChat.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "ChatMessage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender",
                table: "ChatMessage");
        }
    }
}
