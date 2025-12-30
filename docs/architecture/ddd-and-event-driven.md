# Domain-Driven Design (DDD) and Event-Driven Architecture  
## in ImpactHub’s Modular Monolith

**Document Status:** Proposed Architecture Decision Record (ADR)  
**Date:** December 30, 2025  
**Author:** Grok (Software Architect & Senior .NET Fullstack Developer)  
**Location:** `/docs/architecture/ddd-and-event-driven.md`  
(or as a new ADR in `/docs/architecture/ADR/`)

---

## Overview

This document formalizes our commitment to **Domain-Driven Design (DDD)** principles and an **in-process event-driven architecture** within ImpactHub’s **modular monolith**.

It builds on existing governance artifacts:

- `architecture-policy.md`
- `context-map.md`
- Strict module boundaries
- One schema per module
- ID-only cross-references
- No cross-schema foreign keys
- DB-first approach with external schema management

These patterns ensure:

- Long-term maintainability
- Clear domain boundaries
- Loose coupling between modules
- Readiness for future evolution (e.g., extracting modules if needed)

---

## 1. Why DDD in a Modular Monolith?

ImpactHub ERP serves impact organizations with **complex domains**:

- Projects  
- Donors  
- Donations  
- Reporting  
- Beneficiaries  
- …up to 14 modules

DDD helps manage this complexity by aligning the codebase with **business language and real-world processes**.

### Key Benefits (Tailored to ImpactHub)

- **Clear Boundaries**  
  Each module is a *Bounded Context* with its own Ubiquitous Language, models, and rules.

- **Maintainability**  
  Prevents a “big ball of mud” by enforcing isolation  
  (already aligned with our policy: no cross-schema FKs, application-layer integrity).

- **Team Ownership**  
  Teams can own modules end-to-end without coordination overhead.

- **Scalability Path**  
  Modules designed as Bounded Contexts can later be extracted into microservices with minimal rework.

- **Business Focus**  
  Domain logic lives in the **Domain layer**, not scattered across UI or infrastructure.

> We map **Modules ↔ Bounded Contexts** directly (as defined in `context-map.md`).

---

## 2. Strategic DDD Patterns We Adopt

- **Bounded Contexts**  
  One per module (e.g., *Projects Context*, *Donors Context*).

- **Context Mapping**  
  Relationships documented in `context-map.md`:
  - Upstream / Downstream
  - Conformist
  - Anti-Corruption Layer (for integrations)

- **Ubiquitous Language**  
  Business terms are consistent *within* a context.  
  Example:  
  > “Project” may mean different things in *Projects* vs *Finance* contexts — no shared entities.

- **Core Domain**  
  Impact-tracking modules (*Projects, Outcomes, Metrics*) are treated as competitive differentiators.

**Rule:**  
No shared domain models across modules.  
Shared Kernel is limited to primitives only.

---

## 3. Tactical DDD Patterns We Adopt

Within each module (vertical slice or layered structure):

| Pattern | Description | Implementation Guidelines |
|------|-----------|---------------------------|
| **Entities** | Objects with identity and lifecycle (e.g., `Project` with `ProjectId`) | Immutable where possible; private setters |
| **Value Objects** | Immutable objects without identity (e.g., `Money`, `Address`) | Encapsulate validation; override equality |
| **Aggregates** | Cluster of entities/value objects with a root | Enforce invariants here; transactional boundary |
| **Domain Services** | Logic that doesn’t fit entities | Stateless; inject repositories if required |
| **Application Services** | Orchestrate use cases (CQRS) | Thin; implemented as MediatR handlers |
| **Repositories** | Abstract persistence | One per aggregate; DB-first EF Core mapping |

### CQRS Usage

- Applied **lightly**
- Commands = state-changing  
- Queries = read-only  
- Implemented via **MediatR**

---

## 4. Event-Driven Architecture: In-Process Domain Events

To keep modules loosely coupled:

### Communication Strategy

- **Preferred:** Domain Events raised inside a module
- **Cross-Module:** Integration Events published to an in-process bus
- **Avoid:** Direct cross-module method calls

### Why In-Process?

- We are a **monolith**
- No distributed broker needed (yet)
- High performance
- Simple transactions
- Easy deployment

### Tooling

- **MediatR**
  - `INotification`
  - `INotificationHandler`
- Handles:
  - Domain Events (internal)
  - Integration Events (cross-module)

---

### Event Types

- **Domain Events**  
  Internal to a Bounded Context  
  Example:  
  `ProjectCreatedDomainEvent`  
  Used for side effects like auditing.

- **Integration Events**  
  Public contract for other contexts  
  Example:  
  `ProjectApprovedIntegrationEvent`  
  Consumed by Donors or Reporting modules.

---

### Publishing Flow

1. **Aggregate Root / Domain Service**  
   Raise event:
   ```csharp
   AddDomainEvent(new ProjectApprovedIntegrationEvent(...));

    Unit of Work (after SaveChanges)
    Publish events via MediatR.

    Handlers React

        Update read models

        Trigger workflows

        Enforce eventual consistency

Architectural Guarantees

    Eventual Consistency
    No distributed transactions across modules.

    Idempotency & Ordering
    Event handlers must be idempotent.
    Event versioning is required.

    Future-Proofing
    If scaling out:

        Introduce Outbox Pattern

        Replace in-process bus with:

            MassTransit

            RabbitMQ or Azure Service Bus
            Minimal refactoring required.

5. Enforcing the Rules

    No Direct Cross-Module Calls
    Only events or explicitly approved public APIs.

    CI Enforcement
    Extend architecture-check.yml to detect violations.

    PR Reviews
    Violations are rejected.
    Event-based integration is mandatory.

    Exceptions
    Must be documented as ADRs.

6. Example: Projects Module → Donors Module
Projects Module (Domain)

public class Project : AggregateRoot
{
    public void Approve()
    {
        // business logic
        AddDomainEvent(
            new ProjectApprovedIntegrationEvent(
                ProjectId,
                TenantId,
                ApprovedDate
            )
        );
    }
}

Integration Event Contract

public record ProjectApprovedIntegrationEvent(
    Guid ProjectId,
    Guid TenantId,
    DateTime ApprovedDate
) : INotification;

Donors Module (Application Handler)

public class ProjectApprovedHandler 
    : INotificationHandler<ProjectApprovedIntegrationEvent>
{
    public async Task Handle(
        ProjectApprovedIntegrationEvent @event,
        CancellationToken ct)
    {
        // React:
        // - notify potential donors
        // - update statistics
        // eventual consistency, no shared transaction
    }
}

7. References & Inspirations

    Eric Evans — Domain-Driven Design

    Kamil Grzybek — Modular Monolith with DDD

    Vaughn Vernon — Implementing Domain-Driven Design

    Milan Jovanović & Mehmet Ozkaya — .NET Modular Monolith Guides

    MediatR Documentation — In-process messaging

Policy Statement

This policy is binding.

    All new modules (starting with Projects in Phase 3) must comply.....




        ROADMAP.md

        architecture-policy.md
