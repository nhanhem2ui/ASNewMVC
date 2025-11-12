using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    ChatId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    SenderId = table.Column<short>(type: "smallint", maxLength: 20, nullable: false),
                    ReceiverId = table.Column<short>(type: "smallint", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.ChatId);
                    table.ForeignKey(
                        name: "FK_Chat_Receiver",
                        column: x => x.ReceiverId,
                        principalTable: "SystemAccount",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chat_Sender",
                        column: x => x.SenderId,
                        principalTable: "SystemAccount",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_ReceiverId",
                table: "Chat",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_SenderId",
                table: "Chat",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chat");
        }
    }
}
