# StudentGradesPortal

StudentGradesPortal is a WPF application for maintaining a student grade journal. The project continues practical works 2 and 3: modules are separated into projects, and interactions are organized through contracts and services.

## Modules

- `Presentation` contains the WPF interface, commands, view models, converters, and styles.
- `Models` contains grade journal domain objects.
- `Common` contains shared property-change infrastructure.
- `Contracts` contains service interfaces.
- `Logic` contains filtering, validation, and summary rules.
- `Storage` contains JSON loading and saving.
- `Checks` contains integration checks used after merging feature branches.

## Build and checks

```powershell
dotnet build StudentGradesPortal.sln
dotnet run --project Checks\Checks.csproj
```

The project targets .NET 9. The repository allows a newer installed SDK to build the solution when SDK 9.0.307 is not available.
