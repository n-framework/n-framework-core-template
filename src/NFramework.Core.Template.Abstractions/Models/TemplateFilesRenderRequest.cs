namespace NFramework.Core.Template.Abstractions.Models;

public sealed record TemplateFilesRenderRequest
{
    public TemplateFilesRenderRequest(
        IReadOnlyList<string> templateFilePaths,
        string templateDirectoryPath,
        string outputDirectoryPath,
        IDictionary<string, string> pathVariableReplacements,
        ITemplateData data
    )
    {
        if (templateFilePaths is null)
        {
            throw new ArgumentException("TemplateFilePaths cannot be null", nameof(TemplateFilePaths));
        }

        if (templateFilePaths.Count == 0)
        {
            throw new ArgumentException("TemplateFilePaths cannot be empty", nameof(TemplateFilePaths));
        }

        if (templateDirectoryPath is null)
        {
            throw new ArgumentException("TemplateDirectoryPath cannot be null", nameof(TemplateDirectoryPath));
        }

        if (templateDirectoryPath.Length == 0)
        {
            throw new ArgumentException("TemplateDirectoryPath cannot be empty", nameof(TemplateDirectoryPath));
        }

        if (outputDirectoryPath is null)
        {
            throw new ArgumentException("OutputDirectoryPath cannot be null", nameof(OutputDirectoryPath));
        }

        if (outputDirectoryPath.Length == 0)
        {
            throw new ArgumentException("OutputDirectoryPath cannot be empty", nameof(OutputDirectoryPath));
        }

        if (data is null)
        {
            throw new ArgumentException("Data cannot be null", nameof(Data));
        }

        TemplateFilePaths = templateFilePaths;
        TemplateDirectoryPath = templateDirectoryPath;
        OutputDirectoryPath = outputDirectoryPath;
        PathVariableReplacements = pathVariableReplacements ?? new Dictionary<string, string>();
        Data = data;
    }

    public IReadOnlyList<string> TemplateFilePaths { get; }

    public string TemplateDirectoryPath { get; }

    public string OutputDirectoryPath { get; }

    public IDictionary<string, string> PathVariableReplacements { get; }

    public ITemplateData Data { get; }
}
