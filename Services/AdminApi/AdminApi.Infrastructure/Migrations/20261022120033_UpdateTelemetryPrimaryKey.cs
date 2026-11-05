using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTelemetryPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelemetryRecords",
                table: "TelemetryRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelemetryRecords",
                table: "TelemetryRecords",
                columns: new[] { "Id", "RecordedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelemetryRecords",
                table: "TelemetryRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelemetryRecords",
                table: "TelemetryRecords",
                column: "Id");
        }
    }
}
