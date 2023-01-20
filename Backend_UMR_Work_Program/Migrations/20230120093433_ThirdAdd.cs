using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class ThirdAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_OPERATIONS_SAFETY_CASE",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_OPERATIONS_SAFETY_CASE",
                table: "HSE_OPERATIONS_SAFETY_CASE");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
