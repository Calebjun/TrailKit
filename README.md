# TrailKit

Outdoor gear rental system for PROG30000 Enterprise Software Systems, Fall 2026.

Team: Alison, Caleb, Layan, Utsah.

## Solution layout

| Project | Layer | Holds |
| --- | --- | --- |
| src/TrailKit.Web | Presentation and API | MVC controllers, Razor views, API controllers (Controllers/Api), view models |
| src/TrailKit.Application | Business logic | Services, rule classes (Rules), charge and pricing strategies (Policies) |
| src/TrailKit.Domain | Domain | Entities and enums |
| src/TrailKit.Infrastructure | Data access | TrailKitDbContext and DbSeeder (Data), repositories (Repositories) |
| tests/TrailKit.Tests | Unit tests | NUnit test classes, one per service or rule class |

References run one way: Web uses Application and Infrastructure, Application uses Domain and Infrastructure, Infrastructure uses Domain, Tests use Application and Domain.

## Setup

1. Install the .NET 10 SDK and Visual Studio 2026 (ASP.NET and web development workload) or VS Code with the C# Dev Kit.
2. Clone the repo and open TrailKit.sln.
3. Build once so NuGet restores the packages.
4. Apply migrations once they exist: `dotnet ef database update --project src/TrailKit.Infrastructure --startup-project src/TrailKit.Web`
5. Run TrailKit.Web.

The connection string `TrailKitDb` in src/TrailKit.Web/appsettings.json points at SQL Server LocalDB.

## Team rules

Ownership

- Every class has one author. Write the author in the file header.
- See docs/ProjectPlan.md for who owns what.

Comments

- Each file starts with a header: author and a one-line purpose.
- Every method and field gets an XML comment (`/// <summary>`).
- If you add a method to a class you don't own, put your name in that method's comment.

Naming

- PascalCase for classes, methods and properties.
- camelCase for local variables and parameters.
- `_camelCase` for private fields.

Git

- Keep `main` working. Never push broken builds to it.
- One branch per use case, for example `feature/uc3-pickup-return`.
- Merge through a pull request with one teammate reviewing.
- Pull `main` every day. Commit small and often.
- Announce in the group chat before adding an EF Core migration, and pull first.

## File header template

```csharp
// Author: <Your name>
// Purpose: <What this file is for>

namespace TrailKit.Domain.Entities;

/// <summary>
/// <What this class represents>
/// </summary>
public class Example
{
    /// <summary>Unique identifier.</summary>
    private int _id;
}
```
