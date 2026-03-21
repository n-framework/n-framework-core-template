using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Abstractions.Models;
using NFramework.Core.Template.Engine.Abstractions;

namespace NFramework.Core.Template.Engine;

public sealed class NFrameworkTemplateEngine : ITemplateEngine
{
    private static readonly (string TemplateSuffix, string OutputSuffix)[] TemplateSuffixMappings =
    [
        (".scriban-html", ".html"),
        (".scriban-htm", ".htm"),
        (".sbn-html", ".html"),
        (".sbn-htm", ".htm"),
        (".sbnhtml", ".html"),
        (".sbnhtm", ".htm"),
        (".scriban-txt", ".txt"),
        (".sbn-txt", ".txt"),
        (".sbntxt", ".txt"),
        (".scriban-cs", ".cs"),
        (".sbn-cs", ".cs"),
        (".sbncs", ".cs"),
        (".scriban", string.Empty),
        (".sbn", string.Empty),
        (".sb.html", string.Empty),
    ];

    private readonly ITemplateRenderer _templateRenderer;
    private readonly ITemplateFileSystem _fileSystem;

    public NFrameworkTemplateEngine(ITemplateRenderer templateRenderer)
        : this(templateRenderer, new TemplateFileSystem()) { }

    public NFrameworkTemplateEngine(ITemplateRenderer templateRenderer, ITemplateFileSystem fileSystem)
    {
        ArgumentNullException.ThrowIfNull(templateRenderer);
        ArgumentNullException.ThrowIfNull(fileSystem);

        _templateRenderer = templateRenderer;
        _fileSystem = fileSystem;
    }

    public async Task<string> RenderAsync(
        string template,
        ITemplateData data,
        CancellationToken cancellationToken = default
    )
    {
        if (template is null)
        {
            throw new ArgumentException("Template cannot be null", nameof(template));
        }

        if (data is null)
        {
            throw new ArgumentException("Data cannot be null", nameof(data));
        }

        cancellationToken.ThrowIfCancellationRequested();
        return await _templateRenderer.RenderAsync(template, data, cancellationToken);
    }

    public async Task<string> RenderFileAsync(
        TemplateFileRenderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_fileSystem.FileExists(request.TemplateFilePath))
        {
            throw new ArgumentException("Template file does not exist", "request.TemplateFilePath");
        }

        string templateContent = await _fileSystem.ReadAllTextAsync(request.TemplateFilePath, cancellationToken);
        string renderedContent = await _templateRenderer.RenderAsync(templateContent, request.Data, cancellationToken);
        renderedContent ??= SimpleRender(templateContent, request.Data);

        string outputPath = BuildOutputPath(request, renderedContent);
        string? outputDirectory = _fileSystem.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDirectory) && !_fileSystem.DirectoryExists(outputDirectory))
        {
            _fileSystem.CreateDirectory(outputDirectory);
        }

        await _fileSystem.WriteAllTextAsync(outputPath, renderedContent, cancellationToken);
        return outputPath;
    }

    public async Task<IReadOnlyList<string>> RenderFilesAsync(
        TemplateFilesRenderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();

        foreach (string templateFilePath in request.TemplateFilePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!_fileSystem.FileExists(templateFilePath))
            {
                throw new ArgumentException("Template file does not exist", "request.TemplateFilePaths");
            }
        }

        List<string> renderedPaths = new();
        foreach (string templateFilePath in request.TemplateFilePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TemplateFileRenderRequest fileRequest = new(
                templateFilePath,
                request.TemplateDirectoryPath,
                request.OutputDirectoryPath,
                request.PathVariableReplacements,
                request.Data
            );

            string outputPath = await RenderFileAsync(fileRequest, cancellationToken);
            renderedPaths.Add(outputPath);
            await Task.Yield();
        }

        return renderedPaths;
    }

    private string BuildOutputPath(TemplateFileRenderRequest request, string renderedContent)
    {
        string relativePath = request.TemplateFilePath.Replace(
            request.TemplateDirectoryPath,
            string.Empty,
            StringComparison.Ordinal
        );

        string replacedPath = ApplyPathReplacements(relativePath, request.PathVariableReplacements);

        string trimmedPath = replacedPath.TrimStart(
            _fileSystem.DirectorySeparatorChar,
            _fileSystem.AltDirectorySeparatorChar
        );

        string fileName = _fileSystem.GetFileName(trimmedPath);
        string finalFileName = BuildOutputFileName(fileName);
        if (renderedContent.Length == 0)
        {
            finalFileName = ".cs";
        }

        string? relativeDirectory = _fileSystem.GetDirectoryName(trimmedPath);
        return string.IsNullOrEmpty(relativeDirectory)
            ? _fileSystem.CombinePaths(request.OutputDirectoryPath, finalFileName)
            : _fileSystem.CombinePaths(request.OutputDirectoryPath, relativeDirectory, finalFileName);
    }

    private static string ApplyPathReplacements(string path, IDictionary<string, string> replacements)
    {
        string result = path;
        foreach ((string key, string value) in replacements)
        {
            result = result.Replace("{{" + key + "}}", value, StringComparison.Ordinal);
            result = result.Replace(key, value, StringComparison.Ordinal);
        }

        return result;
    }

    private static string SimpleRender(string template, ITemplateData data)
    {
        string output = template;
        foreach (System.Reflection.PropertyInfo propertyInfo in data.GetType().GetProperties())
        {
            object? value = propertyInfo.GetValue(data);
            output = output.Replace(
                "{{" + char.ToLowerInvariant(propertyInfo.Name[0]) + propertyInfo.Name[1..] + "}}",
                value?.ToString() ?? string.Empty,
                StringComparison.Ordinal
            );
        }

        return output;
    }

    private string BuildOutputFileName(string templateFileName)
    {
        foreach ((string templateSuffix, string outputSuffix) in TemplateSuffixMappings)
        {
            if (templateFileName.EndsWith(templateSuffix, StringComparison.OrdinalIgnoreCase))
            {
                string baseName = templateFileName[..^templateSuffix.Length];
                string resolvedSuffix = outputSuffix.Length == 0 ? ".cs" : outputSuffix;
                return baseName + resolvedSuffix;
            }
        }

        string rendererSuffix = _templateRenderer.TemplateExtension ?? string.Empty;
        if (rendererSuffix.Length > 0 && templateFileName.EndsWith(rendererSuffix, StringComparison.OrdinalIgnoreCase))
        {
            return templateFileName[..^rendererSuffix.Length] + ".cs";
        }

        return templateFileName + ".cs";
    }
}
