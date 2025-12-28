# Backend Security Wiring  
IMPACT HUB ERP — Phase 1

## 1. Purpose

This document defines **how the backend application wires security together at runtime**.

It explains:
- how authentication results are consumed
- how identity and tenant context are resolved
- how authorization is enforced
- how database Row-Level Security (RLS) is activated per request

This document assumes:
- no existing backend code
- no specific framework binding
- all security concepts are already defined in the security documentation set

---

## 2. Backend Responsibilities in Security

The backend is responsible for **orchestrating**, not inventing, security.

It must:
1. Validate authentication
2. Resolve identity
3. Resolve tenant
4. Enforce authorization
5. Activate database RLS context

The backend must **never**:
- trust the UI
- trust client-provided tenant IDs
- bypass database security

---

## 3. Runtime Security Context (Core Concept)

Every request must be associated with a **Runtime Security Context**:

RuntimeSecurityContext
├── UserId (int)
├── TenantId (guid)
├── AuthProvider (entra | local)
├── Permissions (set of strings)
└── Flags (e.g. CanBypassDataScope)


This context:
- is created once per request
- is immutable during the request
- is the single source of truth for all downstream checks

---

## 4. Authentication Wiring (High Level)

### 4.1 Internal Users (Enterprise IdP)

Conceptual steps:
1. Receive request with enterprise token
2. Validate token (issuer, signature, expiry)
3. Extract:
   - Provider = `entra`
   - Issuer
   - SubjectId (OID)
4. Pass identity tuple to Identity Resolution

### 4.2 Partner Users (Local)

Conceptual steps:
1. Receive request with local token
2. Validate token signature and expiry
3. Extract:
   - Provider = `local`
   - SubjectId (username key)
4. Pass identity tuple to Identity Resolution

Authentication stops here.  
No tenant or permission logic yet.

---

## 5. Identity Resolution (Mandatory Step)

Identity resolution maps:

(Provider, Issuer, SubjectId)
↓
UserId


Rules:
- Lookup must be done against `iam.UserIdentities`
- If no match is found → request is rejected
- Auto-provisioning is **not allowed** in Phase 1

Output:
- `UserId`

---

## 6. Tenant Resolution (Mandatory Step)

Tenant resolution maps:

UserId → TenantId


Rules:
- TenantId is loaded from `iam.Users`
- TenantId from client or token is ignored
- Tenant must be active
- If tenant inactive → request rejected

Output:
- `TenantId`

At this point, the request is **tenant-bound**.

---

## 7. Authorization Wiring (API Layer)

Authorization occurs **after** identity and tenant resolution.

### 7.1 Permission Loading

The backend must load:
- all permissions assigned to the user
- within the resolved tenant

The result is a **flattened permission set**:

{ "Projects.Read", "Projects.Write", "Donations.Read", ... }


### 7.2 Permission Enforcement

Each endpoint declares:
- required permission(s)

Rules:
- If permission missing → HTTP 403
- Permissions are tenant-scoped
- No “admin means all” shortcuts

---

## 8. UI Access Profile Endpoint

The backend must expose a **single endpoint** that returns the UI access profile.

This endpoint:
- runs once after login
- returns all information the UI needs to render safely

Conceptual response:

```json
{
  "user": {
    "userId": 123,
    "permissions": ["Projects.Read", "Projects.Write"],
    "flags": {
      "canBypassDataScope": false
    }
  },
  "tenant": {
    "tenantId": "GUID",
    "enabledFeatures": ["Projects", "Donations"]
  }
}

The UI must not call authorization endpoints repeatedly.
9. Database Session Context (Critical)

Before any database query, the backend must set session context.
Required Context Keys

    UserId

    TenantId

Why This Is Mandatory

    SQL Server RLS relies on session context

    Connection pooling reuses connections

    Context must be set on every request, not once

Failure here causes:

    random data leaks

    intermittent access bugs

    impossible-to-debug issues

10. Session Context Lifecycle

For each request:

    Open DB connection

    Set session context

    Execute queries

    Dispose connection

Session context must never be assumed to persist.
11. RLS Enforcement Flow

API Endpoint
  ↓
Authorization Check
  ↓
Set DB Session Context
  ↓
Execute Query
  ↓
RLS Predicate Applies Automatically

The backend must never:

    manually filter rows for security

    assume “no rows” means “no permission”

12. Error Handling Rules
Scenario	Response
Invalid token	401
Identity not resolved	401
Tenant inactive	403
Permission missing	403
RLS returns no rows	200 with empty result

RLS returning no rows is not an error.
13. Logging & Auditing

The backend should log:

    authentication success/failure

    authorization failures

    tenant resolution issues

Logs must:

    reference UserId and TenantId

    never log raw tokens or secrets

14. Non-Negotiable Rules (Hard Stops)

❌ Do not trust tenant from request
❌ Do not cache permissions across tenants
❌ Do not bypass RLS for “performance”
❌ Do not allow UI to enforce security alone
❌ Do not reuse DB connections without resetting context
15. Relationship to Other Documents

This document depends on:

    security-overview.md

    identity-identifiers.md

    authentication.md

    authorization.md

    tenant-isolation.md

    row-level-security.md

It defines how those concepts are wired together at runtime.
16. Status

    Phase: Foundation

    Scope: Backend wiring only

    Implementation: Pending

    Stability: Locked

No code should be written until this contract is understood.
