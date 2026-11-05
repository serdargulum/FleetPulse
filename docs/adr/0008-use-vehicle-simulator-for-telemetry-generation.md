# ADR-008: Use Vehicle Simulator for Telemetry Generation

## Status

Accepted

## Context

FleetPulse is designed to collect and process telemetry data from vehicles through dedicated hardware modules.

In production, different vehicles may use different telemetry devices and communication protocols. These devices may transmit different types of data at varying intervals.

Depending on physical vehicles and hardware availability during development would introduce additional costs and slow down development and testing.

A simulation environment is therefore needed to generate realistic telemetry data and represent different vehicle behaviors and data transmission patterns.

The simulator should also allow the backend system to be developed independently of specific vehicle hardware and communication protocols.

## Decision

We will implement a Vehicle Simulator to generate telemetry data representing real-world vehicle behavior.

The simulator will:

- Generate vehicle telemetry, including GPS coordinates, speed, and other vehicle metrics.
- Simulate multiple vehicles transmitting data independently.
- Support different telemetry data types and transmission intervals.
- Send telemetry data to the Vehicle Gateway through the same ingestion interface used by vehicle devices.

The simulator will be treated as a replaceable data source, allowing real vehicle hardware integrations to be introduced without requiring fundamental changes to the backend processing architecture.

## Consequences

### Positive

- Development and testing can proceed without physical vehicles or telemetry hardware.
- Different vehicle behaviors and telemetry transmission patterns can be simulated.
- Backend services remain independent of specific hardware implementations.
- Integration testing can be performed under controlled conditions.
- The simulator provides a foundation for validating future real vehicle integrations.

### Negative

- Simulated data cannot fully represent all real-world vehicle and hardware behaviors.
- Differences between simulated and actual device protocols may require additional integration testing.
- Maintaining realistic simulation scenarios introduces additional development effort.
- The simulator must evolve as new telemetry devices and data formats are introduced.