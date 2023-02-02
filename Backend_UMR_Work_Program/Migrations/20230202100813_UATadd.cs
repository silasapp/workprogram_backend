using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class UATadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME");

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

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_SBU_ApplicationComments",
            //    table: "SBU_ApplicationComments",
            //    column: "Id");

            migrationBuilder.CreateTable(
                name: "BUDGET_CAPEXs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OmL_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OmL_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Companyemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year_of_WP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyNumber = table.Column<int>(type: "int", nullable: true),
                    Acquisition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Processing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reprocessing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exploratory_Well_Drilling = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Appraisal_Well_Drilling = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Development_Well_Drilling = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Workover_Operations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Completions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flowlines = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pipelines = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Generators = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Turbines_Compressors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Buildings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other_Equipment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Civil_Works = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other_Costs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BUDGET_CAPEXs", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BUDGET_CAPEXs");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_SBU_ApplicationComments",
            //    table: "SBU_ApplicationComments");

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

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
            //    table: "HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME",
            //    column: "Id");
        }
    }
}
