# ADR-001: Adopt Microservices Architecture

## Status

Accepted

## Context

FleetPulse is a vehicle telemetry platform that receives, processes, stores, and visualizes real-time vehicle data.

The system has distinct responsibilities, including telemetry ingestion, message processing, data persistence, analytics, and real-time visualization.

A monolithic architecture would initially simplify development, but it would couple these responsibilities and make independent development, scaling, and deployment more difficult.

Telemetry ingestion and processing workloads may have different resource requirements and scaling characteristics from other components.

## Decision

We will adopt a microservices architecture, separating backend responsibilities into independently deployable services.

The backend architecture consists of:

- **Vehicle Gateway:** Receives telemetry data from vehicles and publishes messages to RabbitMQ.
- **Stream Worker:** Consumes telemetry messages and persists them to PostgreSQL.
- **Analytics Worker:** Processes telemetry data for analytical purposes.

Additional system components include:

- **Vehicle Simulator:** Generates simulated vehicle telemetry.
- **Dashboard:** Provides a web interface for monitoring vehicles and telemetry.
- **RabbitMQ:** Provides asynchronous message communication between backend services.
- **SignalR:** Enables real-time communication between the backend and dashboard.
- **PostgreSQL:** Stores telemetry and analytical data.

Services will communicate through explicit interfaces rather than sharing internal implementation details. Asynchronous messaging will be used where appropriate to reduce direct runtime dependencies.

## Consequences

### Positive

- Backend services can be developed, maintained, and deployed independently.
- Individual services can be scaled according to their workloads.
- Failures in one service can be isolated from other services to a certain extent.
- Asynchronous communication reduces direct runtime dependencies.
- The architecture provides a practical environment for demonstrating distributed systems concepts.

### Negative

- Distributed communication introduces network-related failure scenarios.
- Monitoring, debugging, and tracing become more complex.
- Data consistency across services requires additional consideration.
- Deployment and infrastructure management become more involved.
- Operational overhead is higher than with a monolithic architecture, particularly for a relatively small system.