---
name: create-module
description: Creates or extends a ShippingPlatform backend module following the project's established architecture, coding conventions, and API endpoint patterns. Use when implementing a new module or adding a use case to an existing module.
---

# Create or Extend Module Workflow

Follow this procedure when creating a new backend module or adding use cases to an existing module in ShippingPlatform. Root `AGENTS.md` is the authoritative source of truth.

---

## 1. Context and References
- Read [`AGENTS.md`](file:///c:/Projects/ShippingPlatform/AGENTS.md) first. Do not re-analyze the full repository or invent conventions.
- Do not repeat repository analysis that has already been completed.
- Do not read entire module directories recursively.
- Start with the relevant file paths listed in `AGENTS.md` and inspect the project structure only as needed.
- Read only the specific reference files required for the current task:
  - For architecture work, inspect only the relevant [`Customers`](file:///c:/Projects/ShippingPlatform/Src/Modules/Customers/) examples.
  - For API work, inspect only the relevant [`Content`](file:///c:/Projects/ShippingPlatform/Src/Api/Modules/Content/) endpoint examples.
- Avoid reopening files already available in the current context unless necessary.
- Clarify business requirements before writing code. If essential information is missing, ask concise, targeted questions.

## 2. Component Scope Assessment
Determine the minimum required projects and components based on module responsibilities:
- **Domain & Application:** Standard for domain logic, commands, queries, and validation.
- **Infrastructure:** Required only if the module owns a database, DbContext, repositories, or external services.
- **Contracts (`{Module}.Contracts`):** Conditional. Add only if other modules require public query/command abstractions or DTOs.
- **Outbox & Background Worker:** Conditional. Add only if domain events require reliable publishing or background processing.
- **Database Migrations:** Conditional. Required only when introducing or changing schema.

## 3. Planning & Approval Gate (Mandatory)
Before writing or modifying any files, present a concise plan covering:
1. **Target Files:** List of files to create or modify across projects.
2. **Design & Conventions:** Aggregates, commands/queries, handlers, validators, repository interfaces, and DI extensions.
3. **API Endpoints & Permissions:** HTTP verbs, routes, and `PermissionCatalog` keys (if authentication/authorization is required).
4. **Data & Infrastructure:** DbContext, entity configurations, tables, and conditional components (Outbox, Contracts).

> **STOP:** You must wait for explicit user approval before implementing the plan.

## 4. Implementation Guidelines
- Follow the patterns in `AGENTS.md` exactly:
  - Extend `AggregateRoot<Guid>` from `BuildingBlocks.Domain` for new aggregates.
  - Colocate command/handler or split them consistently with existing module style.
  - Implement endpoints under `Src/Api/Modules/{Module}/` with single static class per endpoint matching `Content`.
  - Maintain single entry-point registration via `{Module}ModuleServiceCollectionExtensions.cs` and `{Module}Endpoints.cs`.
- Preserve existing module boundaries and keep changes strictly within the requested scope.
- Do not modify unrelated modules without explicit approval.

## 5. Verification & Safety Constraints
- **Database Migrations:** Never execute `dotnet ef` or migration commands without explicit user confirmation.
- **Verification:** After approved implementation, build the affected projects and run relevant tests when practical. Do not run commands that mutate the repository without approval.
- **Completion Report:** Summarize:
  - Created and modified files with markdown links.
  - Build/test verification results.
  - Any pending manual steps or unresolved items.
