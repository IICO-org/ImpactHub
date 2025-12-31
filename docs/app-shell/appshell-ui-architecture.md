# App Shell Architecture  
IMPACT HUB ERP — Phase 1

## 1. Purpose of This Document

This document defines the **App Shell** architecture for IMPACT HUB ERP.

It explains:
- what the App Shell is and is not
- its responsibilities and boundaries
- how it hosts multiple user experiences
- how it integrates with backend security (auth, permissions, RLS)
- how it evolves without breaking the system

This document is **standalone** and assumes:
- no prior UI documentation
- no frontend framework assumptions
- no existing codebase

It is intended to be read by:
- frontend developers
- backend developers
- solution architects
- future maintainers

---

## 2. What Is the App Shell?

The **App Shell** is the **shared UI foundation** of the system.

It is **not** a portal for a specific audience.

Instead, it is:
- the single entry point to the UI
- the host for multiple user experiences
- the place where security context is initialized and applied

### Definition

> **App Shell**  
> A shared UI container responsible for authentication entry, layout, routing, access control, and experience hosting.

---

## 3. What the App Shell Is NOT

The App Shell is **not**:
- a business module
- a replacement for backend authorization
- a place to implement business rules
- a database client
- a single-user portal

Business logic lives in:
- backend modules
- backend authorization services
- database RLS

The App Shell consumes decisions; it does not invent them.

---

## 4. App Shell in the Overall Architecture

High-level position:

Browser
↓
App Shell
↓
REST APIs (WebHost)
↓
Authorization Services
↓
Database (RLS enforced)


The App Shell:
- never bypasses the backend
- never talks directly to the database
- never trusts client-side logic alone

---

## 5. Experiences Hosted by the App Shell

The App Shell hosts **multiple experiences**.

An **experience** is a bounded UI context for a specific audience.

### Phase 1 Experiences

| Experience | Target Users |
|-----------|--------------|
| Admin | Internal staff & operations |
| Partner | External partners |
| Donor | Donors / public users |
| Executive | Top management |

All experiences:
- share the same shell
- share authentication and security
- differ in routes, menus, and permissions

---

## 6. Repository Structure (UI)

Recommended structure:

/app-shell
/src
/layout # shell layout (header, sidebar, footer)
/routing # route definitions & guards
/guards # feature & permission guards
/auth # login flows (entra, local)
/state # global state (access profile)
/services # API clients
/ui # shared UI components

/experiences
/admin
/partner
/donor
/executive

package.json


Key rule:
> Experiences never implement their own shell or security logic.

---

## 7. Responsibilities of the App Shell

### 7.1 Authentication Entry

The App Shell:
- provides login entry points
- supports:
  - internal (enterprise IdP)
  - partner (local credentials)

After authentication:
- the shell fetches the **Access Profile**
- no UI is rendered before that

---

### 7.2 Global Layout

The App Shell owns:
- sidebar navigation
- top header
- content container
- global loading & error states

Experiences render **inside** the shell.

---

### 7.3 Routing & Navigation

The App Shell:
- defines all application routes
- applies route guards
- delegates route rendering to experiences

Experiences:
- do not define top-level routes independently
- do not bypass guards

---

### 7.4 Access Control (UI Layer)

The App Shell:
- consumes the **Access Profile**
- applies:
  - feature gating
  - permission gating
- controls:
  - menu visibility
  - route accessibility
  - read vs write UI behavior

The App Shell **never** hardcodes permissions.

---

## 8. Access Profile Integration (Critical)

After login, the App Shell must fetch:

> **Access Profile** (single API call)

This profile contains:
- tenant info
- enabled features
- user permissions
- user flags

The App Shell:
- stores it in global state
- treats it as immutable for the session
- rebuilds UI from it

No other permission source is allowed in the UI.

---

## 9. Feature Gating vs Permission Gating

### Feature Gating (Tenant-level)
Controls:
- whether a module exists at all

Example:
- Tenant does not have “Donations” → hide Donations everywhere

---

### Permission Gating (User-level)
Controls:
- what the user can do inside an enabled module

Example:
- User has `Projects.Read` but not `Projects.Write`
  → show Projects, but read-only

Both gates must pass for a screen/action to be available.

---

## 10. Read vs Write UX Semantics

The App Shell enforces consistent semantics:

### Read Permission
- view lists
- view details
- access routes

### Write Permission
- show create/edit buttons
- enable form fields
- allow submit actions

Write permission is **never implied** by Read.

---

## 11. Error Handling & UX Standards

The App Shell must handle:

| Backend Response | UI Behavior |
|-----------------|-------------|
| 401 | Redirect to login |
| 403 | Show “Not Authorized” |
| 200 + empty list | Show empty state |
| Network error | Show retry/error page |

RLS “no rows” is not an error.

---

## 12. Non-Negotiable Rules

❌ App Shell must not trust UI-only checks  
❌ App Shell must not infer permissions from roles  
❌ App Shell must not hardcode admin behavior  
❌ Experiences must not bypass guards  
❌ Multiple shells must not exist  

✅ Single shell, multiple experiences  
✅ Single access profile  
✅ Backend + DB remain authoritative  

---

## 13. Evolution Path (Future-Proof)

This architecture supports:
- adding new experiences without re-auth
- mobile apps using the same access profile
- micro-frontend splitting later
- advanced dashboards

Because:
- security is centralized
- experiences are isolated
- contracts are explicit

---

## 14. Relationship to Other Documents

This document builds on:
- `security-overview.md`
- `authentication.md`
- `authorization.md`
- `ui-access-control.md`

Next documents:
- `access-profile-contract.md`
- `route-map.md`
- `experience-boundaries.md`

---

## 15. Status

- Phase: 1 (Foundation)
- Scope: Shell + routing + access control
- Business screens: Not yet implemented
- Stability: Locked
