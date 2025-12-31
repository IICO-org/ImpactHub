//make the resolver API explicitly “Entra OID”, not generic “SubjectId”

//This is the key: rename the concept.

//2.1 Create a value object in SharedKernel: ExternalIdentityKey
namespace ImpactHub.SharedKernel.Security;

public sealed record ExternalIdentityKey(
    string Provider,
    string Issuer,
    string SubjectId);
