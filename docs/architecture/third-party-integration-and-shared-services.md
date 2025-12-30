# Integrating Third-Party Services and Shared Infrastructure in ImpactHub ERP

**Document Status:** Proposed Architecture Decision Record (ADR)  
**Date:** December 30, 2025  
**Author:** A. Alenah (Software Architect)  
**Proposed Location:** `/docs/architecture/third-party-integration-and-shared-services.md`

---

This ADR addresses two key concerns for maintainability and extensibility in our modular monolith architecture:

- Integrating third-party services (e.g., email providers like SendGrid) without scattering implementation details across the codebase.
- Structuring shared (generic) services that span multiple modules, including decisions on in-house development vs. outsourcing.

These align with our existing DDD principles (bounded contexts via modules), event-driven patterns (MediatR for in-process events), and governance rules (strict boundaries, SharedKernel limited to primitives).

---

## 1. Abstracting Third-Party Services (e.g., Email/Notifications)

### Problem Statement

Third-party tools like SendGrid (for email campaigns and notifications, such as "thank you" emails) will be used across modules. However, direct calls to provider-specific APIs (e.g., `SendGridClient.SendEmailAsync`) should not be embedded in module code, `Program.cs`, or scattered throughout application services. This leads to tight coupling, making it hard to switch providers (e.g., to MailGun) or test/maintain.

---

### Proposed Solution: Adapter Pattern with Dependency Inversion

We will use Clean Architecture principles (onion architecture) combined with the Adapter Pattern and Dependency Inversion Principle (DIP) to abstract third-party integrations. This creates a clear separation:

- **Interface Layer:** Define provider-agnostic interfaces in a shared infrastructure project (e.g., `ImpactHub.Infrastructure.Abstractions`).
- **Implementation Layer:** Concrete adapters (e.g., for SendGrid) in a dedicated infrastructure project (e.g., `ImpactHub.Infrastructure.Email`).
- **Dependency Injection (DI):** Register implementations in a central composition root (e.g., `Program.cs` or a dedicated Startup module), using .NET's built-in DI container.

This way:

- Developers only interact with interfaces (e.g., `IEmailService.SendThankYouEmailAsync(...)`).
- Switching providers requires only updating the adapter implementation and DI registration—no changes to module code.
- Configuration (API keys, endpoints) is handled via `appsettings.json` or Azure Key Vault, injected via `IOptions`.

---

## Key Components

### Abstraction (Interface)

```csharp
// In ImpactHub.Infrastructure.Abstractions (Shared across modules)
public interface IEmailService
{
    Task SendNotificationAsync(string toEmail, string subject, string body, bool isHtml = false);
    Task SendCampaignAsync(IEnumerable<string> recipients, string templateId, object dynamicData);
    // Add more methods as needed (e.g., for attachments, tracking)
}

Concrete Adapter (Implementation)

// In ImpactHub.Infrastructure.Email (Separate project, references SendGrid NuGet)
public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly EmailSettings _settings; // Injected via IOptions<EmailSettings>

    public SendGridEmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
        _client = new SendGridClient(_settings.ApiKey);
    }

    public async Task SendNotificationAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            PlainTextContent = isHtml ? null : body,
            HtmlContent = isHtml ? body : null
        };
        msg.AddTo(new EmailAddress(toEmail));
        await _client.SendEmailAsync(msg);
    }

    // Implement SendCampaignAsync similarly using SendGrid's dynamic templates
}

Configuration Class

// In ImpactHub.Infrastructure.Abstractions
public class EmailSettings
{
    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    // Add provider-specific settings if needed
}

DI Registration (Centralized in Composition Root)

// In Program.cs or a dedicated Startup.cs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddTransient<IEmailService, SendGridEmailService>(); // Switch to MailGunEmailService later

Usage in Modules

Developers inject and use the interface only—no direct provider references:

// In a module's Application Service (e.g., Donors module, MediatR Handler)
public class DonorRegisteredHandler : INotificationHandler<DonorRegisteredIntegrationEvent>
{
    private readonly IEmailService _emailService;

    public DonorRegisteredHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(DonorRegisteredIntegrationEvent @event, CancellationToken ct)
    {
        await _emailService.SendNotificationAsync(
            @event.Email,
            "Thank You for Registering!",
            "Welcome to ImpactHub!");
    }
}

Benefits & Enforcement

    Decoupling: Modules depend on abstractions, not concretions (DIP).

    Testability: Mock IEmailService in unit tests.

    Switching Providers: Create MailGunEmailService implementing IEmailService; update DI registration only.

    Enforcement: CI checks (extend architecture-check.yml) to ban direct NuGet references to providers (e.g., SendGrid) in module projects.

    Extension to Other Services: Apply similarly for other third-parties (e.g., Stripe for payments: IPaymentGateway interface).

