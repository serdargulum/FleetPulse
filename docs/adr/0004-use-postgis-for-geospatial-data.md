# ADR-004: Use PostGIS for Geospatial Data

## Status

Accepted

## Context

FleetPulse collects GPS coordinates from vehicles as part of telemetry data.

The platform needs to store vehicle locations and support geospatial operations such as distance calculations and spatial queries.

Storing latitude and longitude as separate numeric values would allow basic coordinate storage but would require additional implementation for spatial operations.

PostGIS extends PostgreSQL with geospatial data types and functions, enabling efficient storage and querying of geographic data.

## Decision

We will use PostGIS to handle geospatial data in FleetPulse.

Vehicle locations will be stored using the `geography(Point, 4326)` data type.

The SRID 4326 represents the WGS 84 coordinate reference system commonly used by GPS devices.

PostGIS functions will be used when geospatial calculations and queries are required.

## Consequences

### Positive

- Native geospatial data types provide structured storage for geographic coordinates.
- Spatial indexes enable efficient geospatial queries.
- Built-in functions support distance calculations and spatial operations.
- PostGIS integrates directly with PostgreSQL and existing database infrastructure.

### Negative

- PostGIS introduces an additional extension dependency.
- Spatial indexing and query performance require appropriate configuration.
- Geography operations may have higher computational costs than simple numeric coordinate comparisons.
- Developers need familiarity with spatial data types and coordinate reference systems.