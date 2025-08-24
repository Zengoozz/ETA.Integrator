using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETA.Integrator.Server.Migrations
{
    /// <inheritdoc />
    public partial class RejectionReasonJSONColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReasonJSON",
                table: "InvoiceSubmissionLogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReasonJSON",
                table: "InvoiceSubmissionLogs");
        }
    }
}
