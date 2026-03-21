# Data Model: NFramework Core Template Engine

## ITemplateEngine

**Purpose**: Main orchestration interface for template rendering operations.

**Methods**:

- `Task<string> RenderAsync(string template, ITemplateData data, CancellationToken cancellationToken = default)`
  - Renders a template string with the provided data.
  - Returns the rendered output as a string.
  - Throws `ArgumentException` if template or data is null.

- `Task<string> RenderFileAsync(TemplateFileRenderRequest request, CancellationToken cancellationToken = default)`
  - Renders a single template file to the output directory.
  - Returns the path of the rendered output file.
  - Throws `ArgumentException` if request is null or template file does not exist.

- `Task<IReadOnlyList<string>> RenderFilesAsync(TemplateFilesRenderRequest request, CancellationToken cancellationToken = default)`
  - Renders multiple template files to the output directory.
  - Returns a list of output file paths.
  - Throws `ArgumentException` if request is null or any template file does not exist.

## ITemplateRenderer

**Purpose**: Core interface for rendering template content.

**Properties**:

- `string TemplateExtension`: The file extension used by this renderer (e.g., `.sbn`).

**Methods**:

- `Task<string> RenderAsync(string template, ITemplateData data, CancellationToken cancellationToken = default)`
  - Parses and renders the template with the provided data.
  - Returns the rendered output.
  - Throws `InvalidOperationException` for template parsing errors.

## ITemplateData

**Purpose**: Marker interface for template data objects.

**Notes**: Any object can implement this interface or be used as template data. The Scriban renderer will access public properties of the data object.

## TemplateRenderRequest

**Purpose**: Request model for string template rendering.

**Fields**:

- `Template`: The template string to render (required, non-empty).
- `Data`: The template data object (required, non-null).

**Validation Rules**:

- `Template` must not be null or empty.
- `Data` must not be null.

## TemplateFileRenderRequest

**Purpose**: Request model for single file template rendering.

**Fields**:

- `TemplateFilePath`: Path to the template file (required, non-empty).
- `TemplateDirectoryPath`: Base directory for templates (required, non-empty).
- `OutputDirectoryPath`: Directory for rendered output (required, non-empty).
- `PathVariableReplacements`: Dictionary of path variable substitutions (optional, defaults to empty).
- `Data`: The template data object (required, non-null).

**Validation Rules**:

- All path fields must not be null or empty.
- `Data` must not be null.
- `PathVariableReplacements` may be null (defaults to empty dictionary).

## TemplateFilesRenderRequest

**Purpose**: Request model for batch file template rendering.

**Fields**:

- `TemplateFilePaths`: List of template file paths (required, non-empty).
- `TemplateDirectoryPath`: Base directory for templates (required, non-empty).
- `OutputDirectoryPath`: Directory for rendered output (required, non-empty).
- `PathVariableReplacements`: Dictionary of path variable substitutions (optional, defaults to empty).
- `Data`: The template data object (required, non-null).

**Validation Rules**:

- `TemplateFilePaths` must not be null or empty.
- All path fields must not be null or empty.
- `Data` must not be null.
- `PathVariableReplacements` may be null (defaults to empty dictionary).

## ITemplateFileSystem

**Purpose**: Abstraction for file system operations.

**Methods**:

- `bool FileExists(string path)`: Checks if a file exists.
- `Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)`: Reads file contents.
- `Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken)`: Writes content to file.
- `bool DirectoryExists(string path)`: Checks if a directory exists.
- `void CreateDirectory(string path)`: Creates a directory (including parent directories).
- `string CombinePaths(string path1, string path2)`: Combines two paths.
- `string CombinePaths(string path1, string path2, string path3)`: Combines three paths.
- `string? GetDirectoryName(string path)`: Gets the directory portion of a path.
- `string GetFileName(string path)`: Gets the file name from a path.

**Properties**:

- `char DirectorySeparatorChar`: The directory separator character for the current platform.
- `char AltDirectorySeparatorChar`: The alternate directory separator character (e.g., `/` on Windows).

## ScribanTemplateRenderer

**Purpose**: Scriban-based implementation of `ITemplateRenderer`.

**Template Suffix Mappings**:

| Input Suffix                             | Output Extension      |
| ---------------------------------------- | --------------------- |
| `.scriban-html`, `.sbn-html`, `.sbnhtml` | `.html`               |
| `.scriban-htm`, `.sbn-htm`, `.sbnhtm`    | `.htm`                |
| `.scriban-txt`, `.sbn-txt`, `.sbntxt`    | `.txt`                |
| `.scriban-cs`, `.sbn-cs`, `.sbncs`       | `.cs`                 |
| `.scriban`, `.sbn`                       | `.cs`                 |
| `.sb.html`                               | (removes suffix only) |

**Built-in Functions**:

| Function       | Description           | Example Input  | Example Output |
| -------------- | --------------------- | -------------- | -------------- |
| `camel_case`   | Convert to camelCase  | `UserEntity`   | `userEntity`   |
| `pascal_case`  | Convert to PascalCase | `user_entity`  | `UserEntity`   |
| `snake_case`   | Convert to snake_case | `UserEntity`   | `user_entity`  |
| `kebab_case`   | Convert to kebab-case | `UserEntity`   | `user-entity`  |
| `abbreviation` | Create abbreviation   | `UserEntity`   | `ue`           |
| `plural`       | Convert to plural     | `UserEntity`   | `UserEntities` |
| `singular`     | Convert to singular   | `UserEntities` | `UserEntity`   |
| `words`        | Split into words      | `UserEntity`   | `User Entity`  |

## NFrameworkTemplateEngine

**Purpose**: Main implementation of `ITemplateEngine`.

**Path Variable Replacement**:

- Variables in the format `{{variableName}}` in template content are replaced with values from `PathVariableReplacements`.
- Variables in file paths are also replaced before writing.

**Output File Naming**:

- Template file names have their suffixes stripped according to the renderer mappings.
- Default output extension is `.cs` if no mapping matches.
- Empty rendered content results in `.cs` extension.

## TemplateFileSystem

**Purpose**: Default implementation of `ITemplateFileSystem` using `System.IO`.

**Notes**: This is the production implementation. Tests can use a mock or fake implementation.
