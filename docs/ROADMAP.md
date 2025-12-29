# IMPACT HUB ERP — Roadmap & Documentation Index (Living File)

This file prevents scope drift and ensures we never lose previously agreed deliverables.

---

## 1) Naming Decision (Locked)
- UI product name: **App Shell** (not Portal)
- “Portals” are **Experiences** hosted inside the App Shell:
  - Admin Experience
  - Partner Experience
  - Donor Experience
  - Executive Experience

Repo layout (locked):
- `/src` = backend (.NET)
- `/app-shell` = frontend shell
- `/docs` = documentation

---

## 2) Completed Documentation (Security)
- `/docs/security/security-overview.md`
- `/docs/security/identity-identifiers.md`
- `/docs/security/authentication.md`
- `/docs/security/authorization.md`
- `/docs/security/ui-access-control.md`
- `/docs/security/tenant-isolation.md`
- `/docs/security/row-level-security.md`
- `/docs/security/backend-wiring.md`

---

## 3) Completed Implementation Proofs (DB / RLS pattern)
- Core RLS predicate + wrapper pattern validated
- RLS applied to `Project.Projects` (for testing)
- Bypass migrated from “dimension record” → `iam.Users.CanBypassDataScope`
- Identity provider–agnostic model adopted (provider-specific user columns removed)

Note: Module-wide RLS rollout deferred to module build time.

---

## 4) Next Documentation Deliverables (UI / App Shell) — ORDERED
### 4.1 App Shell Architecture (FOUNDATIONAL)
- `/docs/app-shell/app-shell-architecture.md`
  - shell responsibilities
  - auth entry points (Entra + Local)
  - layout & navigation
  - route guards
  - consuming access profile

### 4.2 Access Profile Contract (RENAME from portal)
- `/docs/app-shell/access-profile-contract.md`
  - (rename content from previous “portal” contract)
  - single API response for UI permissions/features
  - versioning strategy

### 4.3 Route Map for Phase 1 (previously suggested — MUST DO)
- `/docs/app-shell/route-map.md`
  - full route list for all modules (placeholders)
  - required permissions per route
  - which experience owns each route

### 4.4 Experience Boundaries
- `/docs/app-shell/experience-boundaries.md`
  - Admin vs Partner vs Donor vs Executive
  - what each can access
  - UI menu structure per experience

---

## 5) Phase 1 Build Roadmap (Engineering) — ORDERED
### 5.1 Backend Foundation
- Auth validation:
  - Entra JWT validation
  - Local partner JWT validation
- Identity resolution via `iam.UserIdentities`
- Tenant resolution via `iam.Users.TenantId`
- Permission evaluation service
- DB session context setter (`UserId`, `TenantId`)

### 5.2 App Shell Skeleton
- Login entry:
  - Entra sign-in
  - Partner local login
- Load access profile
- Build sidebar dynamically
- Placeholder routes for modules

### 5.3 Security Smoke Tests
- RLS enforced end-to-end
- Permissions enforced end-to-end
- UI gating consistent with API

---

## 6) Backlog (Do Later, Not Forgotten)
- `/docs/security/seed-data.md` (dev seed approach with safe constraints)
- `/docs/security/audit-logging.md` (audit strategy and event taxonomy)
- `/docs/security/token-claims.md` (exact claims, mapping rules)
- `/docs/modules/<module>/onboarding.md` (per module playbook: DDL → RLS → permissions → UI)

---

## 7) Rules for This File (Non-Negotiable)
- Any new idea becomes a line item here immediately.
- Renames must be reflected here.
- No work begins unless it is listed here in “Next Deliverables” or “Build Roadmap”.
