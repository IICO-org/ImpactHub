param (
    [Parameter(Mandatory=$true)]
    [string]$ModuleName
)

$base = "src/Modules/$ModuleName"

New-Item -ItemType Directory -Force -Path `
"$base/Domain/Entities",
"$base/Domain/ValueObjects",
"$base/Domain/Events",
"$base/Domain/Interfaces",
"$base/Application/Commands",
"$base/Application/Queries",
"$base/Application/DTOs",
"$base/Application/Validators",
"$base/Infrastructure/Persistence/EntityConfigurations",
"$base/Infrastructure/Repositories",
"$base/Api/Controllers",
"$base/Api/Contracts"

New-Item -ItemType File -Force -Path `
"$base/$ModuleName.csproj",
"$base/Infrastructure/Persistence/${ModuleName}DbContext.cs"
