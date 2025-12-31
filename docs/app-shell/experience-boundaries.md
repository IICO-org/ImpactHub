# App Shell Experience Boundaries  
IMPACT HUB ERP — Phase 1

## 1. Purpose

This document defines **Experience Boundaries** within the **App Shell**.

It answers:
- What an *experience* is
- Which experiences exist
- What each experience can and cannot access
- How experiences differ from modules
- How routing, menus, and permissions are applied per experience

This document is **standalone** and is a **governance contract** for UI and backend teams.

---

## 2. Core Definitions

### 2.1 App Shell
The **App Shell** is the shared UI foundation that handles:
- authentication entry
- layout (header, sidebar)
- routing & guards
- access profile consumption

It is **not** tied to a specific user audience.

---

### 2.2 Experience
An **Experience** is a **bounded UI context** for a specific audience.

Examples:
- Internal staff
- External partners
- Donors
- Executives

Experiences:
- run inside the same App Shell
- share the same security model
- differ in routes, menus, and permissions

> **One App Shell, many Experiences.**

---

## 3. Why Experience Boundaries Exist

Without experience boundaries:
- all users see the same navigation
- permissions become unreadable
- UI logic becomes scattered
- security intent becomes unclear

Experience boundaries provide:
- clarity
- least-privilege UI exposure
- clean ownership
- future scalability

---

## 4. Experiences in Phase 1 (Locked)

Phase 1 includes **four experiences**:

| Experience | Audience |
|-----------|----------|
| Admin | Internal operations & staff |
| Partner | External partner organizations |
| Donor | Donors / public-facing users |
| Executive | Top management |

All experiences are hosted by the **same App Shell**.

---

## 5. Experience vs Tenant vs Permission (Important)

These are **not the same thing**:

| Concept | Purpose |
|------|---------|
| Tenant | Data isolation & ownership |
| Experience | UI context & navigation |
| Permission | Allowed actions |

A user:
- belongs to **one tenant** (Phase 1)
- accesses **one experience at a time**
- has **permissions** that control actions

---

## 6. How Experience Is Determined

### Phase 1 Rule
Experience is determined by:
- user type
- assigned permissions
- implicit routing

Experience is **not**:
- guessed by role name
- inferred from tenant alone

In Phase 1, experience selection is **implicit**:
- Internal staff → Admin or Executive
- External partner → Partner
- Donor → Donor

In Phase 2+, experience selection can be explicit (selector).

---

## 7. Admin Experience

### 7.1 Purpose
The **Admin Experience** is for:
- internal staff
- operational users
- system administrators

### 7.2 Accessible Modules
Admin experience may access **all core ERP modules**, subject to permissions:

- Strategic Planning
- Donors
- Projects
- Sponsorships
- Donations
- Money Transfer
- Partners
- CRM
- Marketing
- Impact Measurement
- Website
- Branches
- Bank Deductions
- Shared Services (Settings)

### 7.3 Admin-Only Screens
Some screens are **exclusive** to Admin:

- User management
- Role & permission management
- Tenant settings
- Audit logs
- System configuration

### 7.4 Default Landing

/admin


---

## 8. Partner Experience

### 8.1 Purpose
The **Partner Experience** is for:
- external organizations
- implementation partners
- data contributors

Partners are **not administrators**.

---

### 8.2 Access Characteristics
Partner users:
- see a **restricted subset** of modules
- never see system configuration
- never manage users or roles
- are limited by permissions and RLS

---

### 8.3 Typical Accessible Modules
(Example — exact permissions still apply)

- Projects (read / limited write)
- Sponsorships (read)
- Donations (read)
- Reporting (limited)

### 8.4 Forbidden Areas
Partner experience must never show:

- Tenant settings
- User management
- Role/permission management
- Audit logs
- Financial posting screens

### 8.5 Default Landing

/partner


---

## 9. Donor Experience

### 9.1 Purpose
The **Donor Experience** is a **public-facing** or semi-public UI for donors.

It is conceptually different from ERP users.

---

### 9.2 Access Characteristics
Donors:
- never see internal ERP modules
- access only donor-specific screens
- see only their own data (via RLS / filters)

### 9.3 Typical Screens
- Donation history
- New donation
- Receipts
- Impact summaries

### 9.4 Security Notes
Donor experience:
- may use different authentication later
- must still integrate with App Shell access profile
- must not reuse Admin or Partner routes

### 9.5 Default Landing

/donor


---

## 10. Executive Experience

### 10.1 Purpose
The **Executive Experience** is for:
- board members
- top management
- decision makers

It is **read-heavy**, not transactional.

---

### 10.2 Access Characteristics
Executives:
- see dashboards
- see aggregated views
- rarely perform write actions

### 10.3 Typical Screens
- Portfolio dashboards
- Financial summaries
- Impact dashboards
- KPI views

### 10.4 Forbidden Actions
Executives typically:
- do not create/edit records
- do not manage configurations

### 10.5 Default Landing

/executive


---

## 11. Sidebar Composition Rules (Per Experience)

Sidebar is built using **three inputs**:

1. Experience
2. Tenant enabled features
3. User permissions

### Rule
A menu item appears if:
- experience allows the module
- feature is enabled for tenant
- user has at least one relevant permission

---

## 12. Routing Rules (Hard Boundaries)

### Non-Negotiable Rules

- An experience must never route to another experience’s root
- Cross-experience navigation is forbidden
- Shared components are allowed; shared routes are not

Examples:
- Admin user visiting `/partner/*` → Not Authorized
- Partner user visiting `/admin/*` → Not Authorized

---

## 13. Experience Boundaries vs Permissions

Experience boundaries are **coarse-grained**.  
Permissions are **fine-grained**.

| Layer | Responsibility |
|----|----------------|
| Experience | Which UI world you are in |
| Feature | Which modules exist |
| Permission | What actions you can perform |
| RLS | Which rows you can see |

All must agree for an action to succeed.

---

## 14. Evolution Strategy (Future-Proof)

Phase 1:
- experience is implicit

Phase 2+:
- experience selector
- multi-experience users
- shared dashboards across experiences

This document ensures future expansion without breaking contracts.

---

## 15. Anti-Patterns (DO NOT DO)

❌ Use roles as experience identifiers  
❌ Infer experience from URL alone  
❌ Share admin routes with partners  
❌ Allow donors to hit ERP APIs  
❌ Mix experience logic into modules  

---

## 16. Relationship to Other Documents

This document builds on:
- `app-shell-architecture.md`
- `route-map.md`
- `access-profile-contract.md`
- `ui-access-control.md`

---

## 17. Status

- Phase: 1 (Foundation)
- Experiences: Locked
- Default routes: Defined
- Stability: Locked
- Changes require architectural review
