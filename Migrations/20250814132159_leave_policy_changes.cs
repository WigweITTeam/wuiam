using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WUIAM.Migrations
{
    /// <inheritdoc />
    public partial class leave_policy_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_LeavePolicy_EmploymentTypes_EmploymentTypeId1",
                //table: "LeavePolicy");

            //migrationBuilder.DropIndex(
            //    name: "IX_LeavePolicy_EmploymentTypeId1",
            //    table: "LeavePolicy");

            //migrationBuilder.DropColumn(
            //    name: "EmploymentTypeId1",
            //    table: "LeavePolicy");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmploymentTypeId",
                table: "LeavePolicy",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_EmploymentTypeId",
                table: "LeavePolicy",
                column: "EmploymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicy_EmploymentTypes_EmploymentTypeId",
                table: "LeavePolicy",
                column: "EmploymentTypeId",
                principalTable: "EmploymentTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeavePolicy_EmploymentTypes_EmploymentTypeId",
                table: "LeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicy_EmploymentTypeId",
                table: "LeavePolicy");

            migrationBuilder.AlterColumn<string>(
                name: "EmploymentTypeId",
                table: "LeavePolicy",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmploymentTypeId1",
                table: "LeavePolicy",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_EmploymentTypeId1",
                table: "LeavePolicy",
                column: "EmploymentTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicy_EmploymentTypes_EmploymentTypeId1",
                table: "LeavePolicy",
                column: "EmploymentTypeId1",
                principalTable: "EmploymentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
