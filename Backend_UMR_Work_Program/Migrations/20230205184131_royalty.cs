using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class royalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OmlName",
                table: "Royalty",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Year",
            //    table: "RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE",
            //    type: "varchar(100)",
            //    unicode: false,
            //    maxLength: 100,
            //    nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Type_of_Processing",
            //    table: "GEOPHYSICAL_ACTIVITIES_PROCESSING",
            //    type: "varchar(500)",
            //    unicode: false,
            //    maxLength: 500,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.CreateTable(
            //    name: "BUDGET_OPEXes",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        OmL_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OmL_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Companyemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Year_of_WP = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Company_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CompanyNumber = table.Column<int>(type: "int", nullable: true),
            //        Variable_Cost = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Fixed_Cost = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Overheads = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Repairs_and_Maintenance_Cost = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        General_Expenses = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Date_Created = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BUDGET_OPEXes", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "BUDGET_OPEXes");

            migrationBuilder.DropColumn(
                name: "OmlName",
                table: "Royalty");

            //migrationBuilder.DropColumn(
            //    name: "Year",
            //    table: "RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Type_of_Processing",
            //    table: "GEOPHYSICAL_ACTIVITIES_PROCESSING",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(500)",
            //    oldUnicode: false,
            //    oldMaxLength: 500,
            //    oldNullable: true);
        }
    }
}
