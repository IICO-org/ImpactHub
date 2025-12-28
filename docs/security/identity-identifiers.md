# Identity Identifiers & External Providers  
IMPACT HUB ERP — Phase 1

## 1. Purpose

This document defines **how identities are represented and resolved** in IMPACT HUB ERP.

It assumes:
- no prior documentation
- no implementation code
- a future multi-tenant SaaS roadmap

Its goal is to prevent **provider lock-in**, **schema pollution**, and **identity ambiguity**.

---

## 2. Problem Statement

Different identity providers return different identifiers:

| Provider | Identifier |
|--------|-----------|
| Microsoft Entra | `oid` |
| Google | `sub` |
| Okta | `sub` |
| Auth0 | `sub` |
| SAML | `NameID` |
| Local auth | username / internal id |

There is **no universal identifier** such as “OID” across providers.

Therefore:
> Storing provider-specific identifiers directly in domain tables is a design error.

---

## 3. Core Principle

IMPACT HUB ERP separates:

- **Who the user is inside the system**
- **How the user authenticates externally**

### Golden Rule
> Domain logic must never depend on provider-specific identifiers.

---

## 4. Internal Principal Model

The system defines a single internal principal:

- **UserId (int)** — primary key
- Stored in `iam.Users`
- Belongs to exactly one tenant in Phase 1

All authorization, auditing, and ownership reference **UserId only**.

---

## 5. External Identity Model

External identities are stored in a dedicated table:

**`iam.UserIdentities`**

Each row represents:

(UserId) ← (Provider, Issuer, SubjectId)


### Fields (Conceptual)
- Provider: identity provider code (`entra`, `local`, future)
- Issuer: identity authority (optional but important for SaaS)
- SubjectId: provider-specific stable identifier

### Uniqueness
- `(Provider, Issuer, SubjectId)` must be globally unique
- Prevents identity collision across tenants or providers

---

## 6. Why This Design Is Mandatory for SaaS

This model allows:
- adding new providers without schema changes
- migrating users between providers
- supporting multiple IdPs per user (future)
- avoiding “drop and rebuild” identity refactors

---

## 7. Provider Mapping Examples

| Provider | Provider Code | SubjectId Source |
|-------|--------------|------------------|
| Entra | `entra` | `oid` claim |
| Local partner | `local` | system username |
| Google (future) | `google` | `sub` claim |
| Okta (future) | `okta` | `sub` claim |

---

## 8. Explicit Non-Goals

This document does NOT:
- prescribe OAuth/OIDC libraries
- describe token validation
- define login UI

Those belong to authentication flow docs.

---

## 9. Status

- Phase: Foundation
- Stability: Locked
- Changes require architectural review
