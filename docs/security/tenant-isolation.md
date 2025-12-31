# Tenant Isolation & Multi-Tenancy Model  
IMPACT HUB ERP — Phase 1

## 1. Purpose

This document defines **how tenants are isolated** in IMPACT HUB ERP.

It explains:
- what a tenant is
- how tenant context is resolved
- how tenant isolation is enforced
- how the system evolves safely toward stronger isolation models

This document is **standalone** and does not assume any prior documentation.

---

## 2. What Is a Tenant?

A **tenant** represents an independent organization using the system.

Examples:
- IICO (internal organization)
- A partner NGO
- A regional branch (future)

Each tenant must be:
- logically isolated
- operationally independent
- protected from data leakage

---

## 3. Multi-Tenancy Goals

The tenant model is designed to support:

1. **Strict data isolation**
   - No tenant can see another tenant’s data
2. **Shared infrastructure**
   - One application
   - One database (Phase 1)
3. **Future scalability**
   - Seamless evolution to stronger isolation if required

---

## 4. Tenant Models (Overview)

IMPACT HUB ERP supports **progressive isolation**.

### Phase 1 (Current)
**Single Database + Tenant Column**

- All tenant-owned tables include `TenantId`
- Isolation enforced by Row-Level Security (RLS)

### Phase 2 (Optional)
**Schema per Tenant**

- Each tenant has its own schema
- Shared reference data remains global

### Phase 3 (Optional)
**Database per Tenant**

- Full physical isolation
- Used only for very large or regulated tenants

> Phase 1 is the foundation.  
> Later phases are **evolution paths**, not rewrites.

---

## 5. Tenant Ownership Rules

### 5.1 Tenant-Owned Data
Any table that represents business data **must include**:

- `TenantId` (NOT NULL)

Examples:
- Projects
- Donations
- Sponsorships
- CRM records

### 5.2 Global Reference Data
Some data is global and **must not** include `TenantId`.

Examples:
- Countries
- Currencies
- ISO codes

### 5.3 Tenant-Specific Reference Data
Some reference data is tenant-defined.

Examples:
- Offices
- Local categories
- Custom classifications

These **must** include `TenantId`.

---

## 6. Tenant Resolution (Runtime)

Tenant resolution answers:

> “Which tenant is this request acting on?”

### Key Rule (Non-Negotiable)
> TenantId is resolved **from the database**, not trusted from client input.

---

## 7. Tenant Resolution Flow (Conceptual)

### Step 1 — Authentication
- User authenticates (Entra or Local)
- External identity is validated

### Step 2 — Identity Resolution
- External identity → `iam.UserIdentities`
- Resolve internal `UserId`

### Step 3 — Tenant Resolution
- Load `TenantId` from `iam.Users`
- Verify tenant is active

### Step 4 — Fix Tenant Context
- TenantId is fixed for the entire request
- Cannot be changed mid-request

---

## 8. Tenant Context Propagation

Once resolved, tenant context is propagated to:

- API authorization checks
- UI access profile
- Database session context

### Database Session Context
Before any data access, the backend must set:

- `UserId`
- `TenantId`

This ensures:
- RLS enforcement
- Safe connection pooling
- No cross-tenant leakage

---

## 9. Tenant Isolation Enforcement Layers

Tenant isolation is enforced at **multiple layers**:

| Layer | Responsibility |
|----|----------------|
| Identity | User belongs to one tenant |
| API Authorization | Tenant-bound permissions |
| UI Access Control | Tenant feature visibility |
| Database (RLS) | Final row isolation |

> Even if one layer fails, others still protect data.

---

## 10. Row-Level Security (RLS) and Tenants

RLS is the **ultimate enforcement mechanism**.

Key principles:
- Every tenant-owned table must have an RLS policy
- RLS predicates must always check:
  - `TenantId`
  - User context

RLS guarantees:
- No accidental cross-tenant reads
- No cross-tenant writes
- Protection against application bugs

---

## 11. Partner Access & Tenant Scope (Phase 1)

In Phase 1:
- Internal users and partners may share the same tenant
- Partners are restricted by:
  - permissions
  - data scopes
  - RLS

This allows:
- controlled collaboration
- future promotion of partners into their own tenants

---

## 12. Tenant Lifecycle

Each tenant has a lifecycle:

1. **Created**
2. **Active**
3. **Suspended**
4. **Decommissioned**

Rules:
- Suspended tenants:
  - cannot access data
  - RLS returns zero rows
- Decommissioned tenants:
  - data retained or archived (policy decision)

---

## 13. Tenant Feature Isolation

Tenants may have different enabled modules.

Examples:
- Tenant A: Projects + Donations
- Tenant B: Projects only

Feature isolation affects:
- UI menus
- API routes
- Background jobs

Feature checks are **independent** of data isolation.

---

## 14. Common Anti-Patterns (DO NOT DO)

❌ Trust TenantId from request headers  
❌ Allow client to switch tenant arbitrarily  
❌ Skip TenantId on “temporary” tables  
❌ Rely only on UI or API for isolation  
❌ Share tenant-owned data via joins without RLS  

---

## 15. Future Evolution (Planned)

The current model supports:

- Seamless migration to schema-per-tenant
- Seamless migration to database-per-tenant
- Hybrid strategies (large tenants isolated, small tenants shared)

Because:
- Tenant context is explicit
- Isolation rules are centralized
- No hard-coded tenant assumptions exist

---

## 16. Summary

Tenant isolation in IMPACT HUB ERP is:

- Explicit
- Layered
- Enforced
- Auditable
- Evolvable

It is a **core architectural pillar**, not a configuration option.

---

## 17. Status

- Phase: Foundation
- Isolation Model: Single DB + RLS
- Stability: Locked
- Changes require architectural review
