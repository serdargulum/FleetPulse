# ADR-002: Use PostgreSQL as the Primary Database

## Status

Accepted

## Context

FleetPulse requires a reliable data store for vehicle telemetry and related application data.

The system needs to support structured data, transactional operations, efficient time-series queries, and geospatial operations.

PostgreSQL provides strong relational database capabilities, ACID compliance, and an extensible architecture that supports specialized extensions.

## Decision

We will use PostgreSQL as the primary database for FleetPulse, with the following extensions:

- **TimescaleDB:** Used for managing vehicle telemetry as time-series data through hypertables.
- **PostGIS:** Used for storing and querying geospatial data, including vehicle locations represented as `geography(Point, 4326)`.

PostgreSQL will provide the underlying relational database capabilities, while TimescaleDB and PostGIS will extend it to address the platform's time-series and geospatial requirements.

## Consequences

### Positive

- ACID transactions provide reliable data consistency.
- Relational capabilities support structured data and complex queries.
- TimescaleDB provides time-series optimizations and hypertable management.
- PostGIS enables efficient geospatial queries and spatial operations.
- A single database platform avoids introducing separate database technologies for telemetry and geospatial data.

### Negative

- Database performance requires appropriate indexing and query optimization as telemetry volume grows.
- High-volume write workloads may require additional tuning.
- Database availability becomes critical to services that depend on persistent storage.
- Extensions introduce additional configuration, compatibility, and maintenance requirements.