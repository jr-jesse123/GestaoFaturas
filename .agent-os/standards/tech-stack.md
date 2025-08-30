# Tech Stack (AI‑First)

## Context

Global tech stack defaults for Agent OS projects, overridable in project-specific `.agent-os/product/tech-stack.md`.

This document favors AI‑first product development: fast iteration on LLM features, strong observability and safety, and resilient operations.

## Development Environment

- Dev containers for reproducible development environments. keept it updated with all necessary tools, and always add claude code to the development environment.
- .NET Aspire for local orchestration of dependencies/services.
- Build tool: `dotnet` CLI.

## Platforms, Languages, and Architecture

- Primary platform: .NET (latest).
- Core domain language: F# (latest).
- Infrastructure/presentation language: C# (latest).
- Architectural style: Event‑driven Modular Monolith (evolve to services only when necessary).
- Contracts via messages/events; versioned message schemas.

## Web and APIs

- Framework: ASP.NET Core (latest).
- Minimal APIs for simple services; Controllers for complex domains.
- OpenAPI (Swagger) with Scalar UI.
- API versioning (URL or header) using `Microsoft.AspNetCore.Mvc.Versioning`.
- Conventions for pagination, filtering, and validation.
- Rate limiting with `AspNetCore.RateLimiter` and appropriate CORS per environment.

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
- Specialized EventStore when needed: EventStoreDB
- ORM: Entity Framework Core.
- Persistence strategy:
    - Prefer JSONB for aggregate/record storage unless strong relations or ACID characteristics are required.
    - Use normalized relational tables as needed.
- Migrations via EF Core; naming/versioning conventions.
- Caching: Redis (distributed cache) and in‑process MemoryCache where appropriate.
- Optional vector search: pgvector or Azure AI Search for RAG scenarios.
- Backups and retention policies defined per environment.

## Messaging and Async Processing

- Persisted messaging: RabbitMQ when necessary.
- Background jobs/scheduling: Hangfire or Azure Functions (timer/queue triggers) where suitable.
- Reliability patterns: idempotency keys, retries with backoff, dead‑letter queues, outbox/inbox.

## AI/LLM Capabilities

- LLM integration: Semantic Kernel with organized plugins.
- Prompts: extract complex prompts to YAML; version control and review.
- Retrieval‑Augmented Generation (RAG): embeddings store (pgvector/Azure AI Search) and citation grounding.
- Safety and compliance: content filtering, PII redaction, and prompt/response tracing.
- Evaluation: offline prompt tests, golden datasets, quality metrics; A/B and canary experiments for model/prompt changes.
- Telemetry: token usage, cost, latency; budgets and alerting.
- Model management: versioning, capability tags, fallback/rollback strategy; feature flags to switch models.
- Caching: semantic/response caching to reduce cost and latency where safe.

## Security and Compliance

- Authentication/Authorization: Keycloack OAuth2/OIDC 
- Secrets management:
    - Local: .NET User Secrets and configuration files.
    - Cloud: Azure Key Vault; environment variables in containers.
- Data protection: externalize ASP.NET Core data protection keys.
- OWASP Top 10 mitigations; input validation; HTTPS/TLS everywhere.
- API protections: rate limiting, request size limits, and strict CORS.
- Privacy/LGPD/GDPR: data minimization, consent tracking, retention and deletion workflows, audit trails.
- Supply chain security: SBOM generation and package allowlists.

## Testing Strategy

- Unit tests: xUnit (every public method where meaningful); bUnit for Blazor components.
- Integration tests: `Microsoft.AspNetCore.Mvc.Testing` with Testcontainers for dependencies.
- E2E tests: Playwright when necessary.
- Contract tests for APIs (e.g., Pact) where multi‑service interactions exist.
- AI evaluation tests: regression harness for prompts, datasets, and metrics.
- Coverage thresholds and flaky test handling in CI.

## Documentation

- ADRs, URL diagrams, Haikai, and OpenAPI specification.
- API docs via Swagger/Swashbuckle; long‑form docs via DocFX.
- Operational runbooks and incident playbooks.

## CI/CD and Environments

- Platform: Azure Pipelines.
- Triggers: push to `main` (production) and `staging` (staging) branches.
- Pipelines: build, test (unit/integration/E2E), security scans (SAST/DAST), SBOM, and artifact publish.
- Deployments: Azure with Terraform IaC.
- Strategies: blue/green or canary when supported; health checks and automated rollback on failure.
- Tests must pass before deployment.

## Release Management

- Environments: staging (from `staging` branch) and production (from `main` branch).
- Feature flags for controlled rollouts (e.g., .NET FeatureManagement or managed service).
- Versioning and changelogs via conventional commits.

## Resilience and Performance

- Health checks endpoints for all services.
- Resilience policies with Polly (retries, circuit breakers, bulkheads, timeouts).
- Load/performance testing (e.g., k6) on critical paths.

## Email Integrations

- SendGrid for email delivery; templates versioned; sandbox/testing modes in non‑prod.

---

Notes

- Scope and defaults are intentionally opinionated but overridable per project via `.agent-os/product/tech-stack.md`.
- Prefer minimal viable architecture with clear upgrade paths (e.g., from modular monolith to microservices) backed by metrics and business needs.
