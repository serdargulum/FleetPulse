using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLatitudeLongitude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "TelemetryRecords");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "TelemetryRecords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "TelemetryRecords",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "TelemetryRecords",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
