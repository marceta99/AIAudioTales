using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAudioTalesServer.Migrations
{
    public partial class removeaudiofrombook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioData",
                table: "Books");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "AudioData",
                table: "Books",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
