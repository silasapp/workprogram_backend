using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class ReasonFourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reasonForNoRemediation",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfPaymentPath",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfPaymentFilename",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year_of_WP",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Number",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_ID",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Email",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reason_for_no_PSP",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidence_of_PSP_path",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidence_of_PSP_filename",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "are_there_point_source_permit",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year_of_WP",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Number",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_ID",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Email",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SBU_ApplicationComments",
                table: "SBU_ApplicationComments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM",
                table: "HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY",
                table: "HSE_CLIMATE_CHANGE_AND_AIR_QUALITY",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SBU_ApplicationComments",
                table: "SBU_ApplicationComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM",
                table: "HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY",
                table: "HSE_CLIMATE_CHANGE_AND_AIR_QUALITY");

            migrationBuilder.AlterColumn<string>(
                name: "reasonForNoRemediation",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfPaymentPath",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfPaymentFilename",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year_of_WP",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Number",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_ID",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Email",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "HSE_REMEDIATION_FUND",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reason_for_no_PSP",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidence_of_PSP_path",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidence_of_PSP_filename",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "are_there_point_source_permit",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year_of_WP",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Number",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_ID",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company_Email",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "HSE_POINT_SOURCE_REGISTRATION",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);
        }
    }
}
