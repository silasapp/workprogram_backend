using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class ReasonPathFourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffPath",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT");

            migrationBuilder.RenameColumn(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffFileName",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                newName: "ReasonWhyOhmWasNotCommunicatedToStaff");

            migrationBuilder.AddColumn<int>(
                name: "Proposed_Well_Number",
                table: "INITIAL_WELL_COMPLETION_JOBS",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Proposed_Well_Number",
                table: "INITIAL_WELL_COMPLETION_JOBS");

            migrationBuilder.RenameColumn(
                name: "ReasonWhyOhmWasNotCommunicatedToStaff",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                newName: "ReasonWhyOhmWasNotCommunicatedToStaffFileName");

            migrationBuilder.AddColumn<string>(
                name: "ReasonWhyOhmWasNotCommunicatedToStaffPath",
                table: "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT",
                type: "varchar(3000)",
                unicode: false,
                maxLength: 3000,
                nullable: true);
        }
    }
}
