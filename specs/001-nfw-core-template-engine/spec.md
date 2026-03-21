# Feature Specification: NFramework Core Template Engine

**Feature Branch**: `001-nfw-core-template-engine`
**Spec Type**: Package-Based
**Package**: `core-template`
**Created**: 2026-03-21
**Status**: Draft
**Input**: Document existing core-template package with Scriban integration, template rendering, and string manipulation functions.

> **Note**: This spec documents the existing core-template package. Implementation is complete.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Render String Templates (Priority: P1)

A developer wants to render a Scriban template string with dynamic data and receive the rendered output.

**Why this priority**: String template rendering is the fundamental operation for the template engine.

**Independent Test**: Can be fully tested by passing a template string and data object, then verifying the rendered output contains expected substitutions.

**Acceptance Scenarios**:

1. **Given** a valid Scriban template string with variable placeholders, **When** `RenderAsync` is called with template data, **Then** the output contains the substituted values.
2. **Given** a template with conditional blocks, **When** the data causes the condition to be true, **Then** the true branch is rendered.
3. **Given** a template with a loop over a collection, **When** rendering with non-empty data, **Then** the loop content appears for each item.
4. **Given** a template with invalid Scriban syntax, **When** `RenderAsync` is called, **Then** an `InvalidOperationException` is thrown with a descriptive error message.

---

### User Story 2 - Render File Templates (Priority: P1)

A developer wants to render template files from disk and write the output to a specified directory.

**Why this priority**: File-based template rendering enables batch processing of multiple templates for code generation.

**Independent Test**: Can be fully tested by creating template files, calling `RenderFileAsync`, and verifying output files exist with correct content.

**Acceptance Scenarios**:

1. **Given** a valid template file exists at the specified path, **When** `RenderFileAsync` is called, **Then** the file is read, rendered, and written to the output directory.
2. **Given** the output directory does not exist, **When** `RenderFileAsync` is called, **Then** the directory is created automatically.
3. **Given** a template file path contains path variable placeholders like `{{ProjectName}}`, **When** rendering with matching data, **Then** the output path reflects the substituted values.
4. **Given** a template file does not exist, **When** `RenderFileAsync` is called, **Then** an `ArgumentException` is thrown.

---

### User Story 3 - Batch Render Multiple Templates (Priority: P2)

A developer wants to render multiple template files in a single operation for project scaffolding.

**Why this priority**: Batch rendering is essential for generating complete project structures with multiple files.

**Independent Test**: Can be fully tested by passing multiple template file paths and verifying all output files are created.

**Acceptance Scenarios**:

1. **Given** multiple valid template file paths, **When** `RenderFilesAsync` is called, **Then** all files are rendered and written to the output directory.
2. **Given** one of the template files does not exist, **When** `RenderFilesAsync` is called, **Then** an `ArgumentException` is thrown and no files are written.
3. **Given** multiple templates are being rendered, **When** cancellation is requested, **Then** rendering stops and already-written files remain.

---

### User Story 4 - Use Built-in String Functions (Priority: P2)

A template author wants to transform entity names into different case formats within templates.

**Why this priority**: String transformations are common in code generation for naming conventions.

**Independent Test**: Can be fully tested by rendering templates with string function calls and verifying the transformed output.

**Acceptance Scenarios**:

1. **Given** a template with `{{ UserEntity | string.camel_case }}`, **When** rendered, **Then** the output contains `userEntity`.
2. **Given** a template with `{{ user_entity | string.pascal_case }}`, **When** rendered, **Then** the output contains `UserEntity`.
3. **Given** a template with `{{ UserEntity | string.snake_case }}`, **When** rendered, **Then** the output contains `user_entity`.
4. **Given** a template with `{{ UserEntity | string.kebab_case }}`, **When** rendered, **Then** the output contains `user-entity`.
5. **Given** a template with `{{ UserEntity | string.plural }}`, **When** rendered, **Then** the output contains `UserEntities`.
6. **Given** a template with `{{ UserEntities | string.singular }}`, **When** rendered, **Then** the output contains `UserEntity`.

---

### User Story 5 - Handle Missing Variables Gracefully (Priority: P3)

A template author wants unknown variables to remain as placeholders rather than causing errors.

**Why this priority**: This allows templates to be more resilient and provides better debugging.

**Independent Test**: Can be fully tested by rendering templates with unknown variables and verifying they appear as `{{variableName}}`.

**Acceptance Scenarios**:

1. **Given** a template contains `{{ unknownVariable }}` and the data does not define this variable, **When** rendered, **Then** the output contains `{{ unknownVariable }}`.
2. **Given** a template contains `{{ knownVariable }}` and the data defines it, **When** rendered, **Then** the output contains the actual value.

---

### Edge Cases

- Empty template string returns empty string
- Template with only whitespace returns rendered whitespace
- Null template returns empty string
- Template data with null property values renders as empty string
- Template file with `.sbn` extension outputs to `.cs` by default
- Template file with `.sbn-html` extension outputs to `.html`
- Circular property references do not cause infinite loops
- Cancellation is checked at each major rendering step

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST provide `ITemplateEngine` with `RenderAsync`, `RenderFileAsync`, and `RenderFilesAsync` methods.
- **FR-002**: The system MUST provide `ITemplateRenderer` as the core rendering interface with `RenderAsync` and `TemplateExtension` properties.
- **FR-003**: The system MUST provide `ITemplateData` as a marker interface for template data objects.
- **FR-004**: The system MUST provide strongly-typed request models: `TemplateRenderRequest`, `TemplateFileRenderRequest`, and `TemplateFilesRenderRequest`.
- **FR-005**: `RenderAsync` MUST accept a template string and `ITemplateData`, returning the rendered string.
- **FR-006**: `RenderFileAsync` MUST read a template file, render it with data, and write to the output directory.
- **FR-007**: `RenderFilesAsync` MUST process multiple template files in sequence, writing each to the output directory.
- **FR-008**: Output directories MUST be created automatically if they do not exist.
- **FR-009**: Template file paths MUST support path variable replacement for dynamic output paths.
- **FR-010**: The system MUST strip template file suffixes (`.sbn`, `.scriban`, etc.) and append appropriate output extensions.
- **FR-011**: `ScribanTemplateRenderer` MUST implement `ITemplateRenderer` using the Scriban library.
- **FR-012**: Template parsing errors MUST throw `InvalidOperationException` with descriptive messages.
- **FR-013**: The system MUST provide built-in string functions: `camel_case`, `pascal_case`, `snake_case`, `kebab_case`.
- **FR-014**: The system MUST provide built-in string functions: `abbreviation`, `plural`, `singular`, `words`.
- **FR-015**: Unknown template variables MUST be preserved as `{{ variableName }}` in the output.
- **FR-016**: All public methods MUST support `CancellationToken` for cancellation support.
- **FR-017**: The system MUST provide `ITemplateFileSystem` abstraction for file operations.
- **FR-018**: All public methods MUST validate null inputs and throw `ArgumentException` with descriptive messages.
- **FR-019**: Template property access MUST support both lowerCamelCase and PascalCase naming conventions.
- **FR-020**: Boolean properties starting with `Is` MUST be accessible without the prefix.

### Key Entities _(include if feature involves data)_

- **ITemplateEngine**: Main orchestration interface for template rendering operations.
- **ITemplateRenderer**: Core interface for rendering template content.
- **ITemplateData**: Marker interface for template data objects.
- **TemplateRenderRequest**: Request model for string template rendering.
- **TemplateFileRenderRequest**: Request model for single file template rendering.
- **TemplateFilesRenderRequest**: Request model for batch file template rendering.
- **ITemplateFileSystem**: Abstraction for file system operations.
- **ScribanTemplateRenderer**: Scriban-based implementation of `ITemplateRenderer`.
- **NFrameworkTemplateEngine**: Main implementation of `ITemplateEngine`.

## Assumptions

- This is a library package, not a CLI tool; no exit codes or user interaction required.
- Template data objects use records for immutability and clear contracts.
- Scriban is the primary and only template renderer for this package.
- File system operations are abstracted for testability.
- The package targets .NET 11.0 or later.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In unit tests, 100% of string template rendering calls with valid inputs produce correct output.
- **SC-002**: In unit tests, 100% of file template rendering calls create output files in the expected locations.
- **SC-003**: In unit tests, all built-in string functions produce expected transformations for common inputs.
- **SC-004**: In unit tests, invalid template syntax throws `InvalidOperationException` with non-empty error messages.
- **SC-005**: In unit tests, missing template files throw `ArgumentException` with the parameter name.
- **SC-006**: In unit tests, cancellation requests stop rendering before completion.
