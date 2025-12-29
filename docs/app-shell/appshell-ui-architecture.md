# Portal UI Architecture (Shell, Routing, Access Control)
IMPACT HUB ERP — Phase 1

## 1. Purpose

This document defines the Portal UI architecture for Phase 1.

It explains:
- the Portal project boundaries (separate from WebHost)
- the Portal shell and navigation model
- routing and route guards
- how the portal consumes the backend “UI Access Profile”
- how read/write UI behavior is enforced consistently

This document is standalone and assumes no prior UI documentation.

---

## 2. Phase 1 UI Scope (What We Build Now)

Phase 1 UI is a **skeleton portal**, not full module screens.

### Included
- Login entry points:
  - Internal (Entra)
  - Partner (Local)
- Portal Shell:
  - sidebar navigation
  - header (tenant/user)
  - module placeholders
- Route structure for all modules (stubs)
- UI access control:
  - feature gating
  - permission gating
  - read-only vs editable behavior

### Excluded
- Complex business screens for each module
- Offline / branch complex workflows
- Advanced dashboards (Phase 2+)

---

## 3. Project Boundary Rules (Non-Negotiable)

### Separate projects
- `WebHost` = APIs + security + backend domain modules
- `Portal`  = UI only

### Portal MUST NOT:
- contain business logic
- bypass API authorization
- query database directly

### Portal MUST:
- work entirely via REST API calls
- apply UI access control using the access profile
- show consistent “read vs write” UX

---

## 4. Portal Structure (High Level)

Recommended portal structure (conceptual):

/Portal
/src
/app
/layout
/routes
/guards
/services
/state
/ui
/modules
/projects
/donations
/donors
... (stubs)
/auth
/entra
/local
/public


The goal is clean separation:
- Auth (login)
- Shell (layout)
- Guards (access checks)
- Module stubs (routes + placeholders)

---

## 5. Portal Shell Layout

Portal shell is the permanent frame that contains:
- left sidebar
- top header bar
- main content area

### Shell must show
- Tenant name
- User display name
- Environment marker (DEV/PROD)

### Shell must support
- Module navigation
- Responsive layout (desktop first)
- Ability to add module tiles later

---

## 6. Navigation Model (Sidebar)

Sidebar entries are built from:
1) Tenant enabled features
2) User permissions

### Rule
A module appears in the sidebar if:
- tenant has the module enabled AND
- user has at least one permission in that module

Example:
- Tenant enabled: Projects
- User permissions: Projects.Read
→ Show Projects menu

If tenant has Projects but user has none:
→ Hide Projects menu

---

## 7. Routing Strategy

Each module has:
- root route
- list route
- details route
- create/edit routes (if write permission exists)

Example for Projects:
- `/projects`
- `/projects/list`
- `/projects/:id`
- `/projects/new`
- `/projects/:id/edit`

In Phase 1, these can be placeholders:
- list view stub
- detail view stub
- “not implemented” messages

But routing must be real so the shell is future-proof.

---

## 8. Route Guards (Access Control)

Every route declares:
- required feature
- required permission

### Example route guard rule
- `/projects/list`
  - feature: Projects
  - permission: Projects.Read

- `/projects/new`
  - feature: Projects
  - permission: Projects.Write

If guard fails:
- Redirect to `/not-authorized`
- Or render a not-authorized component

Route guards must be centralized (not scattered in components).

---

## 9. UI Access Profile (Single Source of Truth)

After successful login, the portal must fetch:

> **UI Access Profile** from the backend (one call)

The profile provides:
- Tenant enabled features (modules)
- User permissions (flattened list)
- User flags (e.g., CanBypassDataScope)

Portal stores this profile in a global state store.

The portal must NOT:
- query permissions repeatedly
- guess permissions from roles
- hardcode admin logic

---

## 10. Read vs Write UX Rules

### If user has Read only
- show list & details pages
- show forms as read-only
- hide create/edit/delete buttons

### If user has Write
- show create/edit buttons
- enable form fields
- enable save actions

Write must never be implied from Read.

---

## 11. Error UX Standardization

Portal must handle these cases consistently:

- 401 Unauthorized (token invalid/expired)
  - redirect to login
- 403 Forbidden (permission missing)
  - show “Not Authorized”
- 200 Empty list (RLS filtered rows)
  - show empty state, not an error

RLS “no rows” is normal behavior.

---

## 12. Authentication UX Flow

### Internal (Entra)
- click “Login with Entra”
- complete provider login
- portal receives token
- portal calls backend access profile
- portal loads shell

### Partner (Local)
- username/password form
- backend issues token
- portal calls backend access profile
- portal loads shell

Both flows must converge after token acquisition.

---

## 13. Minimal Screens Required in Phase 1

- Login page:
  - Entra login button
  - Partner login form
- Tenant selection:
  - not needed in Phase 1 if user belongs to exactly one tenant
- Portal shell:
  - sidebar + placeholders
- Not authorized page
- Error page
- Loading states

---

## 14. Future Readiness (Phase 2+)

This architecture supports:
- adding mobile client using same APIs
- adding GraphQL later for reporting (optional)
- advanced dashboards
- dynamic menus per module
- deep linking from notifications

---

## 15. Non-Negotiable Rules

❌ No UI-only permission logic  
❌ No hidden admin shortcuts in UI  
❌ No direct DB queries  
❌ No mixing WebHost and Portal projects  
✅ UI access profile is mandatory  
✅ Routes are guarded centrally  
✅ Read vs Write semantics enforced consistently

---

## 16. Status

- Phase: 1 (Portal Skeleton)
- Scope: Shell + routing + access control
- Business screens: later
- Stability: Locked
