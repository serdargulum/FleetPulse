using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertTelemetryToHypertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SELECT create_hypertable(
                    '"TelemetryRecords"',
                    'RecordedAt',
                    migrate_data => TRUE,
                    if_not_exists => TRUE
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
