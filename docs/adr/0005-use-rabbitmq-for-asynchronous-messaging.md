# ADR-005: Use RabbitMQ for Asynchronous Messaging

## Status

Accepted

## Context

FleetPulse receives continuous telemetry data from simulated vehicles.

The Vehicle Gateway needs to forward incoming telemetry to downstream processing components without introducing direct runtime dependencies between the producer and consumers.

Synchronous communication would couple telemetry ingestion with data processing and could increase response times or make the gateway dependent on the availability of downstream services.

RabbitMQ provides message queuing capabilities that enable asynchronous communication between independent services.

## Decision

We will use RabbitMQ as the message broker for asynchronous communication between backend services.

The Vehicle Gateway will publish telemetry messages to the `vehicle-data-queue` queue.

The Stream Worker will consume messages from this queue and persist telemetry records to PostgreSQL.

RabbitMQ will handle message delivery between the producer and consumer, decoupling telemetry ingestion from persistence.

## Consequences

### Positive

- Producers and consumers operate independently.
- Telemetry ingestion does not need to wait for database persistence.
- Message queuing helps absorb temporary processing slowdowns.
- Consumers can process messages asynchronously.
- Additional consumers can be introduced as the system evolves.

### Negative

- Message delivery introduces eventual consistency between ingestion and persistence.
- Message acknowledgment and failure handling require careful implementation.
- Duplicate message processing must be considered.
- RabbitMQ introduces additional infrastructure and operational complexity.
- Monitoring queue depth and consumer health becomes necessary.