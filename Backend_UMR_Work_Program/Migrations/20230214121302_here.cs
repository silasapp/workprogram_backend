using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendUMRWorkProgram.Migrations
{
    /// <inheritdoc />
    public partial class here : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Geophysical_Activities",
            //    table: "Geophysical_Activities");

            //migrationBuilder.DropColumn(
            //    name: "Field_ID",
            //    table: "BUDGET_OPEX");

            //migrationBuilder.DropColumn(
            //    name: "TargetTo",
            //    table: "ApplicationProccesses");

            //migrationBuilder.DropColumn(
            //    name: "TriggeredBy",
            //    table: "ApplicationProccesses");

            migrationBuilder.AlterColumn<string>(
                name: "Waste_Service_Permit_Path",
                table: "HSE_WASTE_MANAGEMENT_DZs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "CurrentUserEmail",
            //    table: "Applications",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "FlowStageId",
            //    table: "Applications",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AlterColumn<string>(
            //    name: "UpdatedBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(500)",
            //    maxLength: 500,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Sort",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<int>(
            //    name: "SBU_ID",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<int>(
            //    name: "RoleID",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "ProcessStatus",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "ProcessAction",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "DeletedBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(500)",
            //    maxLength: 500,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "CreatedBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(500)",
            //    maxLength: 500,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "CategoryID",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AddColumn<int>(
            //    name: "TargetedToRole",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "TargetedToSBU",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "TriggeredByRole",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "TriggeredBySBU",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ActionDate",
            //    table: "ApplicationDeskHistories",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<string>(
            //    name: "AppAction",
            //    table: "ApplicationDeskHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Message",
            //    table: "ApplicationDeskHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TargetedTo",
            //    table: "ApplicationDeskHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TriggeredBy",
            //    table: "ApplicationDeskHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TriggeredByRole",
            //    table: "ApplicationDeskHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Geophysical_Activities_",
            //    table: "Geophysical_Activities",
            //    column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Geophysical_Activities_",
            //    table: "Geophysical_Activities");

            //migrationBuilder.DropColumn(
            //    name: "CurrentUserEmail",
            //    table: "Applications");

            //migrationBuilder.DropColumn(
            //    name: "FlowStageId",
            //    table: "Applications");

            //migrationBuilder.DropColumn(
            //    name: "TargetedToRole",
            //    table: "ApplicationProccesses");

            //migrationBuilder.DropColumn(
            //    name: "TargetedToSBU",
            //    table: "ApplicationProccesses");

            //migrationBuilder.DropColumn(
            //    name: "TriggeredByRole",
            //    table: "ApplicationProccesses");

            //migrationBuilder.DropColumn(
            //    name: "TriggeredBySBU",
            //    table: "ApplicationProccesses");

            //migrationBuilder.DropColumn(
            //    name: "ActionDate",
            //    table: "ApplicationDeskHistories");

            //migrationBuilder.DropColumn(
            //    name: "AppAction",
            //    table: "ApplicationDeskHistories");

            //migrationBuilder.DropColumn(
            //    name: "Message",
            //    table: "ApplicationDeskHistories");

            //migrationBuilder.DropColumn(
            //    name: "TargetedTo",
            //    table: "ApplicationDeskHistories");

            //migrationBuilder.DropColumn(
            //    name: "TriggeredBy",
            //    table: "ApplicationDeskHistories");

            //migrationBuilder.DropColumn(
            //    name: "TriggeredByRole",
            //    table: "ApplicationDeskHistories");

            migrationBuilder.AlterColumn<double>(
                name: "Waste_Service_Permit_Path",
                table: "HSE_WASTE_MANAGEMENT_DZs",
                type: "float",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "Field_ID",
            //    table: "BUDGET_OPEX",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "UpdatedBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(500)",
            //    oldMaxLength: 500,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Sort",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "SBU_ID",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "RoleID",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "ProcessStatus",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(100)",
            //    oldMaxLength: 100,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "ProcessAction",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(100)",
            //    oldMaxLength: 100,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "DeletedBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(500)",
            //    oldMaxLength: 500,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "CreatedBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(500)",
            //    oldMaxLength: 500,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "CategoryID",
            //    table: "ApplicationProccesses",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TargetTo",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TriggeredBy",
            //    table: "ApplicationProccesses",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Geophysical_Activities",
            //    table: "Geophysical_Activities",
            //    column: "Id");
        }
    }
}
