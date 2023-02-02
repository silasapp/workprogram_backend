using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_UMR_Work_Program.Migrations
{
    public partial class fieldName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_SBU_ApplicationComments",
            //    table: "SBU_ApplicationComments");

            //migrationBuilder.AddColumn<string>(
            //    name: "Last_Qntr_Royalty",
            //    table: "Royalty",
            //    type: "varchar(1000)",
            //    unicode: false,
            //    maxLength: 1000,
            //    nullable: true);

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
                name: "Field_Name",
                table: "HSE_WASTE_MANAGEMENT_DZs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "ADMIN_COMPANY_DETAILS",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Last_Qntr_Royalty",
            //    table: "Royalty");

            migrationBuilder.DropColumn(
                name: "Field_Name",
                table: "HSE_WASTE_MANAGEMENT_DZs");

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

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "ADMIN_COMPANY_DETAILS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_SBU_ApplicationComments",
            //    table: "SBU_ApplicationComments",
            //    column: "Id");
        }
    }
}
