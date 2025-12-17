# IICO ImpactHub ERP
ImpactHub Modular Monolith

# ImpactHub – Modular Monolith Repository

## Purpose
This repository implements an enterprise-grade Modular Monolith architecture.
It enforces strict module boundaries to support scalability, governance, and distributed teams.

---

## Architectural Principles (Non-Negotiable)

1. One database, multiple schemas.
2. One module owns one schema.
3. One module owns one DbContext.
4. Cross-module references are ID-only.
5. No cross-schema foreign keys between business modules.
6. EF Core is mapping-only (no migrations).
7. Integrity is enforced in the application layer.

> Module Boundary = Schema Boundary = DbContext Boundary

---

## What Is Forbidden?
- Cross-module EF navigation properties
- Cross-schema FK between business modules
- Shared business DbContext
- EF migrations or EnsureCreated
- Cascading deletes across schemas

## Any violation requires:
- Architecture Decision Record (ADR)
- Explicit approval from Architecture Board

---

## Repository Structure
- `/src/ImpactHub.Api` → single executable
- `/src/Modules/*` → business modules
- `/src/ImpactHub.SharedKernel` → shared primitives only
- `/tests` → per-module test projects
- `/deploy` → deployment artifacts
- `/docs/architecture` → ADRs and policies

---

## Ownership
Each module team is responsible for:
- Domain logic
- Application logic
- EF mappings for its schema

Database schema is externally managed.


## DEVELOPER ONBOARDING GUIDE (Short)

## What you MUST know
- One module = one schema
- ID references only across modules
- No FK across schemas
- EF is adapter, not owner

## Golden Rules
- Never reference another module directly.
- Never add FK across schemas.
- Never add navigation properties across modules.
- Always validate existence in Application layer.

## Creating a New Module
1. Create module folder under `/src/Modules`
2. Create schema in DB (handled externally)
3. Create DbContext mapped to that schema
4. Register module in `ImpactHub.Api/Modules`
5. Add test project

## What gets your PR rejected
- Cross-module navigation
- Cross-schema FK
- Shared business DbContext
- 
## If You Are Unsure
Stop!. 
Ask for architectural review.

