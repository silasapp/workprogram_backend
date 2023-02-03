using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class TypeOfProcessing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_SBU_ApplicationComments",
            //    table: "SBU_ApplicationComments");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_BUDGET_CAPEXs",
            //    table: "BUDGET_CAPEXs");

            //migrationBuilder.DropColumn(
            //    name: "Year",
            //    table: "RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE");

            //migrationBuilder.RenameTable(
            //    name: "BUDGET_CAPEXs",
            //    newName: "BUDGET_CAPEX");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Last_Qntr_Royalty",
            //    table: "Royalty",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Year",
            //    table: "NIGERIA_CONTENT_Upload_Succession_Plan",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type_of_Processing",
                table: "GEOPHYSICAL_ACTIVITIES_PROCESSING",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "CompanyId",
            //    table: "ADMIN_COMPANY_DETAILS",
            //    type: "varchar(50)",
            //    unicode: false,
            //    maxLength: 50,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Year_of_WP",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(300)",
            //    unicode: false,
            //    maxLength: 300,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Workover_Operations",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Updated_by",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(300)",
            //    unicode: false,
            //    maxLength: 300,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Turbines_Compressors",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Reprocessing",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(2000)",
            //    unicode: false,
            //    maxLength: 2000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Processing",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(3000)",
            //    unicode: false,
            //    maxLength: 3000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Pipelines",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Other_Equipment",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Other_Costs",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "OmL_Name",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "OmL_ID",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(400)",
            //    unicode: false,
            //    maxLength: 400,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Generators",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Flowlines",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Exploratory_Well_Drilling",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Development_Well_Drilling",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "Date_Updated",
            //    table: "BUDGET_CAPEX",
            //    type: "datetime",
            //    nullable: true,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime2",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "Date_Created",
            //    table: "BUDGET_CAPEX",
            //    type: "datetime",
            //    nullable: true,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime2",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Created_by",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(3000)",
            //    unicode: false,
            //    maxLength: 3000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Completions",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Companyemail",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(200)",
            //    unicode: false,
            //    maxLength: 200,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Company_ID",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(200)",
            //    unicode: false,
            //    maxLength: 200,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "CompanyName",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(2000)",
            //    unicode: false,
            //    maxLength: 2000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Civil_Works",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Buildings",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Appraisal_Well_Drilling",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Acquisition",
            //    table: "BUDGET_CAPEX",
            //    type: "varchar(3000)",
            //    unicode: false,
            //    maxLength: 3000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_BUDGET_CAPEX",
            //    table: "BUDGET_CAPEX",
            //    column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_BUDGET_CAPEX",
            //    table: "BUDGET_CAPEX");

            migrationBuilder.DropColumn(
                name: "Type_of_Processing",
                table: "GEOPHYSICAL_ACTIVITIES_PROCESSING");

            //migrationBuilder.RenameTable(
            //    name: "BUDGET_CAPEX",
            //    newName: "BUDGET_CAPEXs");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Last_Qntr_Royalty",
            //    table: "Royalty",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Year",
            //    table: "RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Year",
            //    table: "NIGERIA_CONTENT_Upload_Succession_Plan",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "CompanyId",
            //    table: "ADMIN_COMPANY_DETAILS",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(50)",
            //    oldUnicode: false,
            //    oldMaxLength: 50,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Year_of_WP",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(300)",
            //    oldUnicode: false,
            //    oldMaxLength: 300,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Workover_Operations",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Updated_by",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(300)",
            //    oldUnicode: false,
            //    oldMaxLength: 300,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Turbines_Compressors",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Reprocessing",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(2000)",
            //    oldUnicode: false,
            //    oldMaxLength: 2000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Processing",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(3000)",
            //    oldUnicode: false,
            //    oldMaxLength: 3000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Pipelines",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Other_Equipment",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Other_Costs",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "OmL_Name",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "OmL_ID",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(400)",
            //    oldUnicode: false,
            //    oldMaxLength: 400,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Generators",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Flowlines",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Exploratory_Well_Drilling",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Development_Well_Drilling",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "Date_Updated",
            //    table: "BUDGET_CAPEXs",
            //    type: "datetime2",
            //    nullable: true,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "Date_Created",
            //    table: "BUDGET_CAPEXs",
            //    type: "datetime2",
            //    nullable: true,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Created_by",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(3000)",
            //    oldUnicode: false,
            //    oldMaxLength: 3000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Completions",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Companyemail",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(200)",
            //    oldUnicode: false,
            //    oldMaxLength: 200,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Company_ID",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(200)",
            //    oldUnicode: false,
            //    oldMaxLength: 200,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "CompanyName",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(2000)",
            //    oldUnicode: false,
            //    oldMaxLength: 2000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Civil_Works",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Buildings",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Appraisal_Well_Drilling",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(1000)",
            //    oldUnicode: false,
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Acquisition",
            //    table: "BUDGET_CAPEXs",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(3000)",
            //    oldUnicode: false,
            //    oldMaxLength: 3000,
            //    oldNullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_SBU_ApplicationComments",
            //    table: "SBU_ApplicationComments",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_BUDGET_CAPEXs",
            //    table: "BUDGET_CAPEXs",
            //    column: "ID");
        }
    }
}
