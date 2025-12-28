# Authorization  
IMPACT HUB ERP — Phase 1

## 1. Purpose

Authorization answers:

> “What is this user allowed to do?”

This document defines how permissions are modeled and enforced.

---

## 2. Authorization Layers

Authorization occurs at two levels:

1. **API Authorization**
   - Controls allowed actions
2. **UI Authorization**
   - Controls what the user sees and can interact with

Both use the same permission model.

---

## 3. Permission Model

Permissions are:
- explicit
- named
- business-oriented

Examples:
- Projects.Read
- Projects.Write
- Donations.Approve

Permissions are granted via roles.

---

## 4. Read vs Write Semantics

- **Read**
  - view lists
  - view details
- **Write**
  - create
  - update
  - delete
  - approve

UI and API must apply the same semantics.

---

## 5. Bypass Privilege

A special privilege exists:

- `CanBypassDataScope`

Characteristics:
- explicit
- auditable
- tenant-bound
- not a data dimension

Used only for:
- platform owners
- controlled support scenarios

---

## 6. UI Access Control

UI access control:
- hides or disables screens and actions
- improves UX
- is NOT a security boundary

API + DB remain authoritative.

---

## 7. Failure Rules

If:
- UI allows → API must allow
- UI blocks → API may still block
- API allows → DB may still block via RLS

---

## 8. Status

- Phase: Foundation
- Stability: Locked
