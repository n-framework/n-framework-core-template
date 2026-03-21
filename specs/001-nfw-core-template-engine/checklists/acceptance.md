# Acceptance Checklist: NFramework Core Template Engine

## String Template Rendering

- [ ] `ITemplateEngine.RenderAsync` accepts template string and `ITemplateData`
- [ ] `ITemplateEngine.RenderAsync` returns rendered string
- [ ] `ITemplateEngine.RenderAsync` throws `ArgumentException` for null template
- [ ] `ITemplateEngine.RenderAsync` throws `ArgumentException` for null data
- [ ] Variable substitution works correctly
- [ ] Conditional blocks render true branch when condition is true
- [ ] Conditional blocks render false/else branch when condition is false
- [ ] Loop/range blocks iterate over collections
- [ ] `InvalidOperationException` thrown for invalid template syntax
- [ ] `InvalidOperationException` message contains descriptive error

## File Template Rendering

- [ ] `ITemplateEngine.RenderFileAsync` reads template from specified path
- [ ] `ITemplateEngine.RenderFileAsync` writes output to specified directory
- [ ] Output directory created automatically if it does not exist
- [ ] Path variable replacement works in file content
- [ ] Path variable replacement works in output file path
- [ ] `ArgumentException` thrown for non-existent template file
- [ ] Template file suffixes stripped according to mapping
- [ ] Default `.cs` extension when no suffix mapping matches
- [ ] Empty rendered content produces `.cs` file

## Batch File Rendering

- [ ] `ITemplateEngine.RenderFilesAsync` processes multiple files
- [ ] All files written to output directory
- [ ] `ArgumentException` thrown if any template file does not exist
- [ ] Cancellation token stops processing at next file boundary
- [ ] Returns list of all output file paths

## Built-in String Functions

- [ ] `camel_case` converts `UserEntity` to `userEntity`
- [ ] `pascal_case` converts `user_entity` to `UserEntity`
- [ ] `snake_case` converts `UserEntity` to `user_entity`
- [ ] `kebab_case` converts `UserEntity` to `user-entity`
- [ ] `abbreviation` converts `UserEntity` to `ue`
- [ ] `plural` converts `UserEntity` to `UserEntities`
- [ ] `singular` converts `UserEntities` to `UserEntity`
- [ ] `words` converts `UserEntity` to `User Entity`

## Property Naming Conventions

- [ ] LowerCamelCase properties accessible as-is
- [ ] PascalCase properties accessible as lowerCamelCase
- [ ] Boolean `Is*` properties accessible without `Is` prefix
- [ ] `text` alias created when `name` property exists

## Unknown Variable Handling

- [ ] Unknown variables preserved as `{{ variableName }}` in output
- [ ] Known variables substituted correctly
- [ ] Nested property access works for known properties

## Cancellation Support

- [ ] All `RenderAsync` overloads accept `CancellationToken`
- [ ] Cancellation stops rendering before completion
- [ ] No partial output written when cancelled

## File System Abstraction

- [ ] `ITemplateFileSystem` abstraction present
- [ ] `TemplateFileSystem` implements abstraction using `System.IO`
- [ ] File operations abstracted for testability

## Validation

- [ ] Null template string throws `ArgumentException`
- [ ] Empty template string accepted (returns empty string)
- [ ] Null data throws `ArgumentException`
- [ ] Null template file path throws `ArgumentException`
- [ ] Empty template file path throws `ArgumentException`
- [ ] Null template directory path throws `ArgumentException`
- [ ] Empty template directory path throws `ArgumentException`
- [ ] Null output directory path throws `ArgumentException`
- [ ] Empty output directory path throws `ArgumentException`
- [ ] Null request object throws `ArgumentException`

## Scriban Template Renderer

- [ ] Implements `ITemplateRenderer`
- [ ] `TemplateExtension` returns `.sbn`
- [ ] Template parsing with error reporting
- [ ] Custom function registration supported
- [ ] Handlebars-style `{{#if}}...{{/if}}` converted to Scriban syntax
- [ ] `{{else}}` and `{{/if}}` normalized to Scriban syntax
