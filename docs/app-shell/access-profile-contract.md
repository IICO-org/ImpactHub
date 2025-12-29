##App Shell Access Profile Contract

IMPACT HUB ERP — Phase 1

1. Purpose

This document defines the Access Profile Contract between the backend and the App Shell.

The Access Profile is the single, authoritative payload that the App Shell uses to:

build navigation (sidebar & menus)

guard routes

control read vs write UI behavior

distinguish experiences (Admin / Partner / Donor / Executive)

avoid repeated permission calls

This document is standalone and assumes no prior documentation.

2. Why an Access Profile Exists

Without an access profile, UI teams typically:

hardcode permissions

infer permissions from roles

call permission APIs repeatedly

drift from backend rules

All of these lead to:

inconsistent UX

security bugs

maintenance hell

Core Rule (Non-Negotiable)

The App Shell must build the entire UI from one backend response.

That response is the Access Profile.

3. Access Profile Lifecycle
When it is fetched

Immediately after successful authentication

Once per session (with refresh on token renewal)

How it is used

Stored in App Shell global state

Treated as read-only

Replaced only by re-fetch (never mutated client-side)

When it is invalidated

Token expiration

401 from backend

Explicit logout

4. Endpoint Definition
Method
GET

URL
/api/me/access-profile

Authentication

Required

Works for:

Internal users (Entra)

Partner users (Local)

Responses
HTTP	Meaning
200	Access profile returned
401	Not authenticated / token invalid
403	User inactive or tenant inactive
5. High-Level Structure

The Access Profile contains four top-level sections:

{
  "contractVersion": "1.0",
  "issuedAtUtc": "ISO-UTC",

  "tenant": { },
  "user": { },
  "permissions": [ ],
  "uiCapabilities": { }
}


Each section has a clear responsibility.

6. Contract Versioning
contractVersion

Semantic version (e.g. "1.0")

Allows:

backward-compatible additions

controlled breaking changes

Rule

The App Shell must fail gracefully if it receives an unknown major version.

7. Tenant Section
Purpose

Defines tenant-level context and feature availability.

Structure
"tenant": {
  "tenantId": "GUID",
  "code": "IICO_1",
  "nameEn": "IICO",
  "isActive": true,
  "enabledFeatures": [
    "Projects",
    "Donations",
    "Donors",
    "CRM"
  ]
}

Rules

enabledFeatures drives module visibility

If a feature is not listed:

the module must not appear in UI

routes under it must be blocked

Feature gating is independent from user permissions.

8. User Section
Purpose

Defines who the user is and non-permission flags.

Structure
"user": {
  "userId": 123,
  "displayNameEn": "Partner User",
  "displayNameAr": "مستخدم شريك",
  "email": "partner.user@partner.org",
  "preferredLang": "ar",
  "authProvider": "local",
  "isSystemAdmin": false,
  "flags": {
    "canBypassDataScope": false
  }
}

Notes

authProvider is informational (UI display / diagnostics)

flags expose explicit privileges, not permissions

UI must not derive permissions from flags

9. Permissions Section (Core)
Purpose

Defines what the user is allowed to do.

Structure
"permissions": [
  "Projects.Read",
  "Projects.Write",
  "Donations.Read",
  "Donations.Approve"
]

Characteristics

Flattened list

Fully evaluated by backend

Tenant-scoped

No role information included

Rules

UI must never guess permissions

UI must never check roles

Permission strings are contracts

10. Read vs Write Semantics

The UI interprets permissions using strict semantics:

Permission	UI Effect
*.Read	Show lists & details
*.Write	Enable create/edit/delete
*.Approve	Enable approval actions
*.Post	Enable posting/finalization

Write is never implied by Read.

11. uiCapabilities Section (Optional but Recommended)
Purpose

Provide precomputed booleans to simplify UI logic.

This avoids repeated string checks across components.

Structure
"uiCapabilities": {
  "projects": {
    "canRead": true,
    "canWrite": true
  },
  "donations": {
    "canRead": true,
    "canApprove": false
  }
}

Rules

Derived only from permissions

Backend-generated

UI treats this as a convenience, not authority

12. Building the UI from the Access Profile
Sidebar

A module appears if:

feature exists in tenant.enabledFeatures

user has any permission with prefix ModuleName.

Routes

A route is accessible if:

feature enabled

required permission exists

Buttons & Actions

Shown/enabled if:

corresponding permission exists

13. Failure & Edge Cases
RLS Returns Zero Rows

UI must show empty state

Not an error

Not a permission failure

Permission Changes Mid-Session

Reflected only after:

token refresh

profile re-fetch

Tenant Suspended

Backend returns 403

UI shows access denied page

14. Security Guarantees

The Access Profile:

guides UI

does not enforce security

Enforcement is done by:

API authorization

Database RLS

UI must assume:

“Even if I show it, backend may still deny.”

15. Anti-Patterns (DO NOT DO)

❌ UI infers permissions from roles
❌ UI hardcodes admin behavior
❌ UI checks database values directly
❌ UI caches permissions indefinitely
❌ UI builds menus without feature checks

16. Relationship to Other Documents

This document depends on:

security-overview.md

authorization.md

ui-access-control.md

app-shell-architecture.md

Related documents:

route-map.md

experience-boundaries.md

17. Status

Phase: 1 (Foundation)

Contract Version: 1.0

Stability: Locked

Changes require architectural review
