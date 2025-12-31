# ImpactHub ERP — Consolidated Architecture & Delivery Guide (Phase 1–2)

## 1. Purpose & Scope
This document consolidates the repository’s architectural, security, UI, and delivery guidance for ImpactHub ERP. It is intended to align **architects** (governance/decision-making) and **developers** (implementation) on a single, authoritative reference.

---

## 2. System Architecture (Non‑Negotiable Principles)

### 2.1 Modular Monolith
ImpactHub ERP is a **Modular Monolith** with strict internal boundaries:
- **One database** with **multiple schemas**
- **One schema per module**
- **One DbContext per module**
- **Cross-module references by ID only**
- **No cross-schema foreign keys**
- **EF Core is mapping-only** (no migrations, no EnsureCreated)
- **Application layer enforces integrity**

> **Module Boundary = Schema Boundary = DbContext Boundary**

### 2.2 Forbidden Practices (Automatic Rejection)
- Cross-module EF navigation properties
- Cross-schema FK between business modules
- Shared business DbContext
- EF migrations / EnsureCreated
- Cascading deletes across schemas

Any violation requires an ADR + Architecture Board approval.

---

## 3. Domain-Driven Design (DDD) + Event-Driven Integration

### 3.1 DDD Commitments
- Each module is a **Bounded Context**
- Ubiquitous Language is module‑specific
- No shared domain models (SharedKernel only primitives)
- Domain logic lives in Domain layer

### 3.2 Tactical Patterns Inside Modules
- **Entities** (identity & lifecycle)
- **Value Objects** (immutable, equality by value)
- **Aggregates** (transaction boundary)
- **Domain Services** (stateless)
- **Application Services** (thin, MediatR handlers)
- **Repositories** (per aggregate, DB‑first EF mappings)

### 3.3 Event-Driven Integration (In‑Process)
- **Domain Events** = internal to module
- **Integration Events** = cross-module contracts

Communication is **event-based**, not direct method calls. Events are published after `SaveChanges` via MediatR; handlers must be **idempotent** and versioned.

---

## 4. Context Map (High-Level Coupling)

```
Donors → Donations → Projects → Finance
```

Rules:
- No direct table access
- No lifecycle coupling
- Events only for side effects

---

## 5. Third-Party Integrations (Adapters + DI)

### 5.1 Principle
Modules must not call external APIs directly. Use **interfaces in shared infrastructure abstractions**, and **concrete adapters** for providers (e.g., SendGrid). DI binds the implementation in a composition root.

### 5.2 Benefits
- Provider swap without module changes
- Testability via mocks
- Centralized configuration (IOptions / secrets)

---

## 6. Security Architecture (Defense in Depth)

### 6.1 Security Layers
1. Authentication
2. Identity Resolution
3. Tenant Resolution
4. Authorization (API)
5. UI Access Control
6. Row-Level Security (RLS)

### 6.2 Identity Model
- Internal principal = `UserId` in `iam.Users`
- External identities stored in `iam.UserIdentities`
- Key `(Provider, Issuer, SubjectId)` mapping prevents provider lock‑in

### 6.3 Tenant Isolation
- Phase 1 = single DB + TenantId column + RLS
- Tenant ID must be resolved from DB (not client input)
- Tenant context is fixed for request lifetime

### 6.4 Authorization
- Permissions are explicit strings (e.g., `Projects.Read`)
- Granted via roles
- Special privilege: `CanBypassDataScope`

### 6.5 UI Access Control
- UI is **not** a security boundary
- UI uses two gates:
  1. Tenant feature gate
  2. Permission gate
- Read vs Write semantics are strict (Write never implied by Read)

### 6.6 RLS (Final Enforcement)
- RLS is mandatory for all tenant‑owned tables
- Depends on DB session context: `UserId`, `TenantId`
- NULL in scoped column = deny access

### 6.7 Backend Wiring (Runtime)
For every request:
1. Validate token
2. Resolve identity → UserId
3. Resolve tenant → TenantId
4. Load permission set
5. Set DB session context
6. Enforce authorization

---

## 7. App Shell Architecture (Frontend Platform)

### 7.1 App Shell Purpose
The App Shell is the **shared UI container** that handles:
- Authentication
- Layout (sidebar/header)
- Routing + route guards
- Access Profile consumption

It is not a business module.

### 7.2 Experiences (Bounded UI Contexts)
Phase 1 experiences:
- Admin
- Partner
- Donor
- Executive

Each experience has strict routing boundaries and cannot cross routes.

### 7.3 Access Profile Contract
Single endpoint: `GET /api/me/access-profile`

Returns:
- Tenant info + enabled features
- User info + permissions
- Optional derived UI capabilities

UI must build itself entirely from this profile.

---

## 8. Database Rules (DBA Checklist)

Allowed:
- FK within same schema
- FK to ref/constants
- Read-only views

Forbidden:
- Cross-schema FK
- Cascading deletes across schemas
- Triggers enforcing business rules

---

## 9. Delivery Roadmap

### Phase 1 Documentation & Governance (Locked)
- Security docs completed
- App Shell docs completed

### Phase 2 Execution Roadmap (Core Deliverables)
- Backend security wiring (auth, identity, tenant resolution, permission engine)
- Access Profile endpoint
- App Shell skeleton (routing, sidebar, experiences)
- RLS + authorization smoke tests

---

## 10. Developer Golden Rules (Quick Reference)
- Never reference another module directly
- Never add FK across schemas
- Never add EF navigation across modules
- Always validate existence in Application Layer

---

## 11. Source Documents (Repository)
- `README.md`
- `docs/architecture/architecture-policy.md`
- `docs/architecture/context-map.md`
- `docs/architecture/ddd-and-event-driven.md`
- `docs/architecture/third-party-integration.md`
- `docs/Database/DBA SQL Review Checklist.md`
- `docs/security/security-overview.md`
- `docs/security/identity-identifiers.md`
- `docs/security/authorization.md`
- `docs/security/backend-wiring.md`
- `docs/security/tenant-isolation.md`
- `docs/security/ui-access-control.md`
- `docs/security/row-level-security.md`
- `docs/app-shell/appshell-ui-architecture.md`
- `docs/app-shell/experience-boundaries.md`
- `docs/app-shell/access-profile-contract.md`
- `docs/ROADMAP.md`
- `docs/roadmap_phase2`
- `docs/onboarding/developer-guide.md`
