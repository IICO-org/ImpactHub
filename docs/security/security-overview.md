# Security Architecture Overview  
IMPACT HUB ERP — Phase 1

## 1. Purpose of This Document

This document provides a **comprehensive, standalone explanation** of the security architecture used in IMPACT HUB ERP.

It is written to be understood **without any prior documentation** and serves as the foundational reference for:
- developers
- architects
- auditors
- future maintainers

All other security documents build on the concepts defined here.

---

## 2. Security Design Goals

The security architecture is designed to support:

1. **Multi-tenant SaaS**
   - Strong tenant isolation
   - Safe evolution to multiple identity providers

2. **Mixed user types**
   - Internal staff (enterprise identity)
   - External partners (local accounts)

3. **Defense in depth**
   - Multiple layers, each with a clear responsibility
   - No single layer trusted alone

4. **Auditability**
   - Explicit privileges
   - Deterministic authorization decisions

5. **Future extensibility**
   - New identity providers without schema redesign
   - New modules without rewriting security foundations

---
# 3. Security Layers (High-Level)

The system uses **six distinct security layers**:

┌─────────────────────────────┐
│ Authentication │ Who are you?
├─────────────────────────────┤
│ Identity Resolution │ Which system user is this?
├─────────────────────────────┤
│ Tenant Resolution │ Which tenant are you acting in?
├─────────────────────────────┤
│ Authorization (API) │ What actions are allowed?
├─────────────────────────────┤
│ UI Access Control (Portal) │ What screens/buttons are shown/enabled?
├─────────────────────────────┤
│ Data Access (RLS) │ What rows can be seen/modified?
└─────────────────────────────┘


Each layer has a different job. **UI access control improves UX** but is not a security boundary by itself. The database (RLS) and API authorization remain the enforcement boundaries.
Each layer is independent and **must not rely on assumptions from other layers**.

Note: UI access control is not a security layer in the same strength as API/RLS. It’s a UX enforcement layer. If someone bypasses the UI and calls the API directly.

API + RLS must still stop them.
---

## 4. Authentication (Conceptual)

Authentication answers the question:

> **“How does the system know who is making the request?”**

In Phase 1, the system supports:

- **Internal users**
  - Authenticated via an enterprise identity provider (e.g. Microsoft Entra)
- **Partner users**
  - Authenticated via local credentials managed by the system

Authentication only proves **identity**, not permissions.

No authorization decisions are made at this layer.

---

## 5. Identity Resolution (Core Concept)

Authentication produces an **external identity** (e.g. OIDC subject).

The system must map that identity to an **internal principal**.

### Key rule:
> The system never relies on provider-specific identifiers inside domain tables.

Instead, all external identities are normalized using:

(Provider, Issuer, SubjectId)


These are stored in a dedicated table and mapped to a single internal user record.

This allows:
- multiple identity providers
- identity migration
- future SSO expansion

---

## 6. Internal Principal Model

The internal principal is represented by:

- `UserId` (integer, primary key)
- Owned by exactly **one tenant** in Phase 1

All authorization, auditing, and data ownership reference this `UserId`.

External identities are **never** used directly in business logic.

---

## 7. Tenant Resolution

Tenant resolution answers the question:

> **“Which tenant’s data is this request allowed to operate on?”**

Rules:
- Tenant is resolved **after** identity resolution
- Tenant is authoritative from the database, not from client input
- A request cannot “choose” a tenant arbitrarily

Once resolved, the tenant context is fixed for the lifetime of the request.

---

## 8. Authorization

Authorization answers the question:

> **“What actions is this user allowed to perform?”**

The system uses:
- Roles
- Permissions
- Optional data scopes

Authorization is **explicit**:
- No implicit admin powers
- No hidden bypass paths

A special privilege exists for controlled bypass:
- `CanBypassDataScope`
- Explicitly granted
- Auditable
- Still tenant-bound

## 9. UI Access Control (Portal Authorization)

UI access control answers the question:

> “What should the user see and be able to click in the portal?”

This layer is used to:
- hide modules the tenant did not subscribe to
- hide screens the user has no permission to access
- disable actions (Create/Edit/Delete) based on permissions
- present read-only views when the user lacks write permissions

### Key Principle (Non-Negotiable)
UI rules must be driven from the **same permission model** used by the API.

- The UI can hide and disable, but it must never be trusted as the final enforcement.
- The API must still enforce permissions.
- RLS must still enforce row-level visibility regardless of UI state.

### Recommended Model
The UI should receive a “User Access Profile” after login containing:
- Tenant feature flags (enabled modules/features)
- User permissions (flattened set, e.g. `Projects.Read`, `Projects.Write`)
- Optional UI capabilities derived from permissions (e.g. `canCreateProject`, `canEditProject`)

The UI uses this profile to:
- build the sidebar menu
- decide which routes are accessible
- show/hide action buttons
- render forms as read-only vs editable

### Read vs Write in UI
- **Read permission** controls access to view/list/details screens.
- **Write permission** controls:
  - showing “New/Edit/Delete” buttons
  - enabling form fields
  - allowing save actions

### Subscription vs Permission (Two Gates)
UI must apply two independent gates:

1. **Tenant Feature Gate**
   - If tenant doesn’t have the module enabled → module is hidden.
2. **User Permission Gate**
   - If user lacks permission → screen hidden or read-only.

If either gate fails, the UI blocks the action.

### Consistency Rule
If UI allows an action:
- API must also allow it
- DB must also allow the row visibility rules

If UI blocks an action:
- API may still allow it (but UI should not call it)
- DB still enforces RLS regardless
---

## 10. Data Access & Row-Level Security (RLS)

Authorization controls **actions**.

Row-Level Security controls **data visibility**.

RLS is enforced **inside the database** and:
- cannot be bypassed by application bugs
- applies to all access paths (API, reports, jobs)

Key principles:
- RLS is the **final enforcement boundary**
- Application-level filters are convenience, not security
- Every tenant-scoped table must be RLS-protected

---

## 11. Defense-in-Depth Summary

| Layer | Responsibility |
|-----|---------------|
| Authentication | Verify identity |
| Identity Resolution | Map to internal user |
| Tenant Resolution | Fix tenant context |
| Authorization | Allow or deny actions |
| RLS | Enforce data isolation |

A failure in one layer **must not compromise others**.

---

## 12. What This Document Is NOT

This document does NOT:
- describe implementation frameworks
- prescribe specific middleware
- include UI or API code
- assume a specific cloud provider

Those details belong in later, focused documents.

---

## 13. Related Documents

- `identity-identifiers.md`
- `authentication.md`
- `authorization.md`
- `tenant-isolation.md`
- `row-level-security.md`

Each expands one layer in detail.

---

## 14. Status

- Phase: **Foundation**
- Stability: **Locked**
- Changes require architectural review

This document is the root of all security documentation.
