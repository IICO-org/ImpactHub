# UI Access Control (Screens, Actions, Read/Write)
IMPACT HUB ERP — Phase 1

## 1. Purpose

This document defines how **UI-level access control** works in IMPACT HUB ERP.

It answers the questions:

- What screens should a user see?
- What actions should be enabled or disabled?
- When is a screen read-only vs editable?
- How does UI security relate to API authorization and database RLS?

This document is **standalone** and assumes no prior documentation.

---

## 2. What UI Access Control Is (and Is Not)

### UI Access Control IS:
- A **user experience enforcement layer**
- A way to:
  - hide irrelevant modules
  - disable forbidden actions
  - prevent users from attempting invalid operations

### UI Access Control IS NOT:
- A security boundary
- A replacement for API authorization
- A replacement for database Row-Level Security (RLS)

> **Never trust the UI alone.**  
> UI rules must always be enforced again by the API and database.

---

## 3. Position in the Security Stack

UI Access Control sits **between Authorization and Data Access**:

Authentication
↓
Identity Resolution
↓
Tenant Resolution
↓
Authorization (API permissions)
↓
UI Access Control (visibility & enablement)
↓
Row-Level Security (DB)


- API Authorization decides *what is allowed*
- UI Access Control decides *what is shown*
- RLS decides *what data is actually returned*

---

## 4. Two Independent Gates in the UI

Every UI element is controlled by **two gates**:

### Gate 1 — Tenant Feature Gate
> “Is this feature/module enabled for this tenant?”

Examples:
- Tenant subscribed to Projects module?
- Tenant enabled Donations workflow?

If **NO** → the module/screen is completely hidden.

---

### Gate 2 — User Permission Gate
> “Does this user have permission to access this screen/action?”

Examples:
- Projects.Read
- Projects.Write
- Donations.Approve

If **NO**:
- Screen may be hidden, OR
- Screen shown in read-only mode

---

## 5. Permission-Driven UI (Core Principle)

The UI must be driven by the **same permission model** used by the backend.

There must be **no UI-only permissions**.

### Example Permission Naming
- `Projects.Read`
- `Projects.Write`
- `Donations.Read`
- `Donations.Approve`

These permissions are:
- stored in the database
- evaluated by the API
- consumed by the UI

---

## 6. Read vs Write Semantics (Critical)

### Read Permission
Controls:
- access to lists
- access to detail views
- visibility of screens

If user has **Read only**:
- UI must render:
  - lists
  - details
- UI must hide or disable:
  - Create
  - Edit
  - Delete
  - Approve

---

### Write Permission
Controls:
- create operations
- edit operations
- delete operations
- approval workflows

If user has **Write**:
- UI enables:
  - buttons
  - form fields
  - submit actions

---

## 7. Read-Only Screens (Preferred UX Pattern)

When a user has:
- Feature enabled
- Read permission
- No Write permission

The recommended UX is:
- Show the screen
- Render fields as **read-only**
- Hide action buttons

This avoids confusion and improves transparency.

---

## 8. UI Access Profile (Login Output)

After login, the backend must return a **User Access Profile**.

This is the **single source of truth** for the UI.

### Conceptual Structure

```json
{
  "tenant": {
    "tenantId": "GUID",
    "enabledFeatures": [
      "Projects",
      "Donations"
    ]
  },
  "user": {
    "userId": 123,
    "permissions": [
      "Projects.Read",
      "Projects.Write",
      "Donations.Read"
    ]
  }
}

The UI must not call permission APIs repeatedly.
It builds the entire UX from this profile.
