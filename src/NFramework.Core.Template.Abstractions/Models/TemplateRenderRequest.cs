namespace NFramework.Core.Template.Abstractions.Models;

public sealed record TemplateRenderRequest
{
    public TemplateRenderRequest(string template, ITemplateData data)
    {
        if (template is null)
        {
            throw new ArgumentException("Template cannot be null", nameof(Template));
        }

        if (data is null)
        {
            throw new ArgumentException("Data cannot be null", nameof(Data));
        }

        Template = template;
        Data = data;
    }

    public string Template { get; }

    public ITemplateData Data { get; }
}
