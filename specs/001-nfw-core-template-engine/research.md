# Research: NFramework Core Template Engine

## Decision: Use Scriban as the template rendering engine

**Rationale**: Scriban is a fast, lightweight, and powerful template engine for .NET with excellent support for text templating. It provides a clean syntax that is familiar to developers who have used Handlebars or Liquid, while offering better performance than many alternatives.

**Alternatives considered**:

- Razor Engine: Rejected because Razor is designed for HTML rendering with tight ASP.NET integration, making it less suitable for general-purpose text templating.
- DotLiquid: Rejected because it has less flexibility for custom functions and transformations.
- Nunjucks: Rejected because it's a JavaScript library without native .NET support.

## Decision: Use `ITemplateData` as a marker interface

**Rationale**: Scriban expects data to be accessible as an object graph. Using a marker interface allows any object to serve as template data while providing a clear contract for what the template engine expects.

**Alternatives considered**:

- Base class with virtual members: Rejected because it limits the types that can be used as data.
- Generic `ITemplateData<T>`: Rejected because it adds unnecessary complexity for a simple marker interface.
- Dictionary-based data: Rejected because it loses type safety and IntelliSense support.

## Decision: Support both lowerCamelCase and PascalCase property naming

**Rationale**: Template authors may use either naming convention in their templates. Supporting both conventions without explicit configuration improves developer experience and reduces friction.

**Alternatives considered**:

- Require explicit configuration: Rejected because it adds boilerplate for common use cases.
- Force one convention: Rejected because different codebases have different conventions.

## Decision: Preserve unknown variables as `{{ variableName }}`

**Rationale**: This behavior allows templates to be more resilient and provides better debugging. Template authors can see exactly which variables are missing rather than getting empty output or errors.

**Alternatives considered**:

- Replace with empty string: Rejected because it makes debugging harder.
- Throw an error: Rejected because it prevents templates from being used with partial data.
- Replace with a placeholder: Rejected because it doesn't follow Scriban conventions.

## Decision: Abstract file system operations with `ITemplateFileSystem`

**Rationale**: This abstraction enables testing without real file I/O and allows for future extensibility (e.g., virtual file systems, embedded resources).

**Alternatives considered**:

- Direct `System.IO` usage: Rejected because it makes testing harder and couples the engine to the file system.
- Static utility methods: Rejected because it doesn't allow for dependency injection or mocking.

## Decision: Use records for request models

**Rationale**: Records provide immutability, value-based equality, and concise syntax. They clearly communicate that these are data transfer objects.

**Alternatives considered**:

- Classes with constructors: Rejected because records provide better semantics for DTOs.
- Mutable classes: Rejected because mutable request objects can lead to bugs.

## Decision: Provide built-in string transformation functions

**Rationale**: Code generation frequently requires transforming entity names into different naming conventions. Having these built-in eliminates the need for template authors to implement them.

**Alternatives considered**:

- Require custom functions: Rejected because common transformations should be available out of the box.
- Only support basic case changes: Rejected because pluralization and abbreviations are also common.

## Decision: Strip template suffixes and map to output extensions

**Rationale**: This convention allows template files to be easily identified while automatically determining the appropriate output extension.

**Suffix mappings**:

- `.scriban-html`, `.sbn-html`, `.sbnhtml` -> `.html`
- `.scriban-txt`, `.sbn-txt`, `.sbntxt` -> `.txt`
- `.scriban-cs`, `.sbn-cs`, `.sbncs` -> `.cs`
- `.scriban`, `.sbn` -> `.cs` (default)
- `.sb.html` -> no suffix change (extension handled by renderer)

**Alternatives considered**:

- Require explicit output extension: Rejected because the convention is intuitive and widely used.
- Use renderer default for all: Rejected because type-specific suffixes provide better control.

## Decision: Support cancellation tokens on all async operations

**Rationale**: Long-running batch operations should be cancellable. Cancellation tokens allow consumers to implement proper cancellation without aborting threads.

**Alternatives considered**:

- No cancellation support: Rejected because it limits usability in interactive scenarios.
- Separate cancellation method: Rejected because it's less idiomatic for async operations.
