using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class FifthWaste : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "NIGERIA_CONTENT_Upload_Succession_Plan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "ADMIN_COMPANY_DETAILS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LEGAL_LITIGATION",
                table: "LEGAL_LITIGATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LEGAL_ARBITRATION",
                table: "LEGAL_ARBITRATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_WASTE_MANAGEMENT_SYSTEM",
                table: "HSE_WASTE_MANAGEMENT_SYSTEM",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES",
                table: "FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_CODE",
                table: "ADMIN_COMPANY_CODE",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LEGAL_LITIGATION",
                table: "LEGAL_LITIGATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LEGAL_ARBITRATION",
                table: "LEGAL_ARBITRATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_WASTE_MANAGEMENT_SYSTEM",
                table: "HSE_WASTE_MANAGEMENT_SYSTEM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES",
                table: "FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_CODE",
                table: "ADMIN_COMPANY_CODE");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "NIGERIA_CONTENT_Upload_Succession_Plan");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ADMIN_COMPANY_DETAILS");
        }
    }
}
