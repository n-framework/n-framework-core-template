namespace NFramework.Core.Template.Abstractions;

public interface ITemplateRenderer
{
    string TemplateExtension { get; }

    Task<string> RenderAsync(string template, ITemplateData data, CancellationToken cancellationToken = default);
}
