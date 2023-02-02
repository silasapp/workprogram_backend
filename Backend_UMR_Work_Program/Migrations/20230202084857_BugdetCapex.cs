using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class BugdetCapex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RESERVES_UPDATES_LIFE_INDEX",
                table: "RESERVES_UPDATES_LIFE_INDEX",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
                table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
                table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU",
                table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RESERVES_UPDATES_LIFE_INDEX",
                table: "RESERVES_UPDATES_LIFE_INDEX");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
                table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
                table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU",
                table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE");
        }
    }
}
