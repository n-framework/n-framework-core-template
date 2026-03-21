# Product Requirements Document (PRD): NFramework Core Template Engine

## 1. Introduction / Overview

The NFramework Core Template Engine is a modular template rendering system designed for code generation, file templating, and dynamic content generation in the NFramework ecosystem. It provides a clean architecture with strict separation between abstractions and implementations, featuring built-in Scriban support with custom string manipulation functions.

## 2. Problem Statement

Developers building code generation and template-based systems face significant challenges:

- Template engines often have tight coupling between interfaces and implementations
- String manipulation requires custom logic scattered throughout the codebase
- Batch processing of templates can be inefficient and error-prone
- File system operations mixed with template logic creates maintenance burdens
- Extension and customization points are often limited or poorly designed

The Core Template Engine solves these problems by providing a clean, extensible architecture with built-in Scriban integration and custom string functions.

## 3. Goals

- Provide a clean separation of concerns between template rendering and file operations
- Deliver a high-performance template engine with Scriban as the default renderer
- Include built-in string manipulation functions for common code generation scenarios
- Support both string-based and file-based template rendering
- Enable batch processing of multiple templates efficiently
- Provide easy dependency injection integration
- Support path variable replacement for flexible output paths
- Include comprehensive error handling and validation

## 4. Target Users

- **Framework Developers**: Building code generation tools for the NFramework ecosystem
- **Template Authors**: Creating template-based project generators and scaffolding tools
- **DevOps Engineers**: Automating infrastructure-as-code generation
- **Application Developers**: Generating boilerplate code and configuration files
- **Platform Teams**: Creating internal tooling for standardized project creation

## 5. Product Principles

1. **Clean Architecture**: Strict separation between abstractions and implementations
2. **Performance**: Efficient template rendering with minimal allocations
3. **Extensibility**: Easy to add new template renderers and custom functions
4. **Type Safety**: Strongly typed request/response objects
5. **Dependency Injection**: Ready-to-use DI integration
6. **Error Handling**: Comprehensive validation and meaningful error messages

## 6. Scope

### In Scope

#### Core Abstractions

- `ITemplateEngine` - Main orchestration interface
- `ITemplateRenderer` - Core template rendering interface
- `ITemplateData` - Template data provider interface
- Request/response types with strong typing

#### Core Engine Implementation

- `NFrameworkTemplateEngine` - File system operations
- Support for string, file, and batch template rendering
- Path variable replacement functionality
- Output directory creation and management

#### Scriban Integration

- `ScribanTemplateRenderer` - Scriban-based renderer
- Custom string manipulation functions:
  - `camel_case` - Convert to camelCase
  - `pascal_case` - Convert to PascalCase
  - `snake_case` - Convert to snake_case
  - `kebab_case` - Convert to kebab-case
  - `abbreviation` - Create abbreviation from words
  - `plural` - Convert to plural form
  - `singular` - Convert to singular form
  - `words` - Split into separate words

#### Dependency Injection

- Service collection extensions for easy registration
- Scoped and singleton service lifetimes
- Proper disposal patterns

#### Validation

- Null reference checks
- File path validation
- Directory path validation
- Template content validation
- Input parameter validation

### Out of Scope

- GUI for template management
- Remote template catalogs (future)
- Template caching mechanisms (future)
- Template compilation optimization (future)
- Multi-language template support beyond Scriban
- Automatic backup of generated files

## 7. Functional Requirements

### FR-1: Template Engine Abstractions

- The system must define `ITemplateEngine` with methods for string, file, and batch rendering
- The system must define `ITemplateRenderer` for actual template processing
- The system must define `ITemplateData` as a marker interface for template data
- Request/response types MUST be strongly typed with clear validation

### FR-2: Core Engine Implementation

- `NFrameworkTemplateEngine` must implement `ITemplateEngine` using dependency injection
- `RenderAsync` must process string templates and return rendered content
- `RenderFileAsync` must read template files, render them, and write to output
- `RenderFilesAsync` must process multiple template files efficiently
- Path variable replacement must work on both template content and file paths
- Output directories must be created automatically if they don't exist

### FR-3: Scriban Renderer

- `ScribanTemplateRenderer` must implement `ITemplateRenderer` using Scriban
- Template parsing must include error reporting for syntax issues
- Template data must be accessible via `ScriptObject` and properly scoped
- Custom string functions must be available globally in template context
- Template rendering must support cancellation tokens

### FR-4: String Functions

- `camel_case` must convert "UserEntity" to "userEntity"
- `pascal_case` must convert "user_entity" to "UserEntity"
- `snake_case` must convert "UserEntity" to "user_entity"
- `kebab_case` must convert "UserEntity" to "user-entity"
- `abbreviation` must convert "UserEntity" to "ue"
- `plural` must use PluralizationProvider for correct pluralization
- `singular` must use PluralizationProvider for correct singularization
- `words` must split PascalCase, camelCase, snake_case, and kebab-case into words

### FR-5: Dependency Injection

- Service collection extensions must provide single-scope registration
- All services must be properly registered with appropriate lifetimes
- Registration must include both abstractions and concrete implementations
- DI setup must be chainable for easy composition

### FR-6: Validation and Error Handling

- All methods must validate null inputs with clear error messages
- File paths must be validated for existence and accessibility
- Template content must be validated before parsing
- Directory paths must be validated for writability
- Cancellation must be respected throughout the rendering process
- Errors must be meaningful and actionable for developers

## 8. Non-Functional Requirements

### Performance

- String rendering must complete in under 100ms for typical templates
- File rendering must complete in under 500ms for typical templates
- Batch rendering of 10 files must complete in under 2 seconds
- Memory allocations must be minimal during template processing
- Template parsing should be cached for repeated use

### Reliability

- The engine must never leave partial files written after failures
- All file operations must be atomic or transactional where possible
- The engine must handle concurrent access appropriately
- Error conditions must not corrupt existing files
- The engine must clean up temporary files if created

### Usability

- API must be intuitive and follow .NET conventions
- Error messages must be specific and actionable
- Template rendering must feel natural for Scriban users
- Integration with DI containers must be straightforward
- Examples must be provided and easy to follow

### Compatibility

- Must target .NET 11.0 or later
- Must be compatible with Native AOT scenarios
- Must work cross-platform (Windows, Linux, macOS)
- Must not introduce breaking changes for existing Scriban usage
- Must be compatible with common DI containers

## 9. Command Reference

```csharp
// String template rendering
string result = await templateEngine.RenderAsync(
    new Models.TemplateRenderRequest(templateString, templateData));

// Single file rendering
string outputPath = await templateEngine.RenderFileAsync(
    new Models.TemplateFileRenderRequest(
        TemplateFilePath: "template.sbn",
        TemplateDirectoryPath: "templates",
        OutputDirectoryPath: "output",
        PathVariableReplacements: replacements,
        Data: templateData));

// Batch file rendering
IReadOnlyList<string> outputPaths = await templateEngine.RenderFilesAsync(
    new Models.TemplateFilesRenderRequest(
        TemplateFilePaths: ["template1.sbn", "template2.sbn"],
        TemplateDirectoryPath: "templates",
        OutputDirectoryPath: "output",
        PathVariableReplacements: replacements,
        Data: templateData));
```

## 10. Exit Codes

Not applicable - this is a library, not a CLI tool.

## 11. Design Considerations

### Template Data Model

Template data classes should use records for immutability and clear contracts:

```csharp
public record TemplateData(
    string ProjectName,
    string Namespace,
    List<Entity> Entities);

public record Entity(string Name, List<string> Properties);
```

### Error Handling Strategy

- Use `ArgumentException` for invalid input parameters
- Use `InvalidOperationException` for business logic violations
- Use `FileNotFoundException` for missing template files
- Include detailed error messages with context
- Preserve stack traces for debugging

### Performance Considerations

- Minimize file I/O operations
- Reuse Scriban templates where possible
- Use compiled regular expressions for string operations
- Avoid unnecessary allocations in hot paths
- Support cancellation for long-running operations

## 12. Open Questions

1. Should template caching be built-in or left to the consumer?
2. Should there be a template compilation step for performance?
3. How should the engine handle template inheritance and includes?
4. Should there be support for template partials or layouts?
5. How should versioning work for custom string functions?

## 13. Success Metrics

- Template rendering completes in under 100ms for typical use cases
- No memory leaks during template processing
- All unit tests pass with 100% coverage
- Custom string functions work as expected for common code generation scenarios
- Integration with DI containers works seamlessly
- Error messages help developers fix issues without documentation

## 14. Dependencies

### Internal

- NFramework.Core.Template.Abstractions - Core interfaces
- Microsoft.Extensions.DependencyInjection.Abstractions - DI abstractions

### External

- Scriban - Template engine
- PluralizeService.Core - String pluralization
- Microsoft.Extensions.DependencyInjection - DI container

## 15. Related Documentation

- [README.md](README.md) - Getting started guide
- [SPECIFICATION_GUIDELINES.md](../../docs/SPECIFICATION_GUIDELINES.md) - Specification guidelines
- [NFramework PRD](../../docs/PRD.md) - Overall framework requirements
