Domain-Driven Design (DDD) and Event-Driven Architecture in ImpactHub's Modular Monolith
Document Status: Proposed Architecture Decision Record (ADR)
Date: December 30, 2025
Author: Grok (as Software Architect & Senior .NET Fullstack Developer)
Location: Upload to /docs/architecture/ddd-and-event-driven.md (or as a new ADR in /docs/architecture/ADR/)
This document formalizes our commitment to Domain-Driven Design (DDD) principles and an in-process event-driven approach within our modular monolith architecture. It builds on existing governance (e.g., architecture-policy.md, context-map.md, strict module boundaries, one schema per module, ID-only cross-references, DB-first with external schema management).
We adopt these patterns to ensure long-term maintainability, clear domain boundaries, loose coupling between modules, and readiness for future evolution (e.g., extracting modules if needed).
1. Why DDD in a Modular Monolith?
Our ImpactHub ERP serves impact organizations with complex domains (Projects, Donors, Donations, Reporting, Beneficiaries, etc.—up to 14 modules). DDD helps manage this complexity by aligning code with business language and realities.
Key Benefits (tailored to our project):

Clear Boundaries: Each module becomes a Bounded Context – a self-contained area with its own Ubiquitous Language, models, and rules.
Maintainability: Prevents "big ball of mud" by enforcing isolation (already in our policy: no cross-schema FKs, application-layer integrity).
Team Ownership: Distributed teams can own modules end-to-end without conflicts.
Scalability Path: Modules designed as Bounded Contexts can be extracted to microservices later with minimal rework.
Business Focus: Domain logic stays in the Domain layer, not scattered in UI or infrastructure.

We map Modules ↔ Bounded Contexts directly (as implied in context-map.md).
2. Strategic DDD Patterns We Adopt

Bounded Contexts: One per module (e.g., Projects Context, Donors Context).
Context Mapping: Document relationships in context-map.md (e.g., Upstream/Downstream, Conformist, Anti-Corruption Layer for integrations).
Ubiquitous Language: Use business terms consistently within a context (e.g., "Project" means different things in Projects vs. Finance contexts – avoid shared entities).
Core Domain: Prioritize impact-tracking modules (Projects, Outcomes, Metrics) as the competitive differentiator.

No shared domain models across modules – aligns with our SharedKernel restriction (primitives only).
3. Tactical DDD Patterns We Adopt
Within each module (Vertical Slice or Layered structure): 
Pattern,Description,Implementation Guidelines
Entities,"Objects with identity and lifecycle (e.g., Project with ProjectId).",Immutable where possible; use private setters.
Value Objects,"Immutable objects without identity (e.g., Money, Address).",Encapsulate validation; override equality.
Aggregates,"Cluster of entities/value objects with a root (e.g., Project Aggregate Root).",Enforce invariants here; transactional consistency boundary.
Domain Services,"Operations not fitting in entities (e.g., calculating impact score).",Stateless; inject repositories if needed.
Application Services,Orchestrate use cases (CQRS commands/queries).,Thin; use MediatR handlers.
Repositories,"Abstract persistence (interface in Domain/Application, impl in Infrastructure).",Per aggregate; DB-first mapping with EF Core (mapping-only).

We use CQRS lightly: Separate commands (state-changing) and queries via MediatR.

4. Event-Driven Architecture: In-Process Domain Events
To achieve loose coupling between modules without direct calls (which would tighten dependencies):

Preferred Communication: Publish Domain Events within a module when significant state changes occur.
Cross-Module Integration: Use Integration Events (published to an in-process bus) for other modules to react asynchronously.
Why In-Process?: We're a monolith – no need for distributed brokers initially. Keeps performance high, transactions simple, and deployment easy.
Tool: MediatR for in-process Publish/Subscribe (INotification & INotificationHandler).
Simple, lightweight, no external dependencies.
Handles both domain events (internal to module) and integration events (subscribed by other modules).


Event Types:

Domain Events: Internal to a Bounded Context (e.g., ProjectCreatedDomainEvent – handled within Projects module for side effects like auditing).
Integration Events: Public contract for cross-context (e.g., ProjectApprovedIntegrationEvent – subscribed by Donors or Reporting modules).

Publishing Flow:

In aggregate root or domain service: Raise event (e.g., AddDomainEvent(new ProjectApprovedIntegrationEvent(...))).
In Unit of Work (after SaveChanges): Publish all events via MediatR.
Handlers react (update read models, trigger workflows, enforce eventual consistency).

Eventual Consistency: Cross-module changes are eventual (no distributed transactions needed in monolith).
Idempotency & Ordering: Handlers must be idempotent; use event versioning.
Future-Proofing: If a module scales out, switch to out-of-process bus (MassTransit + RabbitMQ/Azure Service Bus) with minimal changes (wrap MediatR or add Outbox pattern).
5. Enforcing the Rules

No Direct Cross-Module Calls: Only via events or public APIs (if synchronous needed – rare, requires ADR approval).
CI Checks: Extend existing architecture-check.yml to detect forbidden references.
PR Review: Reject violations; require event-based integration.
Exceptions: Document in ADR folder.
6. Example: Projects Module Integration with Donors
C#
// In Projects Module (Domain)
public class Project : AggregateRoot
{
    public void Approve()
    {
        // ... business logic
        AddDomainEvent(new ProjectApprovedIntegrationEvent(ProjectId, TenantId, ApprovedDate));
    }
}

// Integration Event (in Projects.Contracts or SharedKernel if primitive)
public record ProjectApprovedIntegrationEvent(Guid ProjectId, Guid TenantId, DateTime ApprovedDate) : INotification;

// In Donors Module (Application Handler)
public class ProjectApprovedHandler : INotificationHandler<ProjectApprovedIntegrationEvent>
{
    public async Task Handle(ProjectApprovedIntegrationEvent @event, CancellationToken ct)
    {
        // React: e.g., notify potential donors, update statistics
        // Eventual consistency – no transaction spanning modules
    }
}

7. References & Inspirations

Eric Evans: Domain-Driven Design ( foundational).
Kamil Grzybek: Modular Monolith with DDD (GitHub example).
Vaughn Vernon: Implementing Domain-Driven Design.
Milan Jovanović & Mehmet Ozkaya: .NET Modular Monolith guides.
MediatR documentation for in-process events.

This policy is binding – all new modules (starting with Projects in Phase 3) must follow it. Review annually or on major changes.
Upload this as Markdown to the repo and link from ROADMAP.md and architecture-policy.md for visibility. Let me know if you'd like adjustments or code templates!
