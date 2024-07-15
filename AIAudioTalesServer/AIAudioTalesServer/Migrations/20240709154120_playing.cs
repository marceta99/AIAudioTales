using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAudioTalesServer.Migrations
{
    public partial class playing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_NextPartId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ParentAnswerId",
                table: "BookParts");

            migrationBuilder.AddColumn<bool>(
                name: "IsPlaying",
                table: "BookParts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoot",
                table: "BookParts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PlayingPosition",
                table: "BookParts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.DropColumn(
                name: "IsPlaying",
                table: "BookParts");

            migrationBuilder.DropColumn(
                name: "IsRoot",
                table: "BookParts");

            migrationBuilder.DropColumn(
                name: "PlayingPosition",
                table: "BookParts");

            migrationBuilder.AddColumn<int>(
                name: "ParentAnswerId",
                table: "BookParts",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
