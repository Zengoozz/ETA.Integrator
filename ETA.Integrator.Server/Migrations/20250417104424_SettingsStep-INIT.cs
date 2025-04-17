using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ETA.Integrator.Server.Migrations
{
    /// <inheritdoc />
    public partial class SettingsStepINIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SettingsSteps",
                columns: table => new
                {
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsSteps", x => x.Order);
                });

            migrationBuilder.InsertData(
                table: "SettingsSteps",
                columns: new[] { "Order", "Data", "Name" },
                values: new object[,]
                {
                    { 1, null, "connection-settings" },
                    { 2, null, "issuer-settings" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingsSteps");
        }
    }
}
