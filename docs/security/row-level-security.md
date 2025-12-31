# Row Level Security (RLS)  
IMPACT HUB ERP — Phase 1

## 1. Purpose

RLS enforces:

> “Which rows of data is this user allowed to see or modify?”

It is the **final enforcement boundary**.

---

## 2. Why RLS Is Mandatory

- Prevents data leakage
- Protects against API bugs
- Applies to all access paths (API, reports, jobs)

---

## 3. RLS Pattern

The system uses:

- Core predicate function
- Optional wrapper functions
- Security policy per table

---

## 4. Data Scope (ABAC)

Row access is controlled via:
- TenantId
- Office
- Country
- ProjectCategory

Allow rules:
- explicit inclusion

Deny rules:
- override allow

---

## 5. NULL Handling

If a row has NULL in a scoped column:
- access is denied
- data quality issues are exposed early

---

## 6. Session Context

RLS depends on:
- UserId
- TenantId

These must be set for every DB connection.

---

## 7. Status

- Phase: Foundation
- Stability: Locked
