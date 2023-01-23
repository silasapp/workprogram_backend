using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class AddNewProperties2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HSE_REMEDIATION_FUND");

            migrationBuilder.DropTable(
                name: "HSE_POINT_SOURCE_REGISTRATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StrategicBusinessUnit",
                table: "StrategicBusinessUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff_UT",
                table: "staff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Presentation_Upload",
                table: "PRESENTATION_UPLOAD");

            migrationBuilder.DropPrimaryKey(
                name: "PK_permit_id",
                table: "PermitApprovals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_message_id",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WASTE_MANAGEMENT_NEW",
                table: "HSE_WASTE_MANAGEMENT_NEW");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW",
                table: "HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW");

            migrationBuilder.DropPrimaryKey(
                name: "PK_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW",
                table: "HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geophysical_Activities_Processing",
                table: "GEOPHYSICAL_ACTIVITIES_PROCESSING");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geophysical_Activities_",
                table: "Geophysical_Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Drilling_Operations_",
                table: "DRILLING_OPERATIONS_");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Concession_Situation",
                table: "CONCESSION_SITUATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_applications_id",
                table: "Applications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_application_desk_history_id",
                table: "ApplicationDeskHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_category_id",
                table: "ApplicationCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_Year",
                table: "ADMIN_YEAR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TYPE_OF_TEST",
                table: "ADMIN_TYPE_OF_TEST");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_SCRIBE_new",
                table: "ADMIN_SCRIBE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION",
                table: "ADMIN_COMPANY_INFORMATION_old_18052020");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_AUDIT_",
                table: "ADMIN_COMPANY_INFORMATION_AUDIT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_",
                table: "ADMIN_COMPANY_INFORMATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_CHAIRPERSON_new",
                table: "ADMIN_CHAIRPERSON");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_HSE_OPERATIONS_SAFETY_CASEs",
            //    table: "HSE_OPERATIONS_SAFETY_CASEs");

            //migrationBuilder.RenameTable(
            //    name: "HSE_OPERATIONS_SAFETY_CASEs",
            //    newName: "HSE_OPERATIONS_SAFETY_CASE");

            migrationBuilder.RenameColumn(
                name: "Type_Of_Facility",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                newName: "type_of_facility");

            migrationBuilder.RenameColumn(
                name: "Number_of_Facilities",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                newName: "number_of_facilities");

            migrationBuilder.RenameColumn(
                name: "OML_Name",
                table: "HSE_GHG_MANAGEMENT_PLAN",
                newName: "OmL_Name");

            migrationBuilder.RenameColumn(
                name: "Companyemail",
                table: "HSE_GHG_MANAGEMENT_PLAN",
                newName: "companyemail");

            migrationBuilder.RenameColumn(
                name: "COMPANY_ID",
                table: "HSE_GHG_MANAGEMENT_PLAN",
                newName: "CompanY_ID");

            migrationBuilder.AddColumn<string>(
                name: "CompletionWellName",
                table: "WORKOVERS_RECOMPLETION_JOBS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Proposed_Workover_Date",
                table: "WORKOVERS_RECOMPLETION_JOBS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Proposed_Completion_Days",
                table: "INITIAL_WELL_COMPLETION_JOBS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Proposed_Initial_Name",
                table: "INITIAL_WELL_COMPLETION_JOBS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type_of_facility",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "number_of_facilities",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EvidenceOfTrainingPlanPath",
                table: "HSE_SAFETY_CULTURE_TRAINING",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EvidenceOfTrainingPlanFilename",
                table: "HSE_SAFETY_CULTURE_TRAINING",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreThereTrainingPlansForHSE",
                table: "HSE_SAFETY_CULTURE_TRAINING",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WasOhmPolicyCommunicatedToStaff",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffPath",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "varchar(3000)",
                unicode: false,
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffFileName",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonForNoOhm",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DoYouHaveAnOhm",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "HSE_ENVIRONMENTAL_MANAGEMENT_PLAN",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .Annotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "HSE_EFFLUENT_MONITORING_COMPLIANCE",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UploadIncidentStatisticsPath",
                table: "HSE_ACCIDENT_INCIDENCE_REPORTING_NEW",
                type: "varchar(3000)",
                unicode: false,
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UploadIncidentStatisticsFilename",
                table: "HSE_ACCIDENT_INCIDENCE_REPORTING_NEW",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnnualForecastCondensate",
                table: "GAS_PRODUCTION_ACTIVITIES",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnnualForecastGasAg",
                table: "GAS_PRODUCTION_ACTIVITIES",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnnualForecastGasNag",
                table: "GAS_PRODUCTION_ACTIVITIES",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnnualForecastOil",
                table: "GAS_PRODUCTION_ACTIVITIES",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reasonForNoEvidence",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfDesignSafetyCaseApprovalPath",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfDesignSafetyCaseApprovalFilename",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "areThereEvidenceOfDesignSafetyCaseApproval",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year_of_WP",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Updated_by",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type_of_Facility",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason_If_No_Evidence",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(2000)",
                unicode: false,
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OML_Name",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OML_ID",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number_of_Facilities",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name_Of_Facility",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location_of_Facility",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Evidence_of_Operations_Safety_Case_Approval",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(3000)",
                unicode: false,
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Does_the_Facility_Have_a_Valid_Safety_Case",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date_Updated",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date_Created",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Created_by",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Companyemail",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "COMPANY_ID",
                table: "HSE_OPERATIONS_SAFETY_CASE",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "HSE_OPERATIONS_SAFETY_CASE",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StrategicBusinessUnits",
                table: "StrategicBusinessUnits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_staff",
                table: "staff",
                column: "StaffID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PRESENTATION_UPLOAD",
                table: "PRESENTATION_UPLOAD",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermitApprovals",
                table: "PermitApprovals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_WASTE_MANAGEMENT_NEW",
                table: "HSE_WASTE_MANAGEMENT_NEW",
                column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_HSE_SAFETY_CULTURE_TRAINING",
            //    table: "HSE_SAFETY_CULTURE_TRAINING",
            //    column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW",
                table: "HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW",
                table: "HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GEOPHYSICAL_ACTIVITIES_PROCESSING",
                table: "GEOPHYSICAL_ACTIVITIES_PROCESSING",
                column: "Geophysical_Activities_ProcessingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geophysical_Activities_",
                table: "Geophysical_Activities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DRILLING_OPERATIONS_",
                table: "DRILLING_OPERATIONS_",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CONCESSION_SITUATION",
                table: "CONCESSION_SITUATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationDeskHistories",
                table: "ApplicationDeskHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationCategories",
                table: "ApplicationCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_YEAR",
                table: "ADMIN_YEAR",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_TYPE_OF_TEST",
                table: "ADMIN_TYPE_OF_TEST",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_SCRIBE",
                table: "ADMIN_SCRIBE",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_old_18052020",
                table: "ADMIN_COMPANY_INFORMATION_old_18052020",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_AUDIT",
                table: "ADMIN_COMPANY_INFORMATION_AUDIT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION",
                table: "ADMIN_COMPANY_INFORMATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_CHAIRPERSON",
                table: "ADMIN_CHAIRPERSON",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "HSE_POINT_SOURCE_REGISTRATION",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OML_Name = table.Column<string>(type: "varchar(3000)", unicode: false, maxLength: 3000, nullable: true),
                    OML_ID = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CompanyName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Company_Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Year_of_WP = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    Company_Number = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    Company_ID = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    are_there_point_source_permit = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    evidence_of_PSP_filename = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    evidence_of_PSP_path = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    reason_for_no_PSP = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSE_POINT_SOURCE_REGISTRATION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HSE_REMEDIATION_FUND",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OML_Name = table.Column<string>(type: "varchar(3000)", unicode: false, maxLength: 3000, nullable: true),
                    OML_ID = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CompanyName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Company_Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Year_of_WP = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    Company_Number = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    Company_ID = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    evidenceOfPaymentFilename = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    evidenceOfPaymentPath = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    reasonForNoRemediation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSE_REMEDIATION_FUND", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HSE_POINT_SOURCE_REGISTRATION");

            migrationBuilder.DropTable(
                name: "HSE_REMEDIATION_FUND");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StrategicBusinessUnits",
                table: "StrategicBusinessUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_staff",
                table: "staff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PRESENTATION_UPLOAD",
                table: "PRESENTATION_UPLOAD");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermitApprovals",
                table: "PermitApprovals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_WASTE_MANAGEMENT_NEW",
                table: "HSE_WASTE_MANAGEMENT_NEW");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_SAFETY_CULTURE_TRAINING",
                table: "HSE_SAFETY_CULTURE_TRAINING");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW",
                table: "HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW",
                table: "HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GEOPHYSICAL_ACTIVITIES_PROCESSING",
                table: "GEOPHYSICAL_ACTIVITIES_PROCESSING");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Geophysical_Activities",
                table: "Geophysical_Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DRILLING_OPERATIONS_",
                table: "DRILLING_OPERATIONS_");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CONCESSION_SITUATION",
                table: "CONCESSION_SITUATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationDeskHistories",
                table: "ApplicationDeskHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationCategories",
                table: "ApplicationCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_YEAR",
                table: "ADMIN_YEAR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_TYPE_OF_TEST",
                table: "ADMIN_TYPE_OF_TEST");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_SCRIBE",
                table: "ADMIN_SCRIBE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_old_18052020",
                table: "ADMIN_COMPANY_INFORMATION_old_18052020");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_AUDIT",
                table: "ADMIN_COMPANY_INFORMATION_AUDIT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION",
                table: "ADMIN_COMPANY_INFORMATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMIN_CHAIRPERSON",
                table: "ADMIN_CHAIRPERSON");

            migrationBuilder.DropColumn(
                name: "CompletionWellName",
                table: "WORKOVERS_RECOMPLETION_JOBS");

            migrationBuilder.DropColumn(
                name: "Proposed_Workover_Date",
                table: "WORKOVERS_RECOMPLETION_JOBS");

            migrationBuilder.DropColumn(
                name: "Proposed_Completion_Days",
                table: "INITIAL_WELL_COMPLETION_JOBS");

            migrationBuilder.DropColumn(
                name: "Proposed_Initial_Name",
                table: "INITIAL_WELL_COMPLETION_JOBS");

            migrationBuilder.DropColumn(
                name: "AnnualForecastCondensate",
                table: "GAS_PRODUCTION_ACTIVITIES");

            migrationBuilder.DropColumn(
                name: "AnnualForecastGasAg",
                table: "GAS_PRODUCTION_ACTIVITIES");

            migrationBuilder.DropColumn(
                name: "AnnualForecastGasNag",
                table: "GAS_PRODUCTION_ACTIVITIES");

            migrationBuilder.DropColumn(
                name: "AnnualForecastOil",
                table: "GAS_PRODUCTION_ACTIVITIES");

            migrationBuilder.RenameTable(
                name: "HSE_OPERATIONS_SAFETY_CASE",
                newName: "HSE_OPERATIONS_SAFETY_CASEs");

            migrationBuilder.RenameColumn(
                name: "type_of_facility",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                newName: "Type_Of_Facility");

            migrationBuilder.RenameColumn(
                name: "number_of_facilities",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                newName: "Number_of_Facilities");

            migrationBuilder.RenameColumn(
                name: "companyemail",
                table: "HSE_GHG_MANAGEMENT_PLAN",
                newName: "Companyemail");

            migrationBuilder.RenameColumn(
                name: "OmL_Name",
                table: "HSE_GHG_MANAGEMENT_PLAN",
                newName: "OML_Name");

            migrationBuilder.RenameColumn(
                name: "CompanY_ID",
                table: "HSE_GHG_MANAGEMENT_PLAN",
                newName: "COMPANY_ID");

            migrationBuilder.AlterColumn<string>(
                name: "Type_Of_Facility",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number_of_Facilities",
                table: "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EvidenceOfTrainingPlanPath",
                table: "HSE_SAFETY_CULTURE_TRAINING",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EvidenceOfTrainingPlanFilename",
                table: "HSE_SAFETY_CULTURE_TRAINING",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreThereTrainingPlansForHSE",
                table: "HSE_SAFETY_CULTURE_TRAINING",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WasOhmPolicyCommunicatedToStaff",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffPath",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3000)",
                oldUnicode: false,
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffFileName",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonForNoOhm",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DoYouHaveAnOhm",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "HSE_ENVIRONMENTAL_MANAGEMENT_PLAN",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "HSE_EFFLUENT_MONITORING_COMPLIANCE",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UploadIncidentStatisticsPath",
                table: "HSE_ACCIDENT_INCIDENCE_REPORTING_NEW",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3000)",
                oldUnicode: false,
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UploadIncidentStatisticsFilename",
                table: "HSE_ACCIDENT_INCIDENCE_REPORTING_NEW",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reasonForNoEvidence",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfDesignSafetyCaseApprovalPath",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "evidenceOfDesignSafetyCaseApprovalFilename",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "areThereEvidenceOfDesignSafetyCaseApproval",
                table: "FACILITIES_PROJECT_PERFORMANCE",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year_of_WP",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Updated_by",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type_of_Facility",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason_If_No_Evidence",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldUnicode: false,
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OML_Name",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OML_ID",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Number_of_Facilities",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name_Of_Facility",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location_of_Facility",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000,
                oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "HSE_OPERATIONS_SAFETY_CASEs",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true)
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Evidence_of_Operations_Safety_Case_Approval",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3000)",
                oldUnicode: false,
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Does_the_Facility_Have_a_Valid_Safety_Case",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date_Updated",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date_Created",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Created_by",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Companyemail",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "COMPANY_ID",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StrategicBusinessUnit",
                table: "StrategicBusinessUnits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff_UT",
                table: "staff",
                column: "StaffID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Presentation_Upload",
                table: "PRESENTATION_UPLOAD",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_permit_id",
                table: "PermitApprovals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_message_id",
                table: "Messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WASTE_MANAGEMENT_NEW",
                table: "HSE_WASTE_MANAGEMENT_NEW",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW",
                table: "HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW",
                table: "HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geophysical_Activities_Processing",
                table: "GEOPHYSICAL_ACTIVITIES_PROCESSING",
                column: "Geophysical_Activities_ProcessingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Geophysical_Activities_",
                table: "Geophysical_Activities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Drilling_Operations_",
                table: "DRILLING_OPERATIONS_",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Concession_Situation",
                table: "CONCESSION_SITUATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_applications_id",
                table: "Applications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_application_desk_history_id",
                table: "ApplicationDeskHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_category_id",
                table: "ApplicationCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_Year",
                table: "ADMIN_YEAR",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TYPE_OF_TEST",
                table: "ADMIN_TYPE_OF_TEST",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_SCRIBE_new",
                table: "ADMIN_SCRIBE",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION",
                table: "ADMIN_COMPANY_INFORMATION_old_18052020",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_AUDIT_",
                table: "ADMIN_COMPANY_INFORMATION_AUDIT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_COMPANY_INFORMATION_",
                table: "ADMIN_COMPANY_INFORMATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMIN_CHAIRPERSON_new",
                table: "ADMIN_CHAIRPERSON",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HSE_OPERATIONS_SAFETY_CASEs",
                table: "HSE_OPERATIONS_SAFETY_CASEs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ADMIN_HSE_REMEDIATION_FUNDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Companyemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evidence_Of_Payment_Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evidence_Of_Payment_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OML_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OML_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason_For_No_Remediation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year_of_WP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADMIN_HSE_REMEDIATION_FUNDs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HSE_POINT_SOURCE_REGISTRATIONs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Companyemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OML_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OML_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year_of_WP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    areTherePointSourcePermit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    evidenceOfPSPFilename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    evidenceOfPSPPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reasonForNoPSP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSE_POINT_SOURCE_REGISTRATIONs", x => x.Id);
                });
        }
    }
}
