using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAudioTalesServer.Migrations
{
    public partial class nextpartid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_NextPartId",
                table: "Answers");

            migrationBuilder.AlterColumn<int>(
                name: "NextPartId",
                table: "Answers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_NextPartId",
                table: "Answers",
                column: "NextPartId",
                unique: true,
                filter: "[NextPartId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_NextPartId",
                table: "Answers");

            migrationBuilder.AlterColumn<int>(
                name: "NextPartId",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_NextPartId",
                table: "Answers",
                column: "NextPartId",
                unique: true);
        }
    }
}
