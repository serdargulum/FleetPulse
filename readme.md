# FleetPulse

### Distributed Vehicle Telemetry Processing Platform

FleetPulse is a .NET 10 microservices-based platform for ingesting, processing, normalizing, and analyzing vehicle telemetry from different manufacturers.

The project demonstrates distributed systems concepts through asynchronous messaging, manufacturer-specific parsing, real-time updates, persistent storage, and background analytics.

<p align="center">
  <img src="images/dashboard.png" width="40%" alt="FleetPulse dashboard" />
  <img src="images/vehicle-page.png" width="40%" alt="FleetPulse vehicle details" />
</p>

<p align="center">
<a href="https://www.youtube.com/watch?v=GbtoD71uNpI">
  <img src="images/youtube-placeholder.png" width="40%">
</a>
</p>

## Architecture

![FleetPulse Architecture](images/architecture-diagram.drawio.svg)

### Quick Start

**Prerequisites**

- Docker Desktop with Docker Compose
- Git

**Run the application**

```bash
git clone https://github.com/serdargulum/FleetPulse
cd FleetPulse  
  
# Create a .env file in the project root using .env.test as a reference.
docker compose up --build
```

Wait for the services to initialize. Database readiness and service startup times may vary.

The development environment uses Docker Compose to run the application services and supporting infrastructure.

### Demo Flow

1. Start the environment and initialize the services.
2. Generate synthetic vehicle telemetry using the Vehicle Simulator.
3. Follow the ingestion and asynchronous processing pipeline.
4. Inspect vehicle data and real-time updates in the dashboard.
5. Inspect persisted telemetry and explore the available service interfaces.

*The available demo flow depends on the services and features enabled in the current configuration.*

### Management Interfaces

**RabbitMQ Management**

```text
http://localhost:15672
```

**pgAdmin**

```text
http://localhost:5050
```

Use the development credentials configured in your local  `.env` file

## Core Design Decisions

- **Microservices Architecture:** Separate services for telemetry ingestion, processing, analytics, and administration.
- **Asynchronous Processing:** RabbitMQ decouples telemetry ingestion from downstream processing.
- **Manufacturer-Aware Processing:** Manufacturer-specific parsers handle different telemetry payload formats.
- **Vehicle-Based Parser Resolution:** Parser selection is resolved through vehicle registry information.
- **Time-Series and Geospatial Storage:** PostgreSQL with TimescaleDB and PostGIS supports telemetry and location data.
- **Real-Time Communication:** SignalR delivers live telemetry updates to the dashboard.
- **Vehicle Simulation:** A dedicated simulator generates telemetry data for development and testing.
- **Database Access and Schema Management:** EF Core handles schema migrations through AdminApi, while StreamWorker uses direct SQL with Npgsql for high-throughput telemetry persistence.
- **AI-Driven Telemetry Analytics:** AnalyticsWorker uses AI to analyze fuel consumption and vehicle movement patterns, identifying inconsistencies in telemetry data. Its commercial implementation is not included in the public repository.

Detailed architectural decisions, including their context, rationale, and consequences, are documented in [Architecture Decision Records (ADRs)](docs/adr/README.md).

## Services

| Service | Responsibility | Communication |
|---|---|---|
| `vehicle-gateway` | Telemetry ingestion | HTTP / RabbitMQ |
| `stream-worker` | Real-time processing and normalization | RabbitMQ / PostgreSQL / SignalR (WebSocket/HTTP) |
| `analytics-worker` | AI-driven analysis of fuel consumption and vehicle movement patterns | PostgreSQL |
| `admin-dashboard` | Operational data and management UI | HTTP |
| `admin-api` | Management endpoints and database access | HTTP / PostgreSQL |
| `vehicle-simulator` | Synthetic telemetry generation | HTTP |
| `postgres` | Persistent data storage | TCP |
| `rabbitmq` | Asynchronous messaging | AMQP |
| `pgadmin` | Database administration | PostgreSQL / HTTP |

## Technology Stack

**Application:** C#, .NET 10, ASP.NET Core, Worker Services, SignalR, Entity Framework Core, Mediator

**Infrastructure:** PostgreSQL(TimescaleDB, PostGIS extensions), RabbitMQ, Docker, Docker Compose

**Architectural Patterns:** Microservices, Clean Architecture, Strategy Pattern, Repository Pattern, Dependency Injection, Asynchronous Messaging, Background Processing, Batch Processing

**Tools:** pgAdmin

## Project Status

FleetPulse is a portfolio project focused on demonstrating .NET, C#, distributed systems, and microservice architecture.

The project currently provides the core ingestion → messaging → normalization → persistence pipeline, together with vehicle simulation, real-time updates, and administrative access. The Analytics Worker is included in the public architecture, while its commercial AI-driven analytics implementation remains private. 

The project is intentionally scoped as a technical showcase rather than a complete commercial fleet management product.


# Tests

### Running Integration Tests

Some integration tests require a running RabbitMQ instance.

Make sure Docker Compose services are running and the `.env` file contains the required RabbitMQ credentials.

On macOS or Linux, load the environment variables before running the tests:

```bash
set -a
source .env
set +a

dotnet test  
```

**Note:** The `.env` file is not automatically loaded into the environment of a `dotnet test` process. Keep your `.env` file out of version control and never commit credentials.

## License

This project is licensed under the [MIT License](LICENSE).