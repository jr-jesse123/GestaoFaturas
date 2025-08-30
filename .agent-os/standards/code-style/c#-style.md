# C# Style Guide — AI‑First

This guide defines conventions for modern, readable, and maintainable C# with AI‑first priorities: observability, security/privacy, performance, deterministic testing, and ergonomics for LLMs and data.

— Targets .NET 8+ and C# 12/13. Adjust only if project constraints require older targets.

## Principles

- AI‑first by default: structured telemetry (events/logs with consistent property names — e.g., correlationId, operation, latency, tokens, cost, model — emitted in JSON/OpenTelemetry), cost control, reproducibility, and privacy.
- Secure by default: never log sensitive data; always validate and sanitize.
- Async by default: don’t block threads; respect `CancellationToken`.
- Keep it simple: small, composable, testable modules; avoid tight coupling.
- Deterministic tests: fixed data, seeds, and isolation to avoid flakiness.

## Project configuration

- `LangVersion`: `latest` (use `preview` only when needed).
- `Nullable`: `enable` in all projects.
- Treat warnings as errors in CI: `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.
- `ImplicitUsings`: `enable`; complement with `global using` when helpful.
- Use file‑scoped namespaces and one public type per file.
- Organize solutions by clear layers (Domain, Application, Infrastructure, Web/API) and cohesive modules.
- Prefer `Directory.Build.props` for shared settings across projects.

## Naming and organization

- Public types and members: `PascalCase`.
- Parameters and locals: `camelCase`.
- Private fields: `_camelCase` and `readonly` when possible.
- Use `var` when the right‑hand side makes the type obvious; otherwise be explicit.
- One concept per file; avoid long files.

## Layout and formatting

- Allman braces (newline after declarations).
- Line length: 120 columns (soft limit) — wrap before readability suffers.
- `using` outside the namespace; order alphabetically; remove unused.
- Prefer expression‑bodied members for trivial getters/implementations.
- Avoid `#region` except for generated code.

## Types and immutability

- Prefer `record` for immutable DTOs and value‑like types without identity.
- Use `init`‑only setters to keep construction‑time invariants.
- Consider `readonly struct` for hot paths sensitive to allocations.
- Use `required` (constructors/primary constructors) for mandatory invariants.

## Nullability and validation

- Enable nullable annotations/analysis; avoid the null‑forgiving `!` except as a last resort.
- Validate inputs with guard clauses: `ArgumentNullException.ThrowIfNull(arg)` and specific checks.
- Don’t return `null` for collections; return empty instances.
- Prefer `TryGet...` over throwing for expected control flow.
- Date/time: use `DateTimeOffset` (UTC) and `TimeProvider` for testability.

## Errors and results

- Use exceptions for exceptional cases; prefer specific exception types.
- In Application layer, consider `Result<T>`/discriminated unions to model success/failure without exceptions in control flow.
- Always log with context (IDs, types, limits), never with sensitive data.

## Async and concurrency

- Async all the way: avoid `.Result`/`.Wait()`.
- Async methods should accept `CancellationToken` for I/O, long CPU, or remote operations (default to `CancellationToken ct = default`).
- Use `ValueTask` only when measured and beneficial (avoid for broad public APIs).
- Use `IAsyncEnumerable<T>` for streams; materialize when enumerating multiple times.
- In libraries, use `ConfigureAwait(false)`.
- Prefer `IAsyncDisposable` and `await using` when applicable.

## LINQ, collections, and performance

- Prefer LINQ for clarity; avoid unnecessary multiple enumerations.
- Materialize (`ToList`, `ToArray`) when multiple passes are required.
- Avoid excessive allocations; consider `Span<T>/Memory<T>` and pooling only when justified by measurement.
- Use `System.Text.Json` with source‑generation for hot paths/DTOs.

## Dependency injection and options

- Register by interface; keep constructors small (≤ 4–5 deps) — extract Facades when exceeding.
- Options pattern: `IOptions<T>` / `IOptionsSnapshot<T>` (scoped) with validation (`ValidateDataAnnotations`, custom validators).
- Don’t resolve services directly from `IServiceProvider` outside composition roots.
- Prefer source‑generated logging (`LoggerMessage`) for high‑throughput log paths.

## Logging and telemetry (AI‑first)

- Use `ILogger<T>` with structured logging (named placeholders). Avoid concatenation/interpolation inside messages; use structured properties.
- Correlation/Request ID must exist at ingress (HTTP, workers, messaging) and flow through.
- OpenTelemetry for traces/metrics/logs; environment‑specific exporters.
- AI metrics: per‑stage latency, tokens used, estimated cost, error/timeout rate, fallbacks taken.
- Redaction/masking: never log prompts, documents, or PII without sanitization and/or hashing.
- Logging levels policy: keep `Information` low‑cardinality; use `Debug/Trace` for high‑cardinality dev details; reserve `Warning` for recoverable issues; `Error` for failed operations; `Critical` for outages.
- OpenTelemetry conventions: consistent `ActivitySource` names (Company.Product.Component); standard attributes like `ai.vendor`, `ai.model`, `ai.tokens.prompt`, `ai.tokens.completion`, `ai.cost.usd`, `net.peer.name`, `db.system`; consider env‑based sampling (prod < dev).

## Web APIs and endpoints

- Minimal APIs for simple services; Controllers for complex domains.
- Explicit request/response models; validate input (data annotations or validators).
- Always support `CancellationToken` and per‑endpoint timeouts.
- Return correct HTTP codes; avoid “200 for everything”.
- Prefer RFC 7807 ProblemDetails for errors; consistent error shapes.
- Map exceptions explicitly to `ProblemDetails` (validation → 400, not found → 404, domain rules → 409/422); include `traceId`; hide internal details.
- Enforce security headers: HSTS, CSP, X‑Content‑Type‑Options, X‑Frame‑Options, Referrer‑Policy; secure cookies with `SameSite` and `Secure`.
- Set request/response size limits; stream large payloads; prefer gzip/br.

## HTTP clients and resilience

- Always use `HttpClientFactory` with named/typed clients.
- Centralize Polly policies (timeouts, retry with jitter, circuit breaker, bulkhead); attach via handlers.
- Prefer streaming for large responses; set per‑request timeouts; propagate `CancellationToken` end‑to‑end.

## Tests and quality

- xUnit; clear AAA (Arrange‑Act‑Assert); descriptive test names.
- Determinism: fixed seeds, injectable clocks (`TimeProvider`), isolated storage (temp dirs/containers), predictable doubles.
- Integration tests with fake providers for LLMs and vector stores; prompt “golden tests” (semantic diff with tolerances).
- Simple perf probes (`BenchmarkDotNet`) for hot‑path regressions.
- Coverage is guidance, not a goal — focus on critical paths and public contracts.
 - Snapshot tests (e.g., Verify) for HTTP responses/ProblemDetails and prompts.
 - Property‑based tests (FsCheck) for core invariants.
 - Categorize tests and control parallelization to avoid hidden coupling.

## AI‑first: patterns and practices

- Versioned prompts: external templates under version control with named variables; avoid literals in code.
- Strong contracts: define input/output schemas (JSON Schema) and validate model responses.
- Security and privacy: redact/anonymize PII before sending to AI; apply retention policies.
- AI observability: record cost per call, vendor/model, context (RAG vs pure), and fallback taken.
- RAG baseline: separate retrieval layer; configurable chunking/embeddings; trace retrieval path (documents and scores).
- Resilience: timeouts, cancellation, backoff, circuit‑breaker; queues for reprocessing when appropriate.
- Streaming: support partial responses when useful to UX; design for re‑entrancy/incremental updates.
- Continuous evaluation: an evaluation harness with synthetic/anon datasets; metrics (fidelity, usefulness, toxicity, latency, cost).
 - RAG conventions: token‑aware chunking policy, standard embedding model, similarity metric choice (cosine/dot), score thresholds, citation format, idempotent upserts and document versioning.

## Security and compliance

- Secrets in environment variables/secret managers (e.g., Key Vault); never in source control.
- Minimize data collection; label and isolate PII; encrypt at rest and in transit.
- Dependency and license reviews; regular updates; SBOM when required.

## Documentation

- XML docs on public APIs and a `README` per project.
- Minimal usage examples per layer (Domain, Application, Infrastructure).
- ADRs (Architecture Decision Records) for non‑trivial decisions.

## Tooling and automation

- Run `dotnet format` in CI.
- Recommended analyzers: `Microsoft.CodeAnalysis.NetAnalyzers`, `StyleCop.Analyzers`, `Roslynator.Analyzers`.
- Consider Source Generators when they remove heavy reflection/boilerplate.
- Add more analyzers as relevant to project needs.
 - Additional analyzers to consider: `Meziantou.Analyzer`, `AsyncFixer`, `SonarAnalyzer.CSharp`, `Microsoft.CodeAnalysis.BannedApiAnalyzers`, `Microsoft.Security.CodeAnalysis`.
 - Prefer source‑generated logging (`LoggerMessage`) for high‑throughput paths with stable templates and event IDs.

## JSON and serialization

- Standardize `System.Text.Json` options: camelCase, case‑sensitive, enums as strings, UTC `DateTimeOffset`, ignore nulls when appropriate.
- Prefer source‑generated serializers for DTOs on hot paths.
- Validate external JSON with schemas; fail fast on invalid payloads.

## Background processing

- Use `BackgroundService` with bounded `Channel<T>`/queues; backpressure instead of unbounded growth.
- Respect rate limits (AspNetCore.RateLimiter) and expose liveness/readiness health checks.
- Ensure graceful shutdown drains work items; propagate `CancellationToken`.

## EF Core and data patterns

- Disable lazy loading; prefer explicit includes/selects; use compiled queries for hot paths.
- Use concurrency tokens/rowversion; implement soft deletes via query filters.
- Map aggregates to PostgreSQL JSONB where suitable; use value converters carefully.
- Use `SaveChanges` interceptors for auditing/correlation; parameterize queries; avoid N+1.

## Dependency injection and options (advanced)

- Configuration precedence: env vars > secret manager/Key Vault > appsettings.* > user secrets (local).
- Validate options on start (`ValidateOnStart`) and fail fast on invalid config.

## Suggested .editorconfig (excerpt)

```ini
# C#
[*.cs]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
indent_style = space
indent_size = 4
max_line_length = 120

# Nullability and warnings
dotnet_style_require_accessibility_modifiers = always:suggestion
dotnet_analyzer_diagnostic.category-Style.severity = warning
dotnet_analyzer_diagnostic.category-Usage.severity = warning
dotnet_analyzer_diagnostic.category-Design.severity = warning

# Using directives and ordering
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = true

# var vs explicit type
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = false:suggestion

# Namespaces and members
csharp_style_namespace_declarations = file_scoped:warning
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
csharp_style_prefer_primary_constructors = true:suggestion

# New lines/braces (Allman)
csharp_new_line_before_open_brace = all:warning

# Misc preferences
csharp_style_preferReadonlyStruct = true:suggestion
csharp_style_prefer_readonly_struct = true:suggestion
csharp_style_prefer_readonly = true:suggestion
```

## PR checklist

- [ ] Nullability handled; no unnecessary `!`.
- [ ] I/O methods expose `CancellationToken` and apply timeouts.
- [ ] Structured logs without PII; correlation present.
- [ ] AI telemetry (latency, tokens, cost, fallback) captured for LLM/RAG calls.
- [ ] Deterministic tests; doubles for integrations; controlled data.
- [ ] Specific exceptions with clear messages; no control‑flow via exceptions.
- [ ] Dependencies registered by interface; options validated.
- [ ] `dotnet format`/analyzers clean; no warnings in CI.
 - [ ] Security headers and ProblemDetails mapping covered by tests.
 - [ ] OTEL attributes for AI calls include tokens, cost, model, vendor.
 - [ ] HTTP clients use HttpClientFactory and standard policies.

---

Keep this guide alive: propose improvements via PR with examples and expected impact. Prioritize reproducibility, security, and cost when working with AI.
