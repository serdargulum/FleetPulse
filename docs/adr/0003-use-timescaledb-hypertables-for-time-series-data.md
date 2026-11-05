# ADR-003: Use TimescaleDB Hypertables for Time-Series Data

## Status

Accepted

## Context

FleetPulse continuously receives telemetry data from vehicles, including GPS coordinates, speed, and other vehicle metrics.

Telemetry records are time-dependent and expected to grow significantly over time. Efficient ingestion and querying of large volumes of time-series data are important for the platform.

A conventional PostgreSQL table could store this data, but managing large datasets and optimizing time-based queries would require additional partitioning and maintenance strategies.

TimescaleDB extends PostgreSQL with time-series capabilities, including automatic partitioning through hypertables.

## Decision

We will use TimescaleDB hypertables to store vehicle telemetry records.

The `TelemetryRecords` table will be converted into a hypertable using its timestamp column as the time dimension.

PostgreSQL will remain the underlying database, while TimescaleDB will handle time-based partitioning and provide time-series optimizations.

## Consequences

### Positive

- Automatic partitioning simplifies management of large telemetry datasets.
- Time-based queries can benefit from chunk exclusion.
- High-volume time-series ingestion is supported.
- Continuous data growth can be managed more efficiently.
- TimescaleDB integrates with existing PostgreSQL functionality and tooling.

### Negative

- TimescaleDB introduces an additional extension dependency.
- Hypertable constraints and indexing require consideration during schema design.
- Database operations and maintenance become slightly more complex.
- Performance benefits depend on appropriate schema design, indexing, and query patterns.