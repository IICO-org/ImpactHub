# IICO ImpactHub ERP
ImpactHub Modular Monolith

# ImpactHub-Modular-Monolith
Impact Hub ERP

# ImpactHub – Modular Monolith Architecture
This repository implements a strictly governed Modular Monolith architecture.

This is not a guideline.
This is a contract.

---

## Core Rules (Non-Negotiable)

1. One database, multiple schemas.
2. One module owns one schema.
3. One module owns one DbContext.
4. Cross-module references are ID-only.
5. No cross-schema foreign keys between business modules.
6. EF Core is mapping-only (no migrations).
7. Integrity is enforced in the Application Layer.

> Module Boundary = Schema Boundary = DbContext Boundary

---

## Forbidden

- Cross-module EF navigation properties
- Cross-schema FK between business schemas
- Shared business DbContext
- EF migrations or EnsureCreated
- Cascading deletes across schemas

Any violation requires:
- Architecture Decision Record (ADR)
- Explicit Architecture Board approval

---

## Ownership

Each module team owns:
- Domain logic
- Application logic
- EF mappings for its schema

Database schemas are externally managed.
