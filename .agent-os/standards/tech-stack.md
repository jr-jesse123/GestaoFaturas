# Tech Stack (AI‑First)

## Context

Global tech stack defaults for Agent OS projects, overridable in project-specific `.agent-os/product/tech-stack.md`.

This document favors AI‑first product development: fast iteration on LLM features, strong observability and safety, and resilient operations.

## Development Environment

- Dev containers for reproducible development environments. keep it updated with all necessary tools, and always include Claude Code in the development environment.
- .NET Aspire for local orchestration of dependencies/services.
- Build tool: `dotnet` CLI.

## Platforms, Languages, and Architecture

- Primary platform: .NET (latest).
- Core domain language: F# (latest).
- Infrastructure/presentation language: C# (latest).
- Architectural style: 
    - Event‑driven Modular Monolith 
    - EventSourcing state management with regular sql tables projections

- Contracts via messages/events; versioned message schemas.

## Web and APIs

- Framework: ASP.NET Core (latest).
- Minimal APIs for simple services; Controllers for complex domains.
- OpenAPI with Scalar UI.
- API versioning (URL or header) using `Microsoft.AspNetCore.Mvc.Versioning`.
- Conventions for pagination, filtering, and validation.

## Frontend

- SPA Framework: Blazor (latest) — Server or WebAssembly as appropriate.
- CSS Framework: Tailwind CSS.

## Observability and Logging

- OpenTelemetry (traces, metrics, logs) instrumented across services.
- Exporters: OTLP to Azure Monitor/Application Insights; Seq for structured logs.
- Logging: Serilog + Seq; structured, contextual logs; PII scrubbing.
- Dashboards and alerts in Azure Monitor/Grafana; SLOs for latency, error rates, and LLM costs.

## Data and Persistence

- Primary database: PostgreSQL (latest).
- ORM: Entity Framework Core.
- Migrations via EF Core; naming/versioning conventions.
- Optional vector search: pgvector

## Messaging and Async Processing if necessary

- Persisted messaging: RabbitMQ.
- Background jobs/scheduling: TickerQ, with dashboard and EntityFramework.
- Reliability patterns: idempotency keys, retries with backoff, dead‑letter queues, outbox/inbox.

## AI/LLM Capabilities

- LLM integration: Semantic Kernel with organized plugins.
- Prompts: extract complex prompts to YAML; version control and review.
- Retrieval‑Augmented Generation (RAG): embeddings store (pgvector) and semnatik kernel.
- Model: Gpt-5

## Security and Compliance

- Authentication/Authorization: OIDC Keycloak 
- Secrets management:
    - Local: .NET User Secrets and configuration files.
    - Cloud: Azure Key Vault; environment variables in containers.

## Testing Strategy

- Unit tests: xUnit (every public method where meaningful); bUnit for Blazor components.
- Integration tests: `Microsoft.AspNetCore.Mvc.Testing` with Testcontainers for dependencies.
- E2E tests: Playwright when necessary.

## Documentation

- ADRs, URL diagrams, Haikai, and OpenAPI specification.
- API docs via Swagger/Swashbuckle; long‑form docs via DocFX.
- Operational runbooks and incident playbooks.

## CI/CD and Environments

- Platform: Azure Pipelines.
- Triggers: push to `main` (production) and `staging` (staging) branches.
- Pipelines: build, test (unit/integration/E2E), security scans (SAST/DAST), SBOM, and artifact publish.
- IAC: Terraform

## Release Management

- Environments: staging (from `staging` branch) and production (from `main` branch).
- Feature flags for controlled rollouts (e.g., .NET FeatureManagement or managed service).
- Versioning and changelogs via conventional commits.

## Resilience and Performance

- Health checks endpoints for all services.

## Email Integrations

- SendGrid for email delivery; templates versioned; sandbox/testing modes in non‑prod.

---

Notes

- Scope and defaults are intentionally opinionated but overridable per project via `.agent-os/product/tech-stack.md`.
- Prefer minimal viable architecture with clear upgrade paths (e.g., from modular monolith to microservices) backed by metrics and business needs.
