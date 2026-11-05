# ADR-009: Use EF Core for Schema Management and Direct SQL for Telemetry Persistence

## Status

Accepted

## Context

FleetPulse has different database access requirements across its services.

AdminApi manages the database schema and migrations, while StreamWorker handles high-volume telemetry ingestion, where persistence performance and explicit query control are important.

Using an ORM throughout the system would introduce unnecessary abstraction and overhead in the telemetry ingestion path.

## Decision

- Use Entity Framework Core in AdminApi for database schema management and migrations.
- Use direct SQL with Npgsql in StreamWorker for telemetry persistence.
- Keep database schema management separate from high-throughput data ingestion.

## Consequences

- EF Core provides structured migration management and schema evolution.
- Direct SQL gives StreamWorker explicit control over database operations.
- Avoids ORM overhead in the telemetry ingestion path.
- Requires maintaining SQL queries and database mappings manually in StreamWorker.
- Database schema changes must remain consistent with the SQL operations used by StreamWorker.