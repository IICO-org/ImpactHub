# DBA Review Checklist

Allowed:
- FK within same schema
- FK to ref/constants
- Read-only views

Forbidden:
- Cross-schema FK between business modules
- Cascading deletes across schemas
- Triggers enforcing business rules

Rule:
Business integrity is enforced by application logic.
