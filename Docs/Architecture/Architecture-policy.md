# Architecture Policy – ImpactHub

## Architectural Style
- Modular Monolith
- Single executable
- Database-first
- Strong internal boundaries

## Database
- One database
- One schema per module
- Shared schemas: ref, constants (lookups only)

## EF Core
- DbContext per module
- Mapping only
- No schema creation
- No migrations

## Cross-Module Rules
- Reference by ID only
- No FK across schemas
- No joins across DbContexts
- Validation in Application Layer

This policy is enforceable and audited.
