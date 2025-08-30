# F# Style Guide — AI‑First

This guide proposes conventions for modern F# (F# 7+ / .NET 8+), prioritizing readability, immutability, safety, and AI‑first practices (observability, cost, privacy, deterministic tests, and smooth integration with LLMs/RAG).

## Principles

- Strong domain modeling: prefer Discriminated Unions (DUs) and Records.
- Immutability by default; local and controlled mutability when necessary.
- Expressive > clever: favor pipelines (`|>`), composition (`>>`), and clear pattern matching.
- Async and cancelable by default: use `task {}` and `CancellationToken` for I/O and external calls.
- No `null`: use `option` (translate to `Nullable<>` when interoperating with .NET/C#).
- AI‑first: structured telemetry, token/cost control, PII redaction, reproducibility.

## Structure and organization

- One module per theme; cohesive `namespace` + `module`. Keep types (records/DUs) close to their usage.
- Prefer one public type per file when practical; keep files short and focused.
- Layers: Domain (pure), Application (use cases), Infrastructure (I/O), Web/API.
- Try to avoid classes/inheritance; use interfaces for DI and boundaries with .NET/infra.

## Naming

- Modules, Types, DU Cases: `PascalCase`.
- Values, functions, and parameters: `camelCase`.
- Internal modules and private values with `private`.
- Use `this` only when required in type members; prefer pure functions.

## Formatting

- Tabs/spaces per project standard; target width 100–120 columns.
- `open` at the top; sort alphabetically; remove unused.
- Break pipelines step‑by‑step; one operator per line when long.
- Pattern matching with guards (`when`); prefer explicit exhaustiveness.

## Types and modeling

- Records for data; `with` for copies; `option` for absence.
- DUs for states and protocols; consider `struct` DU on hot paths (after measuring).
- Use `unit` for effects; avoid returning `bool` when a DU expresses the contract better.
- Use `NonEmptyList`, `Id`, and value‑objects (wrappers) for domain invariants.

## Nullability and interoperability

- Expose public APIs without `null`; when interoperating with C#, convert `null` ⇄ `option`.
- Avoid `Unchecked.defaultof`; prefer explicit initialization.

## Errors and results

- Exceptions only for truly exceptional cases (limits, bugs, catastrophic I/O).
- Expected flow with `Result<'T,'E>` and railway‑oriented style (`bind`/`map`/validation).
- Propagate rich errors (error DU) in Application; translate at the edge to HTTP/telemetry.

## Async and concurrency

- Prefer `task {}` to interop well with .NET ecosystem and DI.
- Accept `ct: CancellationToken` in functions that do I/O/long CPU; apply timeouts.
- Use `IAsyncEnumerable<'T>` for streams; materialize when enumerating multiple times.
- Use `ValueTask` only when measured and beneficial (avoid in generic public APIs).

## Collections, LINQ, and performance

- Prefer F# collection functions (`List`, `Array`, `Seq`) and clear pipelines.
- Materialize lazy sequences only when necessary; avoid multiple enumerations when not needed.

## DI, options, and composition

- Expose contracts as simple interfaces or pure functions in modules.
- Register dependencies by interface; keep constructors slim in hosting.
- Use “dependency rejection”: pass functions as dependencies when it simplifies composition.
- Configuration precedence: env vars > secret manager/Key Vault > appsettings.* > user secrets (local). Validate on start and fail fast.
- Consider community‑approved functional abstraction libraries (e.g., FSharpPlus) when they reduce boilerplate and improve composability; adopt incrementally, avoid leaking abstractions into public APIs without team consensus.

## Logging and telemetry (AI‑first)

- Use `ILogger` with structured properties (no PII). Prefer logs with domain data and IDs.
- OpenTelemetry: traces, metrics, and logs; exporters per environment (OTLP/AppInsights/Jaeger).
- AI metrics: per‑stage latency, tokens/estimated cost per call, error/timeout rates, fallbacks.
- Redaction: sanitize prompts/documents/PII; hash/mask when required.
- Logging levels policy: keep `Information` low‑cardinality; `Debug/Trace` for high‑cardinality dev details; `Warning` for recoverable issues; `Error` for failed ops; `Critical` for outages.
- OTEL conventions: consistent `ActivitySource` names (Company.Product.Component); attributes like `ai.vendor`, `ai.model`, `ai.tokens.prompt`, `ai.tokens.completion`, `ai.cost.usd`; consider env‑based sampling (prod < dev).

## Web/API

- Minimal APIs for simple services; use controllers only for more complex domains.
- Request/response types as records; validate with DU/Result.
- Always accept `CancellationToken`; define timeouts and limits (payload, rate, concurrency).
- Map exceptions to RFC 7807 `ProblemDetails` (validation → 400, not found → 404, domain rules → 409/422); include `traceId`; hide internals.
- Enforce security headers: HSTS, CSP, X‑Content‑Type‑Options, X‑Frame‑Options, Referrer‑Policy; secure cookies with `SameSite` and `Secure`.
- Set request/response size limits; stream large payloads; enable gzip/br.

## HTTP clients and resilience

- Always use `HttpClientFactory` with named/typed clients.
- Centralize Polly policies (timeouts, retry with jitter, circuit breaker, bulkhead) and attach via handlers.
- Prefer streaming for large responses; set per‑request timeouts; propagate `CancellationToken` end‑to‑end.

## JSON and serialization

- Standardize `System.Text.Json` options: camelCase, case‑sensitive, enums as strings, UTC `DateTimeOffset`, ignore nulls when appropriate.
- Prefer source‑generated serializers for DTOs on hot paths.
- For F# records/DUs, use compatible serializers (e.g., `FSharp.SystemTextJson` or `FsCodec.SystemTextJson`) when needed; validate external JSON with schemas and fail fast.

## Background processing

- Use `BackgroundService` with bounded `Channel<'T>`/queues; apply backpressure rather than unbounded growth.
- Respect rate limits (AspNetCore.RateLimiter) and expose liveness/readiness health checks.
- Ensure graceful shutdown drains work items; propagate `CancellationToken`.

## Tests and quality

- Frameworks: xUnit + Unquote; prefer descriptive test names.
- Determinism: fixed seeds, injectable clocks, isolated storage, predictable doubles.
- Property‑based testing with FsCheck for invariants and domain laws.
- Golden tests for prompts (tolerant/semantic diffs) and JSON output contracts.
- Snapshot tests (e.g., Verify) for HTTP/ProblemDetails and prompts.

## AI‑first: patterns and practices

- Versioned prompts: external templates with named variables; no literals in code.
- Strong contracts: F# types + validation against JSON Schema; reject invalid model responses.
- RAG: separate retrieval layer (configurable chunking/tokenization/embeddings), trace documents and scores.
- Resilience: timeouts, exponential backoff, circuit‑breaker; queues for reprocessing when applicable.
- Streaming: partial responses (SSE/WebSockets) with safe re‑entrancy.
- AI observability: vendor/model, prompt version, cost/tokens, and chosen fallback on each call.
- RAG conventions: token‑aware chunking, similarity metric (cosine/dot), score thresholds, citation format, idempotent upserts and document versioning.

## Security and compliance

- Secrets via environment/Key Vault; never in code/repo.
- Minimal data retention; label/isolate PII; encryption in transit/at rest.
- Dependabot/Renovate for packages; SBOM where required.

## Documentation

- Top‑level comments on public modules; `README` per project.
- ADRs for significant architectural decisions; simple diagrams when helpful.

## Tooling and automation

- Formatting: Fantomas (project standard) — run in CI.
- Linters: FSharpLint; include .NET analyzers in the solution for interop.
- `dotnet format` and validation of `open`/usings; OpenTelemetry enabled by default.
- Consider F# analyzer packages (e.g., community analyzers via FSharp.Analyzers.SDK) for additional checks.

## .editorconfig/Fantomas (generic excerpt)

```ini
# F#
[*.fs]
indent_style = space
indent_size = 4
end_of_line = crlf
max_line_length = 120

# Fantomas (use defaults or tune as a team)
# e.g., keep pipelines aligned and limit long lines
# fsharp_max_line_length = 120
# fsharp_keep_max_number_of_blank_lines = 1
# fsharp_multiline_bracket_style = aligned
```

## PR checklist

- [ ] No `null` (use `option`); conversions to C# handled.
- [ ] Functions that perform I/O accept `CancellationToken` and apply timeouts.
- [ ] Structured logs without PII; correlation present.
- [ ] AI telemetry (latency, tokens, cost, fallback) captured for LLM/RAG calls.
- [ ] Deterministic tests; FsCheck for invariants; golden tests for prompts.
- [ ] Errors modeled with `Result`/DUs, Unless they're totally not expected or controllable (like HARDWARE problems) and translated at the edge (HTTP/infra) with ProblemDetails.
- [ ] Security headers present; payload limits/streaming considered.
- [ ] HttpClientFactory + standard resilience policies used for outbound calls.
- [ ] Fantomas/FSharpLint clean; CI without significant warnings.

---

Keep this guide alive with small, example‑rich PRs. Optimize for clarity, safety, and cost — especially in AI flows.
