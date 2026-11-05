# ADR-006: Use SignalR for Real-Time Communication

## Status

Accepted

## Context

FleetPulse provides a dashboard for monitoring vehicle locations and telemetry data.

The dashboard needs to receive vehicle updates in near real time without continuously polling the backend for new data.

Traditional HTTP polling introduces unnecessary requests and may increase update latency.

SignalR provides real-time communication capabilities through persistent connections and supports server-to-client messaging.

## Decision

We will use ASP.NET Core SignalR to deliver real-time vehicle updates to the dashboard.

The backend will publish vehicle telemetry updates through SignalR, allowing connected dashboard clients to receive updates without repeatedly requesting them.

SignalR will be used for real-time client communication, while RabbitMQ will remain responsible for asynchronous communication between backend services.

## Consequences

### Positive

- Dashboard clients receive updates without continuous HTTP polling.
- Persistent connections reduce unnecessary request overhead.
- Server-to-client communication simplifies real-time dashboard updates.
- SignalR integrates naturally with the ASP.NET Core ecosystem.

### Negative

- Maintaining persistent connections introduces additional resource requirements.
- Connection management and reconnection handling require consideration.
- Scaling SignalR across multiple backend instances may require a shared backplane or managed service.
- Real-time delivery does not guarantee that disconnected clients receive missed updates.