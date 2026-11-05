# ADR-007: Use Docker Compose for Local Orchestration

## Status

Accepted

## Context

FleetPulse consists of multiple backend services, a message broker, a database, and a web dashboard.

Running and configuring these components individually would make local development and testing more complicated.

The project requires a consistent environment where all infrastructure dependencies and application services can be started and managed together.

Docker provides containerized environments, while Docker Compose allows multiple containers to be defined and managed through a single configuration.

## Decision

We will use Docker Compose to orchestrate FleetPulse components in local development and demonstration environments.

Services and infrastructure dependencies will be defined in a Docker Compose configuration.

Environment-specific settings will be managed through environment variables.

The application will be started using a single command:

`docker compose up --build`

## Consequences

### Positive

- Simplifies environment setup and project onboarding.
- Provides consistent environments across different development machines.
- Allows multiple services and infrastructure dependencies to run together.
- Simplifies starting, stopping, and rebuilding the application.
- Reduces manual configuration requirements.

### Negative

- Docker introduces additional resource overhead.
- Container networking and volume management require configuration.
- Local orchestration does not fully represent production deployment environments.
- Troubleshooting container-related issues requires familiarity with Docker.