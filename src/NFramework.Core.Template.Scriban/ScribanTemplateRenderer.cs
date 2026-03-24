using System.Reflection;
using System.Text.RegularExpressions;
using NFramework.Core.Template.Abstractions;
using Scriban;
using Scriban.Runtime;

namespace NFramework.Core.Template.Scriban;

public sealed partial class ScribanTemplateRenderer : ITemplateRenderer
{
    private readonly Dictionary<string, Func<TemplateContext, ScriptArray, object?>> _customFunctions;

    public ScribanTemplateRenderer()
    {
        _customFunctions = new Dictionary<string, Func<TemplateContext, ScriptArray, object?>>(StringComparer.Ordinal);
    }

    public string TemplateExtension => ".sbn";

    public void AddCustomFunction(string functionName, Func<TemplateContext, ScriptArray, object?> implementation)
    {
        ArgumentException.ThrowIfNullOrEmpty(functionName);
        ArgumentNullException.ThrowIfNull(implementation);

        _customFunctions[functionName] = implementation;
    }

    public async Task<string> RenderAsync(
        string template,
        ITemplateData data,
        CancellationToken cancellationToken = default
    )
    {
        if (data is null)
        {
            throw new ArgumentException("Data cannot be null", nameof(data));
        }

        cancellationToken.ThrowIfCancellationRequested();
        template ??= string.Empty;

        string normalizedTemplate = EvaluateHandlebarsIfBlocks(template, data);
        normalizedTemplate = NormalizeTemplateSyntax(normalizedTemplate);
        normalizedTemplate = evaluateOrFailCustomFunctionCalls(normalizedTemplate, data);
        string preprocessedTemplate = PreserveUnknownSimpleVariables(normalizedTemplate, data);

        global::Scriban.Template parsedTemplate = global::Scriban.Template.Parse(preprocessedTemplate);
        if (parsedTemplate.HasErrors)
        {
            string error = string.Join(Environment.NewLine, parsedTemplate.Messages.Select(message => message.Message));
            throw new InvalidOperationException(error);
        }

        TemplateContext context = buildContext(data);
        string rendered;
        try
        {
            rendered = await parsedTemplate.RenderAsync(context);
        }
        catch (global::Scriban.Syntax.ScriptRuntimeException exception)
        {
            if (exception.Message.Contains("function", StringComparison.OrdinalIgnoreCase))
            {
                string functionName = ExtractUnknownFunctionName(exception.Message) ?? "unknown_function";
                throw new InvalidOperationException($"Function '{functionName}' not found", exception);
            }

            throw;
        }
        cancellationToken.ThrowIfCancellationRequested();
        return RestoreUnknownSimpleVariables(rendered);
    }

    private TemplateContext buildContext(ITemplateData data)
    {
        ScriptObject globals = new();
        globals.Import(data, renamer: member => ToLowerCamelCase(member.Name));
        EnsureAliases(globals, data);

        globals.Import("camel_case", new Func<object?, string>(ScribanStringFunctionsExtensions.CamelCase));
        globals.Import("pascal_case", new Func<object?, string>(ScribanStringFunctionsExtensions.PascalCase));
        globals.Import("snake_case", new Func<object?, string>(ScribanStringFunctionsExtensions.SnakeCase));
        globals.Import("kebab_case", new Func<object?, string>(ScribanStringFunctionsExtensions.KebabCase));

        foreach ((string functionName, Func<TemplateContext, ScriptArray, object?> implementation) in _customFunctions)
        {
            globals.Import(functionName, implementation);
        }

        TemplateContext context = new()
        {
            StrictVariables = true,
            EnableRelaxedMemberAccess = true,
            EnableRelaxedTargetAccess = true,
            EnableRelaxedFunctionAccess = false,
            MemberRenamer = member => ToLowerCamelCase(member.Name),
        };
        context.PushGlobal(globals);
        return context;
    }

    private static string NormalizeTemplateSyntax(string template)
    {
        string result = template;
        result = IfOpenRegex().Replace(result, "{{ if $1 }}");
        result = result.Replace("{{else}}", "{{ else }}", StringComparison.Ordinal);
        result = result.Replace("{{/if}}", "{{ end }}", StringComparison.Ordinal);
        result = MemberPathRegex()
            .Replace(
                result,
                match =>
                {
                    string[] segments = match.Groups[1].Value.Split('.', StringSplitOptions.RemoveEmptyEntries);
                    string normalizedPath = string.Join(".", segments.Select(ToLowerCamelCase));
                    return "{{" + normalizedPath + "}}";
                }
            );

        if (!result.Contains("{{", StringComparison.Ordinal))
        {
            result = SingleBraceVariableRegex().Replace(result, "{{$1}}");
        }

        return result;
    }

    private static string PreserveUnknownSimpleVariables(string template, ITemplateData data)
    {
        HashSet<string> knownVariables = data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x => ToLowerCamelCase(x.Name))
            .ToHashSet(StringComparer.Ordinal);
        if (knownVariables.Contains("name"))
        {
            _ = knownVariables.Add("text");
        }

        return SimpleVariableRegex()
            .Replace(
                template,
                match =>
                {
                    string variableName = match.Groups[1].Value;
                    return knownVariables.Contains(variableName)
                        ? match.Value
                        : $"__NFW_UNKNOWN_OPEN__{variableName}__NFW_UNKNOWN_CLOSE__";
                }
            );
    }

    private static string RestoreUnknownSimpleVariables(string rendered)
    {
        return UnknownPlaceholderRegex().Replace(rendered, "{{$1}}");
    }

    private static string ToLowerCamelCase(string input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    [GeneratedRegex("\\{([a-zA-Z_][a-zA-Z0-9_\\.]*)\\}")]
    private static partial Regex SingleBraceVariableRegex();

    [GeneratedRegex("\\{\\{\\s*([a-zA-Z_][a-zA-Z0-9_]*)\\s*\\}\\}")]
    private static partial Regex SimpleVariableRegex();

    [GeneratedRegex("\\{\\{\\s*([a-zA-Z_][a-zA-Z0-9_]*(?:\\.[a-zA-Z_][a-zA-Z0-9_]*)+)\\s*\\}\\}")]
    private static partial Regex MemberPathRegex();

    [GeneratedRegex("\\{\\{\\s*#if\\s+([^}]+?)\\s*\\}\\}")]
    private static partial Regex IfOpenRegex();

    [GeneratedRegex(
        "\\{\\{#if\\s+([a-zA-Z_][a-zA-Z0-9_\\.]*)\\s*\\}\\}(.*?)(?:\\{\\{else\\}\\}(.*?))?\\{\\{/if\\}\\}",
        RegexOptions.Singleline
    )]
    private static partial Regex HandlebarsIfRegex();

    [GeneratedRegex("__NFW_UNKNOWN_OPEN__([a-zA-Z_][a-zA-Z0-9_]*)__NFW_UNKNOWN_CLOSE__")]
    private static partial Regex UnknownPlaceholderRegex();

    [GeneratedRegex(
        "\\{\\{\\s*([a-z_][a-z0-9_]*)\\s+([a-zA-Z_][a-zA-Z0-9_]*)(?:\\s+([a-zA-Z_][a-zA-Z0-9_]*))?\\s*\\}\\}"
    )]
    private static partial Regex FunctionRegex();

    private string evaluateOrFailCustomFunctionCalls(string template, ITemplateData data)
    {
        return FunctionRegex()
            .Replace(
                template,
                match =>
                {
                    string functionName = match.Groups[1].Value;
                    if (functionName is "if" or "else" or "end")
                    {
                        return match.Value;
                    }

                    if (
                        !_customFunctions.TryGetValue(
                            functionName,
                            out Func<TemplateContext, ScriptArray, object?>? implementation
                        )
                    )
                    {
                        throw new InvalidOperationException($"Function '{functionName}' not found");
                    }

                    ScriptArray arguments = [];
                    string argumentName = match.Groups[2].Value;
                    arguments.Add(ResolveTopLevelValue(data, argumentName));
                    if (match.Groups[3].Success)
                    {
                        arguments.Add(ResolveTopLevelValue(data, match.Groups[3].Value));
                    }

                    object? result = implementation(new TemplateContext(), arguments);
                    return result?.ToString() ?? string.Empty;
                }
            );
    }

    private static object? ResolveTopLevelValue(ITemplateData data, string name)
    {
        PropertyInfo? propertyInfo = data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(x => ToLowerCamelCase(x.Name).Equals(name, StringComparison.Ordinal));
        return propertyInfo?.GetValue(data);
    }

    private static string? ExtractUnknownFunctionName(string message)
    {
        Match match = Regex.Match(message, @"`(?<name>[a-zA-Z_][a-zA-Z0-9_]*)`");
        return match.Success ? match.Groups["name"].Value : null;
    }

    private static void EnsureAliases(ScriptObject globals, ITemplateData data)
    {
        if (
            (!globals.Contains("text") || globals.TryGetValue("text", out object? textValue) && textValue is null)
            && globals.TryGetValue("name", out object? nameValue)
        )
        {
            globals.SetValue("text", nameValue, true);
        }

        foreach (PropertyInfo propertyInfo in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (
                (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                && propertyInfo.Name.StartsWith("Is", StringComparison.Ordinal)
                && propertyInfo.Name.Length > 2
            )
            {
                string aliasName = ToLowerCamelCase(propertyInfo.Name[2..]);
                if (!globals.Contains(aliasName))
                {
                    globals.SetValue(aliasName, propertyInfo.GetValue(data), true);
                }
            }
        }
    }

    private static string EvaluateHandlebarsIfBlocks(string template, ITemplateData data)
    {
        string result = template;
        while (true)
        {
            Match match = HandlebarsIfRegex().Match(result);
            if (!match.Success)
            {
                return result;
            }

            string conditionPath = match.Groups[1].Value;
            string trueBranch = match.Groups[2].Value;
            string falseBranch = match.Groups[3].Success ? match.Groups[3].Value : string.Empty;
            object? conditionValue = ResolvePathValue(data, conditionPath);
            bool condition = conditionValue switch
            {
                bool booleanValue => booleanValue,
                null => false,
                string stringValue => stringValue.Length > 0,
                _ => true,
            };

            string replacement = condition ? trueBranch : falseBranch;
            result = result[..match.Index] + replacement + result[(match.Index + match.Length)..];
        }
    }

    private static object? ResolvePathValue(object? source, string path)
    {
        object? current = source;
        foreach (string segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            if (current is null)
            {
                return null;
            }

            PropertyInfo? propertyInfo = current
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x =>
                    ToLowerCamelCase(x.Name).Equals(segment, StringComparison.Ordinal)
                    || x.Name.Equals(segment, StringComparison.Ordinal)
                );
            propertyInfo ??= current
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x =>
                    x.Name.Equals("Is" + char.ToUpperInvariant(segment[0]) + segment[1..], StringComparison.Ordinal)
                );
            if (propertyInfo is null)
            {
                return null;
            }

            current = propertyInfo.GetValue(current);
        }

        return current;
    }
}
