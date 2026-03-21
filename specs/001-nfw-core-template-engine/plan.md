# Implementation Plan: NFramework Core Template Engine

**Package**: `core-template`
**Spec Type**: Package-Based
**Input**: Feature specification from `spec.md`

## Summary

Document the existing `core-template` package which provides template rendering capabilities using Scriban. The package includes abstractions, a core engine implementation, file system abstraction, and a Scriban-based renderer with custom string functions.

## Technical Context

**Language/Version**: C# on .NET 11  
**Packages**: Scriban, PluralizeService.Core, Microsoft.Extensions.DependencyInjection.\*  
**Storage**: File system for template files; no database  
**Testing**: xUnit under `/tests/unit`  
**Target Platform**: Linux, macOS, and Windows (cross-platform)  
**Project Type**: Library/Framework package

## Project Structure

```text
/home/ac/Code/n-framework/n-framework/src/nfw/packages/core-template/
├── specs/                                    # This documentation
│   ├── spec.md                              # Feature specification
│   ├── research.md                          # Design decisions
│   ├── data-model.md                        # Data model documentation
│   ├── plan.md                              # Implementation plan
│   ├── quickstart.md                        # Quickstart guide
│   ├── contracts/                           # (N/A - library, not CLI)
│   └── checklists/                          # Acceptance checklists
├── src/
│   ├── NFramework.Core.Template.Abstractions/
│   │   ├── ITemplateEngine.cs
│   │   ├── ITemplateRenderer.cs
│   │   ├── ITemplateData.cs
│   │   └── Models/
│   │       ├── TemplateRenderRequest.cs
│   │       ├── TemplateFileRenderRequest.cs
│   │       └── TemplateFilesRenderRequest.cs
│   ├── NFramework.Core.Template.Engine/
│   │   ├── NFrameworkTemplateEngine.cs
│   │   ├── TemplateFileSystem.cs
│   │   ├── Abstractions/
│   │   │   └── ITemplateFileSystem.cs
│   │   └── NFramework.Core.Template.Engine.csproj
│   └── NFramework.Core.Template.Scriban/
│       ├── ScribanTemplateRenderer.cs
│       ├── ScribanStringFunctionsExtensions.cs
│       └── NFramework.Core.Template.Scriban.csproj
├── tests/
│   └── unit/
│       ├── NFramework.Core.Template.Engine.Tests/
│       └── NFramework.Core.Template.Scriban.Tests/
├── NFramework.Core.Template.slnx
└── README.md
```

## Constitution Check

### Single-Step Build And Test

- **Build**: `dotnet build NFramework.Core.Template.slnx` from package root
- **Test**: `dotnet test` from package root

PASS. Both commands work from the package root directory.

### No Suppression

- The package uses standard .NET conventions for warnings-as-errors.

PASS. No suppression mechanisms are used.

### Deterministic Tests

- Tests use fixtures and mocks for file system operations.

PASS. No real file I/O or network access in unit tests.

## Package Dependencies

### NFramework.Core.Template.Abstractions

No dependencies. Contains only interfaces and models.

### NFramework.Core.Template.Engine

- `NFramework.Core.Template.Abstractions`
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- `Microsoft.Extensions.DependencyInjection`

### NFramework.Core.Template.Scriban

- `NFramework.Core.Template.Abstractions`
- `Scriban` (^5.0.0)
- `PluralizeService.Core` (^1.3.0)
- `Microsoft.Extensions.DependencyInjection.Abstractions`

## Completed Implementation

### Phase 1: Abstractions

- [x] `ITemplateEngine` - Main orchestration interface
- [x] `ITemplateRenderer` - Core rendering interface
- [x] `ITemplateData` - Marker interface for data objects
- [x] `TemplateRenderRequest` - String rendering request model
- [x] `TemplateFileRenderRequest` - Single file rendering request model
- [x] `TemplateFilesRenderRequest` - Batch file rendering request model

### Phase 2: Core Engine

- [x] `NFrameworkTemplateEngine` - Main implementation
- [x] `ITemplateFileSystem` - File system abstraction
- [x] `TemplateFileSystem` - Default file system implementation
- [x] Template suffix to output extension mapping
- [x] Path variable replacement
- [x] Automatic output directory creation

### Phase 3: Scriban Integration

- [x] `ScribanTemplateRenderer` - Scriban-based implementation
- [x] Template parsing with error reporting
- [x] `ScribanStringFunctionsExtensions` - Built-in string functions
- [x] Support for lowerCamelCase and PascalCase property names
- [x] Unknown variable preservation as `{{ variableName }}`
- [x] Handlebars-style `{{#if}}...{{/if}}` block support
- [x] Boolean property aliasing (Is* -> * without prefix)

### Phase 4: Testing

- [x] Unit tests for `NFrameworkTemplateEngine`
- [x] Unit tests for `ScribanTemplateRenderer`
- [x] Tests for string function transformations
- [x] Tests for error conditions

## Remaining Work

None. The package is complete.

## Complexity Tracking

No constitution violations.

## Next Steps

This spec documents the completed package. No further implementation work is required.
