# Unit Test Checklist: NFramework Core Template Engine

## NFrameworkTemplateEngine Tests

### RenderAsync Tests

- [ ] Throws `ArgumentException` when template is null
- [ ] Throws `ArgumentException` when data is null
- [ ] Returns rendered string for valid inputs
- [ ] Respects cancellation token
- [ ] Delegates to `ITemplateRenderer.RenderAsync`

### RenderFileAsync Tests

- [ ] Throws `ArgumentException` when request is null
- [ ] Throws `ArgumentException` when template file does not exist
- [ ] Creates output directory if it does not exist
- [ ] Reads template file and renders content
- [ ] Writes rendered content to correct output path
- [ ] Applies path variable replacements to output path
- [ ] Strips template suffixes from output filename
- [ ] Uses default `.cs` extension for unknown suffixes
- [ ] Returns correct output file path
- [ ] Respects cancellation token

### RenderFilesAsync Tests

- [ ] Throws `ArgumentException` when request is null
- [ ] Throws `ArgumentException` when any template file does not exist
- [ ] Throws `ArgumentException` when template file list is empty
- [ ] Renders all template files in order
- [ ] Returns all output file paths
- [ ] Respects cancellation token between files
- [ ] Already-written files remain after cancellation

### BuildOutputPath Tests

- [ ] Correctly strips `.sbn` suffix
- [ ] Correctly strips `.scriban` suffix
- [ ] Correctly strips `.sbn-cs` suffix and maps to `.cs`
- [ ] Correctly strips `.sbn-html` suffix and maps to `.html`
- [ ] Correctly strips `.sbn-txt` suffix and maps to `.txt`
- [ ] Uses default `.cs` for unknown suffixes
- [ ] Preserves directory structure in output path
- [ ] Applies path variable replacements

### ApplyPathReplacements Tests

- [ ] Replaces `{{key}}` patterns with values
- [ ] Replaces bare key patterns with values
- [ ] Handles multiple replacements
- [ ] Handles missing keys gracefully

## ScribanTemplateRenderer Tests

### Constructor Tests

- [ ] Initializes with empty custom functions dictionary

### RenderAsync Tests

- [ ] Throws `ArgumentException` when data is null
- [ ] Returns empty string for null/empty template
- [ ] Renders simple variable substitution
- [ ] Renders conditional blocks correctly
- [ ] Renders else branch when condition is false
- [ ] Renders loops over collections
- [ ] Calls `ITemplateData` methods appropriately
- [ ] Throws `InvalidOperationException` for parse errors
- [ ] Throws `InvalidOperationException` for missing functions
- [ ] Respects cancellation token

### BuildContext Tests

- [ ] Imports data properties with lowerCamelCase names
- [ ] Imports built-in string functions
- [ ] Imports custom functions when added
- [ ] Sets `StrictVariables` to true
- [ ] Enables relaxed member access

### String Function Tests

#### CamelCase

- [ ] Converts `UserEntity` to `userEntity`
- [ ] Converts `user_entity` to `userEntity`
- [ ] Handles empty string
- [ ] Handles null input

#### PascalCase

- [ ] Converts `userEntity` to `UserEntity`
- [ ] Converts `user_entity` to `UserEntity`
- [ ] Handles empty string
- [ ] Handles null input

#### SnakeCase

- [ ] Converts `UserEntity` to `user_entity`
- [ ] Converts `userEntity` to `user_entity`
- [ ] Handles empty string
- [ ] Handles null input

#### KebabCase

- [ ] Converts `UserEntity` to `user-entity`
- [ ] Converts `userEntity` to `user-entity`
- [ ] Handles empty string
- [ ] Handles null input

#### Abbreviation

- [ ] Converts `UserEntity` to `ue`
- [ ] Converts `User` to `u`
- [ ] Handles empty string
- [ ] Handles null input

#### Plural

- [ ] Converts `UserEntity` to `UserEntities`
- [ ] Converts `User` to `Users`
- [ ] Handles irregular plurals
- [ ] Handles empty string
- [ ] Handles null input

#### Singular

- [ ] Converts `UserEntities` to `UserEntity`
- [ ] Converts `Users` to `User`
- [ ] Handles irregular singulars
- [ ] Handles empty string
- [ ] Handles null input

#### Words

- [ ] Splits `UserEntity` to `User Entity`
- [ ] Splits `user_entity` to `user entity`
- [ ] Splits `user-entity` to `user entity`
- [ ] Handles empty string
- [ ] Handles null input

### Variable Preservation Tests

- [ ] Preserves unknown simple variables
- [ ] Substitutes known variables
- [ ] Handles mixed known and unknown variables

### Syntax Normalization Tests

- [ ] Normalizes `{{ variable }}` to `{{ variable }}`
- [ ] Normalizes `{variable}` to `{{ variable }}`
- [ ] Normalizes `{{#if condition}}` to `{{ if condition }}`
- [ ] Normalizes `{{else}}` to `{{ else }}`
- [ ] Normalizes `{{/if}}` to `{{ end }}`
- [ ] Normalizes member paths to lowerCamelCase

### Handlebars Compatibility Tests

- [ ] Evaluates `{{#if boolean}}...{{/if}}` blocks
- [ ] Evaluates `{{#if string}}...{{else}}...{{/if}}` blocks
- [ ] Handles null condition values
- [ ] Handles empty string condition values

## TemplateFileSystem Tests

- [ ] `FileExists` returns true for existing files
- [ ] `FileExists` returns false for non-existent files
- [ ] `ReadAllTextAsync` returns file contents
- [ ] `WriteAllTextAsync` writes contents to file
- [ ] `DirectoryExists` returns true for existing directories
- [ ] `DirectoryExists` returns false for non-existent directories
- [ ] `CreateDirectory` creates directory including parents
- [ ] `CombinePaths` correctly joins paths
- [ ] `GetDirectoryName` extracts directory from path
- [ ] `GetFileName` extracts filename from path
- [ ] `DirectorySeparatorChar` returns correct platform char
- [ ] `AltDirectorySeparatorChar` returns correct platform char
