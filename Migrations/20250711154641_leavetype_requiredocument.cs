using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WUIAM.Migrations
{
    /// <inheritdoc />
    public partial class leavetype_requiredocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireDocument",
                table: "LeaveTypes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportDocument",
                table: "LeaveRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireDocument",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "SupportDocument",
                table: "LeaveRequests");
        }
    }
}
