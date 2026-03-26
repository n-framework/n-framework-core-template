# NFramework.Core.Template

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/NFramework.Core.Template.Abstractions)](https://www.nuget.org/packages/NFramework.Core.Template.Abstractions/)
[![NuGet](https://img.shields.io/nuget/v/NFramework.Core.Template.Engine)](https://www.nuget.org/packages/NFramework.Core.Template.Engine/)
[![NuGet](https://img.shields.io/nuget/v/NFramework.Core.Template.Scriban)](https://www.nuget.org/packages/NFramework.Core.Template.Scriban/)
[![Buy A Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-ffdd00?logo=buy-me-a-coffee&logoColor=black&style=flat)](https://ahmetcetinkaya.me/donate)

A powerful and extensible template engine for .NET with built-in Scriban support, designed for code generation and file templating in the NFramework ecosystem.

**Core Techs:**
[![.NET 11](https://img.shields.io/badge/.NET%2011-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-68217A?style=flat&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Scriban](https://img.shields.io/badge/Scriban-0050A0?style=flat)](https://github.com/scriban/scriban)

## Features

- **Scriban Integration**: Fast and powerful Scriban template engine
- **Template Rendering**: Support for string templates and file templates
- **Batch Processing**: Efficient rendering of multiple files at once
- **Built-in Functions**: Custom string manipulation functions (CamelCase, PascalCase, SnakeCase, KebabCase, etc.)
- **Dependency Injection**: Ready-to-use extension methods
- **Clean Architecture**: Strict separation between abstractions and implementations

## Packages

- `NFramework.Core.Template.Abstractions` - Core interfaces and abstractions
- `NFramework.Core.Template.Engine` - Core template engine implementation
- `NFramework.Core.Template.Scriban` - Scriban-based template renderer with custom functions

## Installation

```bash
dotnet add package NFramework.Core.Template.Abstractions
dotnet add package NFramework.Core.Template.Engine
dotnet add package NFramework.Core.Template.Scriban
```

## Quick Start

### Basic Setup

```csharp
using Microsoft.Extensions.DependencyInjection;
using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Scriban;
using NFramework.Core.Template.Engine.DependencyInjection;

// Add to DI container
var services = new ServiceCollection();
services.AddScribanTemplateEngine();
```

### Template Data

Create simple template data classes:

```csharp
public record MyTemplateData(
    string ProjectName,
    string Namespace,
    bool IncludeApi
);
```

### String Template Rendering

```csharp
var data = new MyTemplateData(
    ProjectName: "MyAwesomeApp",
    Namespace: "MyApp.Domain",
    IncludeApi: true
);

string template = @"
namespace {{ Namespace }}
{
    public class Project
    {
        public string Name { get; set; } = ""{{ ProjectName }}"";
        public bool HasApi = {{ IncludeApi }};
    }
}
";

var renderer = serviceProvider.GetRequiredService<ITemplateRenderer>();
string result = await renderer.RenderAsync(template, data);

// Result: Generated C# class with proper values
```

### File Template Rendering

```csharp
// Create a template file at templates/Project.cs.sbn
// Render it to output directory

var request = new Models.TemplateFileRenderRequest(
    TemplateFilePath: "templates/Project.cs.sbn",
    TemplateDirectoryPath: "templates",
    OutputDirectoryPath: "output",
    PathVariableReplacements: new Dictionary<string, string>
    {
        { "{{region}}", "Demo" }
    },
    Data: data
);

var engine = serviceProvider.GetRequiredService<ITemplateEngine>();
string outputPath = await engine.RenderFileAsync(request);
```

### Batch File Rendering

```csharp
var batchRequest = new Models.TemplateFilesRenderRequest(
    TemplateFilePaths: new[]
    {
        "templates/Project.cs.sbn",
        "tests/ProjectTests.cs.sbn"
    },
    TemplateDirectoryPath: "templates",
    OutputDirectoryPath: "output",
    PathVariableReplacements: new Dictionary<string, string>(),
    Data: data
);

IReadOnlyList<string> outputPaths = await engine.RenderFilesAsync(batchRequest);
```

## Built-in String Functions

The template engine provides additional string manipulation functions:

```csharp
// Input: "UserEntity"
// Functions:
- {{ UserEntity | string.camel_case }}  // "userEntity"
- {{ UserEntity | string.pascal_case }}  // "UserEntity"
- {{ UserEntity | string.snake_case }}  // "user_entity"
- {{ UserEntity | string.kebab_case }}  // "user-entity"
- {{ UserEntity | string.abbreviation }} // "ue"
- {{ UserEntity | string.plural }}       // "UserEntities"
- {{ UserEntity | string.singular }}     // "UserEntity"
- {{ UserEntity | string.words }}        // "User Entity"
```

## Advanced Usage

### Custom Template Data

```csharp
public record ComplexTemplateData
{
    public string Name { get; set; }
    public List<Entity> Entities { get; set; }
    public Config Config { get; set; }

    public record Entity(string Name, List<string> Properties);
    public record Config(bool EnableAuth, string DatabaseProvider);
}
```

### Path Variable Replacement

```csharp
// Path: "src/{{ProjectName}}/Models/{{EntityName}}.cs"
// Data: ProjectName = "MyApp", EntityName = "User"
// Result: "src/MyApp/Models/User.cs"

var pathRequest = new Models.TemplateFileRenderRequest(
    TemplateFilePath: "templates/{{ProjectName}}/Models/{{EntityName}}.cs.sbn",
    TemplateDirectoryPath: "templates",
    OutputDirectoryPath: "src/{{ProjectName}}",
    PathVariableReplacements: new Dictionary<string, string>(),
    Data: new { ProjectName = "MyApp", EntityName = "User" }
);
```

### Error Handling

```csharp
try
{
    string result = await renderer.RenderAsync(template, data);
}
catch (InvalidOperationException ex) when (ex.Message.Contains("Template parse failed"))
{
    // Handle template parsing errors
    Console.WriteLine($"Template error: {ex.Message}");
}
catch (Exception ex)
{
    // Handle other errors
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Architecture

### Abstraction Layer

- `ITemplateEngine` - Main orchestration interface
- `ITemplateRenderer` - Core template rendering interface
- `ITemplateData` - Template data provider interface
- Request/Response types - Strongly typed request objects

### Implementation Layer

- `NFrameworkTemplateEngine` - File system operations and batching
- `ScribanTemplateRenderer` - Scriban-based template parsing and rendering
- `ServiceCollectionExtensions` - Dependency injection setup

### String Functions

- `ScribanBuiltinFunctionsExtensions` - Base Scriban functions
- `ScribanStringFunctionsExtensions` - Custom string manipulation functions

## Testing

Run the test suite:

```bash
dotnet test NFramework.Core.Template.Engine.Tests.csproj
dotnet test NFramework.Core.Template.Scriban.Tests.csproj
```

---

## Contributing

For development setup and contribution guidelines, see **[CONTRIBUTING.md](../../docs/CONTRIBUTING.md)**.

---

## 📄 License

This project is licensed under the **Apache License 2.0** - see the [LICENSE](LICENSE) file for details.

## Related Packages

- [NFramework](https://github.com/n-framework/n-framework) — The parent framework
- [NFramework.Core.CLI](../n-framework-core-cli) — CLI framework library
