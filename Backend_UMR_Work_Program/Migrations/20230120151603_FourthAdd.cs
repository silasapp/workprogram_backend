using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class FourthAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "ADMIN_HSE_REMEDIATION_FUNDs");

            migrationBuilder.AddColumn<string>(
                name: "areThereRemediationFund",
                table: "HSE_REMEDIATION_FUND",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "areThereRemediationFund",
                table: "HSE_REMEDIATION_FUND");

            //migrationBuilder.CreateTable(
            //    name: "ADMIN_HSE_REMEDIATION_FUNDs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Company_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Company_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Companyemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_Of_Payment_Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Evidence_Of_Payment_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OML_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OML_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Reason_For_No_Remediation = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Year_of_WP = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ADMIN_HSE_REMEDIATION_FUNDs", x => x.Id);
            //    });
        }
    }
}
