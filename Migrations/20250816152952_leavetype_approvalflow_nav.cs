using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WUIAM.Migrations
{
    /// <inheritdoc />
    public partial class leavetype_approvalflow_nav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_ApprovalFlowId",
                table: "LeaveTypes",
                column: "ApprovalFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveTypes_ApprovalFlows_ApprovalFlowId",
                table: "LeaveTypes",
                column: "ApprovalFlowId",
                principalTable: "ApprovalFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveTypes_ApprovalFlows_ApprovalFlowId",
                table: "LeaveTypes");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypes_ApprovalFlowId",
                table: "LeaveTypes");
        }
    }
}
