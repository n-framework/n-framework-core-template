namespace NFramework.Core.Template.Abstractions;

public interface ITemplateEngine
{
    Task<string> RenderAsync(string template, ITemplateData data, CancellationToken cancellationToken = default);

    Task<string> RenderFileAsync(
        Models.TemplateFileRenderRequest request,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<string>> RenderFilesAsync(
        Models.TemplateFilesRenderRequest request,
        CancellationToken cancellationToken = default
    );
}
