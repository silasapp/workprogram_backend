using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class CondensateProdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Last_Qntr_Royalty",
            //    table: "Royalty",
            //    type: "nvarchar(max)",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CondensateProd",
                table: "OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Year",
            //    table: "NIGERIA_CONTENT_Upload_Succession_Plan",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "CompanyId",
            //    table: "ADMIN_COMPANY_DETAILS",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_LEGAL_LITIGATION",
            //    table: "LEGAL_LITIGATION",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_LEGAL_ARBITRATION",
            //    table: "LEGAL_ARBITRATION",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_HSE_WASTE_MANAGEMENT_SYSTEM",
            //    table: "HSE_WASTE_MANAGEMENT_SYSTEM",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES",
            //    table: "FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_ADMIN_COMPANY_CODE",
            //    table: "ADMIN_COMPANY_CODE",
            //    column: "Id");

            //migrationBuilder.CreateTable(
            //    name: "HSE_WASTE_MANAGEMENT_DZs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        OML_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OML_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Companyemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Year_of_WP = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        COMPANY_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CompanyNumber = table.Column<int>(type: "int", nullable: true),
            //        Evidence_of_pay_of_DDCFilename = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_of_pay_of_DDCPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Waste_Contractor_Names = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Produce_Water_Manegent_Plan = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_of_Reinjection_Permit_Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_of_Reinjection_Permit_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Reason_for_No_Evidence_of_Reinjection = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Do_You_Have_Previous_Year_Waste_Inventory_Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_of_EWD_Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_of_EWD_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Reason_for_No_Evidence_of_EWD = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ACTUAL_year = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PROPOSED_year = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Date_Created = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Waste_Service_Permit_Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Waste_Service_Permit_Path = table.Column<double>(type: "float", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HSE_WASTE_MANAGEMENT_DZs", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "HSE_WASTE_MANAGEMENT_DZs");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_LEGAL_LITIGATION",
            //    table: "LEGAL_LITIGATION");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_LEGAL_ARBITRATION",
            //    table: "LEGAL_ARBITRATION");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_HSE_WASTE_MANAGEMENT_SYSTEM",
            //    table: "HSE_WASTE_MANAGEMENT_SYSTEM");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES",
            //    table: "FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVES");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_ADMIN_COMPANY_CODE",
            //    table: "ADMIN_COMPANY_CODE");

            //migrationBuilder.DropColumn(
            //    name: "Last_Qntr_Royalty",
            //    table: "Royalty");

            migrationBuilder.DropColumn(
                name: "CondensateProd",
                table: "OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED");

            //migrationBuilder.DropColumn(
            //    name: "Year",
            //    table: "NIGERIA_CONTENT_Upload_Succession_Plan");

            //migrationBuilder.DropColumn(
            //    name: "CompanyId",
            //    table: "ADMIN_COMPANY_DETAILS");
        }
    }
}
