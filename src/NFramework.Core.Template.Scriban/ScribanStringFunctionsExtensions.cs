using System.Text.RegularExpressions;
using PluralizeService.Core;

namespace NFramework.Core.Template.Scriban;

internal static partial class ScribanStringFunctionsExtensions
{
    public static string CamelCase(object? input)
    {
        string pascal = PascalCase(input);
        if (pascal.Length == 0)
        {
            return string.Empty;
        }

        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }

    public static string PascalCase(object? input)
    {
        string value = input?.ToString() ?? string.Empty;
        if (value.Length == 0)
        {
            return string.Empty;
        }

        string[] underscoreSegments = value.Split('_', StringSplitOptions.None);
        List<string> transformed = [];
        foreach (string segment in underscoreSegments)
        {
            string[] words = SplitForPascalRegex().Split(segment).Where(word => word.Length > 0).ToArray();

            string transformedSegment = string.Concat(
                words.Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant())
            );
            transformed.Add(transformedSegment);
        }

        return string.Join("_", transformed);
    }

    public static string SnakeCase(object? input)
    {
        string value = input?.ToString() ?? string.Empty;
        if (value.Length == 0)
        {
            return string.Empty;
        }

        string withUnderscore = UpperBoundaryRegex().Replace(value, "$1_$2");
        string normalized = SeparatorRegex().Replace(withUnderscore, "_");
        return normalized.ToLowerInvariant();
    }

    public static string KebabCase(object? input)
    {
        string value = input?.ToString() ?? string.Empty;
        if (value.Length == 0)
        {
            return string.Empty;
        }

        string withDash = UpperBoundaryRegex().Replace(value, "$1-$2");
        string normalized = SeparatorRegex().Replace(withDash, "-");
        return normalized.ToLowerInvariant();
    }

    public static string Abbreviation(object? input)
    {
        string words = Words(input);
        if (words.Length == 0)
        {
            return string.Empty;
        }

        return string.Concat(
            words.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => char.ToLowerInvariant(word[0]))
        );
    }

    public static string Plural(object? input)
    {
        string value = input?.ToString() ?? string.Empty;
        return value.Length == 0 ? string.Empty : PluralizationProvider.Pluralize(value);
    }

    public static string Singular(object? input)
    {
        string value = input?.ToString() ?? string.Empty;
        return value.Length == 0 ? string.Empty : PluralizationProvider.Singularize(value);
    }

    public static string Words(object? input)
    {
        string value = input?.ToString() ?? string.Empty;
        if (value.Length == 0)
        {
            return string.Empty;
        }

        string splitByCase = UpperBoundaryRegex().Replace(value, "$1 $2");
        string normalized = SeparatorRegex().Replace(splitByCase, " ");
        return normalized.Trim();
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex UpperBoundaryRegex();

    [GeneratedRegex("[\\s_-]+")]
    private static partial Regex SeparatorRegex();

    [GeneratedRegex("[\\s-]+")]
    private static partial Regex SplitForPascalRegex();
}
