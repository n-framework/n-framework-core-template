namespace NFramework.Core.Template.Abstractions.Models;

public sealed record TemplateFileRenderRequest
{
    public TemplateFileRenderRequest(
        string templateFilePath,
        string templateDirectoryPath,
        string outputDirectoryPath,
        IDictionary<string, string> pathVariableReplacements,
        ITemplateData data
    )
    {
        if (templateFilePath is null)
        {
            throw new ArgumentException("TemplateFilePath cannot be null", nameof(TemplateFilePath));
        }

        if (templateFilePath.Length == 0)
        {
            throw new ArgumentException("TemplateFilePath cannot be empty", nameof(TemplateFilePath));
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

        TemplateFilePath = templateFilePath;
        TemplateDirectoryPath = templateDirectoryPath;
        OutputDirectoryPath = outputDirectoryPath;
        PathVariableReplacements = pathVariableReplacements ?? new Dictionary<string, string>();
        Data = data;
    }

    public string TemplateFilePath { get; }

    public string TemplateDirectoryPath { get; }

    public string OutputDirectoryPath { get; }

    public IDictionary<string, string> PathVariableReplacements { get; }

    public ITemplateData Data { get; }
}
