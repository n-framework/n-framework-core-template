# Quickstart: NFramework Core Template Engine

## Prerequisites

- .NET 11 SDK or later
- Clone the repository and navigate to the package directory

## Build

```bash
cd /home/ac/Code/n-framework/n-framework/src/nfw/packages/core-template
dotnet build NFramework.Core.Template.slnx
```

## Test

```bash
cd /home/ac/Code/n-framework/n-framework/src/nfw/packages/core-template
dotnet test
```

## Manual Verification

### 1. Render a String Template

```csharp
using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Scriban;

public record MyData(string ProjectName, string Namespace);

var renderer = new ScribanTemplateRenderer();
var data = new MyData("MyApp", "MyApp.Domain");

string template = "namespace {{ namespace }} { public class Project { public string Name = \"{{ projectName }}\"; } }";
string result = await renderer.RenderAsync(template, data);

// result: namespace MyApp.Domain { public class Project { public string Name = "MyApp"; } }
```

### 2. Render a File Template

```csharp
using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Abstractions.Models;
using NFramework.Core.Template.Engine;
using NFramework.Core.Template.Scriban;

public record FileData(string ProjectName);

var engine = new NFrameworkTemplateEngine(new ScribanTemplateRenderer());
var data = new FileData("MyApp");

var request = new TemplateFileRenderRequest(
    TemplateFilePath: "templates/Project.cs.sbn",
    TemplateDirectoryPath: "templates",
    OutputDirectoryPath: "output",
    PathVariableReplacements: new Dictionary<string, string>(),
    Data: data
);

string outputPath = await engine.RenderFileAsync(request);
// Output: output/Project.cs
```

### 3. Use Built-in String Functions

```csharp
using NFramework.Core.Template.Scriban;

var renderer = new ScribanTemplateRenderer();
var data = new { Name = "UserEntity" };

string template = @"
{{ Name | string.camel_case }}   = userEntity
{{ Name | string.pascal_case }}  = UserEntity
{{ Name | string.snake_case }}   = user_entity
{{ Name | string.kebab_case }}   = user-entity
{{ Name | string.plural }}       = UserEntities
{{ Name | string.singular }}     = UserEntity
{{ Name | string.words }}        = User Entity";

string result = await renderer.RenderAsync(template, data);
```

### 4. Batch Render Multiple Files

```csharp
using NFramework.Core.Template.Abstractions.Models;
using NFramework.Core.Template.Engine;
using NFramework.Core.Template.Scriban;

public record BatchData(string ProjectName);

var engine = new NFrameworkTemplateEngine(new ScribanTemplateRenderer());
var data = new BatchData("MyApp");

var request = new TemplateFilesRenderRequest(
    TemplateFilePaths: new[]
    {
        "templates/Project.cs.sbn",
        "templates/Tests.cs.sbn"
    },
    TemplateDirectoryPath: "templates",
    OutputDirectoryPath: "output",
    PathVariableReplacements: new Dictionary<string, string>(),
    Data: data
);

IReadOnlyList<string> outputPaths = await engine.RenderFilesAsync(request);
```

### 5. Handle Conditional Blocks

```csharp
using NFramework.Core.Template.Scriban;

public record ConditionalData(bool IncludeApi, string ApiName);

var renderer = new ScribanTemplateRenderer();
var data = new ConditionalData(IncludeApi: true, ApiName: "Users");

string template = @"
public class Controller { }
{{ if includeApi }}
public class {{ apiName }}Controller { }
{{ end }}";

string result = await renderer.RenderAsync(template, data);
// Output includes both Controller and UsersController
```

### 6. Handle Missing Variables Gracefully

```csharp
using NFramework.Core.Template.Scriban;

var renderer = new ScribanTemplateRenderer();
var data = new { Known = "value" };

string template = "{{ known }} and {{ unknown }}";
string result = await renderer.RenderAsync(template, data);

// result: "value and {{ unknown }}" (unknown is preserved)
```

## Template Suffix Mappings

| Template File       | Output File     |
| ------------------- | --------------- |
| `template.sbn`      | `template.cs`   |
| `template.scriban`  | `template.cs`   |
| `template.sbn-cs`   | `template.cs`   |
| `template.sbn-html` | `template.html` |
| `template.sbn-txt`  | `template.txt`  |

## Error Handling

```csharp
try
{
    string result = await renderer.RenderAsync("{{ invalid {{ syntax", data);
}
catch (InvalidOperationException ex)
{
    // Template parse error
    Console.WriteLine($"Error: {ex.Message}");
}

try
{
    await engine.RenderFileAsync(new TemplateFileRenderRequest(
        TemplateFilePath: "nonexistent.sbn",
        TemplateDirectoryPath: ".",
        OutputDirectoryPath: "output",
        PathVariableReplacements: new Dictionary<string, string>(),
        Data: data
    ));
}
catch (ArgumentException ex)
{
    // File not found
    Console.WriteLine($"Error: {ex.Message}");
}
```
